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
    public partial class gerir_experiencias : Page
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
                cb_destaque.Checked = false;
                ConfigurarPermissoesPorPerfil();
                CarregarExperiencias();
            }

            ConfigurarPermissoesPorPerfil();
        }

        private void ConfigurarPermissoesPorPerfil()
        {
            btn_nova_experiencia.Visible = !IsGestor;
            if (gv_experiencias.Columns.Count > 7)
                gv_experiencias.Columns[7].Visible = !IsGestor;
        }

        private void CarregarExperiencias()
        {
            string sortBy = ViewState["SortExpression"]?.ToString() ?? "nome";
            string sortDir = ViewState["SortDirection"]?.ToString() ?? "ASC";
            CarregarExperienciasOrdenadas(sortBy, sortDir);
        }

        private void CarregarExperienciasOrdenadas(string sortBy, string sortDir)
        {
            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_listar_experiencias_backoffice", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                DataView dv = dt.DefaultView;
                dv.Sort = sortBy + " " + sortDir;
                DataTable dtOrdenada = dv.ToTable();

                int totalRegistos = dtOrdenada.Rows.Count;
                lbl_total.Text = totalRegistos + " experiencia(s)";

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
                    gv_experiencias.DataSource = paginada;
                }
                else
                {
                    lbl_pagina.Text = "Página 1 de 1";
                    btn_anterior.Visible = false;
                    btn_seguinte.Visible = false;
                    gv_experiencias.DataSource = dtOrdenada;
                }
                gv_experiencias.DataBind();
            }
        }

        protected void gv_experiencias_Sorting(object sender, GridViewSortEventArgs e)
        {
            string sortDirection = "ASC";

            if (ViewState["SortExpression"] != null && ViewState["SortExpression"].ToString() == e.SortExpression)
            {
                sortDirection = ViewState["SortDirection"]?.ToString() == "ASC" ? "DESC" : "ASC";
            }

            ViewState["SortExpression"] = e.SortExpression;
            ViewState["SortDirection"] = sortDirection;
            PaginaActual = 1;

            CarregarExperienciasOrdenadas(e.SortExpression, sortDirection);
        }

        protected void btn_guardar_Click(object sender, EventArgs e)
        {
            if (IsGestor) { Response.Redirect("~/Backoffice/gerir_experiencias.aspx"); return; }

            if (string.IsNullOrEmpty(tb_nome.Text.Trim()))
            {
                MostrarToast("O nome é obrigatório.", false);
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;
            int idExp = int.Parse(hf_id_experiencia.Value);
            bool isNovo = (idExp == 0);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string spNome = isNovo ? "sp_inserir_experiencia" : "sp_actualizar_experiencia";
                SqlCommand cmd = new SqlCommand(spNome, con);
                cmd.CommandType = CommandType.StoredProcedure;

                if (!isNovo)
                    cmd.Parameters.AddWithValue("@id_experiencia", idExp);

                cmd.Parameters.AddWithValue("@nome", tb_nome.Text.Trim());
                cmd.Parameters.AddWithValue("@descricao", tb_descricao.Text.Trim());
                cmd.Parameters.AddWithValue("@tipo", ddl_tipo.SelectedValue);

                SqlParameter pPreco = cmd.Parameters.Add("@preco_por_pessoa", SqlDbType.Decimal);
                pPreco.Precision = 8; pPreco.Scale = 2;
                pPreco.Value = string.IsNullOrEmpty(tb_preco.Text) ? 0m : ParseDecimal(tb_preco.Text);

                SqlParameter pDuracao = cmd.Parameters.Add("@duracao_horas", SqlDbType.Decimal);
                pDuracao.Precision = 4; pDuracao.Scale = 1;
                pDuracao.Value = string.IsNullOrEmpty(tb_duracao.Text) ? 0m : ParseDecimal(tb_duracao.Text);

                cmd.Parameters.AddWithValue("@capacidade_max", string.IsNullOrEmpty(tb_capacidade.Text) ? 0 : int.Parse(tb_capacidade.Text));
                cmd.Parameters.AddWithValue("@imagem_url", tb_imagem_url.Text.Trim());
                cmd.Parameters.AddWithValue("@activo", cb_activo.Checked);
                cmd.Parameters.AddWithValue("@destaque", cb_destaque.Checked);

                SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int);
                outRetorno.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outRetorno);

                object idUtilizador = Session["utilizador_id"] != null
                    ? (object)Convert.ToInt32(Session["utilizador_id"])
                    : DBNull.Value;
                cmd.Parameters.AddWithValue("@id_utilizador", idUtilizador);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            if (!isNovo)
            {
                // Ao actualizar, reabrir modal para o utilizador poder gerir sessões
                int idExpActualizado = int.Parse(hf_id_experiencia.Value);
                PaginaActual = 1;
                CarregarExperiencias();
                PreencherFormularioEdicao(idExpActualizado);
                CarregarDisponibilidades(idExpActualizado);
                MostrarToast("Experiência actualizada com sucesso.", true);
            }
            else
            {
                LimparFormulario();
                PaginaActual = 1;
                CarregarExperiencias();
                MostrarToast("Experiência inserida com sucesso.", true);
            }
        }

        protected void gv_experiencias_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (IsGestor)
                return;

            if (e.CommandName == "EditarExp")
            {
                int idExp = int.Parse(e.CommandArgument.ToString());
                PreencherFormularioEdicao(idExp);
                CarregarDisponibilidades(idExp);
            }
            else if (e.CommandName == "ToggleActivo")
            {
                string[] partes = e.CommandArgument.ToString().Split('|');
                int idExp = int.Parse(partes[0]);
                bool activoActual = partes[1] == "True";
                ToggleActivo(idExp, !activoActual);
                CarregarExperiencias();
            }
        }

        // DISPONIBILIDADES

        private void CarregarDisponibilidades(int idExp)
        {
            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_listar_disponibilidades_backoffice", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_experiencia", idExp);

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gv_disponibilidades.DataSource = dt;
                gv_disponibilidades.DataBind();
            }

            pnl_disponibilidades.Visible = true;
            pnl_aviso_nova.Visible = false;
        }

        protected void btn_adicionar_sessao_Click(object sender, EventArgs e)
        {
            int idExp = int.Parse(hf_id_experiencia.Value);
            if (idExp == 0)
            {
                MostrarToast("Guarde primeiro a experiência.", false);
                return;
            }

            if (string.IsNullOrEmpty(tb_disp_data.Text) || string.IsNullOrEmpty(tb_disp_vagas.Text))
            {
                MostrarToast("Data/hora e vagas são obrigatórios.", false);
                return;
            }

            DateTime dataHora;
            if (!DateTime.TryParseExact(tb_disp_data.Text.Trim(), "dd/MM/yyyy HH:mm",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out dataHora))
            {
                MostrarToast("Formato de data inválido. Use dd/MM/yyyy HH:mm", false);
                return;
            }

            int vagas = int.Parse(tb_disp_vagas.Text);

            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_inserir_disponibilidade", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_experiencia", idExp);
                cmd.Parameters.AddWithValue("@data_hora", dataHora);
                cmd.Parameters.AddWithValue("@vagas_total", vagas);

                SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int);
                outRetorno.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outRetorno);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            tb_disp_data.Text = "";
            tb_disp_vagas.Text = "";
            CarregarDisponibilidades(idExp);
            MostrarToast("Sessão adicionada com sucesso.", true);
        }

        protected void gv_disponibilidades_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RemoverSessao")
            {
                int idDisp = int.Parse(e.CommandArgument.ToString());
                int idExp = int.Parse(hf_id_experiencia.Value);

                string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    SqlCommand cmd = new SqlCommand("sp_desactivar_disponibilidade", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_disponibilidade", idDisp);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                CarregarDisponibilidades(idExp);
                MostrarToast("Sessão removida.", true);
            }
        }

        // FIM DISPONIBILIDADES

        private void PreencherFormularioEdicao(int idExp)
        {
            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_obter_detalhe_experiencia", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_experiencia", idExp);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    hf_id_experiencia.Value = idExp.ToString();
                    tb_nome.Text = dr["nome"].ToString();
                    tb_descricao.Text = dr["descricao"].ToString();
                    ddl_tipo.SelectedValue = dr["tipo"].ToString();
                    tb_preco.Text = dr["preco_por_pessoa"].ToString();
                    tb_duracao.Text = dr["duracao_horas"].ToString();
                    tb_capacidade.Text = dr["capacidade_max"].ToString();
                    tb_imagem_url.Text = dr["imagem_url"].ToString();
                    cb_activo.Checked = Convert.ToBoolean(dr["activo"]);
                    cb_destaque.Checked = Convert.ToBoolean(dr["destaque"]);

                    lit_ultima_actualizacao_experiencia.Text = ObterTextoAuditoria(
                        dr["ultima_actualizacao_por"],
                        dr["ultima_actualizacao_em"]);

                    lit_nome_exp_disp.Text = dr["nome"].ToString();
                    lit_titulo_form.Text = "Editar Experiência";
                    btn_cancelar_edicao.Visible = true;
                }
            }
            AbrirModalNaTabSessoes();
        }

        private void ToggleActivo(int idExp, bool novoEstado)
        {
            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_toggle_experiencia", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_experiencia", idExp);
                cmd.Parameters.AddWithValue("@activo", novoEstado);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void LimparFormulario()
        {
            hf_id_experiencia.Value = "0";
            tb_nome.Text = "";
            tb_descricao.Text = "";
            ddl_tipo.SelectedIndex = 0;
            tb_preco.Text = "";
            tb_duracao.Text = "";
            tb_capacidade.Text = "";
            tb_imagem_url.Text = "";
            cb_activo.Checked = true;
            lit_titulo_form.Text = "Nova Experiência";
            btn_cancelar_edicao.Visible = false;
            cb_destaque.Checked = false;
            pnl_disponibilidades.Visible = false;
            pnl_aviso_nova.Visible = true;
            lit_titulo_form.Text = "Nova Experiência";
        }

        protected void btn_cancelar_edicao_Click(object sender, EventArgs e)
        {
            LimparFormulario();
            // fechar modal tratado pelo data-dismiss do botão
        }

        protected void btn_nova_experiencia_Click(object sender, EventArgs e)
        {
            LimparFormulario();
            AbrirModal("#tab-dados");
        }

        private static decimal ParseDecimal(string valor)
        {
            if (string.IsNullOrEmpty(valor)) return 0m;
            valor = valor.Trim().Replace(',', '.');
            if (decimal.TryParse(valor, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                return result;
            return 0m;
        }

        private void AbrirModal(string tabId = "#tab-dados")
        {
            string script = string.Format(
                "$('#modalExperiencia').modal('show'); " +
                "$('#{0}').tab('show');",
                tabId.TrimStart('#'));
            ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal", script, true);
        }

        private void AbrirModalNaTabSessoes()
        {
            string script =
                "$('#modalExperiencia').modal('show'); " +
                "$('#tab-sessoes-link').tab('show');";
            ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal", script, true);
        }

        private void MostrarToast(string mensagem, bool sucesso = true)
        {
            string tipo = sucesso ? "success" : "error";
            string script = $"toastr.{tipo}('{mensagem.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "toast", script, true);
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

        // PAGINAÇÃO

        protected void btn_anterior_Click(object sender, EventArgs e)
        {
            if (PaginaActual > 1)
            {
                PaginaActual--;
                CarregarExperiencias();
            }
        }

        protected void btn_seguinte_Click(object sender, EventArgs e)
        {
            PaginaActual++;
            CarregarExperiencias();
        }

        protected void ddl_por_pagina_Changed(object sender, EventArgs e)
        {
            PaginaActual = 1;
            CarregarExperiencias();
        }
    }
}
