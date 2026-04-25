using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RaizesDigitais.Pages
{
    public partial class reserva_v2 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarExperiencias();

                // Se vier com ?id=X da página de experiências ou da aba B2B, pré-seleciona
                string idQueryString = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(idQueryString))
                {
                    ddl_experiencia.SelectedValue = idQueryString;
                }

                CarregarDisponibilidade();

                // Verificar segmento do cliente
                int idClienteLogado = 0;
                string segmentoCliente = "";

                if (Session["cliente_id"] != null)
                {
                    idClienteLogado = Convert.ToInt32(Session["cliente_id"]);
                    ViewState["id_cliente_temp"] = idClienteLogado;
                    segmentoCliente = Session["cliente_segmento"]?.ToString() ?? "";

                    // Pré-preencher nome e email
                    if (Session["cliente_nome"] != null)
                        tb_nome.Text = Session["cliente_nome"].ToString();

                    PreencherEmailCliente(idClienteLogado);

                    // Se for B2B, guardar em ViewState e mostrar banner
                    if (segmentoCliente == "B2B")
                    {
                        ViewState["segmento_b2b"] = true;
                        lbl_banner_b2b.Visible = true;
                    }
                }

                // Carregar cupões se cliente está logado
                if (idClienteLogado > 0)
                {
                    CarregarCupoes(idClienteLogado);
                    ViewState["cupoes_carregados"] = true;
                }

                AtualizarPreco();
            }
        }

        // ==========================
        // Carregar dados cliente
        // ==========================
        // Obtem email para preencher.
        private void PreencherEmailCliente(int idCliente)
        {
            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_obter_email_cliente", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_cliente", idCliente);

                SqlParameter outEmail = new SqlParameter("@email", SqlDbType.VarChar);
                outEmail.Size = 200;
                outEmail.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outEmail);

                con.Open();
                cmd.ExecuteNonQuery();

                if (outEmail.Value != DBNull.Value)
                    tb_email.Text = outEmail.Value.ToString();
            }
        }

        // ==========================
        // Carregar cupoes
        // ==========================
        // Lista validos do cliente.
        private void CarregarCupoes(int idCliente)
        {
            ddl_cupao.Items.Clear();
            ddl_cupao.Items.Add(new ListItem("— Sem cupão —", "0"));

            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_listar_cupoes_cliente", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_cliente", idCliente);

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    string codigo = row["codigo"].ToString();
                    string descricao = row["descricao_desconto"].ToString();
                    DateTime validade = Convert.ToDateTime(row["validade"]);
                    string texto = $"{codigo} — {descricao} (válido até {validade:dd/MM/yyyy})";

                    ddl_cupao.Items.Add(new ListItem(texto, row["id_cupao"].ToString()));
                }
            }

            // Recalcular preço após carregar cupões
            RecalcularPrecoComCupao();
        }

        private void CarregarExperiencias()
        {
            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_listar_experiencias", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddl_experiencia.DataSource = dt;
                ddl_experiencia.DataTextField = "nome";
                ddl_experiencia.DataValueField = "id_experiencia";
                ddl_experiencia.DataBind();

                ddl_experiencia.Items.Insert(0, new ListItem("— Selecione uma experiência —", "0"));
            }
        }

        private void CarregarDisponibilidade()
        {
            ddl_disponibilidade.Items.Clear();

            if (ddl_experiencia.SelectedValue == "0")
            {
                ddl_disponibilidade.Items.Add(new ListItem("— Selecione primeiro a experiência —", "0"));
                lbl_vagas.Text = "";
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_listar_disponibilidade", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_experiencia", int.Parse(ddl_experiencia.SelectedValue));

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    ddl_disponibilidade.Items.Add(new ListItem("Sem datas disponíveis", "0"));
                    lbl_vagas.Text = "Não há sessões disponíveis para esta experiência.";
                    return;
                }

                foreach (DataRow row in dt.Rows)
                {
                    DateTime dataHora = Convert.ToDateTime(row["data_hora"]);
                    int vagas = Convert.ToInt32(row["vagas_disponiveis"]);
                    string texto = dataHora.ToString("dd/MM/yyyy") + " às " + dataHora.ToString("HH:mm") + "h  (" + vagas + " vagas)";
                    ddl_disponibilidade.Items.Add(new ListItem(texto, row["id_disponibilidade"].ToString()));
                }

                AtualizarVagasInfo();
            }
        }

        private void AtualizarVagasInfo()
        {
            if (ddl_disponibilidade.SelectedValue == "0") return;

            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_obter_vagas_disponibilidade", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_disponibilidade", int.Parse(ddl_disponibilidade.SelectedValue));

                SqlParameter outVagas = new SqlParameter("@vagas", SqlDbType.Int);
                outVagas.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outVagas);

                con.Open();
                cmd.ExecuteNonQuery();

                if (outVagas.Value != DBNull.Value)
                    lbl_vagas.Text = "Vagas disponíveis: " + outVagas.Value.ToString();
            }
        }

        /// <summary>
        /// Atualiza o preço usando sp_obter_preco_reserva (100% SP — desconto B2B calculado na BD).
        /// </summary>
        private void AtualizarPreco()
        {
            pnl_desconto_b2b_passo1.Visible = false;

            if (ddl_experiencia.SelectedValue == "0")
            {
                lbl_preco_unitario.Text = "—";
                lbl_num_pessoas_resumo.Text = "—";
                lbl_preco_total.Text = "—";
                return;
            }

            int idExp = int.Parse(ddl_experiencia.SelectedValue);
            int numPessoas = 1;
            int.TryParse(tb_pessoas.Text, out numPessoas);
            if (numPessoas < 1) numPessoas = 1;

            int idCliente = ViewState["id_cliente_temp"] != null ? (int)ViewState["id_cliente_temp"] : 0;

            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_obter_preco_reserva", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_experiencia", idExp);
                cmd.Parameters.AddWithValue("@id_cliente", idCliente > 0 ? (object)idCliente : DBNull.Value);
                cmd.Parameters.AddWithValue("@num_pessoas", numPessoas);

                SqlParameter outPrecoPessoa = new SqlParameter("@preco_por_pessoa", SqlDbType.Decimal);
                outPrecoPessoa.Precision = 8; outPrecoPessoa.Scale = 2;
                outPrecoPessoa.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outPrecoPessoa);

                SqlParameter outPrecoOriginal = new SqlParameter("@preco_original", SqlDbType.Decimal);
                outPrecoOriginal.Precision = 8; outPrecoOriginal.Scale = 2;
                outPrecoOriginal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outPrecoOriginal);

                SqlParameter outPrecoFinal = new SqlParameter("@preco_final", SqlDbType.Decimal);
                outPrecoFinal.Precision = 8; outPrecoFinal.Scale = 2;
                outPrecoFinal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outPrecoFinal);

                SqlParameter outDescontoB2b = new SqlParameter("@desconto_b2b", SqlDbType.Decimal);
                outDescontoB2b.Precision = 5; outDescontoB2b.Scale = 2;
                outDescontoB2b.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outDescontoB2b);

                SqlParameter outValorDesconto = new SqlParameter("@valor_desconto", SqlDbType.Decimal);
                outValorDesconto.Precision = 8; outValorDesconto.Scale = 2;
                outValorDesconto.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outValorDesconto);

                SqlParameter outB2bAplicado = new SqlParameter("@b2b_aplicado", SqlDbType.Bit);
                outB2bAplicado.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outB2bAplicado);

                SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int);
                outRetorno.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outRetorno);

                con.Open();
                cmd.ExecuteNonQuery();

                if (outRetorno.Value != DBNull.Value && Convert.ToInt32(outRetorno.Value) == 1)
                {
                    decimal precoPessoa = Convert.ToDecimal(outPrecoPessoa.Value);
                    decimal precoOriginal = Convert.ToDecimal(outPrecoOriginal.Value);
                    decimal precoFinal = Convert.ToDecimal(outPrecoFinal.Value);
                    bool b2bAplicado = outB2bAplicado.Value != DBNull.Value && Convert.ToBoolean(outB2bAplicado.Value);

                    lbl_preco_unitario.Text = precoPessoa.ToString("0.00") + " €";
                    lbl_num_pessoas_resumo.Text = numPessoas.ToString();
                    lbl_preco_total.Text = precoFinal.ToString("0.00") + " €";

                    if (b2bAplicado)
                    {
                        decimal descontoPct = Convert.ToDecimal(outDescontoB2b.Value);
                        decimal valorDesc = Convert.ToDecimal(outValorDesconto.Value);

                        pnl_desconto_b2b_passo1.Visible = true;
                        lbl_pct_b2b_passo1.Text = descontoPct.ToString("0.##");
                        lbl_valor_b2b_passo1.Text = "- " + valorDesc.ToString("0.00") + " €";
                    }

                    ViewState["preco_unitario"] = precoPessoa;
                    ViewState["preco_original"] = precoOriginal;
                    ViewState["preco_total"] = precoFinal;

                    // Guardar dados B2B para reutilização nos outros passos
                    if (b2bAplicado)
                    {
                        ViewState["desconto_b2b"] = Convert.ToDecimal(outDescontoB2b.Value);
                        ViewState["valor_desconto_b2b"] = Convert.ToDecimal(outValorDesconto.Value);
                    }
                    else
                    {
                        ViewState["desconto_b2b"] = null;
                        ViewState["valor_desconto_b2b"] = null;
                    }
                }
            }
        }

        // ==========================
        // Eventos controlos
        // ==========================

        protected void ddl_experiencia_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarregarDisponibilidade();
            AtualizarPreco();

            // Resetar cupao se muda experiencia
            ddl_cupao.SelectedValue = "0";
        }

        protected void ddl_disponibilidade_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtualizarVagasInfo();
        }

        protected void tb_pessoas_TextChanged(object sender, EventArgs e)
        {
            AtualizarPreco();
        }

        protected void ddl_cupao_SelectedIndexChanged(object sender, EventArgs e)
        {
            RecalcularPrecoComCupao();
        }

        // ==========================
        // Recalcular com cupao
        // ==========================
        // Valida na bd. Aplica desconto.
        private void RecalcularPrecoComCupao()
        {
            decimal precoBase = ViewState["preco_total"] != null ? (decimal)ViewState["preco_total"] : 0;
            decimal precoOriginal = ViewState["preco_original"] != null ? (decimal)ViewState["preco_original"] : precoBase;

            // Info b2b no passo 2
            if (ViewState["desconto_b2b"] != null)
            {
                pnl_b2b_info.Visible = true;
                lbl_pct_b2b_passo2.Text = ViewState["desconto_b2b"].ToString();

                pnl_desconto_b2b_passo2.Visible = true;
                lbl_pct_b2b_resumo.Text = ViewState["desconto_b2b"].ToString();
                lbl_valor_b2b_passo2.Text = "- " + ViewState["valor_desconto_b2b"].ToString() + " €";
            }
            else
            {
                pnl_b2b_info.Visible = false;
                pnl_desconto_b2b_passo2.Visible = false;
            }

            lbl_preco_original_passo2.Text = precoOriginal.ToString("0.00") + " €";

            // Sem cupao
            if (ddl_cupao.SelectedValue == "0")
            {
                pnl_desconto_aplicado.Visible = false;
                lbl_cupao_info.Text = "";
                lbl_preco_final_passo2.Text = precoBase.ToString("0.00") + " €";
                ViewState["preco_final"] = precoBase;
                ViewState["id_cupao"] = null;
                return;
            }

            int idCliente = ViewState["id_cliente_temp"] != null ? (int)ViewState["id_cliente_temp"] : 0;
            int idCupao = int.Parse(ddl_cupao.SelectedValue);

            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_aplicar_cupao_reserva", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id_cliente", idCliente);
                cmd.Parameters.AddWithValue("@id_cupao", idCupao);
                cmd.Parameters.AddWithValue("@preco_original", precoBase);

                SqlParameter outPrecoFinal = new SqlParameter("@preco_final", SqlDbType.Decimal);
                outPrecoFinal.Precision = 10;
                outPrecoFinal.Scale = 2;
                outPrecoFinal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outPrecoFinal);

                SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int);
                outRetorno.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outRetorno);

                con.Open();
                cmd.ExecuteNonQuery();

                int retorno = (int)outRetorno.Value;
                decimal precoFinal = (decimal)outPrecoFinal.Value;

                if (retorno == 1)
                {
                    // Valido. Aplica sobre b2b.
                    decimal descontoCupao = precoBase - precoFinal;

                    lbl_preco_final_passo2.Text = precoFinal.ToString("0.00") + " €";
                    lbl_desconto_cupao.Text = "- " + descontoCupao.ToString("0.00") + " €";
                    pnl_desconto_aplicado.Visible = true;
                    lbl_cupao_info.Text = "✓ Cupão aplicado com sucesso!";
                    lbl_cupao_info.CssClass = "form-text text-success";

                    ViewState["preco_final"] = precoFinal;
                    ViewState["id_cupao"] = idCupao;
                    ViewState["codigo_cupao"] = ddl_cupao.SelectedItem.Text.Split('—')[0].Trim();
                }
                else
                {
                    // Invalido
                    pnl_desconto_aplicado.Visible = false;
                    ViewState["preco_final"] = precoBase;
                    ViewState["id_cupao"] = null;

                    string mensagemErro = "";
                    switch (retorno)
                    {
                        case -1: mensagemErro = "✗ Cupão não encontrado."; break;
                        case -2: mensagemErro = "✗ Este cupão já foi utilizado."; break;
                        case -3: mensagemErro = "✗ Cupão expirado."; break;
                        case -4: mensagemErro = "✗ Este cupão não está disponível para si."; break;
                        default: mensagemErro = "✗ Erro ao validar cupão."; break;
                    }

                    lbl_cupao_info.Text = mensagemErro;
                    lbl_cupao_info.CssClass = "form-text text-danger";

                    // Resetar ddl
                    ddl_cupao.SelectedValue = "0";
                }
            }

            up_cupao.Update();
        }

        // ==========================
        // Navegacao passos
        // ==========================

        protected void btn_passo1_Click(object sender, EventArgs e)
        {
            // Validações do Passo 1
            if (ddl_experiencia.SelectedValue == "0")
            {
                lbl_erro.Text = "Por favor, selecione uma experiência.";
                lbl_erro.Visible = true;
                return;
            }
            if (ddl_disponibilidade.SelectedValue == "0")
            {
                lbl_erro.Text = "Por favor, selecione uma data disponível.";
                lbl_erro.Visible = true;
                return;
            }

            int numPessoas;
            if (!int.TryParse(tb_pessoas.Text, out numPessoas) || numPessoas < 1)
            {
                lbl_erro.Text = "Por favor, indique um número válido de pessoas.";
                lbl_erro.Visible = true;
                return;
            }

            lbl_erro.Visible = false;

            // Guardar dados do Passo 1 em ViewState
            ViewState["id_experiencia"] = ddl_experiencia.SelectedValue;
            ViewState["id_disponibilidade"] = ddl_disponibilidade.SelectedValue;
            ViewState["num_pessoas"] = numPessoas;
            ViewState["experiencia_nome"] = ddl_experiencia.SelectedItem.Text;
            ViewState["disponibilidade_texto"] = ddl_disponibilidade.SelectedItem.Text;

            // Preencher preço no Passo 2 (já com desconto B2B se aplicável)
            decimal precoFinal = ViewState["preco_total"] != null ? (decimal)ViewState["preco_total"] : 0;
            decimal precoOriginal = ViewState["preco_original"] != null ? (decimal)ViewState["preco_original"] : precoFinal;

            lbl_preco_original_passo2.Text = precoOriginal.ToString("0.00") + " €";
            lbl_preco_final_passo2.Text = precoFinal.ToString("0.00") + " €";
            ViewState["preco_final"] = precoFinal;

            // Mostrar info B2B no passo 2
            if (ViewState["desconto_b2b"] != null)
            {
                pnl_b2b_info.Visible = true;
                lbl_pct_b2b_passo2.Text = ViewState["desconto_b2b"].ToString();

                pnl_desconto_b2b_passo2.Visible = true;
                lbl_pct_b2b_resumo.Text = ViewState["desconto_b2b"].ToString();
                lbl_valor_b2b_passo2.Text = "- " + ViewState["valor_desconto_b2b"].ToString() + " €";
            }
            else
            {
                pnl_b2b_info.Visible = false;
                pnl_desconto_b2b_passo2.Visible = false;
            }

            // Avançar para Passo 2
            MostrarPasso(2);
        }

        protected void btn_voltar_passo1_Click(object sender, EventArgs e)
        {
            lbl_erro.Visible = false;
            ViewState["cupoes_carregados"] = null;
            ViewState["id_cupao"] = null;
            ViewState["codigo_cupao"] = null;
            ViewState["id_cliente_temp"] = null;
            ViewState["segmento_b2b"] = null;
            MostrarPasso(1);
        }

        protected void btn_passo2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_nome.Text))
            {
                lbl_erro.Text = "Por favor, introduza o seu nome.";
                lbl_erro.Visible = true;
                return;
            }
            if (string.IsNullOrWhiteSpace(tb_email.Text))
            {
                lbl_erro.Text = "Por favor, introduza o seu email.";
                lbl_erro.Visible = true;
                return;
            }

            lbl_erro.Visible = false;

            // Verificar se é cliente existente e obter ID
            int idCliente = ObterIdClientePorEmail(tb_email.Text.Trim());

            if (idCliente > 0)
            {
                ViewState["id_cliente_temp"] = idCliente;

                // Verificar se é cliente B2B
                string segmento = VerificarSegmentoCliente(idCliente);
                if (segmento == "B2B")
                {
                    ViewState["segmento_b2b"] = true;
                }

                // Se cupões ainda não foram carregados nesta sessão, carregar agora
                if (ViewState["cupoes_carregados"] == null)
                {
                    CarregarCupoes(idCliente);
                    ViewState["cupoes_carregados"] = true;

                    // Só bloqueia se houver cupões disponíveis para mostrar ao utilizador
                    if (ddl_cupao.Items.Count > 1)
                    {
                        lbl_cupao_info.Text = "Tem cupões disponíveis! Selecione um para aplicar desconto.";
                        lbl_cupao_info.CssClass = "form-text text-info";
                        return;
                    }
                }
            }

            // Preencher resumo do Passo 3
            lbl_resumo_experiencia.Text = ViewState["experiencia_nome"]?.ToString();
            lbl_resumo_data.Text = ViewState["disponibilidade_texto"]?.ToString();
            lbl_resumo_pessoas.Text = ViewState["num_pessoas"]?.ToString() + " pessoa(s)";
            lbl_resumo_nome.Text = tb_nome.Text.Trim();
            lbl_resumo_email.Text = tb_email.Text.Trim();

            decimal total = ViewState["preco_final"] != null ? (decimal)ViewState["preco_final"] :
                           (ViewState["preco_total"] != null ? (decimal)ViewState["preco_total"] : 0);
            lbl_resumo_total.Text = total.ToString("0.00") + " €";

            // Mostrar desconto B2B no resumo
            if (ViewState["desconto_b2b"] != null && ViewState["valor_desconto_b2b"] != null)
            {
                decimal valorB2b = (decimal)ViewState["valor_desconto_b2b"];
                decimal pctB2b = (decimal)ViewState["desconto_b2b"];
                lbl_resumo_b2b.Text = $"-{pctB2b.ToString("0.##")}% ({valorB2b.ToString("0.00")} €)";
                pnl_resumo_b2b.Visible = true;
            }
            else
            {
                pnl_resumo_b2b.Visible = false;
            }

            // Mostrar cupão aplicado no resumo
            if (ViewState["id_cupao"] != null && ViewState["codigo_cupao"] != null)
            {
                lbl_resumo_cupao.Text = ViewState["codigo_cupao"].ToString();
                pnl_resumo_cupao.Visible = true;
            }
            else
            {
                pnl_resumo_cupao.Visible = false;
            }

            MostrarPasso(3);
        }

        protected void btn_voltar_passo2_Click(object sender, EventArgs e)
        {
            lbl_erro.Visible = false;
            MostrarPasso(2);
        }

        protected void btn_confirmar_Click(object sender, EventArgs e)
        {
            int idCliente = ObterOuCriarCliente(tb_nome.Text.Trim(), tb_email.Text.Trim(), tb_telefone.Text.Trim());

            if (idCliente <= 0)
            {
                lbl_erro.Text = "Erro ao processar cliente.";
                lbl_erro.Visible = true;
                return;
            }

            string numReserva = "";
            int retorno = InserirReserva(idCliente, out numReserva);

            if (retorno == 1)
            {
                lbl_num_reserva.Text = numReserva;

                // Copiar notas da reserva para o perfil do cliente (só se estiver vazio)
                if (!string.IsNullOrWhiteSpace(tb_notas.Text))
                {
                    string connStrNotas = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;
                    using (SqlConnection conNotas = new SqlConnection(connStrNotas))
                    {
                        SqlCommand cmdNotas = new SqlCommand("sp_actualizar_notas_cliente_reserva", conNotas);
                        cmdNotas.CommandType = CommandType.StoredProcedure;
                        cmdNotas.Parameters.AddWithValue("@id_cliente", idCliente);
                        cmdNotas.Parameters.AddWithValue("@notas_alergias", tb_notas.Text.Trim());
                        conNotas.Open();
                        cmdNotas.ExecuteNonQuery();
                    }
                }

                try
                {
                    // Usar preço final (com B2B + cupão aplicado)
                    decimal precoFinal = ViewState["preco_final"] != null ? (decimal)ViewState["preco_final"] :
                                        (ViewState["preco_total"] != null ? (decimal)ViewState["preco_total"] : 0);

                    // Gera o PDF com os dados correctos
                    byte[] pdfBytes = GeradorPDF.GerarConfirmacaoReserva(
                        numReserva,
                        ViewState["experiencia_nome"].ToString(),
                        ViewState["disponibilidade_texto"].ToString(),
                        ViewState["num_pessoas"].ToString(),
                        tb_nome.Text.Trim(),
                        tb_email.Text.Trim(),
                        precoFinal.ToString("0.00") + " €"
                    );

                    // Envia o email com o PDF em anexo
                    Email.EnviarConfirmacaoReserva(
                        tb_email.Text.Trim(),
                        tb_nome.Text.Trim(),
                        ViewState["experiencia_nome"].ToString(),
                        DateTime.Now,
                        Convert.ToInt32(ViewState["num_pessoas"]),
                        precoFinal,
                        numReserva,
                        pdfBytes
                    );
                }
                catch (Exception)
                {
                    lbl_erro.Text = "Ocorreu um erro ao enviar o email de confirmação. A sua reserva foi registada.";
                    lbl_erro.Visible = true;
                }

                MostrarPasso(0); // Painel de sucesso
            }
        }

        // ─────────────────────────────────────────────────────────
        // LÓGICA DE NEGÓCIO
        // ─────────────────────────────────────────────────────────

        /// <summary>
        /// Verifica o segmento CRM do cliente via SP (100% SP, sem SQL inline).
        /// </summary>
        private string VerificarSegmentoCliente(int idCliente)
        {
            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_obter_segmento_cliente", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_cliente", idCliente);

                SqlParameter outSegmento = new SqlParameter("@segmento_crm", SqlDbType.VarChar);
                outSegmento.Size = 20;
                outSegmento.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outSegmento);

                SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int);
                outRetorno.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outRetorno);

                con.Open();
                cmd.ExecuteNonQuery();

                return outRetorno.Value != DBNull.Value && Convert.ToInt32(outRetorno.Value) == 1
                    ? (outSegmento.Value != DBNull.Value ? outSegmento.Value.ToString() : "")
                    : "";
            }
        }

        private int ObterIdClientePorEmail(string email)
        {
            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_obter_id_cliente_por_email", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@email", email);

                SqlParameter outId = new SqlParameter("@id_cliente", SqlDbType.Int);
                outId.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outId);

                SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int);
                outRetorno.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outRetorno);

                con.Open();
                cmd.ExecuteNonQuery();

                return outRetorno.Value != DBNull.Value && Convert.ToInt32(outRetorno.Value) == 1
                    ? Convert.ToInt32(outId.Value) : 0;
            }
        }

        private int ObterOuCriarCliente(string nome, string email, string telefone)
        {
            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_obter_ou_criar_cliente", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@telefone", string.IsNullOrEmpty(telefone) ? (object)DBNull.Value : telefone);

                string meuSalt = Seguranca.GerarSalt();
                string minhaHash = Seguranca.HashPassword("Raizes2026!", meuSalt);
                cmd.Parameters.AddWithValue("@hash", minhaHash);
                cmd.Parameters.AddWithValue("@salt", meuSalt);

                SqlParameter outIdCliente = new SqlParameter("@id_cliente_out", SqlDbType.Int);
                outIdCliente.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outIdCliente);

                SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int);
                outRetorno.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outRetorno);

                con.Open();
                cmd.ExecuteNonQuery();

                int retorno = outRetorno.Value != DBNull.Value ? Convert.ToInt32(outRetorno.Value) : 0;
                int idCliente = outIdCliente.Value != DBNull.Value ? Convert.ToInt32(outIdCliente.Value) : 0;

                if (retorno == 1)
                    try { Email.EnviarBoasVindas(email, nome); } catch { }

                return idCliente;
            }
        }

        private int InserirReserva(int idCliente, out string numReserva)
        {
            numReserva = "";
            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_inserir_reserva", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // Usar o preço final (com B2B + cupão aplicado)
                decimal precoFinal = ViewState["preco_final"] != null ? (decimal)ViewState["preco_final"] :
                                    (ViewState["preco_total"] != null ? (decimal)ViewState["preco_total"] : 0);

                cmd.Parameters.AddWithValue("@id_cliente", idCliente);
                cmd.Parameters.AddWithValue("@id_disponibilidade", int.Parse(ViewState["id_disponibilidade"].ToString()));
                cmd.Parameters.AddWithValue("@num_pessoas", int.Parse(ViewState["num_pessoas"].ToString()));
                cmd.Parameters.AddWithValue("@preco_total", precoFinal);
                cmd.Parameters.AddWithValue("@notas", string.IsNullOrEmpty(tb_notas.Text) ? (object)DBNull.Value : tb_notas.Text.Trim());

                // Adicionar id_cupao se foi aplicado
                if (ViewState["id_cupao"] != null)
                {
                    cmd.Parameters.AddWithValue("@id_cupao", (int)ViewState["id_cupao"]);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@id_cupao", DBNull.Value);
                }

                SqlParameter outNumReserva = new SqlParameter("@num_reserva", SqlDbType.VarChar, 20);
                outNumReserva.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outNumReserva);

                SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int);
                outRetorno.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outRetorno);

                con.Open();
                cmd.ExecuteNonQuery();

                numReserva = outNumReserva.Value != DBNull.Value ? outNumReserva.Value.ToString() : "";
                return outRetorno.Value != DBNull.Value ? Convert.ToInt32(outRetorno.Value) : -1;
            }
        }

        // ─────────────────────────────────────────────────────────
        // CONTROLO DE VISIBILIDADE DOS PAINÉIS
        // ─────────────────────────────────────────────────────────

        private void MostrarPasso(int passo)
        {
            pnl_passo1.Visible = (passo == 1);
            pnl_passo2.Visible = (passo == 2);
            pnl_passo3.Visible = (passo == 3);
            pnl_sucesso.Visible = (passo == 0);

            // Atualizar indicador
            AtualizarIndicador(passo);
        }

        private void AtualizarIndicador(int passo)
        {
            string estiloAtivo = "background-color: var(--cor-primaria); color: white; font-size: 0.85rem;";
            string estiloInativo = "background-color: var(--cor-neutro); color: var(--cor-texto-leve); font-size: 0.85rem;";

            lbl_passo1_indicador.Style.Value = passo == 1 ? estiloAtivo : estiloInativo;
            lbl_passo2_indicador.Style.Value = passo == 2 ? estiloAtivo : estiloInativo;
            lbl_passo3_indicador.Style.Value = passo == 3 ? estiloAtivo : estiloInativo;
        }
    }
}
