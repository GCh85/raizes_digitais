using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using Newtonsoft.Json;

namespace RaizesDigitais.Api
{
    public class listar_vinhos : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            int idCliente = 0;
            int.TryParse(context.Request.QueryString["id_cliente"], out idCliente);

            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // Médias de avaliações (todos os vinhos)
                var medias = new Dictionary<int, object>();
                SqlCommand cmdMedias = new SqlCommand("sp_media_avaliacoes_vinhos", conn);
                cmdMedias.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader r = cmdMedias.ExecuteReader())
                    while (r.Read())
                        medias[Convert.ToInt32(r["id_vinho"])] = new
                        {
                            media = r["media"] != DBNull.Value ? Convert.ToDouble(r["media"]) : 0.0,
                            total = Convert.ToInt32(r["total"])
                        };

                // Avaliações do cliente
                var jaAvaliou = new HashSet<int>();
                // Favoritos do cliente
                var favoritos = new HashSet<int>();
                if (idCliente > 0)
                {
                    SqlCommand cmdAval = new SqlCommand(
                        "SELECT id_vinho FROM avaliacoes_vinhos WHERE id_cliente = @id_cliente", conn);
                    cmdAval.Parameters.AddWithValue("@id_cliente", idCliente);
                    using (SqlDataReader r = cmdAval.ExecuteReader())
                        while (r.Read())
                            jaAvaliou.Add(Convert.ToInt32(r["id_vinho"]));

                    SqlCommand cmdFav = new SqlCommand("sp_listar_favoritos_cliente", conn);
                    cmdFav.CommandType = CommandType.StoredProcedure;
                    cmdFav.Parameters.AddWithValue("@id_cliente", idCliente);
                    using (SqlDataReader r = cmdFav.ExecuteReader())
                        while (r.Read())
                            favoritos.Add(Convert.ToInt32(r["id_vinho"]));
                }

                // Lista de vinhos
                SqlCommand cmdVinhos = new SqlCommand("sp_listar_vinhos", conn);
                cmdVinhos.CommandType = CommandType.StoredProcedure;

                var lista = new List<object>();
                using (SqlDataReader r = cmdVinhos.ExecuteReader())
                {
                    while (r.Read())
                    {
                        int idVinho = Convert.ToInt32(r["id_vinho"]);
                        dynamic mediaInfo = medias.ContainsKey(idVinho)
                            ? medias[idVinho] : new { media = 0.0, total = 0 };

                        lista.Add(new
                        {
                            id_vinho = idVinho,
                            nome = r["nome"]?.ToString() ?? "",
                            casta = r["casta"]?.ToString() ?? "",
                            ano = r["ano"] != DBNull.Value ? (int?)Convert.ToInt32(r["ano"]) : null,
                            preco = r["preco"] != DBNull.Value ? Convert.ToDouble(r["preco"]) : 0,
                            tipo = r["tipo"]?.ToString() ?? "",
                            descricao = r["descricao"]?.ToString() ?? "",
                            imagem_url = r["imagem_url"]?.ToString() ?? "",
                            ja_avaliou = jaAvaliou.Contains(idVinho),
                            media_estrelas = mediaInfo.media,
                            total_avaliacoes = mediaInfo.total,
                            favorito = favoritos.Contains(idVinho)
                        });
                    }
                }

                context.Response.Write(JsonConvert.SerializeObject(new
                {
                    sucesso = true,
                    vinhos = lista
                }));
            }
        }

        public bool IsReusable => false;
    }
}