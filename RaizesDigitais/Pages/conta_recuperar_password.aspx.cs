using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

// Fluxo:
//   1. Cliente introduz o email
//   2. sp_guardar_token_reset_cliente(@email, @token, @expira) → @retorno
//      1 = email existe e conta activa | 0 = email não existe ou conta inactiva
//   3. Se @retorno = 1 → Email.EnviarResetPasswordCliente(@email, @email, @token)
//   4. Mostrar sempre a mesma mensagem — não revelar se o email existe ou não

namespace RaizesDigitais.Pages
{
    public partial class conta_recuperar_password : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["cliente_id"] != null)
                Response.Redirect("~/Pages/conta.aspx");
        }

        protected void btn_enviar_Click(object sender, EventArgs e)
        {
            string email = tb_email.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                lbl_mensagem.Text = "Introduza o seu email.";
                lbl_mensagem.CssClass = "alert alert-danger d-block mb-3";
                lbl_mensagem.Visible = true;
                return;
            }

            string token = Seguranca.GerarToken();
            DateTime expira = DateTime.Now.AddHours(1);
            int retorno = 0;

            // --- 1. Guardar token na BD ---
            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand myCommand = new SqlCommand("sp_guardar_token_reset_cliente", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@email", email);
            myCommand.Parameters.AddWithValue("@token", token);
            myCommand.Parameters.AddWithValue("@expira", expira);

            SqlParameter paramRetorno = new SqlParameter("@retorno", SqlDbType.Int);
            paramRetorno.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(paramRetorno);

            myConn.Open();
            myCommand.ExecuteNonQuery();
            myConn.Close();

            retorno = Convert.ToInt32(paramRetorno.Value);

            // --- 2. Enviar email (só se o email existe na BD) ---
            // A lógica de envio fica fora do using — ligação já foi fechada correctamente acima
            if (retorno == 1)
            {
                try { Email.EnviarResetPasswordCliente(email, email, token); }
                catch { }
            }

            // --- 3. Mostrar sempre a mesma mensagem — não revelar se o email existe ---
            lbl_mensagem.Text = "Se o email existir na nossa base de dados, receberá um link para redefinir a password.";
            lbl_mensagem.CssClass = "alert alert-success d-block mb-3";
            lbl_mensagem.Visible = true;
            btn_enviar.Visible = false;
        }
    }
}
