using RaizesDigitais;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Web;
using System.Web.UI;


// Fluxo login normal (SHA-256 + Salt):
//   1. sp_obter_salt(@email) → @salt
//   2. Seguranca.HashPassword(pw, salt) → hash
//   3. sp_login(@email, @hash) → @retorno + @retorno_perfil + @id_utilizador
//   4. retorno=1 → 2FA: guardar session parcial + enviar código + redirect
//   5. retorno=2 → conta não activada
//   6. retorno=0 → credenciais inválidas
//
// Fluxo Google OAuth:
//   1. btn_google_Click → Challenge OWIN Google
//   2. Page_Load detecta ClaimsIdentity externo → sp_login_google
//   3. Sessão completa → redirect dashboard



namespace RaizesDigitais
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Se já tem sessão activa vai directo ao dashboard
            if (Session["perfil"] != null)
            {
                Response.Redirect("~/dashboard.aspx");
                return;
            }

            // ── Callback do Google OAuth ──────────────────────
            if (!IsPostBack && Session["google_login_pending"] != null)
            {
                var ctx = HttpContext.Current.GetOwinContext();
                var identity = ctx.Authentication.AuthenticateAsync("ExternalCookie")
                                  .Result?.Identity as ClaimsIdentity;

                if (identity != null && identity.IsAuthenticated)
                {
                    ProcessarLoginGoogle(identity);
                }

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

            string utilizador = tb_utilizador.Text.Trim();
            string pw = tb_pw.Text;
            string salt = "";
            int retSalt = ObterSalt(utilizador, ref salt);



            if (retSalt == 0)
            {
                lbl_erro.Text = "Email ou palavra-passe incorrectos.";
                return;
            }

            string hash = Seguranca.HashPassword(pw, salt);

            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand myCommand = new SqlCommand("sp_login", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@utilizador", utilizador);
            myCommand.Parameters.AddWithValue("@hash", hash);

            SqlParameter paramRetorno = new SqlParameter("@retorno", SqlDbType.Int);
            paramRetorno.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(paramRetorno);

            SqlParameter paramPerfil = new SqlParameter("@retorno_perfil", SqlDbType.VarChar);
            paramPerfil.Direction = ParameterDirection.Output;
            paramPerfil.Size = 50;
            myCommand.Parameters.Add(paramPerfil);

            SqlParameter paramId = new SqlParameter("@id_utilizador", SqlDbType.Int);
            paramId.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(paramId);

            myConn.Open();
            myCommand.ExecuteNonQuery();
            myConn.Close();

            int retorno = Convert.ToInt32(myCommand.Parameters["@retorno"].Value);
            string perfil = myCommand.Parameters["@retorno_perfil"].Value.ToString();
            int id_utilizador = Convert.ToInt32(myCommand.Parameters["@id_utilizador"].Value);

            if (retorno == 0)
            {
                lbl_erro.Text = "Email ou palavra-passe incorrectos.";
                return;
            }

            if (retorno == 2)
            {
                lbl_erro.Text = "Conta ainda não activada. Verifique o seu email.";
                return;
            }

            string emailUtilizador = ObterEmailUtilizador(id_utilizador);
            IniciarFluxo2FA(id_utilizador, emailUtilizador, perfil);
        }
        private string ObterEmailUtilizador(int id_utilizador)
        {
            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand myCommand = new SqlCommand("sp_obter_utilizador", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@id_utilizador", id_utilizador);

            myConn.Open();
            SqlDataReader reader = myCommand.ExecuteReader();
            string email = "";
            if (reader.Read())
                email = reader["email"].ToString();
            reader.Close();
            myConn.Close();

            return email;
        }

        // ── GOOGLE OAUTH — iniciar challenge ─────────────────
        protected void btn_google_Click(object sender, EventArgs e)
        {
            // Flag para saber que viemos do botão Google (evita login automático)
            Session["google_login_pending"] = true;

            var ctx = HttpContext.Current.GetOwinContext();
            ctx.Authentication.Challenge(
                new Microsoft.Owin.Security.AuthenticationProperties
                {
                    RedirectUri = "/login.aspx"
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
                return;
            }

            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand myCommand = new SqlCommand("sp_login_google", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@email", email);
            myCommand.Parameters.AddWithValue("@google_id", googleId);
            myCommand.Parameters.AddWithValue("@nome", nome);

            SqlParameter paramRetorno = new SqlParameter("@retorno", SqlDbType.Int);
            paramRetorno.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(paramRetorno);

            SqlParameter paramPerfil = new SqlParameter("@retorno_perfil", SqlDbType.VarChar);
            paramPerfil.Direction = ParameterDirection.Output;
            paramPerfil.Size = 50;
            myCommand.Parameters.Add(paramPerfil);

            SqlParameter paramId = new SqlParameter("@id_utilizador", SqlDbType.Int);
            paramId.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(paramId);

            myConn.Open();
            myCommand.ExecuteNonQuery();
            myConn.Close();

            int retorno = Convert.ToInt32(myCommand.Parameters["@retorno"].Value);
            string perfil = myCommand.Parameters["@retorno_perfil"].Value.ToString();
            int id_utilizador = Convert.ToInt32(myCommand.Parameters["@id_utilizador"].Value);

            if (retorno == 0)
            {
                lbl_erro.Text = "Erro ao autenticar com o Google. Tente novamente.";
                return;
            }

            string utilizador = ObterNomeUtilizador(id_utilizador);
            Session["perfil"] = perfil;
            Session["utilizador"] = utilizador;
            Session["utilizador_id"] = id_utilizador;

            HttpContext.Current.GetOwinContext().Authentication.SignOut("ExternalCookie");

            Response.Redirect("~/Backoffice/dashboard.aspx");
        }

        // ── FLUXO 2FA ─────────────────────────────────────────
        private void IniciarFluxo2FA(int id_utilizador, string email, string perfil)
        {
            string codigo = Seguranca.GerarCodigo2FA();
            DateTime expira = DateTime.Now.AddMinutes(10);

            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand myCommand = new SqlCommand("sp_guardar_codigo_2fa", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@id_utilizador", id_utilizador);
            myCommand.Parameters.AddWithValue("@codigo", codigo);
            myCommand.Parameters.AddWithValue("@expira", expira);

            myConn.Open();
            myCommand.ExecuteNonQuery();
            myConn.Close();

            string nome = ObterNomeUtilizador(id_utilizador);
            Email.EnviarCodigo2FA(email, nome, codigo);

            // Sessão PARCIAL — ainda não autenticado
            Session["2fa_user_id"] = id_utilizador;
            Session["2fa_perfil"] = perfil;

            Response.Redirect("~/verificar_2fa.aspx");
        }

        // ── AUXILIARES ────────────────────────────────────────

        private int ObterSalt(string utilizador, ref string salt)
        {
            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand myCommand = new SqlCommand("sp_obter_salt", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@utilizador", utilizador);

            SqlParameter paramSalt = new SqlParameter("@salt", SqlDbType.VarChar);
            paramSalt.Direction = ParameterDirection.Output;
            paramSalt.Size = 64;
            myCommand.Parameters.Add(paramSalt);

            SqlParameter paramRetorno = new SqlParameter("@retorno", SqlDbType.Int);
            paramRetorno.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(paramRetorno);

            myConn.Open();
            myCommand.ExecuteNonQuery();
            myConn.Close();

            int retorno = Convert.ToInt32(myCommand.Parameters["@retorno"].Value);
            if (retorno == 1)
                salt = myCommand.Parameters["@salt"].Value.ToString();

            return retorno;
        }

        private string ObterNomeUtilizador(int id_utilizador)
        {
            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand myCommand = new SqlCommand("sp_obter_utilizador", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@id_utilizador", id_utilizador);

            myConn.Open();
            SqlDataReader reader = myCommand.ExecuteReader();
            string nome = "";
            if (reader.Read())
                nome = reader["utilizador"].ToString();
            reader.Close();
            myConn.Close();

            return nome;
        }

        private void MostrarToast(string mensagem, bool sucesso = true)
        {
            string tipo = sucesso ? "success" : "error";
            string script = $"toastr.{tipo}('{mensagem.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "toast", script, true);
        }

        private void MostrarMensagem(string texto, bool sucesso)
        {
            // Manter compatibilidade com código existente (usa lbl_erro)
            lbl_erro.Text = texto;
            lbl_erro.Visible = true;

            // Também mostrar Toastr (só erro, porque login só mostra erros)
            MostrarToast(texto, sucesso);
        }
    }
}