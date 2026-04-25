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
//   1. Page_Load — verificar sessão parcial (2fa_user_id)
//      Se não existe -> redirect login
//   2. btn_verificar_Click:
//      sp_validar_codigo_2fa(@id_utilizador, @codigo) -> @retorno
//      1 = válido -> sessão completa → redirect dashboard
//      2 = expirado -> mensagem
//      0 = inválido -> mensagem



namespace RaizesDigitais
{
    public partial class verificar_2fa : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Sem sessão parcial -> não passou pelo login -> redirect
            if (Session["2fa_user_id"] == null)
                Response.Redirect("~/login.aspx");
        }

        protected void btn_verificar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int id_utilizador = Convert.ToInt32(Session["2fa_user_id"]);
            string perfil = Session["2fa_perfil"].ToString();
            string codigo = tb_codigo.Text.Trim();

            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand myCommand = new SqlCommand("sp_validar_codigo_2fa", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@id_utilizador", id_utilizador);
            myCommand.Parameters.AddWithValue("@codigo", codigo);

            SqlParameter paramRetorno = new SqlParameter("@retorno", SqlDbType.Int);
            paramRetorno.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(paramRetorno);

            myConn.Open();
            myCommand.ExecuteNonQuery();
            myConn.Close();

            int retorno = Convert.ToInt32(myCommand.Parameters["@retorno"].Value);

            if (retorno == 0)
            {
                lbl_erro.Text = "Código inválido. Tente novamente.";
                return;
            }

            if (retorno == -1)
            {
                lbl_erro.Text = "Código expirado. Volte ao login para receber um novo código.";
                return;
            }

            // retorno == 1 — código válido, completar sessão
            string utilizador = ObterNomeUtilizador(id_utilizador);

            Session["perfil"] = perfil;
            Session["utilizador"] = utilizador;
            Session["utilizador_id"] = id_utilizador;

            // Limpar sessão parcial
            Session.Remove("2fa_user_id");
            Session.Remove("2fa_perfil");

            Response.Redirect("~/Backoffice/dashboard.aspx");
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
    }
}