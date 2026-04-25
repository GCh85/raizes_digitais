using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RaizesDigitais.Backoffice
{
    public partial class gerir_clientes : System.Web.UI.Page
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

        string cs = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["perfil"] == null)
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                ConfigurarPermissoesPorPerfil();
                CarregarKPIs();
                CarregarClientes(null);
            }

            ConfigurarPermissoesPorPerfil();
        }

        // ═══════════════════════════════════════════════════════════════
        // NOVO: Carregar KPIs do topo
        // ═══════════════════════════════════════════════════════════════
        private void CarregarKPIs()
        {
            SqlConnection myConn = new SqlConnection(cs);
            SqlCommand myCommand = new SqlCommand(@"
                SELECT
                    COUNT(*) AS total_clientes,
                    SUM(CASE WHEN segmento_crm = 'VIP' THEN 1 ELSE 0 END) AS clientes_vip,
                    SUM(CASE WHEN segmento_crm = 'B2B' THEN 1 ELSE 0 END) AS clientes_b2b,
                    SUM(CASE WHEN MONTH(data_registo) = MONTH(GETDATE())
                              AND YEAR(data_registo) = YEAR(GETDATE()) THEN 1 ELSE 0 END) AS novos_mes
                FROM clientes", myConn);

            myConn.Open();
            SqlDataReader dr = myCommand.ExecuteReader();

            if (dr.Read())
            {
                lit_total_clientes.Text = dr["total_clientes"].ToString();
                lit_clientes_vip.Text = dr["clientes_vip"].ToString();
                lit_clientes_b2b.Text = dr["clientes_b2b"].ToString();
                lit_clientes_novos_mes.Text = dr["novos_mes"].ToString();
            }
            myConn.Close();
        }

        private void ConfigurarPermissoesPorPerfil()
        {
            btn_exportar_emails.Visible = !IsGestor;
            btn_guardar.Visible = !IsGestor;
            if (IsGestor)
                pnl_detalhe.Visible = false;
            if (gv_clientes.Columns.Count > 7)
                gv_clientes.Columns[7].Visible = !IsGestor;
        }

        private void CarregarClientes(string pesquisa)
        {
            string sortBy = ViewState["SortExpression"]?.ToString() ?? "nome";
            string sortDir = ViewState["SortDirection"]?.ToString() ?? "ASC";
            CarregarClientesOrdenados(pesquisa, ObterSegmentoFiltro(), sortBy, sortDir);
        }

        private void CarregarClientesOrdenados(string pesquisa, string segmento, string sortBy, string sortDir)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_listar_clientes", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pesquisa",
                    string.IsNullOrEmpty(pesquisa) ? (object)DBNull.Value : pesquisa);
                cmd.Parameters.AddWithValue("@segmento",
                    string.IsNullOrEmpty(segmento) ? (object)DBNull.Value : segmento);

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                DataView dv = dt.DefaultView;
                dv.Sort = sortBy + " " + sortDir;

                AplicarPaginacao(dv.ToTable(), gv_clientes, "cliente(s)");
                gv_clientes.DataBind();

                // Controlar visibilidade da coluna de detalhe após DataBind
                if (gv_clientes.Columns.Count > 7)
                    gv_clientes.Columns[7].Visible = !IsGestor;

                // Actualizar badge de total
                lit_total_lista.Text = dt.Rows.Count.ToString();
            }
        }

        private void AplicarPaginacao(DataTable dt, GridView gv, string entidade)
        {
            int totalRegistos = dt.Rows.Count;
            lbl_total.Text = totalRegistos + " " + entidade;

            if (PorPagina > 0 && totalRegistos > 0)
            {
                int totalPaginas = (int)Math.Ceiling((double)totalRegistos / PorPagina);
                if (PaginaActual > totalPaginas)
                    PaginaActual = Math.Max(1, totalPaginas);

                lbl_pagina.Text = "Página " + PaginaActual + " de " + (totalPaginas == 0 ? 1 : totalPaginas);
                btn_anterior.Visible = PaginaActual > 1;
                btn_seguinte.Visible = PaginaActual < totalPaginas;

                var pageRows = dt.AsEnumerable()
                    .Skip((PaginaActual - 1) * PorPagina)
                    .Take(PorPagina)
                    .ToList();
                DataTable paginada = dt.Clone();
                foreach (var row in pageRows)
                    paginada.ImportRow(row);
                gv.DataSource = paginada;
            }
            else
            {
                lbl_pagina.Text = "Página 1 de 1";
                btn_anterior.Visible = false;
                btn_seguinte.Visible = false;
                gv.DataSource = dt;
            }
        }

        protected void btn_pesquisar_Click(object sender, EventArgs e)
        {
            CarregarComFiltros();
        }

        protected void btn_limpar_Click(object sender, EventArgs e)
        {
            tb_pesquisa.Text = "";
            ddl_segmento_filtro.SelectedValue = "";
            pnl_emails_export.Visible = false;
            PaginaActual = 1;
            CarregarClientes(null);
        }

        private void CarregarComFiltros()
        {
            string pesquisa = string.IsNullOrEmpty(tb_pesquisa.Text.Trim())
                ? null
                : tb_pesquisa.Text.Trim();
            string sortBy = ViewState["SortExpression"]?.ToString() ?? "nome";
            string sortDir = ViewState["SortDirection"]?.ToString() ?? "ASC";
            CarregarClientesOrdenados(pesquisa, ObterSegmentoFiltro(), sortBy, sortDir);
        }

        protected void gv_clientes_Sorting(object sender, GridViewSortEventArgs e)
        {
            string sortDirection = "ASC";

            if (ViewState["SortExpression"] != null && ViewState["SortExpression"].ToString() == e.SortExpression)
            {
                sortDirection = ViewState["SortDirection"]?.ToString() == "ASC" ? "DESC" : "ASC";
            }

            ViewState["SortExpression"] = e.SortExpression;
            ViewState["SortDirection"] = sortDirection;
            PaginaActual = 1;

            CarregarClientesOrdenados(tb_pesquisa.Text.Trim(), ObterSegmentoFiltro(), e.SortExpression, sortDirection);
        }

        protected void gv_clientes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (IsGestor)
                return;

            if (e.CommandName == "VerFicha")
            {
                int idCliente = int.Parse(e.CommandArgument.ToString());
                ViewState["id_cliente_detalhe"] = idCliente;
                CarregarDetalhe(idCliente);
            }
        }

        protected void btn_exportar_emails_Click(object sender, EventArgs e)
        {
            if (IsGestor)
            {
                Response.Redirect("~/Backoffice/gerir_clientes.aspx");
                return;
            }

            string segmento = string.IsNullOrEmpty(ddl_segmento_filtro.SelectedValue)
                ? null
                : ddl_segmento_filtro.SelectedValue;

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_obter_emails_por_segmento", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@segmento",
                    string.IsNullOrEmpty(segmento) ? (object)DBNull.Value : segmento);

                con.Open();
                var emails = new StringBuilder();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        if (emails.Length > 0) emails.Append(", ");
                        emails.Append(dr["email"].ToString());
                    }
                }

                if (emails.Length > 0)
                {
                    tb_emails_export.Text = emails.ToString();
                    pnl_emails_export.Visible = true;
                }
                else
                {
                    MostrarToast("Nenhum email encontrado.", false);
                    pnl_emails_export.Visible = false;
                }
            }
        }

        private void CarregarDetalhe(int idCliente)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_obter_cliente_detalhe", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_cliente", idCliente);

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                // RESULTSET 0: Perfil
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    lit_nome.Text = dr["nome"].ToString();
                    tb_nome.Text = dr["nome"].ToString();
                    lit_email.Text = dr["email"].ToString();
                    tb_telefone.Text = dr["telefone"].ToString();
                    lit_data_registo.Text = Convert.ToDateTime(dr["data_registo"]).ToString("dd/MM/yyyy");
                    lit_segmento_actual.Text = dr["segmento_crm"].ToString();
                    ddl_segmento.SelectedValue = dr["segmento_crm"].ToString();
                    tb_alergias.Text = dr["notas_alergias"] == DBNull.Value ? "" : dr["notas_alergias"].ToString();
                    tb_preferencias.Text = dr["preferencias_vinho"] == DBNull.Value ? "" : dr["preferencias_vinho"].ToString();

                    // NOVO: notas_backoffice (verificar se coluna existe)
                    try
                    {
                        tb_notas_backoffice.Text = dr["notas_backoffice"] == DBNull.Value ? "" : dr["notas_backoffice"].ToString();
                    }
                    catch { /* coluna ainda não existe */ }

                    lit_ultima_actualizacao.Text = ObterTextoAuditoria(
                        dr["ultima_actualizacao_por"], dr["ultima_actualizacao_em"]);
                }

                // RESULTSET 1: Reservas
                if (ds.Tables.Count > 1)
                {
                    gv_reservas.DataSource = ds.Tables[1];
                    gv_reservas.DataBind();
                    lit_total_reservas_cliente.Text = ds.Tables[1].Rows.Count.ToString();
                }

                // RESULTSET 2: Favoritos
                if (ds.Tables.Count > 2)
                {
                    gv_favoritos.DataSource = ds.Tables[2];
                    gv_favoritos.DataBind();
                    lbl_sem_favoritos.Visible = (ds.Tables[2].Rows.Count == 0);
                }
            }

            // Pontos de fidelizacao
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_obter_pontos_cliente", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_cliente", idCliente);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                    lit_total_pontos.Text = dr["total_pontos"].ToString();
                dr.Close();
            }

            // NOVO: Carregar timeline de actividade
            CarregarTimeline(idCliente);

            AbrirModal();
        }

        // ═══════════════════════════════════════════════════════════════
        // NOVO: Carregar timeline de actividade (CSS puro)
        // ═══════════════════════════════════════════════════════════════
        private void CarregarTimeline(int idCliente)
        {
            DataTable dtTimeline = new DataTable();
            dtTimeline.Columns.Add("data_formatada", typeof(string));
            dtTimeline.Columns.Add("cor_data", typeof(string));
            dtTimeline.Columns.Add("icone", typeof(string));
            dtTimeline.Columns.Add("cor_icone", typeof(string));
            dtTimeline.Columns.Add("hora", typeof(string));
            dtTimeline.Columns.Add("titulo", typeof(string));
            dtTimeline.Columns.Add("descricao", typeof(string));

            // Obter últimas 10 reservas para timeline
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT TOP 10
                        r.id_reserva,
                        r.num_reserva,
                        e.nome AS experiencia,
                        d.data_hora,
                        r.estado,
                        r.num_pessoas
                    FROM reservas r
                    JOIN disponibilidade d ON r.id_disponibilidade = d.id_disponibilidade
                    JOIN experiencias e ON d.id_experiencia = e.id_experiencia
                    WHERE r.id_cliente = @id_cliente
                    ORDER BY d.data_hora DESC", con);

                cmd.Parameters.AddWithValue("@id_cliente", idCliente);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                bool temRegistos = false;
                while (dr.Read())
                {
                    temRegistos = true;
                    DateTime dataHora = Convert.ToDateTime(dr["data_hora"]);
                    string estado = dr["estado"].ToString();

                    DataRow row = dtTimeline.NewRow();
                    row["data_formatada"] = dataHora.ToString("dd MMM yyyy");
                    row["cor_data"] = "success";
                    row["icone"] = GetIconeTimeline(estado);
                    row["cor_icone"] = GetCorIconeTimeline(estado);
                    row["hora"] = dataHora.ToString("HH:mm");
                    row["titulo"] = $"Reserva {dr["num_reserva"]} — {estado}";
                    row["descricao"] = $"{dr["experiencia"]} ({dr["num_pessoas"]} pessoas)";

                    dtTimeline.Rows.Add(row);
                }

                lbl_sem_actividade.Visible = !temRegistos;
            }

            rpt_timeline.DataSource = dtTimeline;
            rpt_timeline.DataBind();
        }

        private string GetIconeTimeline(string estado)
        {
            switch (estado)
            {
                case "Confirmada": return "fa-calendar-check";
                case "Pendente": return "fa-clock";
                case "Cancelada": return "fa-times-circle";
                case "Concluida": return "fa-check-circle";
                default: return "fa-calendar";
            }
        }

        private string GetCorIconeTimeline(string estado)
        {
            switch (estado)
            {
                case "Confirmada": return "success";
                case "Pendente": return "warning";
                case "Cancelada": return "danger";
                case "Concluida": return "info";
                default: return "secondary";
            }
        }

        protected void btn_guardar_Click(object sender, EventArgs e)
        {
            if (IsGestor)
            {
                Response.Redirect("~/Backoffice/gerir_clientes.aspx");
                return;
            }

            int idCliente = (int)ViewState["id_cliente_detalhe"];
            int idUtilizador = Session["utilizador_id"] != null
                ? Convert.ToInt32(Session["utilizador_id"])
                : 0;

            // Actualizar todos os campos via SP (incluindo nome)
            SqlConnection myConn = new SqlConnection(cs);
            SqlCommand myCommand = new SqlCommand("sp_actualizar_cliente", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@id_cliente", idCliente);
            myCommand.Parameters.AddWithValue("@nome", tb_nome.Text.Trim());
            myCommand.Parameters.AddWithValue("@telefone", tb_telefone.Text.Trim());
            myCommand.Parameters.AddWithValue("@segmento_crm", ddl_segmento.SelectedValue);
            myCommand.Parameters.AddWithValue("@notas_alergias", tb_alergias.Text.Trim());
            myCommand.Parameters.AddWithValue("@preferencias_vinho", tb_preferencias.Text.Trim());
            myCommand.Parameters.AddWithValue("@notas_backoffice", tb_notas_backoffice.Text.Trim());
            myCommand.Parameters.AddWithValue("@id_utilizador", idUtilizador);

            SqlParameter paramRetorno = new SqlParameter("@retorno", SqlDbType.Int);
            paramRetorno.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(paramRetorno);

            myConn.Open();
            myCommand.ExecuteNonQuery();
            myConn.Close();

            int retorno = Convert.ToInt32(paramRetorno.Value);

            MostrarToast(retorno == 1
                ? "Dados do cliente actualizados com sucesso."
                : "Erro ao actualizar dados do cliente.", retorno == 1);

            CarregarKPIs();
            CarregarClientes(tb_pesquisa.Text.Trim());
            CarregarDetalhe(idCliente);
        }

        protected void btn_fechar_detalhe_Click(object sender, EventArgs e)
        {
            // Fechar tratado pelo data-dismiss do modal
        }

        // ═══════════════════════════════════════════════════════════════
        // Helpers para badges
        // ═══════════════════════════════════════════════════════════════
        protected string GetSegmentoBadgeClass(string segmento)
        {
            switch (segmento)
            {
                case "VIP": return "danger";
                case "Regular": return "success";
                case "B2B": return "primary";
                case "Inactivo": return "secondary";
                default: return "info";
            }
        }

        protected string GetEstadoBadgeClass(string estado)
        {
            switch (estado)
            {
                case "Pendente": return "warning";
                case "Confirmada": return "success";
                case "Cancelada": return "danger";
                case "Concluida": return "secondary";
                default: return "info";
            }
        }

        private string ObterSegmentoFiltro()
        {
            return string.IsNullOrEmpty(ddl_segmento_filtro.SelectedValue)
                ? null
                : ddl_segmento_filtro.SelectedValue;
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
            using (SqlConnection con = new SqlConnection(cs))
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
            return null;
        }

        private void AbrirModal()
        {
            ScriptManager.RegisterStartupScript(
                this, GetType(), "abrirModal",
                "$(document).ready(function(){ $('#modalFichaCliente').modal('show'); });",
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
                CarregarComFiltros();
            }
        }

        protected void btn_seguinte_Click(object sender, EventArgs e)
        {
            PaginaActual++;
            CarregarComFiltros();
        }

        protected void ddl_por_pagina_Changed(object sender, EventArgs e)
        {
            PaginaActual = 1;
            CarregarComFiltros();
        }
    }
}