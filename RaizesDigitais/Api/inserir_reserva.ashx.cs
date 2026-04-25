using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using Newtonsoft.Json;

namespace RaizesDigitais.Api
{
    /// <summary>
    /// Summary description for inserir_reserva
    /// </summary>
    public class inserir_reserva : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            if (context.Request.HttpMethod != "POST")
            {
                context.Response.Write("{\"erro\":\"método não permitido\"}");
                return;
            }

            int id_cliente, id_disponibilidade, num_pessoas;
            decimal preco_total;
            string notas = context.Request.Form["notas"] ?? "";

            if (!int.TryParse(context.Request.Form["id_cliente"], out id_cliente) ||
                !int.TryParse(context.Request.Form["id_disponibilidade"], out id_disponibilidade) ||
                !int.TryParse(context.Request.Form["num_pessoas"], out num_pessoas) ||
                !decimal.TryParse(context.Request.Form["preco_total"], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out preco_total))
            {
                context.Response.Write("{\"erro\":\"parâmetros inválidos\"}");
                return;
            }

            string cs = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_inserir_reserva", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_cliente", id_cliente);
                cmd.Parameters.AddWithValue("@id_disponibilidade", id_disponibilidade);
                cmd.Parameters.AddWithValue("@num_pessoas", num_pessoas);
                cmd.Parameters.AddWithValue("@preco_total", preco_total);
                cmd.Parameters.AddWithValue("@notas", notas);

                var pNumReserva = new SqlParameter("@num_reserva", SqlDbType.VarChar, 20);
                pNumReserva.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(pNumReserva);

                var pRetorno = new SqlParameter("@retorno", SqlDbType.Int);
                pRetorno.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(pRetorno);

                con.Open();
                cmd.ExecuteNonQuery();

                int retorno = (int)pRetorno.Value;
                string num_reserva = retorno == 1 ? pNumReserva.Value.ToString() : "";

                context.Response.Write(JsonConvert.SerializeObject(new
                {
                    sucesso = retorno == 1,
                    num_reserva = num_reserva
                }));
            }
        }

        public bool IsReusable => false;
    }
}