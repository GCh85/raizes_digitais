// Copyright (c) 2026 GCh85. All rights reserved.
// This code is part of the Raizes Digitais project.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace RaizesDigitais
{
    // ======
    // Autenticacao Segura
    // ======
    // Usa SHA-256 e Salt.
    // Garante hashes unicos.

    public class Seguranca
    {
        // ======
        // Gestao Salts
        // ======
        // Gera salt aleatorio.
        // Unico por utilizador.
        public static string GerarSalt()
        {
            byte[] saltBytes = new byte[32];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        // ======
        // Criptografia Passwords
        // ======
        // Calcula hash SHA-256.
        // Concatena pass e salt.
        public static string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                string combined = password + salt;
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
                return Convert.ToBase64String(bytes);
            }
        }

        // ======
        // Validacao Acesso
        // ======
        // Compara hashes calculados.
        // Verifica integridade pass.
        public static bool ValidarPassword(string password, string salt, string hashGuardado)
        {
            string hashCalculado = HashPassword(password, salt);
            return hashCalculado == hashGuardado;
        }

        // ======
        // Gestao Tokens
        // ======
        // Gera Guid alfanumerico.
        // Usado activacao e reset.
        public static string GerarToken()
        {
            return Guid.NewGuid().ToString("N");
        }

        // ======
        // Autenticacao 2FA
        // ======
        // Gera codigo 6 digitos.
        // Valido por 10 min.
        public static string GerarCodigo2FA()
        {
            byte[] bytes = new byte[4];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                rng.GetBytes(bytes);
            int valor = Math.Abs(BitConverter.ToInt32(bytes, 0) % 900000) + 100000;
            return valor.ToString();
        }
    }
}
