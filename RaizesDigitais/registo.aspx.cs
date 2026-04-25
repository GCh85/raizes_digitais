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
//   1. Validar campos (validators do ASPX tratam o básico)
//   2. GerarSalt() + HashPassword()
//   3. GerarToken() para activação
//   4. sp_registar_utilizador → @retorno
//      1 = sucesso | 0 = email já existe | -1 = utilizador já existe
//   5. Email.EnviarActivacao()
//   6. Mostrar mensagem + redirect para login



namespace RaizesDigitais
{
    public partial class registo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Se já tem sessão activa não precisa de registar
            if (Session["perfil"] != null)
                Response.Redirect("~/dashboard.aspx");
        }

        protected void btn_registar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string utilizador = tb_utilizador.Text.Trim();
            string email = tb_email.Text.Trim();
            string pw = tb_pw.Text;

            // Gerar salt e hash
            string salt = Seguranca.GerarSalt();
            string hash = Seguranca.HashPassword(pw, salt);

            // Gerar token de activação
            string token = Seguranca.GerarToken();

            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand myCommand = new SqlCommand("sp_registar_utilizador", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@utilizador", utilizador);
            myCommand.Parameters.AddWithValue("@email", email);
            myCommand.Parameters.AddWithValue("@hash", hash);
            myCommand.Parameters.AddWithValue("@salt", salt);
            myCommand.Parameters.AddWithValue("@token", token);

            SqlParameter paramRetorno = new SqlParameter("@retorno", SqlDbType.Int);
            paramRetorno.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(paramRetorno);

            myConn.Open();
            myCommand.ExecuteNonQuery();
            myConn.Close();

            int retorno = Convert.ToInt32(myCommand.Parameters["@retorno"].Value);

            if (retorno == 0)
            {
                lbl_erro.Text = "Este email já está registado.";
                return;
            }

            if (retorno == -1)
            {
                lbl_erro.Text = "Este nome de utilizador já existe. Escolha outro.";
                return;
            }

            // retorno == 1 — conta criada, enviar email de activação
            try
            {
                Email.EnviarActivacao(email, utilizador, token);
                lbl_sucesso.Text = "Conta criada! Verifique o seu email para activar a conta.";
                btn_registar.Visible = false;
            }
            catch
            {
                // Conta criada mas email falhou — não é bloqueante
                lbl_sucesso.Text = "Conta criada! Se não receber o email de activação, contacte o administrador.";
                btn_registar.Visible = false;
            }
        }
    }
}