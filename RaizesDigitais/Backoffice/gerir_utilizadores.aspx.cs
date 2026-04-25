using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RaizesDigitais;


// Só acessível ao Administrador — verificado no Page_Load
// sp_listar_utilizadores  -> GridView com paginação
// sp_editar_utilizador    -> btn_guardar
// sp_eliminar_utilizador  -> botão Eliminar
// sp_obter_utilizador     -> pré-preencher form edição



namespace RaizesDigitais.Backoffice
{
    public partial class gerir_utilizadores : System.Web.UI.Page
    {
        private bool ModoCriacao
        {
            get { return hf_id_utilizador.Value == "0"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["perfil"] == null)
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            // Só Admin pode aceder
            if (Session["perfil"].ToString() != "Administrador")
            {
                Response.Redirect("~/dashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                ViewState["pagina"] = 1;
                ViewState["pesquisa"] = "";
                CarregarPerfis();
                CarregarUtilizadores();
            }
        }

        // CARREGAR

        private void CarregarUtilizadores()
        {
            int pagina = Convert.ToInt32(ViewState["pagina"]);
            int porPagina = Convert.ToInt32(ddl_por_pagina.SelectedValue);
            string pesquisa = ViewState["pesquisa"].ToString();

            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand myCommand = new SqlCommand("sp_listar_utilizadores", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@pesquisa", pesquisa);
            myCommand.Parameters.AddWithValue("@pagina", pagina);
            myCommand.Parameters.AddWithValue("@por_pagina", porPagina);

            SqlParameter paramTotal = new SqlParameter("@total", SqlDbType.Int);
            paramTotal.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(paramTotal);

            SqlDataAdapter da = new SqlDataAdapter(myCommand);
            DataTable dt = new DataTable();

            myConn.Open();
            da.Fill(dt);
            myConn.Close();

            int total = Convert.ToInt32(myCommand.Parameters["@total"].Value);

            gv_utilizadores.DataSource = dt;
            gv_utilizadores.DataBind();

            // Paginação
            int totalPaginas = (porPagina == 0) ? 1 : (int)Math.Ceiling((double)total / porPagina);
            lbl_total.Text = total + " utilizador(es)";
            lbl_pagina.Text = "Página " + pagina + " de " + totalPaginas;
            btn_anterior.Enabled = (pagina > 1);
            btn_seguinte.Enabled = (pagina < totalPaginas);
        }

        private void CarregarPerfis()
        {
            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand myCommand = new SqlCommand("sp_listar_perfis", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter da = new SqlDataAdapter(myCommand);
            DataTable dt = new DataTable();

            myConn.Open();
            da.Fill(dt);
            myConn.Close();

            ddl_perfil.DataSource = dt;
            ddl_perfil.DataTextField = "designacao";
            ddl_perfil.DataValueField = "id_perfil";
            ddl_perfil.DataBind();
        }

        // PESQUISA

        protected void btn_pesquisar_Click(object sender, EventArgs e)
        {
            ViewState["pagina"] = 1;
            ViewState["pesquisa"] = tb_pesquisa.Text.Trim();
            CarregarUtilizadores();
        }


        protected void btn_anterior_Click(object sender, EventArgs e)
        {
            ViewState["pagina"] = Convert.ToInt32(ViewState["pagina"]) - 1;
            CarregarUtilizadores();
        }

        protected void btn_seguinte_Click(object sender, EventArgs e)
        {
            ViewState["pagina"] = Convert.ToInt32(ViewState["pagina"]) + 1;
            CarregarUtilizadores();
        }

        protected void ddl_por_pagina_Changed(object sender, EventArgs e)
        {
            ViewState["pagina"] = 1;
            CarregarUtilizadores();
        }

        // AÇÕES GRIDVIEW

        protected void gv_utilizadores_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id_utilizador = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "Editar")
                CarregarUtilizadorParaEdicao(id_utilizador);
            else if (e.CommandName == "Eliminar")
                EliminarUtilizador(id_utilizador);
        }

        private void CarregarUtilizadorParaEdicao(int id_utilizador)
        {
            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand myCommand = new SqlCommand("sp_obter_utilizador", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@id_utilizador", id_utilizador);

            myConn.Open();
            SqlDataReader reader = myCommand.ExecuteReader();

            if (reader.Read())
            {
                hf_id_utilizador.Value = id_utilizador.ToString();
                tb_utilizador.Text = reader["utilizador"].ToString();
                tb_email.Text = reader["email"].ToString();

                string idPerfil = reader["id_perfil"].ToString();
                string activo = reader["activo"].ToString();
                reader.Close();
                myConn.Close();

                CarregarPerfis();
                ListItem itemPerfil = ddl_perfil.Items.FindByValue(idPerfil);
                if (itemPerfil != null) ddl_perfil.SelectedValue = idPerfil;

                ddl_activo.SelectedValue = (activo == "True") ? "1" : "0";
            }
            else
            {
                reader.Close();
                myConn.Close();
            }

            pnl_editar.Visible = true;
            lit_titulo_form.Text = "Editar Utilizador";
            pnl_password.Visible = false;
            pnl_activo.Visible = true;
            tb_password_inicial.Text = "";
            tb_password_confirmar.Text = "";
            lbl_sucesso.Visible = false;
            lbl_erro.Visible = false;
            CarregarUtilizadores();
        }

        private void EliminarUtilizador(int id_utilizador)
        {
            // Não permitir eliminar o próprio utilizador
            if (id_utilizador == Convert.ToInt32(Session["utilizador_id"]))
            {
                MostrarMensagem("Não pode eliminar o seu próprio utilizador.", false);
                return;
            }

            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand myCommand = new SqlCommand("sp_desactivar_utilizador", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@id_utilizador", id_utilizador);

            myConn.Open();
            myCommand.ExecuteNonQuery();
            myConn.Close();

            CarregarUtilizadores();
            MostrarMensagem("Utilizador eliminado com sucesso.", true);
        }

        // GUARDAR EDIÇÃO

        protected void btn_guardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            if (ModoCriacao)
            {
                CriarUtilizador();
                return;
            }

            int id_utilizador = Convert.ToInt32(hf_id_utilizador.Value);

            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand myCommand = new SqlCommand("sp_editar_utilizador", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@id_utilizador", id_utilizador);
            myCommand.Parameters.AddWithValue("@utilizador", tb_utilizador.Text.Trim());
            myCommand.Parameters.AddWithValue("@email", tb_email.Text.Trim());
            myCommand.Parameters.AddWithValue("@id_perfil", Convert.ToInt32(ddl_perfil.SelectedValue));
            myCommand.Parameters.AddWithValue("@activo", Convert.ToInt32(ddl_activo.SelectedValue));

            SqlParameter paramRetorno = new SqlParameter("@retorno", SqlDbType.Int);
            paramRetorno.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(paramRetorno);

            myConn.Open();
            myCommand.ExecuteNonQuery();
            myConn.Close();

            int retorno = Convert.ToInt32(myCommand.Parameters["@retorno"].Value);

            if (retorno == -1)
            {
                MostrarMensagem("Este nome de utilizador já existe.", false);
                return;
            }

            if (retorno == 0)
            {
                MostrarMensagem("Este email já está registado noutro utilizador.", false);
                return;
            }

            pnl_editar.Visible = false;
            LimparFormulario();
            CarregarUtilizadores();
            MostrarMensagem("Utilizador actualizado com sucesso.", true);
        }

        private void CriarUtilizador()
        {
            if (string.IsNullOrWhiteSpace(tb_password_inicial.Text))
            {
                MostrarMensagem("Defina a password inicial do novo utilizador.", false);
                return;
            }

            if (tb_password_inicial.Text != tb_password_confirmar.Text)
            {
                MostrarMensagem("A confirmação da password não coincide.", false);
                return;
            }

            string salt = Seguranca.GerarSalt();
            string hash = Seguranca.HashPassword(tb_password_inicial.Text, salt);

            SqlConnection myConn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString);

            SqlCommand myCommand = new SqlCommand("sp_inserir_utilizador", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@utilizador", tb_utilizador.Text.Trim());
            myCommand.Parameters.AddWithValue("@email", tb_email.Text.Trim());
            myCommand.Parameters.AddWithValue("@hash", hash);
            myCommand.Parameters.AddWithValue("@salt", salt);
            myCommand.Parameters.AddWithValue("@id_perfil", Convert.ToInt32(ddl_perfil.SelectedValue));

            SqlParameter paramRetorno = new SqlParameter("@retorno", SqlDbType.Int);
            paramRetorno.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(paramRetorno);

            myConn.Open();
            myCommand.ExecuteNonQuery();
            myConn.Close();

            int retorno = Convert.ToInt32(myCommand.Parameters["@retorno"].Value);

            if (retorno == -1)
            {
                MostrarMensagem("Este nome de utilizador já existe.", false);
                return;
            }

            if (retorno == 0)
            {
                MostrarMensagem("Este email já está registado noutro utilizador.", false);
                return;
            }

            pnl_editar.Visible = false;
            LimparFormulario();
            CarregarUtilizadores();
            MostrarMensagem("Utilizador criado com sucesso.", true);
        }

        // CANCELAR

        protected void btn_cancelar_Click(object sender, EventArgs e)
        {
            LimparFormulario();
        }

        protected void btn_novo_utilizador_Click(object sender, EventArgs e)
        {
            LimparFormulario();
            hf_id_utilizador.Value = "0";
            lit_titulo_form.Text = "Novo Utilizador";
            pnl_password.Visible = true;
            pnl_activo.Visible = false;
            pnl_editar.Visible = true;
        }

        private void LimparFormulario()
        {
            hf_id_utilizador.Value = "0";
            tb_utilizador.Text = "";
            tb_email.Text = "";
            tb_password_inicial.Text = "";
            tb_password_confirmar.Text = "";
            if (ddl_perfil.Items.Count > 0)
                ddl_perfil.SelectedIndex = 0;
            ddl_activo.SelectedValue = "1";
            lit_titulo_form.Text = "Editar Utilizador";
            pnl_password.Visible = false;
            pnl_activo.Visible = true;
            pnl_editar.Visible = false;
            lbl_sucesso.Visible = false;
            lbl_erro.Visible = false;
        }

        private void MostrarToast(string mensagem, bool sucesso = true)
        {
            string tipo = sucesso ? "success" : "error";
            string script = $"toastr.{tipo}('{mensagem.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "toast", script, true);
        }

        private void MostrarMensagem(string texto, bool sucesso)
        {
            MostrarToast(texto, sucesso);
            lbl_sucesso.Visible = false;
            lbl_erro.Visible = false;
        }
    }
}
