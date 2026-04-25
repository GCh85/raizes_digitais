<%@ Page Title="Reservar | Quinta da Azenha" Language="C#" MasterPageFile="~/MasterSite.Master" AutoEventWireup="true" CodeBehind="reserva_v2.aspx.cs" Inherits="RaizesDigitais.Pages.reserva_v2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />

    <!-- CABEÇALHO -->
    <div class="text-center py-5" style="margin-top: 56px; background-color: var(--cor-texto); border-bottom: 2px solid var(--cor-destaque);">
        <div class="container">
            <h1 class="fw-normal text-white">Fazer Reserva</h1>
            <p class="fst-italic" style="color: var(--cor-destaque);">Reserve a sua experiência na Quinta da Azenha</p>
            <!-- Banner B2B visível apenas para clientes B2B -->
            <asp:Label ID="lbl_banner_b2b" runat="server" Visible="false"
                CssClass="badge px-3 py-2 mt-2 d-inline-block"
                style="background-color: var(--cor-destaque); color: white; font-size: 0.9rem; letter-spacing: 0.05em;">
                <i class="bi bi-briefcase me-1"></i>Desconto B2B ativo
            </asp:Label>
        </div>
    </div>

    <main class="py-5">
        <div class="container">
            <div class="row justify-content-center">
                <div class="col-lg-7">

                    <!-- INDICADOR DE PASSOS -->
                    <div class="d-flex justify-content-center mb-5">
                        <div class="d-flex align-items-center gap-2">
                            <asp:Label ID="lbl_passo1_indicador" runat="server"
                                CssClass="badge rounded-pill px-3 py-2"
                                style="background-color: var(--cor-primaria); color: white; font-size: 0.85rem;">
                                1 · Experiência
                            </asp:Label>
                            <span class="text-muted">›</span>
                            <asp:Label ID="lbl_passo2_indicador" runat="server"
                                CssClass="badge rounded-pill px-3 py-2"
                                style="background-color: var(--cor-neutro); color: var(--cor-texto-leve); font-size: 0.85rem;">
                                2 · Os seus dados
                            </asp:Label>
                            <span class="text-muted">›</span>
                            <asp:Label ID="lbl_passo3_indicador" runat="server"
                                CssClass="badge rounded-pill px-3 py-2"
                                style="background-color: var(--cor-neutro); color: var(--cor-texto-leve); font-size: 0.85rem;">
                                3 · Confirmação
                            </asp:Label>
                        </div>
                    </div>

                    <!-- MENSAGEM DE ERRO GERAL -->
                    <asp:Label ID="lbl_erro" runat="server" CssClass="lbl-erro d-block mb-3" Visible="false" />

                    <!-- ═══════════════════════════════════════════
                         PASSO 1 — Escolha da experiência e data
                         ═══════════════════════════════════════════ -->
                    <asp:Panel ID="pnl_passo1" runat="server">
                        <span class="label-secao">Passo 1 de 3</span>
                        <h2 class="mb-0">Escolha a Experiência</h2>
                        <div class="linha-verde-esq"></div>

                        <!-- Dropdown de experiências -->
                        <div class="mb-3">
                            <label class="form-label fw-bold small text-uppercase" style="letter-spacing: 0.05em;">
                                Experiência *
                            </label>
                            <asp:DropDownList ID="ddl_experiencia" runat="server"
                                CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddl_experiencia_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>

                        <!-- Dropdown de datas disponíveis -->
                        <div class="mb-3">
                            <label class="form-label fw-bold small text-uppercase" style="letter-spacing: 0.05em;">
                                Data e Hora *
                            </label>
                            <asp:DropDownList ID="ddl_disponibilidade" runat="server"
                                CssClass="form-select"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddl_disponibilidade_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:Label ID="lbl_vagas" runat="server"
                                CssClass="form-text text-muted" />
                        </div>

                        <!-- Número de pessoas -->
                        <div class="mb-3">
                            <label class="form-label fw-bold small text-uppercase" style="letter-spacing: 0.05em;">
                                Número de Pessoas *
                            </label>
                            <asp:TextBox ID="tb_pessoas" runat="server"
                                CssClass="form-control"
                                TextMode="Number"
                                Text="1"
                                AutoPostBack="true"
                                OnTextChanged="tb_pessoas_TextChanged" />
                            <asp:RequiredFieldValidator ID="rfv_pessoas" runat="server"
                                ControlToValidate="tb_pessoas"
                                ErrorMessage="Indique o número de pessoas."
                                CssClass="lbl-erro d-block"
                                Display="Dynamic"
                                ValidationGroup="vg_passo1"
                                EnableClientScript="false" />
                        </div>

                        <!-- Resumo do preço -->
                        <div class="p-3 mb-4" style="background-color: var(--cor-fundo); border-left: 3px solid var(--cor-primaria); border: 1px solid var(--cor-neutro);">
                            <div class="d-flex justify-content-between">
                                <span class="text-muted small">Preço por pessoa</span>
                                <span class="small">
                                    <asp:Label ID="lbl_preco_unitario" runat="server" Text="—" />
                                </span>
                            </div>
                            <div class="d-flex justify-content-between mt-1">
                                <span class="text-muted small">Participantes</span>
                                <span class="small">
                                    <asp:Label ID="lbl_num_pessoas_resumo" runat="server" Text="—" />
                                </span>
                            </div>
                            <!-- LINHA DE DESCONTO B2B (visível só quando aplicável) -->
                            <asp:Panel ID="pnl_desconto_b2b_passo1" runat="server" Visible="false">
                                <div class="d-flex justify-content-between mt-1" style="color: var(--cor-destaque);">
                                    <span class="small"><i class="bi bi-briefcase me-1"></i>Desconto B2B (<asp:Label ID="lbl_pct_b2b_passo1" runat="server" />%)</span>
                                    <span class="small">
                                        <asp:Label ID="lbl_valor_b2b_passo1" runat="server" Text="—" />
                                    </span>
                                </div>
                                <hr class="my-2" />
                            </asp:Panel>
                            <div class="d-flex justify-content-between">
                                <strong style="color: var(--cor-texto);">Total</strong>
                                <strong style="color: var(--cor-primaria);">
                                    <asp:Label ID="lbl_preco_total" runat="server" Text="—" />
                                </strong>
                            </div>
                        </div>

                        <asp:Button ID="btn_passo1" runat="server"
                            Text="Continuar →"
                            CssClass="btn-quinta w-100 py-2"
                            OnClick="btn_passo1_Click"
                            ValidationGroup="vg_passo1"
                            EnableClientScript="false" />
                    </asp:Panel>


                    <!-- ═══════════════════════════════════════════
                         PASSO 2 — Dados pessoais
                         ═══════════════════════════════════════════ -->
                    <asp:Panel ID="pnl_passo2" runat="server" Visible="false">
                        <span class="label-secao">Passo 2 de 3</span>
                        <h2 class="mb-0">Os Seus Dados</h2>
                        <div class="linha-verde-esq"></div>

                        <div class="mb-3">
                            <label class="form-label fw-bold small text-uppercase" style="letter-spacing: 0.05em;">Nome Completo *</label>
                            <asp:TextBox ID="tb_nome" runat="server" CssClass="form-control" placeholder="O seu nome" />
                            <asp:RequiredFieldValidator ID="rfv_nome" runat="server"
                                ControlToValidate="tb_nome"
                                ErrorMessage="Introduza o seu nome."
                                CssClass="lbl-erro d-block"
                                Display="Dynamic"
                                ValidationGroup="vg_passo2"
                                EnableClientScript="false" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label fw-bold small text-uppercase" style="letter-spacing: 0.05em;">Email *</label>
                            <asp:TextBox ID="tb_email" runat="server" CssClass="form-control" placeholder="o.seu@email.com" TextMode="Email" />
                            <asp:RequiredFieldValidator ID="rfv_email" runat="server"
                                ControlToValidate="tb_email"
                                ErrorMessage="Introduza o seu email."
                                CssClass="lbl-erro d-block"
                                Display="Dynamic"
                                ValidationGroup="vg_passo2"
                                EnableClientScript="false" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label fw-bold small text-uppercase" style="letter-spacing: 0.05em;">Telefone</label>
                            <asp:TextBox ID="tb_telefone" runat="server" CssClass="form-control" placeholder="+351 9XX XXX XXX" />
                        </div>

                        <div class="mb-4">
                            <label class="form-label fw-bold small text-uppercase" style="letter-spacing: 0.05em;">Notas Especiais</label>
                            <asp:TextBox ID="tb_notas" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"
                                placeholder="Alergias, necessidades especiais, pedidos..." />
                        </div>

                        <!-- DESCONTO B2B APLICADO (visível só para clientes B2B) -->
                        <asp:Panel ID="pnl_b2b_info" runat="server" Visible="false">
                            <div class="p-3 mb-3" style="background: linear-gradient(135deg, var(--cor-fundo) 0%, rgba(74, 124, 47, 0.08) 100%); border: 1px solid var(--cor-destaque); border-left: 3px solid var(--cor-destaque);">
                                <div class="d-flex align-items-center mb-1">
                                    <i class="bi bi-briefcase-fill me-2" style="color: var(--cor-destaque); font-size: 1.1rem;"></i>
                                    <strong class="small" style="color: var(--cor-destaque);">Desconto B2B Aplicado</strong>
                                </div>
                                <p class="mb-1 small text-muted">
                                    Como cliente empresarial, tem <strong style="color: var(--cor-texto);"><asp:Label ID="lbl_pct_b2b_passo2" runat="server" />% de desconto</strong> nesta experiência.
                                </p>
                            </div>
                        </asp:Panel>

                        <!-- CUPÃO DE DESCONTO -->
                        <asp:UpdatePanel ID="up_cupao" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="mb-3">
                                    <label class="form-label fw-bold small text-uppercase" style="letter-spacing: 0.05em;">
                                        <i class="bi bi-ticket-perforated me-1"></i>Cupão de Desconto
                                    </label>
                                    <asp:DropDownList ID="ddl_cupao" runat="server"
                                        CssClass="form-select"
                                        AutoPostBack="true"
                                        OnSelectedIndexChanged="ddl_cupao_SelectedIndexChanged">
                                        <asp:ListItem Value="0">— Sem cupão —</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:Label ID="lbl_cupao_info" runat="server" CssClass="form-text" style="color: var(--cor-primaria);" />
                                </div>

                                <!-- Resumo do preço com descontos -->
                                <div class="p-3 mb-4" style="background-color: var(--cor-fundo); border-left: 3px solid var(--cor-primaria); border: 1px solid var(--cor-neutro);">
                                    <div class="d-flex justify-content-between">
                                        <span class="text-muted small">Preço original</span>
                                        <span class="small">
                                            <asp:Label ID="lbl_preco_original_passo2" runat="server" Text="—" />
                                        </span>
                                    </div>
                                    <!-- Desconto B2B no passo 2 -->
                                    <asp:Panel ID="pnl_desconto_b2b_passo2" runat="server" Visible="false">
                                        <div class="d-flex justify-content-between mt-1" style="color: var(--cor-destaque);">
                                            <span class="small"><i class="bi bi-briefcase me-1"></i>Desconto B2B (<asp:Label ID="lbl_pct_b2b_resumo" runat="server" />%)</span>
                                            <span class="small">
                                                <asp:Label ID="lbl_valor_b2b_passo2" runat="server" Text="—" />
                                            </span>
                                        </div>
                                    </asp:Panel>
                                    <asp:Panel ID="pnl_desconto_aplicado" runat="server" Visible="false">
                                        <div class="d-flex justify-content-between mt-1" style="color: var(--cor-primaria);">
                                            <span class="small">Desconto do cupão</span>
                                            <span class="small">
                                                <asp:Label ID="lbl_desconto_cupao" runat="server" Text="—" />
                                            </span>
                                        </div>
                                        <hr class="my-2" />
                                    </asp:Panel>
                                    <div class="d-flex justify-content-between">
                                        <strong style="color: var(--cor-texto);">Total a pagar</strong>
                                        <strong style="color: var(--cor-primaria);">
                                            <asp:Label ID="lbl_preco_final_passo2" runat="server" Text="—" />
                                        </strong>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>

                        <div class="d-flex gap-2">
                            <asp:Button ID="btn_voltar_passo1" runat="server"
                                Text="← Voltar"
                                CssClass="btn-quinta-outline py-2 flex-fill"
                                OnClick="btn_voltar_passo1_Click"
                                CausesValidation="false" />
                            <asp:Button ID="btn_passo2" runat="server"
                                Text="Continuar →"
                                CssClass="btn-quinta py-2 flex-fill"
                                OnClick="btn_passo2_Click"
                                ValidationGroup="vg_passo2"
                                EnableClientScript="false" />
                        </div>
                    </asp:Panel>


                    <!-- ═══════════════════════════════════════════
                         PASSO 3 — Confirmação
                         ═══════════════════════════════════════════ -->
                    <asp:Panel ID="pnl_passo3" runat="server" Visible="false">
                        <span class="label-secao">Passo 3 de 3</span>
                        <h2 class="mb-0">Confirmar Reserva</h2>
                        <div class="linha-verde-esq"></div>

                        <!-- Resumo completo -->
                        <div class="p-4 mb-4" style="background-color: var(--cor-fundo); border: 1px solid var(--cor-neutro);">
                            <h5 class="mb-3" style="color: var(--cor-texto);">Resumo da sua reserva</h5>

                            <div class="d-flex justify-content-between mb-2">
                                <span class="text-muted small text-uppercase" style="letter-spacing: 0.05em;">Experiência</span>
                                <asp:Label ID="lbl_resumo_experiencia" runat="server" CssClass="small fw-bold" />
                            </div>
                            <div class="d-flex justify-content-between mb-2">
                                <span class="text-muted small text-uppercase" style="letter-spacing: 0.05em;">Data e Hora</span>
                                <asp:Label ID="lbl_resumo_data" runat="server" CssClass="small fw-bold" />
                            </div>
                            <div class="d-flex justify-content-between mb-2">
                                <span class="text-muted small text-uppercase" style="letter-spacing: 0.05em;">Participantes</span>
                                <asp:Label ID="lbl_resumo_pessoas" runat="server" CssClass="small fw-bold" />
                            </div>
                            <div class="d-flex justify-content-between mb-2">
                                <span class="text-muted small text-uppercase" style="letter-spacing: 0.05em;">Nome</span>
                                <asp:Label ID="lbl_resumo_nome" runat="server" CssClass="small fw-bold" />
                            </div>
                            <div class="d-flex justify-content-between mb-2">
                                <span class="text-muted small text-uppercase" style="letter-spacing: 0.05em;">Email</span>
                                <asp:Label ID="lbl_resumo_email" runat="server" CssClass="small fw-bold" />
                            </div>

                            <!-- Linha de desconto B2B no resumo -->
                            <asp:Panel ID="pnl_resumo_b2b" runat="server" Visible="false">
                                <div class="d-flex justify-content-between mb-2">
                                    <span class="text-muted small text-uppercase" style="letter-spacing: 0.05em;">
                                        <i class="bi bi-briefcase me-1"></i>Desconto B2B
                                    </span>
                                    <asp:Label ID="lbl_resumo_b2b" runat="server" CssClass="small fw-bold" style="color: var(--cor-destaque);" />
                                </div>
                            </asp:Panel>

                            <asp:Panel ID="pnl_resumo_cupao" runat="server" Visible="false">
                                <div class="d-flex justify-content-between mb-2">
                                    <span class="text-muted small text-uppercase" style="letter-spacing: 0.05em;">
                                        <i class="bi bi-ticket-perforated me-1"></i>Cupão Aplicado
                                    </span>
                                    <asp:Label ID="lbl_resumo_cupao" runat="server" CssClass="small fw-bold" style="color: var(--cor-primaria);" />
                                </div>
                            </asp:Panel>

                            <hr />
                            <div class="d-flex justify-content-between">
                                <strong style="color: var(--cor-texto);">Total a Pagar</strong>
                                <strong style="color: var(--cor-primaria); font-size: 1.1rem;">
                                    <asp:Label ID="lbl_resumo_total" runat="server" />
                                </strong>
                            </div>
                        </div>

                        <p class="text-muted small mb-4">
                            <i class="bi bi-info-circle me-1"></i>
                            A reserva fica em estado <strong>Pendente</strong> até confirmação da Quinta. Receberá um email de confirmação com os detalhes.
                        </p>

                        <div class="d-flex gap-2">
                            <asp:Button ID="btn_voltar_passo2" runat="server"
                                Text="← Voltar"
                                CssClass="btn-quinta-outline py-2 flex-fill"
                                OnClick="btn_voltar_passo2_Click"
                                CausesValidation="false" />
                            <asp:Button ID="btn_confirmar" runat="server"
                                Text="Confirmar Reserva"
                                CssClass="btn-quinta py-2 flex-fill"
                                OnClick="btn_confirmar_Click"
                                CausesValidation="false" />
                        </div>
                    </asp:Panel>


                    <!-- ═══════════════════════════════════════════
                         SUCESSO — após submissão
                         ═══════════════════════════════════════════ -->
                    <asp:Panel ID="pnl_sucesso" runat="server" Visible="false">
                        <div class="text-center py-4">
                            <i class="bi bi-check-circle-fill mb-3 d-block" style="font-size: 3rem; color: var(--cor-primaria);"></i>
                            <h3 style="color: var(--cor-texto);">Reserva Enviada!</h3>
                            <p class="text-muted mb-2">
                                O seu pedido foi recebido com sucesso.<br />
                                Entraremos em contacto em breve para confirmar.
                            </p>
                            <div class="p-3 mb-4 d-inline-block" style="background-color: var(--cor-fundo); border: 1px solid var(--cor-neutro);">
                                <small class="text-muted text-uppercase" style="letter-spacing: 0.1em;">Número de Reserva</small>
                                <div style="font-size: 1.3rem; font-weight: 700; color: var(--cor-primaria);">
                                    <asp:Label ID="lbl_num_reserva" runat="server" />
                                </div>
                            </div>
                            <div class="d-flex gap-2 justify-content-center flex-wrap">
                                <a href="~/Pages/index.aspx" runat="server" class="btn-quinta-outline">← Voltar ao Início</a>
                                <a href="~/Pages/experiencias.aspx" runat="server" class="btn-quinta">Ver Mais Experiências</a>
                            </div>
                        </div>
                    </asp:Panel>

                </div>

                <!-- COLUNA DIREITA — informações estáticas -->
                <div class="col-lg-4 offset-lg-1">
                    <div class="sticky-top" style="top: 80px;">
                        <span class="label-secao">Informação</span>
                        <h4 class="mb-0">A Saber</h4>
                        <div class="linha-verde-esq"></div>

                        <div class="d-flex gap-3 mb-3">
                            <i class="bi bi-calendar-check fs-4 flex-shrink-0" style="color: var(--cor-primaria);"></i>
                            <div>
                                <strong class="small text-uppercase" style="letter-spacing: 0.05em;">Antecedência</strong>
                                <p class="text-muted small mb-0">Reserve com pelo menos 48h de antecedência.</p>
                            </div>
                        </div>

                        <div class="d-flex gap-3 mb-3">
                            <i class="bi bi-envelope fs-4 flex-shrink-0" style="color: var(--cor-primaria);"></i>
                            <div>
                                <strong class="small text-uppercase" style="letter-spacing: 0.05em;">Confirmação</strong>
                                <p class="text-muted small mb-0">Receberá confirmação por email após a reserva ser validada.</p>
                            </div>
                        </div>

                        <div class="d-flex gap-3 mb-3">
                            <i class="bi bi-geo-alt fs-4 flex-shrink-0" style="color: var(--cor-primaria);"></i>
                            <div>
                                <strong class="small text-uppercase" style="letter-spacing: 0.05em;">Localização</strong>
                                <p class="text-muted small mb-0">Quinta da Azenha, Bucelas<br />A 25km de Lisboa · A1 saída Bucelas</p>
                            </div>
                        </div>

                        <div class="d-flex gap-3 mb-3">
                            <i class="bi bi-telephone fs-4 flex-shrink-0" style="color: var(--cor-primaria);"></i>
                            <div>
                                <strong class="small text-uppercase" style="letter-spacing: 0.05em;">Dúvidas?</strong>
                                <p class="text-muted small mb-0">+351 219 000 000<br />info.quintadaazenha@gmail.com</p>
                            </div>
                        </div>

                        <div class="d-flex gap-3">
                            <i class="bi bi-x-circle fs-4 flex-shrink-0" style="color: var(--cor-primaria);"></i>
                            <div>
                                <strong class="small text-uppercase" style="letter-spacing: 0.05em;">Cancelamento</strong>
                                <p class="text-muted small mb-0">Gratuito até 48h antes.<br />Entre 48h e 24h: 50% de reembolso.<br />Menos de 24h: sem reembolso.</p>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </main>

</asp:Content>
