// Copyright (c) 2026 GCh85. All rights reserved.
// This code is part of the Raizes Digitais project.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.

using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace RaizesDigitais.Api
{
    /// <summary>
    /// Login de cliente para a app mobile
    /// POST: recebe JSON { email, password }
    /// Retorna: { sucesso, id_cliente, nome, email } ou { sucesso: false, erro }
    /// </summary>
    public class login_cliente : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            // Apenas POST
            if (context.Request.HttpMethod != "POST")
            {
                context.Response.Write(JsonConvert.SerializeObject(new
                {
                    sucesso = false,
                    erro = "Método não permitido. Use POST."
                }));
                return;
            }

            // Ler JSON do body
            string jsonInput;
            using (StreamReader reader = new StreamReader(context.Request.InputStream))
            {
                jsonInput = reader.ReadToEnd();
            }

            string email = "";
            string password = "";

            try
            {
                var dados = JsonConvert.DeserializeObject<dynamic>(jsonInput);
                email = dados.email?.ToString()?.Trim() ?? "";
                password = dados.password?.ToString() ?? "";
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

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                context.Response.Write(JsonConvert.SerializeObject(new
                {
                    sucesso = false,
                    erro = "Email e password obrigatórios"
                }));
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // Passo 1: obter salt
                SqlCommand cmdSalt = new SqlCommand("sp_obter_salt_cliente", conn);
                cmdSalt.CommandType = CommandType.StoredProcedure;
                cmdSalt.Parameters.AddWithValue("@email", email);

                SqlParameter outSalt = new SqlParameter("@salt", SqlDbType.VarChar) { Size = 100, Direction = ParameterDirection.Output };
                SqlParameter outRetSalt = new SqlParameter("@retorno", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmdSalt.Parameters.Add(outSalt);
                cmdSalt.Parameters.Add(outRetSalt);

                cmdSalt.ExecuteNonQuery();

                if (Convert.ToInt32(outRetSalt.Value) != 1)
                {
                    context.Response.Write(JsonConvert.SerializeObject(new
                    {
                        sucesso = false,
                        erro = "Credenciais inválidas"
                    }));
                    return;
                }

                string salt = outSalt.Value.ToString();

                // Passo 2: calcular hash
                string hash = HashPassword(password, salt);

                // Passo 3: validar login
                SqlCommand cmdLogin = new SqlCommand("sp_login_cliente", conn);
                cmdLogin.CommandType = CommandType.StoredProcedure;
                cmdLogin.Parameters.AddWithValue("@email", email);
                cmdLogin.Parameters.AddWithValue("@hash", hash);

                SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int) { Direction = ParameterDirection.Output };
                SqlParameter outIdCliente = new SqlParameter("@id_cliente", SqlDbType.Int) { Direction = ParameterDirection.Output };
                SqlParameter outNome = new SqlParameter("@nome_cliente", SqlDbType.VarChar) { Size = 150, Direction = ParameterDirection.Output };
                cmdLogin.Parameters.Add(outRetorno);
                cmdLogin.Parameters.Add(outIdCliente);
                cmdLogin.Parameters.Add(outNome);

                cmdLogin.ExecuteNonQuery();

                if (Convert.ToInt32(outRetorno.Value) != 1)
                {
                    context.Response.Write(JsonConvert.SerializeObject(new
                    {
                        sucesso = false,
                        erro = "Credenciais inválidas"
                    }));
                    return;
                }

                // Sucesso
                context.Response.Write(JsonConvert.SerializeObject(new
                {
                    sucesso = true,
                    id_cliente = Convert.ToInt32(outIdCliente.Value),
                    nome = outNome.Value?.ToString() ?? "",
                    email = email
                }));
            }
        }

        /// <summary>
        /// Calcula hash SHA-256 da password + salt (igual ao site)
        /// </summary>
        private string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                string combined = password + salt;
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
                return Convert.ToBase64String(bytes);
            }
        }

        public bool IsReusable => false;
    }
}