using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RaizesDigitais.Backoffice
{
    public partial class gerir_cupoes : System.Web.UI.Page
    {
        protected bool IsGestor
        {
            get { return string.Equals(Session["perfil"]?.ToString(), "Gestor", StringComparison.OrdinalIgnoreCase); }
        }

        private int PaginaActual
        {
            get { return ViewState["pagina"] != null ? (int)ViewState["pagina"] : 1; }
            set { ViewState["pagina"] = value; }
        }
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

            if (IsGestor)
            {
                Response.Redirect("~/Backoffice/dashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CarregarClientes();
                CarregarContadoresBadges();
                CarregarCupoes();

                // Controlar visibilidade da coluna "Uso" (índice 6) baseado no perfil
                // A coluna só deve ser visível para não-gestores
                if (gv_cupoes.Columns.Count > 6)
                {
                    gv_cupoes.Columns[6].Visible = !IsGestor;
                }
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // NOVO: Carregar contadores para badges
        // ═══════════════════════════════════════════════════════════════
        private void CarregarContadoresBadges()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT
                        COUNT(*) AS total,
                        SUM(CASE WHEN utilizado = 0 AND validade >= CAST(GETDATE() AS DATE) THEN 1 ELSE 0 END) AS validos,
                        SUM(CASE WHEN utilizado = 1 THEN 1 ELSE 0 END) AS usados,
                        SUM(CASE WHEN utilizado = 0 AND validade < CAST(GETDATE() AS DATE) THEN 1 ELSE 0 END) AS expirados,
                        SUM(CASE WHEN utilizado = 0 AND validade BETWEEN CAST(GETDATE() AS DATE) AND DATEADD(day, 7, GETDATE()) THEN 1 ELSE 0 END) AS expirar_breve
                    FROM cupoes", con);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    lit_count_todos.Text = dr["total"].ToString();
                    lit_count_validos.Text = dr["validos"].ToString();
                    lit_count_usados.Text = dr["usados"].ToString();
                    lit_count_expirados.Text = dr["expirados"].ToString();

                    int expirarBreve = Convert.ToInt32(dr["expirar_breve"]);
                    if (expirarBreve > 0)
                    {
                        pnl_alerta_expirar.Visible = true;
                        lit_expirar_breve.Text = expirarBreve.ToString();
                    }
                    else
                    {
                        pnl_alerta_expirar.Visible = false;
                    }
                }
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // NOVO: Handlers dos badges de filtro rápido
        // ═══════════════════════════════════════════════════════════════
        protected void badge_todos_Click(object sender, EventArgs e)
        {
            ddl_filtro_estado.SelectedIndex = 0;
            PaginaActual = 1;
            CarregarCupoes();
        }

        protected void badge_validos_Click(object sender, EventArgs e)
        {
            ddl_filtro_estado.SelectedValue = "validos";
            PaginaActual = 1;
            CarregarCupoes();
        }

        protected void badge_usados_Click(object sender, EventArgs e)
        {
            ddl_filtro_estado.SelectedValue = "usados";
            PaginaActual = 1;
            CarregarCupoes();
        }

        protected void badge_expirados_Click(object sender, EventArgs e)
        {
            ddl_filtro_estado.SelectedValue = "expirados";
            PaginaActual = 1;
            CarregarCupoes();
        }

        private void CarregarClientes()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_listar_clientes", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pesquisa", DBNull.Value);
                cmd.Parameters.AddWithValue("@segmento", DBNull.Value);

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddl_cupao_cliente.Items.Clear();
                ddl_cupao_cliente.Items.Add(new ListItem("— Genérico (todos os clientes) —", ""));

                foreach (DataRow row in dt.Rows)
                {
                    string texto = row["nome"].ToString() + " (" + row["email"].ToString() + ")";
                    ddl_cupao_cliente.Items.Add(new ListItem(texto, row["id_cliente"].ToString()));
                }
            }
        }

        private void CarregarCupoes()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_listar_cupoes_backoffice", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Filtrar em memória
                DataView dv = dt.DefaultView;
                string filtroEstado = ddl_filtro_estado.SelectedValue;
                string filtroTipo = ddl_filtro_tipo.SelectedValue;
                string filtroCodigo = tb_filtro_codigo.Text.Trim().ToUpper();

                var query = dt.AsEnumerable().AsQueryable();

                if (!string.IsNullOrEmpty(filtroEstado))
                {
                    switch (filtroEstado)
                    {
                        case "validos":
                            query = query.Where(r =>
                                !Convert.ToBoolean(r["utilizado"]) &&
                                Convert.ToDateTime(r["validade"]) >= DateTime.Now);
                            break;
                        case "usados":
                            query = query.Where(r => Convert.ToBoolean(r["utilizado"]));
                            break;
                        case "expirados":
                            query = query.Where(r =>
                                !Convert.ToBoolean(r["utilizado"]) &&
                                Convert.ToDateTime(r["validade"]) < DateTime.Now);
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(filtroTipo))
                    query = query.Where(r => r["tipo_desconto"].ToString() == filtroTipo);

                if (!string.IsNullOrEmpty(filtroCodigo))
                    query = query.Where(r => r["codigo"].ToString().Contains(filtroCodigo));

                DataTable dtFiltrado = dt.Clone();
                foreach (var row in query)
                    dtFiltrado.ImportRow(row);

                AplicarPaginacao(dtFiltrado, gv_cupoes, "cupão(ões)");
                gv_cupoes.DataBind();

                lit_total_lista.Text = dtFiltrado.Rows.Count.ToString();
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // NOVO: Highlight cupões a expirar em breve
        // ═══════════════════════════════════════════════════════════════
        protected void gv_cupoes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            DataRowView row = (DataRowView)e.Row.DataItem;
            bool utilizado = Convert.ToBoolean(row["utilizado"]);
            DateTime validade = Convert.ToDateTime(row["validade"]);

            // Verificar se expira em breve
            if (!utilizado && validade >= DateTime.Now && validade <= DateTime.Now.AddDays(7))
            {
                // Destacar linha
                e.Row.CssClass = "table-warning";

                // Marcar validade como texto de alerta
                Literal litValidade = (Literal)e.Row.FindControl("lit_validade");
                if (litValidade != null)
                {
                    litValidade.Text = $"<span class='text-danger font-weight-bold'>{validade:dd/MM/yyyy}</span> <i class='fas fa-exclamation-triangle text-warning' title='Expira em breve!'></i>";
                }
            }
            else
            {
                Literal litValidade = (Literal)e.Row.FindControl("lit_validade");
                if (litValidade != null)
                {
                    litValidade.Text = validade.ToString("dd/MM/yyyy");
                }
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

        protected void btn_filtrar_Click(object sender, EventArgs e)
        {
            PaginaActual = 1;
            CarregarCupoes();
        }

        protected void btn_limpar_Click(object sender, EventArgs e)
        {
            ddl_filtro_estado.SelectedValue = "";
            ddl_filtro_tipo.SelectedValue = "";
            tb_filtro_codigo.Text = "";
            PaginaActual = 1;
            CarregarCupoes();
        }

        protected void btn_novo_cupao_Click(object sender, EventArgs e)
        {
            tb_codigo.Text = "";
            tb_desconto.Text = "";
            tb_validade.Text = DateTime.Now.AddMonths(6).ToString("yyyy-MM-dd");
            ddl_tipo_desconto.SelectedIndex = 0;
            ddl_cupao_cliente.SelectedIndex = 0;
            ddl_cupao_segmento.SelectedIndex = 0;
            lbl_erro_modal.Visible = false;

            AbrirModal();
        }

        protected void btn_gerar_codigo_Click(object sender, EventArgs e)
        {
            tb_codigo.Text = "RD" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
            AbrirModal();
        }

        protected void btn_criar_cupao_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_codigo.Text))
            {
                lbl_erro_modal.Text = "O código é obrigatório.";
                lbl_erro_modal.Visible = true;
                AbrirModal();
                return;
            }

            if (string.IsNullOrWhiteSpace(tb_desconto.Text) ||
                !decimal.TryParse(tb_desconto.Text, out decimal valorDesconto) ||
                valorDesconto <= 0)
            {
                lbl_erro_modal.Text = "Insira um valor de desconto válido (maior que zero).";
                lbl_erro_modal.Visible = true;
                AbrirModal();
                return;
            }

            if (string.IsNullOrWhiteSpace(tb_validade.Text) ||
                !DateTime.TryParse(tb_validade.Text, out DateTime validade) ||
                validade <= DateTime.Now)
            {
                lbl_erro_modal.Text = "A validade deve ser uma data futura.";
                lbl_erro_modal.Visible = true;
                AbrirModal();
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_inserir_cupao_manual", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@codigo", tb_codigo.Text.Trim().ToUpper());
                cmd.Parameters.AddWithValue("@tipo_desconto", ddl_tipo_desconto.SelectedValue);
                cmd.Parameters.AddWithValue("@valor_desconto", valorDesconto);
                cmd.Parameters.AddWithValue("@id_cliente",
                    string.IsNullOrEmpty(ddl_cupao_cliente.SelectedValue)
                        ? (object)DBNull.Value
                        : int.Parse(ddl_cupao_cliente.SelectedValue));
                cmd.Parameters.AddWithValue("@validade", validade);
                cmd.Parameters.AddWithValue("@segmento_crm",
                    string.IsNullOrEmpty(ddl_cupao_segmento.SelectedValue)
                        ? (object)DBNull.Value
                        : ddl_cupao_segmento.SelectedValue);

                SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int);
                outRetorno.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outRetorno);

                con.Open();
                cmd.ExecuteNonQuery();

                int retorno = (int)outRetorno.Value;

                if (retorno == 1)
                {
                    MostrarToast("Cupão criado com sucesso.", true);
                    CarregarContadoresBadges();
                    CarregarCupoes();
                }
                else if (retorno == -1)
                {
                    lbl_erro_modal.Text = "Já existe um cupão com este código. Escolha outro.";
                    lbl_erro_modal.Visible = true;
                    AbrirModal();
                }
                else
                {
                    lbl_erro_modal.Text = "Erro ao criar cupão. Tente novamente.";
                    lbl_erro_modal.Visible = true;
                    AbrirModal();
                }
            }
        }

        protected void gv_cupoes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ApagarCupao")
            {
                int idCupao = int.Parse(e.CommandArgument.ToString());
                ApagarCupao(idCupao);
            }
        }

        private void ApagarCupao(int idCupao)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM cupoes WHERE id_cupao = @id_cupao AND utilizado = 0", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id_cupao", idCupao);

                con.Open();
                int linhasAfectadas = cmd.ExecuteNonQuery();

                if (linhasAfectadas > 0)
                {
                    MostrarToast("Cupão apagado.", true);
                    CarregarContadoresBadges();
                }
                else
                    MostrarToast("Não foi possível apagar. O cupão pode já ter sido usado.", false);
            }

            CarregarCupoes();
        }

        protected void btn_anterior_Click(object sender, EventArgs e)
        {
            if (PaginaActual > 1) { PaginaActual--; CarregarCupoes(); }
        }

        protected void btn_seguinte_Click(object sender, EventArgs e)
        {
            PaginaActual++;
            CarregarCupoes();
        }

        protected void ddl_por_pagina_Changed(object sender, EventArgs e)
        {
            PaginaActual = 1;
            CarregarCupoes();
        }

        protected string FormatarDesconto(string tipo, string valor)
        {
            if (!decimal.TryParse(valor, out decimal v)) return valor;
            return tipo == "percentagem"
                ? v.ToString("0.##") + "%"
                : v.ToString("0.##") + "€";
        }

        protected string FormatarDestinatario(object nomeCliente, object segmentoCrm)
        {
            bool temCliente = nomeCliente != DBNull.Value && !string.IsNullOrEmpty(nomeCliente.ToString());
            bool temSegmento = segmentoCrm != DBNull.Value && !string.IsNullOrEmpty(segmentoCrm.ToString());

            if (temCliente)
                return nomeCliente.ToString();

            if (temSegmento)
                return "<span class='badge badge-warning'>" + segmentoCrm.ToString() + "</span>";

            return "<span class='text-muted'>Genérico</span>";
        }

        protected string GetEstadoBadge(bool utilizado, DateTime validade)
        {
            if (utilizado)
                return "<span class='badge badge-secondary'>Usado</span>";
            if (validade < DateTime.Now)
                return "<span class='badge badge-danger'>Expirado</span>";
            return "<span class='badge badge-success'>Válido</span>";
        }

        private void AbrirModal()
        {
            ScriptManager.RegisterStartupScript(
                this, GetType(), "abrirModal",
                "$('#modalCriarCupao').modal('show');",
                true);
        }

        private void MostrarToast(string mensagem, bool sucesso = true)
        {
            string tipo = sucesso ? "success" : "error";
            string script = $"toastr.{tipo}('{mensagem.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "toast", script, true);
        }
    }
}