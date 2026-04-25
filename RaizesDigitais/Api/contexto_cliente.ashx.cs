using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Configuration;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RaizesDigitais.Api
{
    public class contexto_cliente : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            int idCliente;
            if (!int.TryParse(context.Request.QueryString["id"], out idCliente))
            {
                context.Response.Write("{\"erro\":\"id inválido\"}");
                return;
            }

            string connStr = WebConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_obter_cliente_detalhe", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_cliente", idCliente);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);

                        var resultado = new Dictionary<string, object>();

                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow r = ds.Tables[0].Rows[0];
                            resultado["nome"] = r["nome"].ToString();
                            resultado["preferencias_vinho"] = r["preferencias_vinho"] != DBNull.Value
                                ? r["preferencias_vinho"].ToString() : "";
                            resultado["notas_alergias"] = r["notas_alergias"] != DBNull.Value
                                ? r["notas_alergias"].ToString() : "";
                            resultado["segmento"] = r["segmento_crm"] != DBNull.Value
                                ? r["segmento_crm"].ToString() : "Regular";
                        }

                        if (ds.Tables.Count > 3 && ds.Tables[3].Rows.Count > 0)
                        {
                            DataRow r = ds.Tables[3].Rows[0];
                            resultado["total_pontos"] = r["total_pontos"];
                            resultado["nivel"] = r["nivel_fidelizacao"].ToString();
                        }
                        else
                        {
                            resultado["total_pontos"] = 0;
                            resultado["nivel"] = "Visitante";
                        }

                        var favoritos = new List<string>();
                        if (ds.Tables.Count > 2)
                            foreach (DataRow r in ds.Tables[2].Rows)
                                favoritos.Add(r["nome"].ToString());
                        resultado["favoritos"] = favoritos;

                        var reservas = new List<string>();
                        if (ds.Tables.Count > 1)
                        {
                            int count = 0;
                            foreach (DataRow r in ds.Tables[1].Rows)
                            {
                                if (count >= 3) break;
                                if (r["estado"].ToString() == "Concluida")
                                {
                                    reservas.Add(r["experiencia"] + " em "
                                        + Convert.ToDateTime(r["data_hora"]).ToString("dd/MM/yyyy"));
                                    count++;
                                }
                            }
                        }
                        // Total real de reservas
                        resultado["num_reservas"] = ds.Tables.Count > 1 ? ds.Tables[1].Rows.Count : 0;
                        resultado["ultimas_reservas"] = reservas;

                        context.Response.Write(JsonConvert.SerializeObject(resultado));
                    }
                }
            }
        }

        public bool IsReusable { get { return false; } }
    }
}