using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

// Fluxo:
//   1. Page_Load verifica Session["temp_cliente_id"] — se null, redireciona para login
//   2. Cliente introduz o código de 6 dígitos recebido por email
//   3. sp_validar_codigo_2fa_cliente(@id_cliente, @codigo) → @retorno
//      1 = válido | -1 = expirado | 0 = inválido
//   4. Se válido → promover sessão temporária para sessão definitiva

namespace RaizesDigitais.Pages
{
    public partial class conta_verificar_2fa : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["temp_cliente_id"] == null)
                Response.Redirect("~/Pages/conta_login.aspx");
        }

        protected void btn_verificar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string codigo = tb_codigo.Text.Trim();
            int idCliente = Convert.ToInt32(Session["temp_cliente_id"]);

            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand myCommand = new SqlCommand("sp_validar_codigo_2fa_cliente", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@id_cliente", idCliente);
            myCommand.Parameters.AddWithValue("@codigo", codigo);

            SqlParameter paramRetorno = new SqlParameter("@retorno", SqlDbType.Int);
            paramRetorno.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(paramRetorno);

            myConn.Open();
            myCommand.ExecuteNonQuery();
            myConn.Close();

            int retorno = Convert.ToInt32(paramRetorno.Value);

            if (retorno == 1) // Válido
            {
                Session["cliente_id"] = Session["temp_cliente_id"];
                Session["cliente_nome"] = Session["temp_cliente_nome"];

                Session.Remove("temp_cliente_id");
                Session.Remove("temp_cliente_nome");

                Response.Redirect("~/Pages/conta.aspx");
            }
            else if (retorno == -1) // Expirado
            {
                lbl_erro.Text = "O código expirou. Tente entrar novamente.";
                lbl_erro.Visible = true;
            }
            else // Inválido (0)
            {
                lbl_erro.Text = "Código incorrecto. Verifique o seu email.";
                lbl_erro.Visible = true;
            }
        }

        protected void btn_voltar_Click(object sender, EventArgs e)
        {
            Session.Remove("temp_cliente_id");
            Session.Remove("temp_cliente_nome");
            Response.Redirect("~/Pages/conta_login.aspx");
        }
    }
}
