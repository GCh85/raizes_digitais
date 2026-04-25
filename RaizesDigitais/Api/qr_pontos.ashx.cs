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
    /// Registar pontos por leitura de QR Code na app mobile
    /// POST: { id_cliente, id_qr }
    /// - Atribui 30 pontos (limite 1x por dia via SP)
    /// </summary>
    public class qr_pontos : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            if (context.Request.HttpMethod != "POST")
            {
                context.Response.Write(JsonConvert.SerializeObject(new { sucesso = false, erro = "Método não permitido." }));
                return;
            }

            string jsonInput;
            using (StreamReader reader = new StreamReader(context.Request.InputStream))
            {
                jsonInput = reader.ReadToEnd();
            }

            int idCliente, idQr;
            try
            {
                var dados = JsonConvert.DeserializeObject<dynamic>(jsonInput);
                idCliente = Convert.ToInt32(dados.id_cliente);
                idQr = Convert.ToInt32(dados.id_qr);
            }
            catch
            {
                context.Response.Write(JsonConvert.SerializeObject(new { sucesso = false, erro = "JSON inválido." }));
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_registar_qr_pontos", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_cliente", idCliente);
                cmd.Parameters.AddWithValue("@id_qr", idQr);

                SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outRetorno);

                conn.Open();
                cmd.ExecuteNonQuery();

                int retorno = Convert.ToInt32(outRetorno.Value);

                if (retorno == 1)
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { sucesso = true, pontos_ganhos = 30 }));
                }
                else
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { sucesso = false, erro = "Já acumulou pontos por leitura de QR hoje." }));
                }
            }
        }

        public bool IsReusable => false;
    }
}