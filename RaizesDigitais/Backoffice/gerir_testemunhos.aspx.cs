using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RaizesDigitais.Backoffice
{
    public partial class gerir_testemunhos : Page
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

            // Apenas Admin pode gerir testemunhos
            if (IsGestor)
            {
                Response.Redirect("~/Backoffice/dashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CarregarExperienciasDropdown();
                CarregarContadoresBadges();
                CarregarTestemunhos(null, null);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // NOVO: Carregar dropdown de experiências para filtro
        // ═══════════════════════════════════════════════════════════════
        private void CarregarExperienciasDropdown()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_listar_experiencias_backoffice", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                ddl_experiencia.Items.Clear();
                ddl_experiencia.Items.Add(new ListItem("— Todas —", ""));

                while (dr.Read())
                {
                    ddl_experiencia.Items.Add(new ListItem(
                        dr["nome"].ToString(),
                        dr["id_experiencia"].ToString()
                    ));
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
                SqlCommand cmd = new SqlCommand("sp_contar_testemunhos_badges", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    lit_count_todos.Text = dr["total"].ToString();
                    lit_count_pendentes.Text = dr["pendentes"].ToString();
                    lit_count_publicados.Text = dr["publicados"].ToString();

                    int pendentes = Convert.ToInt32(dr["pendentes"]);
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
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // NOVO: Handlers dos badges de filtro rápido
        // ═══════════════════════════════════════════════════════════════
        protected void badge_todos_Click(object sender, EventArgs e)
        {
            ddl_experiencia.SelectedIndex = 0;
            ddl_estrelas.SelectedIndex = 0;
            PaginaActual = 1;
            CarregarTestemunhos(null, null);
        }

        protected void badge_pendentes_Click(object sender, EventArgs e)
        {
            // Pendentes = não publicados
            ddl_experiencia.SelectedIndex = 0;
            ddl_estrelas.SelectedIndex = 0;
            PaginaActual = 1;
            CarregarTestemunhosFiltrados(false, null, null);
        }

        protected void badge_publicados_Click(object sender, EventArgs e)
        {
            ddl_experiencia.SelectedIndex = 0;
            ddl_estrelas.SelectedIndex = 0;
            PaginaActual = 1;
            CarregarTestemunhosFiltrados(true, null, null);
        }

        // ═══════════════════════════════════════════════════════════════
        // Filtros
        // ═══════════════════════════════════════════════════════════════
        protected void btn_filtrar_Click(object sender, EventArgs e)
        {
            PaginaActual = 1;
            CarregarTestemunhos(ddl_experiencia.SelectedValue, ddl_estrelas.SelectedValue);
        }

        protected void btn_limpar_Click(object sender, EventArgs e)
        {
            ddl_experiencia.SelectedIndex = 0;
            ddl_estrelas.SelectedIndex = 0;
            PaginaActual = 1;
            CarregarTestemunhos(null, null);
        }

        private void CarregarTestemunhos(string idExperiencia, string estrelas)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_listar_testemunhos_backoffice", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Filtrar em memória
                var query = dt.AsEnumerable().AsQueryable();

                if (!string.IsNullOrEmpty(idExperiencia))
                {
                    int id = int.Parse(idExperiencia);
                    query = query.Where(r => Convert.ToInt32(r["id_experiencia"]) == id);
                }

                if (!string.IsNullOrEmpty(estrelas))
                {
                    int stars = int.Parse(estrelas);
                    query = query.Where(r => Convert.ToInt32(r["estrelas"]) == stars);
                }

                DataTable dtFiltrado = dt.Clone();
                foreach (var row in query)
                    dtFiltrado.ImportRow(row);

                AplicarPaginacao(dtFiltrado);
            }
        }

        private void CarregarTestemunhosFiltrados(bool publicado, string idExperiencia, string estrelas)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_listar_testemunhos_backoffice", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                var query = dt.AsEnumerable().Where(r => Convert.ToBoolean(r["publicado"]) == publicado);

                if (!string.IsNullOrEmpty(idExperiencia))
                {
                    int id = int.Parse(idExperiencia);
                    query = query.Where(r => Convert.ToInt32(r["id_experiencia"]) == id);
                }

                if (!string.IsNullOrEmpty(estrelas))
                {
                    int stars = int.Parse(estrelas);
                    query = query.Where(r => Convert.ToInt32(r["estrelas"]) == stars);
                }

                DataTable dtFiltrado = dt.Clone();
                foreach (var row in query)
                    dtFiltrado.ImportRow(row);

                AplicarPaginacao(dtFiltrado);
            }
        }

        private void AplicarPaginacao(DataTable dt)
        {
            int totalRegistos = dt.Rows.Count;
            lbl_total.Text = totalRegistos + " testemunho(s)";
            lit_total_lista.Text = totalRegistos.ToString();

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
                gv_testemunhos.DataSource = paginada;
            }
            else
            {
                lbl_pagina.Text = "Página 1 de 1";
                btn_anterior.Visible = false;
                btn_seguinte.Visible = false;
                gv_testemunhos.DataSource = dt;
            }
            gv_testemunhos.DataBind();
        }

        protected void gv_testemunhos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            DataRowView row = (DataRowView)e.Row.DataItem;
            bool publicado = Convert.ToBoolean(row["publicado"]);

            Label lblEstado = (Label)e.Row.FindControl("lbl_estado");
            if (lblEstado != null)
            {
                lblEstado.Text = publicado ? "Publicado" : "Pendente";
                lblEstado.CssClass = publicado ? "badge badge-success" : "badge badge-warning";
            }

            // Comentário gerido via CSS (ellipsis) directamente no .aspx
        }

        protected void gv_testemunhos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            bool aprovar = (e.CommandName == "Aprovar");
            if (e.CommandName != "Aprovar" && e.CommandName != "Reprovar") return;

            int idAvaliacao = int.Parse(e.CommandArgument.ToString());
            AtualizarEstado(idAvaliacao, aprovar);
            PaginaActual = 1;
            CarregarContadoresBadges();
            CarregarTestemunhos(ddl_experiencia.SelectedValue, ddl_estrelas.SelectedValue);

            MostrarToast(
                aprovar ? "Testemunho aprovado e já visível no site público." : "Testemunho removido do site público.",
                aprovar);
        }

        private void AtualizarEstado(int idAvaliacao, bool publicado)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("sp_toggle_publicacao_testemunho", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_avaliacao", idAvaliacao);
                cmd.Parameters.AddWithValue("@publicado", publicado ? 1 : 0);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void MostrarToast(string mensagem, bool sucesso = true)
        {
            string tipo = sucesso ? "success" : "error";
            string script = $"toastr.{tipo}('{mensagem.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "toast", script, true);
        }

        protected string GetEstrelasSimples(object estrelasObj)
        {
            int estrelas = Convert.ToInt32(estrelasObj);
            string result = "";
            for (int i = 1; i <= 5; i++)
                result += (i <= estrelas) ? "★" : "☆";
            return result;
        }

        // PAGINAÇÃO
        protected void btn_anterior_Click(object sender, EventArgs e)
        {
            if (PaginaActual > 1)
            {
                PaginaActual--;
                CarregarTestemunhos(ddl_experiencia.SelectedValue, ddl_estrelas.SelectedValue);
            }
        }

        protected void btn_seguinte_Click(object sender, EventArgs e)
        {
            PaginaActual++;
            CarregarTestemunhos(ddl_experiencia.SelectedValue, ddl_estrelas.SelectedValue);
        }

        protected void ddl_por_pagina_Changed(object sender, EventArgs e)
        {
            PaginaActual = 1;
            CarregarTestemunhos(ddl_experiencia.SelectedValue, ddl_estrelas.SelectedValue);
        }
    }
}