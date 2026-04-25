using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace RaizesDigitais.Api
{
    /// <summary>
    /// Handler de favoritos para a app mobile
    /// GET: lista favoritos do cliente (?id_cliente=X)
    /// POST: toggle favorito (adiciona/remove com pontos)
    /// </summary>
    public class favorito : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            if (context.Request.HttpMethod == "GET")
            {
                // GET: listar favoritos
                int idCliente;
                if (!int.TryParse(context.Request.QueryString["id_cliente"], out idCliente))
                {
                    context.Response.Write(JsonConvert.SerializeObject(new
                    {
                        sucesso = false,
                        erro = "id_cliente inválido"
                    }));
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_listar_favoritos_cliente", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_cliente", idCliente);

                    var lista = new List<object>();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new
                            {
                                id_vinho = reader["id_vinho"],
                                nome = reader["nome"]?.ToString() ?? "",
                                tipo = reader["tipo"]?.ToString() ?? "",
                                casta = reader["casta"]?.ToString() ?? "",
                                preco = reader["preco"]
                            });
                        }
                    }

                    context.Response.Write(JsonConvert.SerializeObject(new
                    {
                        sucesso = true,
                        favoritos = lista
                    }));
                }
            }
            else if (context.Request.HttpMethod == "POST")
            {
                // POST: toggle favorito
                string jsonInput;
                using (StreamReader reader = new StreamReader(context.Request.InputStream))
                {
                    jsonInput = reader.ReadToEnd();
                }

                int idCliente;
                int idVinho;

                try
                {
                    var dados = JsonConvert.DeserializeObject<dynamic>(jsonInput);
                    idCliente = Convert.ToInt32(dados.id_cliente);
                    idVinho = Convert.ToInt32(dados.id_vinho);
                }
                catch
                {
                    context.Response.Write(JsonConvert.SerializeObject(new
                    {
                        sucesso = false,
                        erro = "JSON inválido"
                    }));
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_toggle_favorito_app", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_cliente", idCliente);
                    cmd.Parameters.AddWithValue("@id_vinho", idVinho);

                    SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outRetorno);

                    cmd.ExecuteNonQuery();

                    int retorno = Convert.ToInt32(outRetorno.Value);
                    // retorno: -1=limite atingido, 0=removido, 1=adicionado

                    if (retorno == -1)
                    {
                        context.Response.Write(JsonConvert.SerializeObject(new
                        {
                            sucesso = false,
                            erro = "Limite de favoritos atingido (máximo 5)"
                        }));
                    }
                    else
                    {
                        context.Response.Write(JsonConvert.SerializeObject(new
                        {
                            sucesso = true,
                            adicionado = retorno == 1,
                            pontos_ganhos = retorno == 1 ? 10 : 0
                        }));
                    }
                }
            }
            else
            {
                context.Response.Write(JsonConvert.SerializeObject(new
                {
                    sucesso = false,
                    erro = "Método não permitido. Use GET ou POST."
                }));
            }
        }

        public bool IsReusable => false;
    }
}