using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RaizesDigitais.Backoffice
{
    public partial class dashboard : Page
    {
        protected bool IsGestor
        {
            get { return string.Equals(Session["perfil"]?.ToString(), "Gestor", StringComparison.OrdinalIgnoreCase); }
        }

        public string LabelsGrafico = "";
        public string DadosGrafico = "";
        public string ReceitaGrafico = "";
        public string LabelsEstados = "";
        public string DadosEstados = "";
        public string LabelsExperiencias = "";
        public string DadosExperiencias = "";
        public string EventosCalendario = "[]";

        public string AlertasStockCss = "bg-success";
        public string ChegadasCss = "bg-info";
        public string PendentsCss = "bg-warning";
        public string ReceitaCss = "bg-info";
        public string ClientesCss = "bg-success";
        public string CupoesActivosCss = "bg-primary";
        public string ClientesB2BCss = "bg-info";
        public string OfertasB2BAtivasCss = "bg-success";

        string cs = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // ==========================
            // Valida seguranca
            // ==========================
            // Verifica sessao staff. Redireciona login.
            if (Session["perfil"] == null)
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            if (!IsPostBack)
                CarregarDadosDashboard();
        }

        private void CarregarDadosDashboard()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                // ==========================
                // Proc dados globais
                // ==========================
                // Multiplos result sets. Kpis e graficos.
                SqlCommand cmd = new SqlCommand("sp_obter_dados_dashboard", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                // ==========================
                // Logica perfil gestor
                // ==========================
                // Ignora kpis. Salta para chegadas.
                if (IsGestor)
                {
                    // Avançar do Result Set 0 até ao 4 (chegadas)
                    for (int i = 0; i < 4; i++)
                    {
                        if (!dr.NextResult())
                            return;
                    }

                    // RESULTADO 4: Próximas chegadas HOJE
                    DataTable dtChegadas = new DataTable();
                    dtChegadas.Columns.Add("hora");
                    dtChegadas.Columns.Add("cliente");
                    dtChegadas.Columns.Add("pax");

                    while (dr.Read())
                    {
                        DataRow row = dtChegadas.NewRow();
                        row["hora"] = dr["hora"];
                        row["cliente"] = dr["cliente"];
                        row["pax"] = dr["pax"];
                        dtChegadas.Rows.Add(row);
                    }

                    gv_proximas_chegadas.DataSource = dtChegadas;
                    gv_proximas_chegadas.DataBind();
                    lit_total_chegadas.Text = dtChegadas.Rows.Count.ToString();

                    // RESULTADO 5: Eventos para o calendário
                    if (dr.NextResult())
                    {
                        EventosCalendario = ConstruirJsonEventos(dr);
                    }
                    return;
                }

                // ═══════════════════════════════════════════════════════════════
                // ADMIN: Processar todos os resultados
                // ═══════════════════════════════════════════════════════════════

                int chegadas = 0;

                // RESULTADO 1: KPIs
                if (dr.Read())
                {
                    chegadas = Convert.ToInt32(dr["ReservasHoje"]);
                    int pendentes = Convert.ToInt32(dr["ReservasPendentes"]);
                    int alertas = Convert.ToInt32(dr["AlertasStock"]);
                    int clientesNovos = Convert.ToInt32(dr["ClientesNovosMes"]);
                    decimal receitaMes = Convert.ToDecimal(dr["ReceitaMes"]);
                    decimal receitaAnterior = Convert.ToDecimal(dr["ReceitaMesAnterior"]);
                    int totalCupoesActivos = Convert.ToInt32(dr["TotalCupoesActivos"]);
                    int totalClientesB2B = Convert.ToInt32(dr["TotalClientesB2B"]);
                    int totalOfertasB2BAtivas = Convert.ToInt32(dr["TotalOfertasB2BAtivas"]);

                    lit_reservas_hoje.Text = chegadas.ToString();
                    lit_reservas_pendentes.Text = pendentes.ToString();
                    lit_receita_mes.Text = string.Format("{0:N2}", receitaMes);
                    lit_receita_anterior.Text = string.Format("{0:N2}", receitaAnterior);
                    lit_total_clientes.Text = dr["TotalClientes"].ToString();
                    lit_clientes_novos.Text = clientesNovos.ToString();
                    lit_alertas_stock.Text = alertas.ToString();
                    lit_total_cupoes_activos.Text = totalCupoesActivos.ToString();
                    lit_total_clientes_b2b.Text = totalClientesB2B.ToString();
                    lit_total_ofertas_b2b_activas.Text = totalOfertasB2BAtivas.ToString();

                    // Cores CSS para os KPIs
                    ChegadasCss = chegadas == 0 ? "bg-success" : chegadas <= 3 ? "bg-warning" : "bg-danger";
                    PendentsCss = pendentes == 0 ? "bg-success" : pendentes <= 5 ? "bg-warning" : "bg-danger";
                    ReceitaCss = receitaMes >= receitaAnterior ? "bg-success" : "bg-danger";
                    AlertasStockCss = alertas > 0 ? "bg-danger" : "bg-success";
                    ClientesCss = "bg-success";
                    CupoesActivosCss = totalCupoesActivos == 0 ? "bg-secondary" : totalCupoesActivos <= 5 ? "bg-warning" : "bg-primary";
                    ClientesB2BCss = totalClientesB2B == 0 ? "bg-secondary" : totalClientesB2B <= 3 ? "bg-info" : "bg-success";
                    OfertasB2BAtivasCss = totalOfertasB2BAtivas == 0 ? "bg-secondary" : totalOfertasB2BAtivas <= 2 ? "bg-info" : "bg-success";

                    // ═══════════════════════════════════════════════════════════
                    // MELHORIA: Callout de alerta para reservas pendentes
                    // ═══════════════════════════════════════════════════════════
                    if (pendentes > 0)
                    {
                        pnl_alerta_pendentes.Visible = true;
                        lit_pendentes_alerta.Text = pendentes.ToString();
                    }
                    else
                    {
                        pnl_alerta_pendentes.Visible = false;
                    }
                }

                // RESULTADO 2: Receita + Reservas por mês
                if (dr.NextResult())
                {
                    var meses = new List<string>();
                    var totais = new List<string>();
                    var receitas = new List<string>();

                    while (dr.Read())
                    {
                        meses.Add("'" + dr["Mes"].ToString() + "'");
                        receitas.Add(Convert.ToDecimal(dr["Receita"]).ToString(
                            System.Globalization.CultureInfo.InvariantCulture));
                        totais.Add(dr["Total"].ToString());
                    }

                    LabelsGrafico = string.Join(",", meses);
                    ReceitaGrafico = string.Join(",", receitas);
                    DadosGrafico = string.Join(",", totais);
                }

                // RESULTADO 3: Top experiências
                if (dr.NextResult())
                {
                    var labelsExp = new List<string>();
                    var dadosExp = new List<string>();

                    while (dr.Read())
                    {
                        labelsExp.Add("'" + dr["Experiencia"].ToString().Replace("'", "\\'") + "'");
                        dadosExp.Add(dr["Total"].ToString());
                    }

                    LabelsExperiencias = string.Join(",", labelsExp);
                    DadosExperiencias = string.Join(",", dadosExp);
                }

                // RESULTADO 4: Estados
                if (dr.NextResult())
                {
                    var labelsEst = new List<string>();
                    var dadosEst = new List<string>();

                    while (dr.Read())
                    {
                        labelsEst.Add("'" + dr["estado"].ToString() + "'");
                        dadosEst.Add(dr["Total"].ToString());
                    }

                    LabelsEstados = string.Join(",", labelsEst);
                    DadosEstados = string.Join(",", dadosEst);
                }

                // RESULTADO 5: Próximas chegadas HOJE
                if (dr.NextResult())
                {
                    gv_proximas_chegadas.DataSource = dr;
                    gv_proximas_chegadas.DataBind();

                    // Contar para o badge (reutilizar valor de ReservasHoje do primeiro result set)
                    lit_total_chegadas.Text = chegadas.ToString();
                }

                // RESULTADO 6: Eventos para o calendário
                if (dr.NextResult())
                {
                    EventosCalendario = ConstruirJsonEventos(dr);
                }

                // ═══════════════════════════════════════════════════════════
                // MELHORIA: Progress bars de ocupação (opcional - descomentar se existir SP)
                // ═══════════════════════════════════════════════════════════
                // CarregarOcupacaoExperiencias();
            }
        }

        /// <summary>
        /// Constrói o JSON de eventos para o FullCalendar
        /// </summary>
        private string ConstruirJsonEventos(SqlDataReader dr)
        {
            var eventos = new StringBuilder();
            eventos.Append("[");
            bool primeiro = true;

            while (dr.Read())
            {
                if (!primeiro) eventos.Append(",");
                primeiro = false;

                string titulo = dr["experiencia"].ToString().Replace("'", "\\'").Replace("\"", "\\\"")
                                + " (" + dr["pax"] + " pax) — "
                                + dr["cliente"].ToString().Replace("\"", "\\\"");

                eventos.Append("{");
                eventos.Append("\"title\":\"" + titulo + "\",");
                eventos.Append("\"start\":\"" + dr["data_inicio"] + "\"");
                eventos.Append("}");
            }

            eventos.Append("]");
            return eventos.ToString();
        }

        /// <summary>
        /// MELHORIA: Retorna a classe CSS para a progress bar de ocupação
        /// Verde: 0-50%, Amarelo: 51-80%, Vermelho: 81-100%
        /// </summary>
        protected string GetOcupacaoClass(object ocupacao)
        {
            if (ocupacao == null || ocupacao == DBNull.Value)
                return "secondary";

            int pct = Convert.ToInt32(ocupacao);
            if (pct <= 50) return "success";
            if (pct <= 80) return "warning";
            return "danger";
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // MELHORIA OPCIONAL: Progress bars de ocupação por experiência
        // Descomentar quando existir uma SP sp_obter_ocupacao_experiencias
        // ═══════════════════════════════════════════════════════════════════════════
        /*
        private void CarregarOcupacaoExperiencias()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_obter_ocupacao_experiencias", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                rpt_ocupacao.DataSource = dr;
                rpt_ocupacao.DataBind();

                pnl_ocupacao.Visible = rpt_ocupacao.Items.Count > 0;
            }
        }
        */
    }
}