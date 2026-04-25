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
//   1. Ler token da querystring
//   2. sp_activar_conta(@token) -> @retorno
//      1 = activado com sucesso
//      0 = token inválido ou expirado
//   3. Mostrar mensagem e link para login



namespace RaizesDigitais
{
    public partial class activar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            string token = Request.QueryString["token"];

            if (string.IsNullOrEmpty(token))
            {
                lbl_mensagem.Text = "Link inválido.";
                lbl_mensagem.CssClass = "lbl-erro d-block mb-3";
                return;
            }

            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand myCommand = new SqlCommand("sp_activar_conta", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@token", token);

            SqlParameter paramRetorno = new SqlParameter("@retorno", SqlDbType.Int);
            paramRetorno.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(paramRetorno);

            myConn.Open();
            myCommand.ExecuteNonQuery();
            myConn.Close();

            int retorno = Convert.ToInt32(myCommand.Parameters["@retorno"].Value);

            if (retorno == 1)
            {
                lbl_mensagem.Text = "Conta activada com sucesso! Já pode iniciar sessão.";
                lbl_mensagem.CssClass = "lbl-sucesso d-block mb-3";
                lnk_login.Visible = true;
            }
            else
            {
                lbl_mensagem.Text = "Link inválido ou expirado. Registe-se novamente.";
                lbl_mensagem.CssClass = "lbl-erro d-block mb-3";
            }
        }
    }
}