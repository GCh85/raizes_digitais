// Copyright (c) 2026 GCh85. All rights reserved.
// This code is part of the Raizes Digitais project.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace RaizesDigitais.Api
{
    public class narrativa_ia : IHttpHandler
    {
        private const string OPENROUTER_URL = "https://openrouter.ai/api/v1/chat/completions";

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            if (context.Request.HttpMethod != "POST")
            {
                context.Response.Write("{\"sucesso\":false,\"erro\":\"Metodo nao suportado\"}");
                return;
            }

            try
            {
                string body;
                using (var reader = new StreamReader(context.Request.InputStream))
                    body = reader.ReadToEnd();

                var serializer = new JavaScriptSerializer();
                var payload = serializer.Deserialize<Dictionary<string, object>>(body);

                if (!payload.ContainsKey("id_cliente") || !payload.ContainsKey("id_qr"))
                {
                    context.Response.Write("{\"sucesso\":false,\"erro\":\"Parametros em falta\"}");
                    return;
                }

                int idCliente = Convert.ToInt32(payload["id_cliente"]);
                int idQr = Convert.ToInt32(payload["id_qr"]);

                string clienteJson = ObterContextoCliente(idCliente);
                string qrJson = ObterQr(idQr);

                string narrativa = GerarNarrativa(qrJson, clienteJson);

                var resultado = new Dictionary<string, object>
                {
                    { "sucesso", true },
                    { "narrativa", narrativa }
                };

                context.Response.Write(JsonConvert.SerializeObject(resultado));
            }
            catch (Exception ex)
            {
                var erro = new Dictionary<string, object>
                {
                    { "sucesso", false },
                    { "erro", ex.Message }
                };
                context.Response.Write(JsonConvert.SerializeObject(erro));
            }
        }

        private string ObterContextoCliente(int idCliente)
        {
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
                        resultado["num_reservas"] = ds.Tables.Count > 1 ? ds.Tables[1].Rows.Count : 0;
                        resultado["ultimas_reservas"] = reservas;

                        return JsonConvert.SerializeObject(resultado);
                    }
                }
            }
        }

        private string ObterQr(int idQr)
        {
            string connStr = WebConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_obter_qr", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_qr", idQr);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            var resultado = new Dictionary<string, object>
                            {
                                { "id_qr", dr["id_qr"] },
                                { "titulo", dr["titulo"] },
                                { "tipo", dr["tipo"] },
                                { "descricao", dr["descricao"] },
                                { "imagem_url", dr["imagem_url"] },
                                { "localizacao", dr["localizacao"] },
                                { "oferta_descricao", dr["oferta_descricao"] },
                                { "id_vinho", dr["id_vinho"] },
                                { "id_experiencia", dr["id_experiencia"] }
                            };
                            return JsonConvert.SerializeObject(resultado);
                        }
                    }
                }
            }
            return "{}";
        }

        private string GerarNarrativa(string qrJson, string clienteJson)
        {
            var qr = JsonConvert.DeserializeObject<Dictionary<string, object>>(qrJson);
            var cliente = JsonConvert.DeserializeObject<Dictionary<string, object>>(clienteJson);

            string nome = cliente.ContainsKey("nome") ? cliente["nome"].ToString() : "visitante";
            string nivel = cliente.ContainsKey("nivel") ? cliente["nivel"].ToString() : "Visitante";
            int pontos = cliente.ContainsKey("total_pontos") ? Convert.ToInt32(cliente["total_pontos"]) : 0;
            int numReservas = cliente.ContainsKey("num_reservas") ? Convert.ToInt32(cliente["num_reservas"]) : 0;
            string preferencias = cliente.ContainsKey("preferencias_vinho") ? cliente["preferencias_vinho"].ToString() : "";
            string favoritos = cliente.ContainsKey("favoritos") ? String.Join(", ", (List<object>)cliente["favoritos"]) : "";
            string ultimasReservas = cliente.ContainsKey("ultimas_reservas") ? String.Join("; ", (List<object>)cliente["ultimas_reservas"]) : "";

            string tituloQr = qr.ContainsKey("titulo") ? qr["titulo"].ToString() : "";
            string descricaoQr = qr.ContainsKey("descricao") ? qr["descricao"].ToString() : "";
            string tipoQr = qr.ContainsKey("tipo") ? qr["tipo"].ToString() : "local";

            string tomPorNivel;
            switch (nivel)
            {
                case "Embaixador":
                    tomPorNivel = "exclusivo e intimista, como se fosse o proprio produtor a falar";
                    break;
                case "Sommelier":
                    tomPorNivel = "entre pares entendidos, com linguagem tecnica de enofilo";
                    break;
                case "Conhecedor":
                    tomPorNivel = "detalhado e aprofundado, pressupondo interesse genuino pelo vinho";
                    break;
                default:
                    tomPorNivel = "acolhedor e introdutorio, como um guia atencioso numa primeira visita";
                    break;
            }

            string prompt = String.Format(@"
Eres o guia pessoal da Quinta da Azenha, uma propriedade vitivinicola familiar em Bucelas, Portugal.
Escreve uma narrativa personalizada para {0}, que e {1} no programa de fidelizacao com {2} pontos e {3} visitas anteriores.
{4}
{5}
{6}

O cliente acabou de fotografar o QR: ""{7}""
Tipo: {8}
Descricao: {9}

Regras:
- Tom: {10}
- Exactamente 3 paragrafos curtos (3-4 frases cada)
- Nao uses linguagem de marketing nem adjectivos vazios
- Se especifico sobre o que o cliente ja conhece, se tiveres dados
- Termina com algo que ele ainda nao sabe mas vai querer descobrir
- Escreve em portugues europeu, sem emojis
",
                nome,
                nivel,
                pontos,
                numReservas,
                string.IsNullOrEmpty(favoritos) ? "" : $"Os seus vinhos favoritos sao: {favoritos}.",
                string.IsNullOrEmpty(preferencias) ? "" : $"As suas preferencias: {preferencias}.",
                string.IsNullOrEmpty(ultimasReservas) ? "" : $"As suas ultimas visitas: {ultimasReservas}.",
                tituloQr,
                tipoQr,
                descricaoQr,
                tomPorNivel);

            string apiKey = WebConfigurationManager.AppSettings["OpenRouterApiKey"];

            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("API key da OpenRouter nao configurada no servidor");

            var requestBody = new Dictionary<string, object>
            {
                { "model", "openrouter/auto" },
                { "messages", new object[] {
                    new Dictionary<string, string> { { "role", "user" }, { "content", prompt } }
                }}
            };

            var httpRequest = (HttpWebRequest)WebRequest.Create(OPENROUTER_URL);
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/json";
            httpRequest.Headers["Authorization"] = "Bearer " + apiKey;

            var requestJson = JsonConvert.SerializeObject(requestBody);
            var requestBytes = Encoding.UTF8.GetBytes(requestJson);
            httpRequest.ContentLength = requestBytes.Length;

            using (var stream = httpRequest.GetRequestStream())
                stream.Write(requestBytes, 0, requestBytes.Length);

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var reader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var responseBody = reader.ReadToEnd();
                var respJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);

                if (respJson.ContainsKey("choices"))
                {
                    var choices = (List<object>)respJson["choices"];
                    var firstChoice = (Dictionary<string, object>)choices[0];
                    var message = (Dictionary<string, object>)firstChoice["message"];
                    return message["content"].ToString();
                }
                else
                {
                    throw new Exception("Resposta invalida da OpenRouter API");
                }
            }
        }

        public bool IsReusable { get { return false; } }
    }
}