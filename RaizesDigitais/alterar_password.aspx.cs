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
//   1. Obter salt da BD para o utilizador autenticado
//   2. Validar password actual com Seguranca.ValidarPassword
//   3. Gerar novo salt + hash
//   4. sp_alterar_password(@id_utilizador, @hash, @salt)


namespace RaizesDigitais
{
    public partial class alterar_password : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["perfil"] == null)
                Response.Redirect("~/login.aspx");
        }

        protected void btn_alterar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            int id_utilizador = Convert.ToInt32(Session["utilizador_id"]);

            // Obter hash e salt actuais da BD
            string hashActual = "";
            string saltActual = "";
            ObterHashSalt(id_utilizador, ref hashActual, ref saltActual);

            // Validar password actual ANTES de alterar
            if (!Seguranca.ValidarPassword(tb_pw_actual.Text, saltActual, hashActual))
            {
                lbl_erro.Text = "A password actual está incorrecta.";
                lbl_erro.Visible = true;
                lbl_sucesso.Visible = false;
                return;
            }

            // Gerar novo salt e hash
            string novoSalt = Seguranca.GerarSalt();
            string novoHash = Seguranca.HashPassword(tb_pw_nova.Text, novoSalt);

            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand myCommand = new SqlCommand("sp_alterar_password", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@id_utilizador", id_utilizador);
            myCommand.Parameters.AddWithValue("@hash_nova", novoHash);
            myCommand.Parameters.AddWithValue("@salt_novo", novoSalt);

            SqlParameter paramRetorno = new SqlParameter("@retorno", SqlDbType.Int);
            paramRetorno.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(paramRetorno);

            myConn.Open();
            myCommand.ExecuteNonQuery();
            myConn.Close();

            lbl_sucesso.Text = "Password alterada com sucesso.";
            lbl_sucesso.Visible = true;
            lbl_erro.Visible = false;
            tb_pw_actual.Text = "";
            tb_pw_nova.Text = "";
            tb_pw_confirmar.Text = "";
        }
        private void ObterHashSalt(int id_utilizador, ref string hash, ref string salt)
        {
            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand myCommand = new SqlCommand("sp_obter_hash_salt", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@id_utilizador", id_utilizador);

            myConn.Open();
            SqlDataReader reader = myCommand.ExecuteReader();
            if (reader.Read())
            {
                hash = reader["password_hash"].ToString();
                salt = reader["password_salt"].ToString();
            }
            reader.Close();
            myConn.Close();
        }
    }
}