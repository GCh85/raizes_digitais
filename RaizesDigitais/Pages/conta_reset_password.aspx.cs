using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

// Fluxo:
//   1. Page_Load verifica se o token está presente na QueryString
//      → se não estiver, esconde o formulário e mostra erro
//   2. Cliente introduz a nova password e confirma
//   3. sp_reset_password_cliente(@token, @hash_nova, @salt_novo) → @retorno
//      1 = sucesso | 0 = token inválido ou expirado
//   4. Mostrar mensagem de sucesso ou erro

namespace RaizesDigitais.Pages
{
    public partial class conta_reset_password : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            if (string.IsNullOrEmpty(Request.QueryString["token"]))
            {
                lbl_mensagem.Text = "Link inválido.";
                lbl_mensagem.CssClass = "alert alert-danger d-block mb-3";
                lbl_mensagem.Visible = true;
                pnl_form.Visible = false;
            }
        }

        protected void btn_redefinir_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string token = Request.QueryString["token"];
            if (string.IsNullOrEmpty(token))
            {
                pnl_form.Visible = false;
                return;
            }

            if (tb_pw_nova.Text != tb_pw_confirmar.Text)
            {
                lbl_mensagem.Text = "As passwords não coincidem.";
                lbl_mensagem.CssClass = "alert alert-danger d-block mb-3";
                lbl_mensagem.Visible = true;
                return;
            }

            string salt = Seguranca.GerarSalt();
            string hash = Seguranca.HashPassword(tb_pw_nova.Text, salt);
            int retorno = 0;

            // --- Actualizar password na BD via token ---
            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand myCommand = new SqlCommand("sp_reset_password_cliente", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@token", token);
            myCommand.Parameters.AddWithValue("@hash_nova", hash);
            myCommand.Parameters.AddWithValue("@salt_novo", salt);

            SqlParameter paramRetorno = new SqlParameter("@retorno", SqlDbType.Int);
            paramRetorno.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(paramRetorno);

            myConn.Open();
            myCommand.ExecuteNonQuery();
            myConn.Close();

            retorno = Convert.ToInt32(paramRetorno.Value);

            if (retorno == 1)
            {
                lbl_mensagem.Text = "Password redefinida com sucesso! <a href='conta_login.aspx' style='color:var(--cor-primaria)'>Entrar</a>.";
                lbl_mensagem.CssClass = "alert alert-success d-block mb-3";
            }
            else
            {
                lbl_mensagem.Text = "Link inválido ou expirado. <a href='conta_recuperar_password.aspx' style='color:var(--cor-primaria)'>Solicitar novo link</a>.";
                lbl_mensagem.CssClass = "alert alert-danger d-block mb-3";
            }

            lbl_mensagem.Visible = true;
            pnl_form.Visible = false;
        }
    }
}
