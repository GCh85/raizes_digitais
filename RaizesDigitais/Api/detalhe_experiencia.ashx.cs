using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using Newtonsoft.Json;

namespace RaizesDigitais.Api
{
    /// <summary>
    /// Summary description for detalhe_experiencia
    /// </summary>
    public class detalhe_experiencia : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            int id;
            if (!int.TryParse(context.Request.QueryString["id"], out id))
            {
                context.Response.Write("{\"erro\":\"id inválido\"}");
                return;
            }

            string cs = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;
            object resultado = null;

            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_obter_detalhe_experiencia", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_experiencia", id);
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        resultado = new
                        {
                            id_experiencia = dr["id_experiencia"],
                            nome = dr["nome"],
                            descricao = dr["descricao"],
                            tipo = dr["tipo"],
                            preco_por_pessoa = dr["preco_por_pessoa"],
                            duracao_horas = dr["duracao_horas"],
                            capacidade_max = dr["capacidade_max"],
                            imagem_url = dr["imagem_url"]
                        };
                    }
                }
            }
            context.Response.Write(JsonConvert.SerializeObject(resultado));
        }

        public bool IsReusable => false;
    }
}