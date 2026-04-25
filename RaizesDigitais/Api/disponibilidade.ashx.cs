using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace RaizesDigitais.Api
{
    /// <summary>
    /// Summary description for disponibilidade
    /// </summary>
    public class disponibilidade : IHttpHandler
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
            var lista = new List<object>();

            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_listar_disponibilidade", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_experiencia", id);
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new
                        {
                            id_disponibilidade = dr["id_disponibilidade"],
                            data_hora = dr["data_hora"],
                            vagas_total = dr["vagas_total"],
                            vagas_disponiveis = dr["vagas_disponiveis"]
                        });
                    }
                }
            }
            context.Response.Write(JsonConvert.SerializeObject(lista));
        }

        public bool IsReusable => false;
    }
}