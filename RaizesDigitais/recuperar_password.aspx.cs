using RaizesDigitais;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


// Fluxo:
//   1. Utilizador introduz o nome de utilizador
//   2. sp_guardar_token_reset(@utilizador) → @token + @email + @retorno
//      1 = sucesso | 0 = utilizador não existe
//   3. Email.EnviarResetPassword(@email, @utilizador, @token)
//   4. Mostrar mensagem de sucesso (não revelar se existe ou não)



namespace RaizesDigitais
{
    public partial class recuperar_password : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["perfil"] != null)
                Response.Redirect("~/dashboard.aspx");
        }

        protected void btn_enviar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string utilizador = tb_utilizador.Text.Trim();

            // Verificar se o utilizador existe e obter o email
            string email = ObterEmailPorUtilizador(utilizador);

            if (!string.IsNullOrEmpty(email))
            {
                string token = Seguranca.GerarToken();
                DateTime expira = DateTime.Now.AddHours(1);

                SqlConnection myConn = new SqlConnection(
                    ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

                SqlCommand myCommand = new SqlCommand("sp_guardar_token_reset", myConn);
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

                try { Email.EnviarResetPassword(email, utilizador, token); }
                catch { }
            }

            // Mostrar sempre a mesma mensagem — não revelar se o utilizador existe
            lbl_sucesso.Text = "Se o utilizador existir, receberá um email com instruções.";
            btn_enviar.Visible = false;
        }

        private string ObterEmailPorUtilizador(string utilizador)
        {
            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand myCommand = new SqlCommand("sp_obter_email_por_utilizador", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@utilizador", utilizador);

            myConn.Open();
            object resultado = myCommand.ExecuteScalar();
            myConn.Close();

            return resultado == null ? "" : resultado.ToString();
        }
    }
}