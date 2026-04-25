using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Web;
using System.Web.UI;

// Fluxo login normal:
//   1. Obter salt via sp_obter_salt_cliente(@email) → @salt + @retorno
//   2. HashPassword(pw, salt)
//   3. sp_login_cliente(@email, @hash) → @retorno + @id_cliente + @nome_cliente
//   4. ConcluirLogin → verificar reservas → verificar 2FA → redirecionar
//
// Fluxo Google OAuth:
//   1. btn_google_Click → Challenge → Google → redirect /Pages/conta_login.aspx
//   2. Page_Load detecta google_login_pending → ProcessarLoginGoogle
//   3. sp_login_google_cliente → ConcluirLogin

namespace RaizesDigitais.Pages
{
    public partial class conta_login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["cliente_id"] != null)
                Response.Redirect("~/Pages/conta.aspx");

            // ── Callback do Google OAuth ──────────────────────
            if (!IsPostBack && Session["google_login_pending"] != null)
            {
                var ctx = HttpContext.Current.GetOwinContext();
                var identity = ctx.Authentication.AuthenticateAsync("ExternalCookie")
                                  .Result?.Identity as ClaimsIdentity;

                if (identity != null && identity.IsAuthenticated)
                    ProcessarLoginGoogle(identity);

                Session.Remove("google_login_pending");
            }
            else if (!IsPostBack)
            {
                // Limpar cookie externo residual de tentativas anteriores
                var ctx = HttpContext.Current.GetOwinContext();
                ctx.Authentication.SignOut("ExternalCookie");
            }
        }

        // ── LOGIN NORMAL ──────────────────────────────────────
        protected void btn_entrar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string email = tb_email.Text.Trim();
            string pw = tb_pw.Text;

            // Passo 1 — obter salt
            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand cmdSalt = new SqlCommand("sp_obter_salt_cliente", myConn);
            cmdSalt.CommandType = CommandType.StoredProcedure;
            cmdSalt.Parameters.AddWithValue("@email", email);

            SqlParameter outSalt = new SqlParameter("@salt", SqlDbType.VarChar) { Size = 100, Direction = ParameterDirection.Output };
            SqlParameter outRetSalt = new SqlParameter("@retorno", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmdSalt.Parameters.Add(outSalt);
            cmdSalt.Parameters.Add(outRetSalt);

            myConn.Open();
            cmdSalt.ExecuteNonQuery();

            if (Convert.ToInt32(outRetSalt.Value) != 1)
            {
                myConn.Close();
                lbl_erro.Text = "Email ou password incorrectos.";
                lbl_erro.Visible = true;
                return;
            }

            string hash = Seguranca.HashPassword(pw, outSalt.Value.ToString());

            // Passo 2 — login
            SqlCommand cmdLogin = new SqlCommand("sp_login_cliente", myConn);
            cmdLogin.CommandType = CommandType.StoredProcedure;
            cmdLogin.Parameters.AddWithValue("@email", email);
            cmdLogin.Parameters.AddWithValue("@hash", hash);

            SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int) { Direction = ParameterDirection.Output };
            SqlParameter outId = new SqlParameter("@id_cliente", SqlDbType.Int) { Direction = ParameterDirection.Output };
            SqlParameter outNome = new SqlParameter("@nome_cliente", SqlDbType.VarChar) { Size = 150, Direction = ParameterDirection.Output };
            cmdLogin.Parameters.Add(outRetorno);
            cmdLogin.Parameters.Add(outId);
            cmdLogin.Parameters.Add(outNome);

            cmdLogin.ExecuteNonQuery();
            myConn.Close();

            if (Convert.ToInt32(outRetorno.Value) != 1)
            {
                lbl_erro.Text = "Email ou password incorrectos.";
                lbl_erro.Visible = true;
                return;
            }

            ConcluirLogin(Convert.ToInt32(outId.Value), outNome.Value.ToString(), email);
        }

        // ── GOOGLE OAUTH — iniciar challenge ─────────────────
        protected void btn_google_Click(object sender, EventArgs e)
        {
            Session["google_login_pending"] = true;

            var ctx = HttpContext.Current.GetOwinContext();
            ctx.Authentication.Challenge(
                new Microsoft.Owin.Security.AuthenticationProperties
                {
                    RedirectUri = "/Pages/conta_login.aspx"
                },
                "Google"
            );
            Response.End();
        }

        // ── GOOGLE OAUTH — processar callback ────────────────
        private void ProcessarLoginGoogle(ClaimsIdentity identity)
        {
            string email = identity.FindFirst(ClaimTypes.Email)?.Value ?? "";
            string googleId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            string nome = identity.FindFirst(ClaimTypes.Name)?.Value ?? email;

            if (string.IsNullOrEmpty(email))
            {
                lbl_erro.Text = "Não foi possível obter o email da conta Google.";
                lbl_erro.Visible = true;
                return;
            }

            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand cmd = new SqlCommand("sp_login_google_cliente", myConn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@google_id", googleId);
            cmd.Parameters.AddWithValue("@nome", nome);

            SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int) { Direction = ParameterDirection.Output };
            SqlParameter outId = new SqlParameter("@id_cliente", SqlDbType.Int) { Direction = ParameterDirection.Output };
            SqlParameter outNome = new SqlParameter("@nome_cliente", SqlDbType.VarChar) { Size = 150, Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outRetorno);
            cmd.Parameters.Add(outId);
            cmd.Parameters.Add(outNome);

            myConn.Open();
            cmd.ExecuteNonQuery();
            myConn.Close();

            if (Convert.ToInt32(outRetorno.Value) != 1)
            {
                lbl_erro.Text = "Erro ao autenticar com o Google. Tente novamente.";
                lbl_erro.Visible = true;
                return;
            }

            HttpContext.Current.GetOwinContext().Authentication.SignOut("ExternalCookie");
            ConcluirLogin(Convert.ToInt32(outId.Value), outNome.Value.ToString(), email);
        }

        // ── LÓGICA COMUM — verificar 2FA e redirecionar ──────
        private void ConcluirLogin(int idCliente, string nomeCliente, string email)
        {
            // Passo 1 — verificar se o cliente tem pelo menos uma reserva
            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand cmdReservas = new SqlCommand("sp_obter_total_reservas_cliente", myConn);
            cmdReservas.CommandType = CommandType.StoredProcedure;
            cmdReservas.Parameters.AddWithValue("@id_cliente", idCliente);

            SqlParameter outTotal = new SqlParameter("@total", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmdReservas.Parameters.Add(outTotal);

            myConn.Open();
            cmdReservas.ExecuteNonQuery();
            myConn.Close();

            int totalReservas = outTotal.Value != DBNull.Value ? Convert.ToInt32(outTotal.Value) : 0;

            if (totalReservas == 0)
            {
                HttpContext.Current.GetOwinContext().Authentication.SignOut("ExternalCookie");
                string urlExp = ResolveUrl("~/Pages/experiencias.aspx");
                lbl_erro.Text = "A sua área pessoal fica disponível após a sua primeira visita à Quinta. " +
                                "<a href='" + urlExp + "'>Descubra as nossas experiências</a>.";
                lbl_erro.Visible = true;
                return;
            }

            // Passo 2 — verificar preferência 2FA
            SqlConnection myConn2 = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand cmd2FA = new SqlCommand("sp_obter_preferencia_2fa_cliente", myConn2);
            cmd2FA.CommandType = CommandType.StoredProcedure;
            cmd2FA.Parameters.AddWithValue("@id_cliente", idCliente);

            SqlParameter out2FA = new SqlParameter("@dois_factor_activado", SqlDbType.Bit) { Direction = ParameterDirection.Output };
            cmd2FA.Parameters.Add(out2FA);

            myConn2.Open();
            cmd2FA.ExecuteNonQuery();
            myConn2.Close();

            bool tem2FA = out2FA.Value != DBNull.Value && Convert.ToBoolean(out2FA.Value);

            if (tem2FA)
            {
                // Passo 3a — gerar e guardar código 2FA
                string codigo = Seguranca.GerarCodigo2FA();
                DateTime expira = DateTime.Now.AddMinutes(10);

                SqlConnection myConn3 = new SqlConnection(
                    ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

                SqlCommand cmdGuardar = new SqlCommand("sp_guardar_codigo_2fa_cliente", myConn3);
                cmdGuardar.CommandType = CommandType.StoredProcedure;
                cmdGuardar.Parameters.AddWithValue("@id_cliente", idCliente);
                cmdGuardar.Parameters.AddWithValue("@codigo", codigo);
                cmdGuardar.Parameters.AddWithValue("@expira", expira);

                myConn3.Open();
                cmdGuardar.ExecuteNonQuery();
                myConn3.Close();

                try { Email.EnviarCodigo2FA(email, nomeCliente, codigo); } catch { }

                Session["temp_cliente_id"] = idCliente;
                Session["temp_cliente_nome"] = nomeCliente;
                Response.Redirect("~/Pages/conta_verificar_2fa.aspx");
            }
            else
            {
                // Passo 3b — login directo sem 2FA
                Session["cliente_id"] = idCliente;
                Session["cliente_nome"] = nomeCliente;
                Response.Redirect("~/Pages/conta.aspx");
            }
        }
    }
}
