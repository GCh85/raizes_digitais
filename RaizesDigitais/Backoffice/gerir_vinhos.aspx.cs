using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RaizesDigitais.Backoffice
{
    public partial class gerir_vinhos : Page
    {
        protected bool IsGestor
        {
            get { return string.Equals(Session["perfil"]?.ToString(), "Gestor", StringComparison.OrdinalIgnoreCase); }
        }

        private int PaginaActual { get { return ViewState["pagina"] != null ? (int)ViewState["pagina"] : 1; } set { ViewState["pagina"] = value; } }
        private int PorPagina
        {
            get
            {
                string val = ddl_por_pagina?.SelectedValue ?? "15";
                return val == "0" ? 0 : int.Parse(val);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["perfil"] == null)
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                cb_activo.Checked = true;
                ConfigurarPermissoesPorPerfil();
                CarregarVinhos();
            }

            ConfigurarPermissoesPorPerfil();
        }

        private void ConfigurarPermissoesPorPerfil()
        {
            btn_novo_vinho.Visible = !IsGestor;
            if (gv_vinhos.Columns.Count > 8)
                gv_vinhos.Columns[8].Visible = !IsGestor;
        }

        private void CarregarVinhos()
        {
            string sortBy = ViewState["SortExpression"]?.ToString() ?? "nome";
            string sortDir = ViewState["SortDirection"]?.ToString() ?? "ASC";
            CarregarVinhosOrdenados(sortBy, sortDir);
        }

        private void CarregarVinhosOrdenados(string sortBy, string sortDir)
        {
            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_listar_vinhos", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                DataView dv = dt.DefaultView;
                dv.Sort = sortBy + " " + sortDir;
                DataTable dtOrdenada = dv.ToTable();

                int totalRegistos = dtOrdenada.Rows.Count;
                lbl_total.Text = totalRegistos + " vinho(s)";

                if (PorPagina > 0 && totalRegistos > 0)
                {
                    int totalPaginas = (int)Math.Ceiling((double)totalRegistos / PorPagina);
                    if (PaginaActual > totalPaginas)
                        PaginaActual = Math.Max(1, totalPaginas);

                    lbl_pagina.Text = "Página " + PaginaActual + " de " + (totalPaginas == 0 ? 1 : totalPaginas);
                    btn_anterior.Visible = PaginaActual > 1;
                    btn_seguinte.Visible = PaginaActual < totalPaginas;

                    var pageRows = dtOrdenada.AsEnumerable()
                        .Skip((PaginaActual - 1) * PorPagina)
                        .Take(PorPagina)
                        .ToList();
                    DataTable paginada = dtOrdenada.Clone();
                    foreach (var row in pageRows)
                        paginada.ImportRow(row);
                    gv_vinhos.DataSource = paginada;
                }
                else
                {
                    lbl_pagina.Text = "Página 1 de 1";
                    btn_anterior.Visible = false;
                    btn_seguinte.Visible = false;
                    gv_vinhos.DataSource = dtOrdenada;
                }
                gv_vinhos.DataBind();
            }
        }

        protected void gv_vinhos_Sorting(object sender, GridViewSortEventArgs e)
        {
            string sortDirection = "ASC";

            if (ViewState["SortExpression"] != null && ViewState["SortExpression"].ToString() == e.SortExpression)
            {
                sortDirection = ViewState["SortDirection"]?.ToString() == "ASC" ? "DESC" : "ASC";
            }

            ViewState["SortExpression"] = e.SortExpression;
            ViewState["SortDirection"] = sortDirection;
            PaginaActual = 1;

            CarregarVinhosOrdenados(e.SortExpression, sortDirection);
        }

        // ─────────────────────────────────────────────────────────
        // GUARDAR — insere ou actualiza conforme hf_id_vinho
        // ─────────────────────────────────────────────────────────
        protected void btn_guardar_Click(object sender, EventArgs e)
        {
            if (IsGestor)
            {
                Response.Redirect("~/Backoffice/gerir_vinhos.aspx");
                return;
            }

            if (string.IsNullOrEmpty(tb_nome.Text.Trim()))
            {
                MostrarToast("O nome é obrigatório.", false);
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;
            int idVinho = int.Parse(hf_id_vinho.Value);
            bool isNovo = (idVinho == 0);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string spNome = isNovo ? "sp_inserir_vinho" : "sp_actualizar_vinho";
                SqlCommand cmd = new SqlCommand(spNome, con);
                cmd.CommandType = CommandType.StoredProcedure;

                if (!isNovo)
                    cmd.Parameters.AddWithValue("@id_vinho", idVinho);

                cmd.Parameters.AddWithValue("@nome", tb_nome.Text.Trim());
                cmd.Parameters.AddWithValue("@casta", tb_casta.Text.Trim());
                cmd.Parameters.AddWithValue("@ano", string.IsNullOrEmpty(tb_ano.Text) ? (object)DBNull.Value : int.Parse(tb_ano.Text));
                cmd.Parameters.AddWithValue("@preco", string.IsNullOrEmpty(tb_preco.Text) ? 0m : ParseDecimal(tb_preco.Text));
                cmd.Parameters.AddWithValue("@descricao", tb_descricao.Text.Trim());
                cmd.Parameters.AddWithValue("@stock_actual", string.IsNullOrEmpty(tb_stock_actual.Text) ? 0 : int.Parse(tb_stock_actual.Text));
                cmd.Parameters.AddWithValue("@stock_minimo", string.IsNullOrEmpty(tb_stock_minimo.Text) ? 0 : int.Parse(tb_stock_minimo.Text));
                cmd.Parameters.AddWithValue("@tipo", ddl_tipo.SelectedValue);
                cmd.Parameters.AddWithValue("@activo", cb_activo.Checked);
                cmd.Parameters.AddWithValue("@imagem_url", string.IsNullOrEmpty(tb_imagem_url.Text.Trim()) ? (object)DBNull.Value : tb_imagem_url.Text.Trim());
                cmd.Parameters.AddWithValue("@docura", string.IsNullOrEmpty(tb_docura.Text) ? 0 : int.Parse(tb_docura.Text));
                cmd.Parameters.AddWithValue("@acidez", string.IsNullOrEmpty(tb_acidez.Text) ? 0 : int.Parse(tb_acidez.Text));
                cmd.Parameters.AddWithValue("@corpo", string.IsNullOrEmpty(tb_corpo.Text) ? 0 : int.Parse(tb_corpo.Text));
                cmd.Parameters.AddWithValue("@harmonizacao", string.IsNullOrEmpty(tb_harmonizacao.Text.Trim()) ? (object)DBNull.Value : tb_harmonizacao.Text.Trim());

                SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int);
                outRetorno.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outRetorno);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            LimparFormulario();
            CarregarVinhos();
            MostrarToast(isNovo ? "Vinho inserido com sucesso." : "Vinho actualizado com sucesso.", true);
        }

        // ─────────────────────────────────────────────────────────
        // COMANDOS DO GRIDVIEW — Editar e Toggle Activo
        // ─────────────────────────────────────────────────────────
        protected void gv_vinhos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (IsGestor)
                return;

            if (e.CommandName == "EditarVinho")
            {
                int idVinho = int.Parse(e.CommandArgument.ToString());
                PreencherFormularioEdicao(idVinho);
            }
            else if (e.CommandName == "ToggleActivo")
            {
                string[] partes = e.CommandArgument.ToString().Split('|');
                int idVinho = int.Parse(partes[0]);
                bool activoActual = partes[1] == "True";
                ToggleActivo(idVinho, !activoActual);
                PaginaActual = 1;
                CarregarVinhos();
            }
        }

        // ─────────────────────────────────────────────────────────
        // FORMATAÇÃO CONDICIONAL — linha vermelha se stock baixo
        // ─────────────────────────────────────────────────────────
        protected void gv_vinhos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            DataRowView row = (DataRowView)e.Row.DataItem;
            int stockActual = Convert.ToInt32(row["stock_actual"]);
            int stockMinimo = Convert.ToInt32(row["stock_minimo"]);

            if (stockActual <= stockMinimo)
            {
                e.Row.CssClass = "table-danger";
            }
        }

        // ─────────────────────────────────────────────────────────
        // AUXILIARES
        // ─────────────────────────────────────────────────────────
        protected void btn_novo_vinho_Click(object sender, EventArgs e)
        {
            if (IsGestor) { Response.Redirect("~/Backoffice/gerir_vinhos.aspx"); return; }
            LimparFormulario();
            AbrirModal();
        }

        private void PreencherFormularioEdicao(int idVinho)
        {
            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM vinhos WHERE id_vinho = @id", con);
                cmd.Parameters.AddWithValue("@id", idVinho);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    hf_id_vinho.Value = idVinho.ToString();
                    tb_nome.Text = dr["nome"].ToString();
                    tb_casta.Text = dr["casta"].ToString();
                    tb_ano.Text = dr["ano"].ToString();
                    tb_preco.Text = dr["preco"].ToString();
                    tb_descricao.Text = dr["descricao"].ToString();
                    tb_stock_actual.Text = dr["stock_actual"].ToString();
                    tb_stock_minimo.Text = dr["stock_minimo"].ToString();
                    ddl_tipo.SelectedValue = dr["tipo"].ToString();
                    cb_activo.Checked = Convert.ToBoolean(dr["activo"]);
                    tb_imagem_url.Text = dr["imagem_url"] == DBNull.Value ? "" : dr["imagem_url"].ToString();
                    tb_harmonizacao.Text = dr["harmonizacao"] == DBNull.Value ? "" : dr["harmonizacao"].ToString();
                    tb_docura.Text = dr["docura"] != DBNull.Value ? dr["docura"].ToString() : "0";
                    tb_acidez.Text = dr["acidez"] != DBNull.Value ? dr["acidez"].ToString() : "0";
                    tb_corpo.Text = dr["corpo"] != DBNull.Value ? dr["corpo"].ToString() : "0";

                    // Última actualização
                    lit_ultima_actualizacao_vinho.Text = ObterTextoAuditoria(
                        dr["ultima_actualizacao_por"],
                        dr["ultima_actualizacao_em"]);

                    lit_titulo_form.Text = "Editar Vinho";
                }
            }

            AbrirModal();
        }


        private void ToggleActivo(int idVinho, bool novoEstado)
        {
            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_toggle_vinho", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_vinho", idVinho);
                cmd.Parameters.AddWithValue("@activo", novoEstado);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void LimparFormulario()
        {
            hf_id_vinho.Value = "0";
            tb_nome.Text = "";
            tb_casta.Text = "";
            tb_ano.Text = "";
            tb_preco.Text = "";
            tb_descricao.Text = "";
            tb_stock_actual.Text = "";
            tb_stock_minimo.Text = "";
            tb_imagem_url.Text = "";
            tb_harmonizacao.Text = "";
            tb_docura.Text = "0";
            tb_acidez.Text = "0";
            tb_corpo.Text = "0";
            ddl_tipo.SelectedIndex = 0;
            cb_activo.Checked = true;
            lit_titulo_form.Text = "Novo Vinho";
            lit_ultima_actualizacao_vinho.Text = "Por actualizar";
        }

        protected void btn_fechar_modal_Click(object sender, EventArgs e)
        {
            LimparFormulario();
        }

        private void AbrirModal()
        {
            ScriptManager.RegisterStartupScript(
                this, GetType(), "abrirModalVinho",
                "$('#modalVinho').modal('show');",
                true);
        }

        private void MostrarToast(string mensagem, bool sucesso = true)
        {
            string tipo = sucesso ? "success" : "error";
            string script = $"toastr.{tipo}('{mensagem.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "toast", script, true);
        }

        // PAGINAÇÃO

        protected void btn_anterior_Click(object sender, EventArgs e)
        {
            if (PaginaActual > 1)
            {
                PaginaActual--;
                CarregarVinhos();
            }
        }

        protected void btn_seguinte_Click(object sender, EventArgs e)
        {
            PaginaActual++;
            CarregarVinhos();
        }

        protected void ddl_por_pagina_Changed(object sender, EventArgs e)
        {
            PaginaActual = 1;
            CarregarVinhos();
        }

        // Aceita tanto . como , separador decimal
        private static decimal ParseDecimal(string valor)
        {
            if (string.IsNullOrEmpty(valor)) return 0m;
            valor = valor.Replace(',', '.');
            if (decimal.TryParse(valor, out decimal result))
                return result;
            return 0m;
        }

        private string ObterTextoAuditoria(object idUtilizador, object em)
        {
            if (idUtilizador == DBNull.Value || em == DBNull.Value)
                return "Por actualizar";

            int idUtil = Convert.ToInt32(idUtilizador);
            DateTime data = Convert.ToDateTime(em);

            string nome = ObterNomeUtilizador(idUtil);
            return string.IsNullOrEmpty(nome)
                ? $"{idUtil} — {data:dd/MM/yyyy HH:mm}"
                : $"{nome} — {data:dd/MM/yyyy HH:mm}";
        }

        private string ObterNomeUtilizador(int idUtilizador)
        {
            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_obter_utilizador", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_utilizador", idUtilizador);

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                        return dr["utilizador"].ToString();
                }
            }
            return "";
        }
    }
}
