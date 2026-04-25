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
    /// Summary description for listar_experiencias
    /// </summary>
    public class listar_experiencias : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            var lista = new List<object>();
            string cs = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("sp_listar_experiencias", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new
                        {
                            id_experiencia = dr["id_experiencia"],
                            nome = dr["nome"],
                            descricao = dr["descricao"],
                            tipo = dr["tipo"],
                            preco_por_pessoa = dr["preco_por_pessoa"],
                            duracao_horas = dr["duracao_horas"],
                            capacidade_max = dr["capacidade_max"],
                            imagem_url = dr["imagem_url"]
                        });
                    }
                }
            }
            context.Response.Write(JsonConvert.SerializeObject(lista));
        }

        public bool IsReusable => false;
    }
}