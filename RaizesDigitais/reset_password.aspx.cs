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
//   1. Page_Load - ler token da querystring
//      Token inválido/expirado -> esconder form + mensagem erro
//   2. btn_redefinir_Click:
//      Gerar novo salt + hash
//      sp_reset_password(@token, @hash, @salt) -> @retorno
//      1 = sucesso | 0 = token inválido/expirado



namespace RaizesDigitais
{
    public partial class reset_password : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            string token = Request.QueryString["token"];

            if (string.IsNullOrEmpty(token))
            {
                lbl_erro.Text = "Link inválido.";
                pnl_form.Visible = false;
            }
        }

        protected void btn_redefinir_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string token = Request.QueryString["token"];

            if (string.IsNullOrEmpty(token))
            {
                lbl_erro.Text = "Link inválido.";
                pnl_form.Visible = false;
                return;
            }

            string novoSalt = Seguranca.GerarSalt();
            string novoHash = Seguranca.HashPassword(tb_pw_nova.Text, novoSalt);

            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand myCommand = new SqlCommand("sp_reset_password", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@token", token);
            myCommand.Parameters.AddWithValue("@hash_nova", novoHash);
            myCommand.Parameters.AddWithValue("@salt_novo", novoSalt);

            SqlParameter paramRetorno = new SqlParameter("@retorno", SqlDbType.Int);
            paramRetorno.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(paramRetorno);

            myConn.Open();
            myCommand.ExecuteNonQuery();
            myConn.Close();

            int retorno = Convert.ToInt32(myCommand.Parameters["@retorno"].Value);

            if (retorno == 0)
            {
                lbl_erro.Text = "Link inválido ou expirado. Solicite um novo link.";
                pnl_form.Visible = false;
                return;
            }

            lbl_sucesso.Text = "Palavra-passe redefinida com sucesso! Já pode iniciar sessão.";
            pnl_form.Visible = false;
        }
    }
}