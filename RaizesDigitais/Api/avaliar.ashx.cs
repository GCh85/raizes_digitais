using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using Newtonsoft.Json;

namespace RaizesDigitais.Api
{
    /// <summary>
    /// Avaliar vinho na app mobile
    /// POST: { id_cliente, id_vinho, estrelas, comentario }
    /// - Avalia vinho com 1-5 estrelas
    /// - Registar 10 pontos se bem-sucedido
    /// </summary>
    public class avaliar : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            // ==========================
            // Configuracao resposta
            // ==========================
            // Formato json. Permite cors.
            context.Response.ContentType = "application/json";
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            // ==========================
            // Validacao metodo
            // ==========================
            if (context.Request.HttpMethod != "POST")
            {
                context.Response.Write(JsonConvert.SerializeObject(new { sucesso = false, erro = "Metodo nao permitido" }));
                return;
            }

            // ==========================
            // Leitura input
            // ==========================
            // Obtem json stream.
            string jsonInput;
            using (StreamReader reader = new StreamReader(context.Request.InputStream))
            {
                jsonInput = reader.ReadToEnd();
            }

            int idCliente;
            int idVinho;
            int estrelas;
            string comentario = "";

            try
            {
                var dados = JsonConvert.DeserializeObject<dynamic>(jsonInput);
                idCliente = Convert.ToInt32(dados.id_cliente);
                idVinho = Convert.ToInt32(dados.id_vinho);
                estrelas = Convert.ToInt32(dados.estrelas);

                if (estrelas < 1 || estrelas > 5)
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { sucesso = false, erro = "Estrelas 1 a 5" }));
                    return;
                }

                if (dados.comentario != null)
                {
                    comentario = dados.comentario.ToString();
                }
            }
            catch
            {
                context.Response.Write(JsonConvert.SerializeObject(new { sucesso = false, erro = "Json invalido" }));
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // ==========================
                // Inserir avaliacao bd
                // ==========================
                // Via sp. Pontos automaticos.
                SqlCommand cmdAvaliar = new SqlCommand("sp_inserir_avaliacao", conn);
                cmdAvaliar.CommandType = CommandType.StoredProcedure;
                cmdAvaliar.Parameters.AddWithValue("@id_cliente", idCliente);
                cmdAvaliar.Parameters.AddWithValue("@id_vinho", idVinho);
                cmdAvaliar.Parameters.AddWithValue("@estrelas", estrelas);
                cmdAvaliar.Parameters.AddWithValue("@comentario", comentario ?? (object)DBNull.Value);

                SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmdAvaliar.Parameters.Add(outRetorno);

                cmdAvaliar.ExecuteNonQuery();

                int retorno = Convert.ToInt32(outRetorno.Value);

                // ==========================
                // Resposta final app
                // ==========================
                if (retorno == -1)
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { sucesso = false, erro = "Ja avaliou este vinho" }));
                    return;
                }

                if (retorno == 1)
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { sucesso = true, pontos_ganhos = 10, estrelas = estrelas }));
                }
                else
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { sucesso = false, erro = "Erro ao registar" }));
                }
            }
        }

        public bool IsReusable => false;
    }
}