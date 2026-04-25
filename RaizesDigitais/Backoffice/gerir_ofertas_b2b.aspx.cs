using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RaizesDigitais.Backoffice
{
    public partial class gerir_ofertas_b2b : System.Web.UI.Page
    {
        // Paginação
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

        // Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["perfil"] == null)
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            if (Session["perfil"].ToString() == "Gestor")
            {
                Response.Redirect("~/Backoffice/dashboard.aspx");
                return;
            }

            if (!IsPostBack)
                CarregarExperiencias();
        }

        // Carregar grid
        private void CarregarExperiencias()
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

                // Filtrar em memória
                string filtroB2B = ddl_filtro_b2b.SelectedValue;
                string filtroTipo = ddl_filtro_tipo.SelectedValue;

                DataTable dtFiltrado = dt.Clone();
                foreach (DataRow row in dt.Rows)
                {
                    if (!string.IsNullOrEmpty(filtroB2B))
                    {
                        bool ativo = filtroB2B == "1";
                        if (Convert.ToBoolean(row["oferta_b2b"]) != ativo)
                            continue;
                    }

                    if (!string.IsNullOrEmpty(filtroTipo))
                    {
                        if (row["tipo"].ToString() != filtroTipo)
                            continue;
                    }

                    dtFiltrado.ImportRow(row);
                }

                AplicarPaginacao(dtFiltrado);
                gv_experiencias.DataBind();
            }
        }

        // Paginação
        private void AplicarPaginacao(DataTable dt)
        {
            int total = dt.Rows.Count;
            lbl_total.Text = total + " experiência(s)";

            if (PorPagina > 0 && total > 0)
            {
                int totalPaginas = (int)Math.Ceiling((double)total / PorPagina);
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

                gv_experiencias.DataSource = paginada;
            }
            else
            {
                lbl_pagina.Text = "Página 1 de 1";
                btn_anterior.Visible = false;
                btn_seguinte.Visible = false;
                gv_experiencias.DataSource = dt;
            }
        }

        // Filtros
        protected void btn_filtrar_Click(object sender, EventArgs e)
        {
            PaginaActual = 1;
            CarregarExperiencias();
        }

        protected void btn_limpar_Click(object sender, EventArgs e)
        {
            ddl_filtro_b2b.SelectedValue = "";
            ddl_filtro_tipo.SelectedValue = "";
            PaginaActual = 1;
            CarregarExperiencias();
        }

        // RowCommand: abre modal com dados da experiência
        protected void gv_experiencias_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Configurar") return;

            int idExp = int.Parse(e.CommandArgument.ToString());
            CarregarDadosModal(idExp);
            AbrirModal();
        }

        private void CarregarDadosModal(int idExp)
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
                    lit_nome_experiencia.Text = dr["nome"].ToString();
                    cb_oferta_b2b.Checked = Convert.ToBoolean(dr["oferta_b2b"]);

                    bool temDesconto = dr["desconto_b2b"] != DBNull.Value;
                    tb_desconto_b2b.Text = temDesconto
                        ? Convert.ToDecimal(dr["desconto_b2b"]).ToString("0.##")
                        : "";

                    // Pré-visualização do preço
                    decimal preco = Convert.ToDecimal(dr["preco_por_pessoa"]);
                    lit_preco_original.Text = preco.ToString("0.00");

                    if (cb_oferta_b2b.Checked && temDesconto)
                    {
                        decimal desconto = Convert.ToDecimal(dr["desconto_b2b"]);
                        decimal precoFinal = preco * (1 - desconto / 100m);
                        lit_preco_b2b.Text = precoFinal.ToString("0.00");
                        pnl_preview.Visible = true;
                    }
                    else
                    {
                        pnl_preview.Visible = false;
                    }
                }
            }

            lbl_erro_modal.Visible = false;
        }

        // Guardar configuração B2B
        protected void btn_guardar_b2b_Click(object sender, EventArgs e)
        {
            int idExp = int.Parse(hf_id_experiencia.Value);
            bool ofertaActiva = cb_oferta_b2b.Checked;

            // Se oferta activa, desconto é obrigatório e tem de ser válido
            decimal desconto = 0;
            if (ofertaActiva)
            {
                if (string.IsNullOrWhiteSpace(tb_desconto_b2b.Text) ||
                    !decimal.TryParse(tb_desconto_b2b.Text, out desconto) ||
                    desconto <= 0 || desconto > 100)
                {
                    lbl_erro_modal.Text = "Insira um desconto válido entre 1 e 100%.";
                    lbl_erro_modal.Visible = true;
                    CarregarDadosModal(idExp);
                    AbrirModal();
                    return;
                }
            }

            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_toggle_oferta_b2b", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id_experiencia", idExp);
                cmd.Parameters.AddWithValue("@oferta_b2b", ofertaActiva);

                SqlParameter pDesconto = cmd.Parameters.Add("@desconto_b2b", SqlDbType.Decimal);
                pDesconto.Precision = 5;
                pDesconto.Scale = 2;
                pDesconto.Value = ofertaActiva ? (object)desconto : DBNull.Value;

                SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int);
                outRetorno.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outRetorno);

                con.Open();
                cmd.ExecuteNonQuery();

                int retorno = (int)outRetorno.Value;

                if (retorno == 1)
                {
                    string msg = ofertaActiva
                        ? $"Oferta B2B activada com {desconto:0.##}% de desconto."
                        : "Oferta B2B desactivada.";
                    MostrarToast(msg, true);
                    CarregarExperiencias();
                }
                else
                {
                    lbl_erro_modal.Text = "Experiência não encontrada.";
                    lbl_erro_modal.Visible = true;
                    AbrirModal();
                }
            }
        }

        // Helpers de apresentação
        protected string GetPrecoBadge(object ofertaB2b, object precoPessoa, object descontoB2b)
        {
            if (!Convert.ToBoolean(ofertaB2b) || descontoB2b == DBNull.Value)
                return "<span class='text-muted'>—</span>";

            decimal preco = Convert.ToDecimal(precoPessoa);
            decimal desconto = Convert.ToDecimal(descontoB2b);
            decimal final = preco * (1 - desconto / 100m);

            return $"<span class='badge badge-success'>{final:0.00} €</span>";
        }

        // Paginação: botões e dropdown
        protected void btn_anterior_Click(object sender, EventArgs e)
        {
            if (PaginaActual > 1) { PaginaActual--; CarregarExperiencias(); }
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

        // Modal e Toast
        private void AbrirModal()
        {
            ScriptManager.RegisterStartupScript(
                this, GetType(), "abrirModal",
                "$('#modalB2B').modal('show');",
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
