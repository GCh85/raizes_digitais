using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RaizesDigitais.Pages
{
    public partial class conta : Page
    {
        readonly string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;
        public string ProgressoPontos = "0";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["cliente_id"] == null)
            {
                Response.Redirect("~/Pages/conta_login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                int idCliente = Convert.ToInt32(Session["cliente_id"]);
                lit_nome_cliente.Text = Session["cliente_nome"].ToString();

                CarregarDadosConta(idCliente);
                CarregarFidelizacao(idCliente);
                CarregarFavoritos(idCliente);
                CarregarCupoes(idCliente);

                // Verificar se é cliente B2B e carregar ofertas
                string segmento = Session["cliente_segmento"]?.ToString();
                if (!string.IsNullOrEmpty(segmento) && segmento == "B2B")
                {
                    pnl_tab_ofertas_b2b.Visible = true;
                    pnl_conteudo_ofertas_b2b.Visible = true;
                    CarregarOfertasB2B();
                }
            }
        }

        private void CarregarDadosConta(int idCliente)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_obter_cliente_detalhe", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_cliente", idCliente);
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    lit_email.Text = dr["email"].ToString();
                    tb_telefone.Text = dr["telefone"].ToString();
                    tb_alergias.Text = dr["notas_alergias"] == DBNull.Value ? "" : dr["notas_alergias"].ToString();
                    tb_pref_vinhos.Text = dr["preferencias_vinho"] == DBNull.Value ? "" : dr["preferencias_vinho"].ToString();

                    // Guardar segmento na Session para verificar acesso a ofertas B2B
                    Session["cliente_segmento"] = dr["segmento_crm"].ToString();

                    bool is2FA = Convert.ToBoolean(dr["dois_factor_activado"]);
                    btn_toggle_2fa.Text = is2FA ? "Desativar Segurança Extra" : "Ativar Segurança Extra";
                    btn_toggle_2fa.CssClass = is2FA ? "btn btn-sm btn-danger" : "btn btn-sm btn-success";
                }

                if (ds.Tables.Count > 1)
                {
                    gv_reservas.DataSource = ds.Tables[1];
                    gv_reservas.DataBind();
                }
            }
        }

        // ─────────────────────────────────────────────────────────
        // AVALIAR EXPERIÊNCIA (TESTEMUNHOS)
        // ─────────────────────────────────────────────────────────
        protected void btn_enviar_testemunho_Click(object sender, EventArgs e)
        {
            int estrelas = int.Parse(hf_estrelas.Value);
            if (estrelas < 1 || estrelas > 5)
            {
                lbl_mensagem_testemunho.Text = "Seleccione uma nota de 1 a 5 estrelas.";
                lbl_mensagem_testemunho.CssClass = "alert alert-danger d-block mb-3";
                lbl_mensagem_testemunho.Visible = true;
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;
            int idCliente = Convert.ToInt32(Session["cliente_id"]);
            int idReserva = int.Parse(hf_id_reserva_testemunho.Value);
            int idExperiencia = int.Parse(hf_id_experiencia_testemunho.Value);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_inserir_avaliacao_experiencia", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id_cliente", idCliente);
                cmd.Parameters.AddWithValue("@id_reserva", idReserva);
                cmd.Parameters.AddWithValue("@id_experiencia", idExperiencia);
                cmd.Parameters.AddWithValue("@estrelas", estrelas);

                string comentario = tb_comentario_testemunho.Text.Trim();
                cmd.Parameters.AddWithValue("@comentario",
                    string.IsNullOrEmpty(comentario) ? (object)DBNull.Value : comentario);

                SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outRetorno);

                con.Open();
                cmd.ExecuteNonQuery();

                int retorno = Convert.ToInt32(outRetorno.Value);

                if (retorno == 1)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "fecharModalERefresh",
                        "var m = bootstrap.Modal.getInstance(document.getElementById('modal_testemunho')); if(m) m.hide(); setTimeout(function(){ location.href = location.pathname + '?tab=reservas'; }, 400);",
                        true);
                    return;
                }
                else if (retorno == 0)
                {
                    lbl_mensagem_testemunho.Text = "Já avaliou esta experiência.";
                    lbl_mensagem_testemunho.CssClass = "alert alert-warning d-block mb-3";
                    lbl_mensagem_testemunho.Visible = true;
                }
                else
                {
                    lbl_mensagem_testemunho.Text = "Não foi possível enviar o testemunho. Apenas reservas concluídas podem ser avaliadas.";
                    lbl_mensagem_testemunho.CssClass = "alert alert-danger d-block mb-3";
                    lbl_mensagem_testemunho.Visible = true;
                }
            }
        }

        public bool Avaliada(string estado, object avaliadaObj)
        {
            bool avaliada = (avaliadaObj != null && avaliadaObj != DBNull.Value && Convert.ToInt32(avaliadaObj) == 1);
            return estado == "Concluida" && !avaliada;
        }

        protected void btn_toggle_2fa_Click(object sender, EventArgs e)
        {
            int idCliente = Convert.ToInt32(Session["cliente_id"]);
            CarregarDadosConta(idCliente);
            CarregarFidelizacao(idCliente);
            CarregarFavoritos(idCliente);
            bool estadoActual = btn_toggle_2fa.Text.Contains("Desativar");

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_actualizar_preferencia_2fa", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_cliente", idCliente);
                cmd.Parameters.AddWithValue("@activo", !estadoActual);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            CarregarDadosConta(idCliente);

            DefinirTab("#tab-perfil");
        }

        protected void btn_guardar_perfil_Click(object sender, EventArgs e)
        {
            int idCliente = Convert.ToInt32(Session["cliente_id"]);

            // O cliente só edita telefone, alergias e preferências de vinho.
            // Nome, segmento e notas_backoffice são preservados via SP.
            // @nome é passado da sessão — o cliente não altera o próprio nome na área pessoal.
            // @notas_backoffice é passado como NULL — a SP preserva o valor existente na BD.
            SqlConnection myConn = new SqlConnection(connStr);
            SqlCommand myCommand = new SqlCommand("sp_actualizar_cliente", myConn);
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.Parameters.AddWithValue("@id_cliente", idCliente);
            myCommand.Parameters.AddWithValue("@nome", Session["cliente_nome"].ToString());
            myCommand.Parameters.AddWithValue("@telefone", tb_telefone.Text.Trim());
            myCommand.Parameters.AddWithValue("@segmento_crm", Session["cliente_segmento"]?.ToString() ?? "Regular");
            myCommand.Parameters.AddWithValue("@notas_alergias", tb_alergias.Text.Trim());
            myCommand.Parameters.AddWithValue("@preferencias_vinho", tb_pref_vinhos.Text.Trim());
            myCommand.Parameters.Add(new SqlParameter("@notas_backoffice", SqlDbType.VarChar) { Value = DBNull.Value });
            myCommand.Parameters.AddWithValue("@id_utilizador", DBNull.Value);

            SqlParameter paramRetorno = new SqlParameter("@retorno", SqlDbType.Int);
            paramRetorno.Direction = ParameterDirection.Output;
            myCommand.Parameters.Add(paramRetorno);

            myConn.Open();
            myCommand.ExecuteNonQuery();
            myConn.Close();

            lbl_mensagem_perfil.Text = "Perfil actualizado com sucesso.";
            lbl_mensagem_perfil.CssClass = "alert alert-success d-block mb-4";
            lbl_mensagem_perfil.Visible = true;

            DefinirTab("#tab-perfil");
        }

        protected void btn_alterar_password_Click(object sender, EventArgs e)
        {
            lbl_mensagem_password.Visible = false;

            if (string.IsNullOrEmpty(tb_pw_actual.Text) ||
                string.IsNullOrEmpty(tb_pw_nova.Text) ||
                string.IsNullOrEmpty(tb_pw_confirmar.Text))
            {
                MostrarErroPw("Preencha todos os campos.");
                return;
            }

            if (tb_pw_nova.Text != tb_pw_confirmar.Text)
            {
                MostrarErroPw("A nova password e a confirmação não coincidem.");
                return;
            }

            if (tb_pw_nova.Text.Length < 6)
            {
                MostrarErroPw("A nova password deve ter pelo menos 6 caracteres.");
                return;
            }

            string email = lit_email.Text;
            string saltActual = "";

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmdSalt = new SqlCommand("sp_obter_salt_cliente", con);
                cmdSalt.CommandType = CommandType.StoredProcedure;
                cmdSalt.Parameters.AddWithValue("@email", email);

                SqlParameter outSalt = new SqlParameter("@salt", SqlDbType.VarChar);
                outSalt.Size = 100;
                outSalt.Direction = ParameterDirection.Output;
                cmdSalt.Parameters.Add(outSalt);

                SqlParameter outRetSalt = new SqlParameter("@retorno", SqlDbType.Int);
                outRetSalt.Direction = ParameterDirection.Output;
                cmdSalt.Parameters.Add(outRetSalt);

                con.Open();
                cmdSalt.ExecuteNonQuery();
                saltActual = outSalt.Value.ToString();
            }

            if (string.IsNullOrEmpty(saltActual))
            {
                MostrarErroPw("Não foi possível verificar a password actual.");
                return;
            }

            string hashActual = Seguranca.HashPassword(tb_pw_actual.Text, saltActual);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmdCheck = new SqlCommand("sp_obter_hash_salt_cliente", con);
                cmdCheck.CommandType = CommandType.StoredProcedure;
                cmdCheck.Parameters.AddWithValue("@id_cliente", Convert.ToInt32(Session["cliente_id"]));

                con.Open();
                SqlDataReader dr = cmdCheck.ExecuteReader();
                string hashBD = "";
                if (dr.Read())
                    hashBD = dr["password_hash"].ToString();

                if (string.IsNullOrEmpty(hashBD) || hashBD != hashActual)
                {
                    MostrarErroPw("A password actual está incorrecta.");
                    return;
                }
            }

            string novoSalt = Seguranca.GerarSalt();
            string novoHash = Seguranca.HashPassword(tb_pw_nova.Text, novoSalt);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_alterar_password_cliente", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_cliente", Convert.ToInt32(Session["cliente_id"]));
                cmd.Parameters.AddWithValue("@hash_nova", novoHash);
                cmd.Parameters.AddWithValue("@salt_novo", novoSalt);

                SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int);
                outRetorno.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outRetorno);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            tb_pw_actual.Text = "";
            tb_pw_nova.Text = "";
            tb_pw_confirmar.Text = "";

            lbl_mensagem_password.Text = "Password alterada com sucesso.";
            lbl_mensagem_password.CssClass = "alert alert-success d-block mb-3";
            lbl_mensagem_password.Visible = true;

            DefinirTab("#tab-perfil");
        }

        private void MostrarErroPw(string msg)
        {
            lbl_mensagem_password.Text = msg;
            lbl_mensagem_password.CssClass = "alert alert-danger d-block mb-3";
            lbl_mensagem_password.Visible = true;

            DefinirTab("#tab-perfil");
        }

        private void CarregarFidelizacao(int idCliente)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_obter_pontos_cliente", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_cliente", idCliente);
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                int pontos = 0;
                if (ds.Tables[0].Rows.Count > 0)
                    pontos = Convert.ToInt32(ds.Tables[0].Rows[0]["total_pontos"]);

                lit_pontos_total.Text = pontos.ToString();

                string nivelActual, nivelSeguinte, textoProximo;
                int progressoPct;

                if (pontos < 100)
                {
                    nivelActual = "Visitante"; nivelSeguinte = "Conhecedor";
                    progressoPct = (pontos * 100) / 100;
                    textoProximo = "Faltam " + (100 - pontos) + " pontos para Conhecedor";
                }
                else if (pontos < 300)
                {
                    nivelActual = "Conhecedor"; nivelSeguinte = "Sommelier";
                    progressoPct = ((pontos - 100) * 100) / 200;
                    textoProximo = "Faltam " + (300 - pontos) + " pontos para Sommelier";
                }
                else if (pontos < 600)
                {
                    nivelActual = "Sommelier"; nivelSeguinte = "Embaixador";
                    progressoPct = ((pontos - 300) * 100) / 300;
                    textoProximo = "Faltam " + (600 - pontos) + " pontos para Embaixador";
                }
                else
                {
                    nivelActual = "Embaixador"; nivelSeguinte = "";
                    progressoPct = 100;
                    textoProximo = "Nível máximo atingido. Obrigado pela fidelidade!";
                }

                lit_nivel.Text = nivelActual;
                lit_nivel_actual.Text = nivelActual;
                lit_nivel_seguinte.Text = nivelSeguinte;
                lit_pontos_proximo.Text = textoProximo;
                ProgressoPontos = progressoPct.ToString();

                // Sprint 3: promoção automática VIP + geração de cupão automático
                AtualizarSegmentoAutomatico(idCliente, pontos);
                GerarCupaoAutomaticoSeNecessario(idCliente);

                if (ds.Tables.Count > 1)
                {
                    gv_pontos.DataSource = ds.Tables[1];
                    gv_pontos.DataBind();
                }
            }
        }

        private void CarregarFavoritos(int idCliente)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_listar_favoritos_cliente", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_cliente", idCliente);
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rpt_favoritos.DataSource = dt;
                rpt_favoritos.DataBind();

                lbl_sem_favoritos.Visible = (dt.Rows.Count == 0);
            }
        }

        protected void gv_reservas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idCliente = Convert.ToInt32(Session["cliente_id"]);

            // ─────────────────────────────────────────────────────
            // AVALIAR EXPERIÊNCIA
            // ─────────────────────────────────────────────────────
            if (e.CommandName == "AvaliarExperiencia")
            {
                string[] partes = e.CommandArgument.ToString().Split('|');
                int idReserva = int.Parse(partes[0]);
                int idExperiencia = int.Parse(partes[1]);

                hf_id_reserva_testemunho.Value = idReserva.ToString();
                hf_id_experiencia_testemunho.Value = idExperiencia.ToString();
                hf_id_experiencia_testemunho.Value = idExperiencia.ToString();

                // Recarregar fidelização para manter a barra de pontos visível
                CarregarFidelizacao(idCliente);

                DefinirTab("#tab-reservas");
                ViewState["abrirModalTestemunho"] = "1";
                return;
            }
                // ─────────────────────────────────────────────────────
                // CANCELAR RESERVA
                // ─────────────────────────────────────────────────────
                if (e.CommandName != "CancelarReserva") return;

                int idReservaCancelar = Convert.ToInt32(e.CommandArgument);
                int idClienteCancelar = Convert.ToInt32(Session["cliente_id"]);

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    SqlCommand cmd = new SqlCommand("sp_cancelar_reserva", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_reserva", idReservaCancelar);
                    cmd.Parameters.AddWithValue("@id_cliente", idClienteCancelar);

                    SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int);
                    outRetorno.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outRetorno);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    int retorno = Convert.ToInt32(outRetorno.Value);
                    lbl_mensagem.Text = retorno == 1
                        ? "Reserva cancelada com sucesso."
                        : "Não foi possível cancelar esta reserva.";
                    lbl_mensagem.CssClass = retorno == 1
                        ? "alert alert-success d-block mb-4"
                        : "alert alert-danger d-block mb-4";
                    lbl_mensagem.Visible = true;
                }

                CarregarDadosConta(idCliente);

                DefinirTab("#tab-reservas");
            }

        protected bool PodeCancelar(string estado, string dataHoraStr)
        {
            if (estado == "Cancelada" || estado == "Concluida") return false;
            DateTime dataHora;
            return DateTime.TryParse(dataHoraStr, out dataHora) && dataHora > DateTime.Now;
        }

        protected string GetEstadoBadge(string estado)
        {
            switch (estado)
            {
                case "Confirmada": return "badge bg-success";
                case "Pendente": return "badge bg-warning text-dark";
                case "Cancelada": return "badge bg-danger";
                case "Concluida": return "badge bg-secondary";
                default: return "badge bg-light text-dark";
            }
        }

        // ─────────────────────────────────────────────────────
        // REMOVER FAVORITO
        // ─────────────────────────────────────────────────────
        protected void rpt_favoritos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "RemoverFavorito") return;
            if (Session["cliente_id"] == null) return;

            int idVinho = int.Parse(e.CommandArgument.ToString());
            int idCliente = Convert.ToInt32(Session["cliente_id"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_toggle_favorito", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_cliente", idCliente);
                cmd.Parameters.AddWithValue("@id_vinho", idVinho);

                SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int);
                outRetorno.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outRetorno);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            CarregarFidelizacao(idCliente);
            CarregarFavoritos(idCliente);

            DefinirTab("#tab-favoritos");
        }

        // ─────────────────────────────────────────────────────
        // CARREGAR CUPÕES (Sprint 3)
        // ─────────────────────────────────────────────────────
        private void CarregarCupoes(int idCliente)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_listar_cupoes_cliente", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_cliente", idCliente);

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rpt_cupoes_disponiveis.DataSource = dt;
                rpt_cupoes_disponiveis.DataBind();
                lbl_sem_cupoes.Visible = (dt.Rows.Count == 0);
            }

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_listar_cupoes_usados_cliente", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_cliente", idCliente);

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gv_cupoes_usados.DataSource = dt;
                gv_cupoes_usados.DataBind();
            }
        }

        // ─────────────────────────────────────────────────────
        // CARREGAR OFERTAS B2B (Sprint 3)
        // ─────────────────────────────────────────────────────
        private void CarregarOfertasB2B()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_listar_ofertas_b2b", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rpt_ofertas_b2b.DataSource = dt;
                rpt_ofertas_b2b.DataBind();
                lbl_sem_ofertas_b2b.Visible = (dt.Rows.Count == 0);
            }
        }

        // ─────────────────────────────────────────────────────
        // PROMOÇÃO AUTOMÁTICA DE SEGMENTO (Sprint 3)
        // ─────────────────────────────────────────────────────
        private void AtualizarSegmentoAutomatico(int idCliente, int pontos)
        {
            if (pontos < 300) return;

            string segmentoActual = Session["cliente_segmento"]?.ToString() ?? "";

            if (segmentoActual == "B2B" || segmentoActual == "VIP") return;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_actualizar_segmento_automatico", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_cliente", idCliente);
                cmd.Parameters.AddWithValue("@segmento_novo", "VIP");

                SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int);
                outRetorno.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outRetorno);

                con.Open();
                cmd.ExecuteNonQuery();

                Session["cliente_segmento"] = "VIP";
            }
        }

        // ─────────────────────────────────────────────────────
        // GERAÇÃO AUTOMÁTICA DE CUPÕES (Sprint 3)
        // ─────────────────────────────────────────────────────
        private void GerarCupaoAutomaticoSeNecessario(int idCliente)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_gerar_cupao_automatico", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_cliente", idCliente);

                SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int);
                outRetorno.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outRetorno);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        protected void btn_sair_Click(object sender, EventArgs e)
        {
            Session.Remove("cliente_id");
            Session.Remove("cliente_nome");
            Response.Redirect("~/Pages/conta_login.aspx");
        }
        // ── Helpers de navegação de tab ───────────────────────────
        // Escreve o ID da tab alvo no HiddenField — o script do
        // ContentPlaceHolder scripts lê-o após o Bootstrap carregar.
        private void DefinirTab(string tabId)
        {
            hf_tab_activa.Value = tabId;
        }

    }
}