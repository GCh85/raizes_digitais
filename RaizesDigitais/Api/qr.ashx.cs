using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using Newtonsoft.Json;

namespace RaizesDigitais.Api
{
    /// <summary>
    /// Summary description for qr
    /// </summary>
    public class qr : IHttpHandler
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
            using (var cmd = new SqlCommand("sp_obter_qr", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_qr", id);
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        resultado = new
                        {
                            id_qr = dr["id_qr"],
                            titulo = dr["titulo"],
                            tipo = dr["tipo"],
                            descricao = dr["descricao"],
                            imagem_url = dr["imagem_url"],
                            localizacao = dr["localizacao"],
                            oferta_descricao = dr["oferta_descricao"],
                            id_vinho = dr["id_vinho"],
                            id_experiencia = dr["id_experiencia"]
                        };
                    }
                }
            }
            context.Response.Write(JsonConvert.SerializeObject(resultado));
        }

        public bool IsReusable => false;
    }
}