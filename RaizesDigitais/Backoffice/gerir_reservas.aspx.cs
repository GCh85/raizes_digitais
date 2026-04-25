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
    public partial class gerir_reservas : Page
    {
        protected bool IsGestor
        {
            get { return string.Equals(Session["perfil"]?.ToString(), "Gestor", StringComparison.OrdinalIgnoreCase); }
        }

        private int PaginaActual { get { return ViewState["pagina"] != null ? (int)ViewState["pagina"] : 1; } set { ViewState["pagina"] = value; } }
        private int PorPagina { get { return ddl_por_pagina.SelectedValue == "0" ? 0 : int.Parse(ddl_por_pagina.SelectedValue); } }

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
                DefinirPeriodoPadrao();
                CarregarExperienciasDropdown();
                ProcessarDeepLink();
                CarregarContadoresBadges();
                CarregarReservas(ddl_estado.SelectedValue, tb_data_inicio.Text, tb_data_fim.Text, ddl_experiencia.SelectedValue);
            }

            ConfigurarPermissoesPorPerfil();
        }

        private void CarregarExperienciasDropdown()
        {
            SqlConnection myConn = new SqlConnection(cs);
            SqlCommand myCommand = new SqlCommand("sp_listar_experiencias_backoffice", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;

            myConn.Open();
            SqlDataReader dr = myCommand.ExecuteReader();

            ddl_experiencia.Items.Clear();
            ddl_experiencia.Items.Add(new ListItem("— Todas —", ""));

            while (dr.Read())
            {
                ddl_experiencia.Items.Add(new ListItem(
                    dr["nome"].ToString(),
                    dr["id_experiencia"].ToString()
                ));
            }
            myConn.Close();
        }

        private void ProcessarDeepLink()
        {
            string estadoFiltro = Request.QueryString["estado"];
            if (!string.IsNullOrEmpty(estadoFiltro))
            {
                string[] estadosValidos = { "Pendente", "Confirmada", "Cancelada", "Concluida" };
                if (estadosValidos.Contains(estadoFiltro))
                    ddl_estado.SelectedValue = estadoFiltro;
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // Badges — contam TODAS as reservas, sem filtro de datas
        // para que pendentes futuras não fiquem invisíveis
        // ═══════════════════════════════════════════════════════════════
        private void CarregarContadoresBadges()
        {
            SqlConnection myConn = new SqlConnection(cs);

            SqlCommand myCommand = new SqlCommand(@"
                SELECT
                    COUNT(*) AS total,
                    SUM(CASE WHEN estado = 'Pendente'   THEN 1 ELSE 0 END) AS pendentes,
                    SUM(CASE WHEN estado = 'Confirmada' THEN 1 ELSE 0 END) AS confirmadas,
                    SUM(CASE WHEN estado = 'Cancelada'  THEN 1 ELSE 0 END) AS canceladas,
                    SUM(CASE WHEN estado = 'Concluida'  THEN 1 ELSE 0 END) AS concluidas
                FROM reservas", myConn);

            myConn.Open();
            SqlDataReader dr = myCommand.ExecuteReader();

            if (dr.Read())
            {
                lit_count_todas.Text = dr["total"].ToString();
                lit_count_pendentes.Text = dr["pendentes"].ToString();
                lit_count_confirmadas.Text = dr["confirmadas"].ToString();
                lit_count_canceladas.Text = dr["canceladas"].ToString();
                lit_count_concluidas.Text = dr["concluidas"].ToString();

                int pendentes = Convert.ToInt32(dr["pendentes"]);
                pnl_alerta_pendentes.Visible = pendentes > 0;
                if (pendentes > 0)
                    lit_pendentes_alerta.Text = pendentes.ToString();
            }
            myConn.Close();
        }

        private void ConfigurarPermissoesPorPerfil()
        {
            btn_actualizar_estado.Visible = !IsGestor;
            btn_exportar.Visible = !IsGestor;

            if (IsGestor)
                pnl_detalhe.Visible = false;

            if (gv_reservas.Columns.Count > 7)
                gv_reservas.Columns[7].Visible = !IsGestor;
        }

        // ═══════════════════════════════════════════════════════════════
        // Badges de filtro rápido — passam null nas datas para não
        // limitar o resultado ao período do filtro principal
        // ═══════════════════════════════════════════════════════════════
        protected void badge_todas_Click(object sender, EventArgs e)
        {
            ddl_estado.SelectedIndex = 0;
            PaginaActual = 1;
            CarregarReservas(null, tb_data_inicio.Text, tb_data_fim.Text, ddl_experiencia.SelectedValue);
        }

        protected void badge_pendentes_Click(object sender, EventArgs e)
        {
            ddl_estado.SelectedValue = "Pendente";
            PaginaActual = 1;
            CarregarReservas("Pendente", null, null, ddl_experiencia.SelectedValue);
        }

        protected void badge_confirmadas_Click(object sender, EventArgs e)
        {
            ddl_estado.SelectedValue = "Confirmada";
            PaginaActual = 1;
            CarregarReservas("Confirmada", tb_data_inicio.Text, tb_data_fim.Text, ddl_experiencia.SelectedValue);
        }

        protected void badge_canceladas_Click(object sender, EventArgs e)
        {
            ddl_estado.SelectedValue = "Cancelada";
            PaginaActual = 1;
            CarregarReservas("Cancelada", tb_data_inicio.Text, tb_data_fim.Text, ddl_experiencia.SelectedValue);
        }

        protected void badge_concluidas_Click(object sender, EventArgs e)
        {
            ddl_estado.SelectedValue = "Concluida";
            PaginaActual = 1;
            CarregarReservas("Concluida", tb_data_inicio.Text, tb_data_fim.Text, ddl_experiencia.SelectedValue);
        }

        protected void btn_ver_pendentes_Click(object sender, EventArgs e)
        {
            ddl_estado.SelectedValue = "Pendente";
            PaginaActual = 1;
            // Pendentes: sem filtro de datas para mostrar todas, incluindo futuras
            CarregarReservas("Pendente", null, null, ddl_experiencia.SelectedValue);
        }

        protected void btn_exportar_Click(object sender, EventArgs e)
        {
            DataTable dt = ObterReservasParaExportacao();

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "text/csv";
            Response.AddHeader("content-disposition", "attachment;filename=reservas.csv");
            Response.ContentEncoding = Encoding.UTF8;
            Response.BinaryWrite(Encoding.UTF8.GetPreamble());

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Nº Reserva;Cliente;Experiência;Data Visita;Pessoas;Total;Estado");

            foreach (DataRow row in dt.Rows)
            {
                sb.AppendLine(string.Format("{0};{1};{2};{3};{4};{5};{6}",
                    row["num_reserva"],
                    row["cliente"],
                    row["experiencia"],
                    Convert.ToDateTime(row["data_hora"]).ToString("dd/MM/yyyy HH:mm"),
                    row["num_pessoas"],
                    Convert.ToDecimal(row["preco_total"]).ToString("0.00") + " €",
                    row["estado"]
                ));
            }

            Response.Write(sb.ToString());
            Response.End();
        }

        private DataTable ObterReservasParaExportacao()
        {
            SqlConnection myConn = new SqlConnection(cs);
            SqlCommand myCommand = new SqlCommand("sp_listar_reservas", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;

            myCommand.Parameters.AddWithValue("@estado",
                string.IsNullOrEmpty(ddl_estado.SelectedValue) ? (object)DBNull.Value : ddl_estado.SelectedValue);
            myCommand.Parameters.AddWithValue("@data_inicio",
                string.IsNullOrEmpty(tb_data_inicio.Text) ? (object)DBNull.Value : DateTime.Parse(tb_data_inicio.Text));
            myCommand.Parameters.AddWithValue("@data_fim",
                string.IsNullOrEmpty(tb_data_fim.Text) ? (object)DBNull.Value : DateTime.Parse(tb_data_fim.Text));
            myCommand.Parameters.AddWithValue("@id_experiencia",
                string.IsNullOrEmpty(ddl_experiencia.SelectedValue) ? (object)DBNull.Value : int.Parse(ddl_experiencia.SelectedValue));

            myConn.Open();
            SqlDataAdapter da = new SqlDataAdapter(myCommand);
            DataTable dt = new DataTable();
            da.Fill(dt);
            myConn.Close();

            return dt;
        }

        // ═══════════════════════════════════════════════════════════════
        // Filtros e paginação
        // ═══════════════════════════════════════════════════════════════
        protected void btn_filtrar_Click(object sender, EventArgs e)
        {
            pnl_detalhe.Visible = false;
            PaginaActual = 1;
            CarregarContadoresBadges();
            CarregarReservas(ddl_estado.SelectedValue, tb_data_inicio.Text, tb_data_fim.Text, ddl_experiencia.SelectedValue);
        }

        protected void btn_limpar_Click(object sender, EventArgs e)
        {
            ddl_estado.SelectedIndex = 0;
            ddl_experiencia.SelectedIndex = 0;
            DefinirPeriodoPadrao();
            pnl_detalhe.Visible = false;
            PaginaActual = 1;
            CarregarContadoresBadges();
            CarregarReservas(null, null, null, null);
        }

        protected void btn_anterior_Click(object sender, EventArgs e)
        {
            if (PaginaActual > 1)
            {
                PaginaActual--;
                CarregarReservas(ddl_estado.SelectedValue, tb_data_inicio.Text, tb_data_fim.Text, ddl_experiencia.SelectedValue);
            }
        }

        protected void btn_seguinte_Click(object sender, EventArgs e)
        {
            PaginaActual++;
            CarregarReservas(ddl_estado.SelectedValue, tb_data_inicio.Text, tb_data_fim.Text, ddl_experiencia.SelectedValue);
        }

        protected void ddl_por_pagina_Changed(object sender, EventArgs e)
        {
            PaginaActual = 1;
            CarregarReservas(ddl_estado.SelectedValue, tb_data_inicio.Text, tb_data_fim.Text, ddl_experiencia.SelectedValue);
        }

        // ═══════════════════════════════════════════════════════════════
        // Carregar reservas — o filtro de experiência é agora passado
        // directamente à SP em vez de ser feito em memória
        // ═══════════════════════════════════════════════════════════════
        private void CarregarReservas(string estado, string dataInicio, string dataFim, string idExperiencia)
        {
            string sortBy = ViewState["SortExpression"]?.ToString() ?? "data_reserva";
            string sortDir = ViewState["SortDirection"]?.ToString() ?? "DESC";
            CarregarReservasOrdenadas(estado, dataInicio, dataFim, idExperiencia, sortBy, sortDir);
        }

        private void CarregarReservasOrdenadas(string estado, string dataInicio, string dataFim, string idExperiencia, string sortBy, string sortDir)
        {
            SqlConnection myConn = new SqlConnection(cs);
            SqlCommand myCommand = new SqlCommand("sp_listar_reservas", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;

            myCommand.Parameters.AddWithValue("@estado",
                string.IsNullOrEmpty(estado) ? (object)DBNull.Value : estado);
            myCommand.Parameters.AddWithValue("@data_inicio",
                string.IsNullOrEmpty(dataInicio) ? (object)DBNull.Value : DateTime.Parse(dataInicio));
            myCommand.Parameters.AddWithValue("@data_fim",
                string.IsNullOrEmpty(dataFim) ? (object)DBNull.Value : DateTime.Parse(dataFim));
            myCommand.Parameters.AddWithValue("@id_experiencia",
                string.IsNullOrEmpty(idExperiencia) ? (object)DBNull.Value : int.Parse(idExperiencia));

            myConn.Open();
            SqlDataAdapter da = new SqlDataAdapter(myCommand);
            DataTable dt = new DataTable();
            da.Fill(dt);
            myConn.Close();

            DataView dv = dt.DefaultView;
            dv.Sort = sortBy + " " + sortDir;
            DataTable dtOrdenada = dv.ToTable();

            int totalRegistos = dtOrdenada.Rows.Count;
            lbl_total.Text = totalRegistos + " reserva(s) encontrada(s)";
            lit_total_registos.Text = totalRegistos.ToString();

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
                gv_reservas.DataSource = paginada;
            }
            else
            {
                lbl_pagina.Text = "Página 1 de 1";
                btn_anterior.Visible = false;
                btn_seguinte.Visible = false;
                gv_reservas.DataSource = dtOrdenada;
            }

            gv_reservas.DataBind();

            if (gv_reservas.Columns.Count > 7)
                gv_reservas.Columns[7].Visible = !IsGestor;
        }

        protected void gv_reservas_Sorting(object sender, GridViewSortEventArgs e)
        {
            string sortDirection = "ASC";

            if (ViewState["SortExpression"] != null && ViewState["SortExpression"].ToString() == e.SortExpression)
                sortDirection = ViewState["SortDirection"]?.ToString() == "ASC" ? "DESC" : "ASC";

            ViewState["SortExpression"] = e.SortExpression;
            ViewState["SortDirection"] = sortDirection;
            PaginaActual = 1;

            CarregarReservasOrdenadas(ddl_estado.SelectedValue, tb_data_inicio.Text, tb_data_fim.Text,
                ddl_experiencia.SelectedValue, e.SortExpression, sortDirection);
        }

        protected void gv_reservas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (IsGestor) return;
            if (e.CommandName != "VerDetalhe") return;

            int idReserva = int.Parse(e.CommandArgument.ToString());
            ViewState["id_reserva_detalhe"] = idReserva;
            CarregarDetalhe(idReserva);
        }

        protected void gv_reservas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            DataRowView row = (DataRowView)e.Row.DataItem;
            string estado = row["estado"].ToString();
            Label lblEstado = (Label)e.Row.FindControl("lbl_estado");

            if (lblEstado == null) return;

            switch (estado)
            {
                case "Pendente": lblEstado.CssClass = "badge badge-warning"; break;
                case "Confirmada": lblEstado.CssClass = "badge badge-success"; break;
                case "Cancelada": lblEstado.CssClass = "badge badge-danger"; break;
                case "Concluida": lblEstado.CssClass = "badge badge-secondary"; break;
            }
        }

        private void CarregarDetalhe(int idReserva)
        {
            SqlConnection myConn = new SqlConnection(cs);
            SqlCommand myCommand = new SqlCommand("sp_obter_reserva_detalhe", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@id_reserva", idReserva);

            myConn.Open();
            SqlDataReader dr = myCommand.ExecuteReader();

            if (dr.Read())
            {
                lit_num_reserva.Text = dr["num_reserva"].ToString();
                lit_cliente.Text = dr["cliente"].ToString();
                lit_email.Text = dr["email"].ToString();
                lit_telefone.Text = dr["telefone"].ToString();
                lit_experiencia.Text = dr["experiencia"].ToString();
                lit_data_hora.Text = Convert.ToDateTime(dr["data_hora"]).ToString("dd/MM/yyyy HH:mm");
                lit_num_pessoas.Text = dr["num_pessoas"].ToString();
                lit_preco_total.Text = Convert.ToDecimal(dr["preco_total"]).ToString("0.00") + " €";
                lit_notas.Text = dr["notas"] == DBNull.Value ? "—" : dr["notas"].ToString();
                ddl_novo_estado.SelectedValue = dr["estado"].ToString();

                lit_ultima_actualizacao.Text = ObterTextoAuditoria(
                    dr["ultima_actualizacao_por"],
                    dr["ultima_actualizacao_em"]);

                AbrirModal();
            }
            myConn.Close();
        }

        protected void btn_actualizar_estado_Click(object sender, EventArgs e)
        {
            if (IsGestor)
            {
                Response.Redirect("~/Backoffice/gerir_reservas.aspx");
                return;
            }

            if (ViewState["id_reserva_detalhe"] == null) return;

            int idReserva = (int)ViewState["id_reserva_detalhe"];
            string novoEstado = ddl_novo_estado.SelectedValue;
            int idUtilizador = Session["utilizador_id"] != null
                ? Convert.ToInt32(Session["utilizador_id"])
                : 0;

            SqlConnection myConn = new SqlConnection(cs);
            SqlCommand myCommand = new SqlCommand("sp_actualizar_estado_reserva", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@id_reserva", idReserva);
            myCommand.Parameters.AddWithValue("@estado", novoEstado);
            myCommand.Parameters.AddWithValue("@id_utilizador", idUtilizador);

            SqlParameter paramRetorno = new SqlParameter("@retorno", SqlDbType.Int);
            paramRetorno.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(paramRetorno);

            myConn.Open();
            myCommand.ExecuteNonQuery();
            myConn.Close();

            int retorno = Convert.ToInt32(paramRetorno.Value);
            MostrarToast(retorno == 1 ? "Estado actualizado com sucesso." : "Erro ao actualizar estado.", retorno == 1);

            CarregarContadoresBadges();
            CarregarReservas(ddl_estado.SelectedValue, tb_data_inicio.Text, tb_data_fim.Text, ddl_experiencia.SelectedValue);
            CarregarDetalhe(idReserva);
        }

        protected void btn_fechar_detalhe_Click(object sender, EventArgs e)
        {
            // Fechar tratado pelo data-dismiss do modal
        }

        private void AbrirModal()
        {
            // Envolver em $(document).ready() garante que o Bootstrap
            // já está carregado quando o script corre, evitando o
            // problema da modal não abrir após postback
            ScriptManager.RegisterStartupScript(
                this, GetType(), "abrirModal",
                "$(document).ready(function(){ $('#modalDetalheReserva').modal('show'); });",
                true);
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
            SqlConnection myConn = new SqlConnection(cs);
            SqlCommand myCommand = new SqlCommand("sp_obter_utilizador", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@id_utilizador", idUtilizador);

            myConn.Open();
            SqlDataReader dr = myCommand.ExecuteReader();
            string nome = dr.Read() ? dr["utilizador"].ToString() : "";
            myConn.Close();

            return nome;
        }

        // ═══════════════════════════════════════════════════════════════
        // Período padrão: início do mês actual até 3 meses à frente
        // Garante que reservas pendentes para datas futuras aparecem
        // ═══════════════════════════════════════════════════════════════
        private void DefinirPeriodoPadrao()
        {
            DateTime hoje = DateTime.Today;
            DateTime inicio = new DateTime(hoje.Year, hoje.Month, 1);
            DateTime fim = inicio.AddMonths(3).AddDays(-1);

            tb_data_inicio.Text = inicio.ToString("yyyy-MM-dd");
            tb_data_fim.Text = fim.ToString("yyyy-MM-dd");
        }
    }
}
