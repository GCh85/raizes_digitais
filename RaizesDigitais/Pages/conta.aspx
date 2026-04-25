<%@ Page Title="A Minha Conta | Quinta da Azenha" Language="C#" MasterPageFile="~/MasterSite.Master" AutoEventWireup="true" CodeBehind="conta.aspx.cs" Inherits="RaizesDigitais.Pages.conta" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
    .nav-tabs .nav-link { color: var(--cor-texto-leve); }
    .nav-tabs .nav-link.active { color: var(--cor-primaria); border-bottom-color: var(--cor-primaria); font-weight: 600; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="text-center py-5" style="margin-top: 56px; background-color: var(--cor-texto); border-bottom: 2px solid var(--cor-destaque);">
        <div class="container">
            <h1 class="fw-normal text-white">A Minha Conta</h1>
            <p class="fst-italic" style="color: var(--cor-destaque);">
                Bem-vindo, <asp:Literal ID="lit_nome_cliente" runat="server" />
            </p>
        </div>
    </div>

    <main class="py-5">
        <div class="container">

            <asp:Label ID="lbl_mensagem" runat="server" CssClass="d-block mb-4" Visible="false" />

            <!-- PONTOS DE FIDELIZAÇÃO -->
            <div class="p-4 mb-5" style="background-color: var(--cor-fundo); border: 1px solid var(--cor-neutro);">
                <div class="row align-items-center">
                    <div class="col-md-6">
                        <span class="label-secao">Programa de Fidelização</span>
                        <h3 class="mb-0" style="color: var(--cor-texto);">
                            <asp:Literal ID="lit_pontos_total" runat="server" Text="0" /> pontos
                        </h3>
                        <p class="text-muted small mb-0">
                            <asp:Literal ID="lit_nivel" runat="server" />
                        </p>
                    </div>
                    <div class="col-md-6 mt-3 mt-md-0">
                        <div class="d-flex justify-content-between small text-muted mb-1">
                            <span><asp:Literal ID="lit_nivel_actual" runat="server" /></span>
                            <span><asp:Literal ID="lit_nivel_seguinte" runat="server" /></span>
                        </div>
                        <div class="progress" style="height: 6px; background-color: var(--cor-neutro);">
                            <div class="progress-bar" role="progressbar"
                                 style="width: <%= ProgressoPontos %>%; background-color: var(--cor-primaria);">
                            </div>
                        </div>
                        <p class="small text-muted mt-1 mb-0"><asp:Literal ID="lit_pontos_proximo" runat="server" /></p>
                    </div>
                </div>
            </div>

            <!-- TABS -->
            <ul class="nav nav-tabs mb-4">
            <li class="nav-item">
                <button class="nav-link active" data-bs-toggle="tab" data-bs-target="#tab-reservas" type="button">
                    Reservas
                </button>
            </li>
            <li class="nav-item">
                <button class="nav-link" data-bs-toggle="tab" data-bs-target="#tab-pontos" type="button">
                    Pontos
                </button>
            </li>
            <li class="nav-item">
                <button class="nav-link" data-bs-toggle="tab" data-bs-target="#tab-cupoes" type="button">
                    <i class="bi bi-ticket-perforated me-1"></i>Cupões
                </button>
            </li>
            <asp:Panel ID="pnl_tab_ofertas_b2b" runat="server" Visible="false">
                <li class="nav-item">
                    <button class="nav-link" data-bs-toggle="tab" data-bs-target="#tab-ofertas-b2b" type="button">
                        <i class="bi bi-briefcase me-1"></i>Ofertas Exclusivas
                    </button>
                </li>
            </asp:Panel>
            <li class="nav-item">
                <button class="nav-link" data-bs-toggle="tab" data-bs-target="#tab-favoritos" type="button">
                    Favoritos
                </button>
            </li>
            <li class="nav-item">
                <button class="nav-link" data-bs-toggle="tab" data-bs-target="#tab-perfil" type="button">
                    Perfil
                </button>
            </li>
        </ul>

            <div class="tab-content">

                <!-- TAB RESERVAS -->
                <div class="tab-pane fade show active" id="tab-reservas">
                    <asp:GridView ID="gv_reservas" runat="server"
                        CssClass="table table-hover mt-2"
                        AutoGenerateColumns="false"
                        GridLines="None"
                        OnRowCommand="gv_reservas_RowCommand"
                        EmptyDataText="Ainda não tem reservas.">
                        <Columns>
                            <asp:BoundField DataField="num_reserva" HeaderText="Nº Reserva" />
                            <asp:BoundField DataField="experiencia" HeaderText="Experiência" />
                            <asp:BoundField DataField="data_hora" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                            <asp:BoundField DataField="num_pessoas" HeaderText="Pessoas" />
                            <asp:BoundField DataField="preco_total" HeaderText="Total" DataFormatString="{0:0.00} €" />
                            <asp:TemplateField HeaderText="Estado">
                                <ItemTemplate>
                                    <span class='<%# GetEstadoBadge(Eval("estado").ToString()) %>'>
                                        <%# Eval("estado") %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btn_deixar_testemunho" runat="server"
                                        CommandName="AvaliarExperiencia"
                                        CommandArgument='<%# Eval("id_reserva") + "|" + Eval("id_experiencia") %>'
                                        CssClass="btn btn-sm btn-outline-success"
                                        Visible='<%# Avaliada(Eval("estado").ToString(), Eval("avaliada")) %>'
                                        CausesValidation="false">
                                        Deixar Testemunho
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btn_cancelar" runat="server"
                                        CommandName="CancelarReserva"
                                        CommandArgument='<%# Eval("id_reserva") %>'
                                        CssClass="btn btn-sm btn-outline-danger"
                                        Visible='<%# PodeCancelar(Eval("estado").ToString(), Eval("data_hora").ToString()) %>'
                                        OnClientClick="return confirm('Tem a certeza que quer cancelar esta reserva?');">
                                        Cancelar
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>

                <!-- TAB PONTOS -->
                <div class="tab-pane fade" id="tab-pontos">
                    <span class="label-secao">Histórico</span>
                    <h4 class="mb-0">Movimentos de Pontos</h4>
                    <div class="linha-verde-esq"></div>
                    <asp:GridView ID="gv_pontos" runat="server"
                        CssClass="table table-hover mt-3"
                        AutoGenerateColumns="false"
                        GridLines="None"
                        EmptyDataText="Ainda não tem movimentos de pontos.">
                        <Columns>
                            <asp:BoundField DataField="data_accao" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}" />
                            <asp:BoundField DataField="tipo_accao" HeaderText="Motivo" />
                            <asp:TemplateField HeaderText="Pontos">
                                <ItemTemplate>
                                    <span class='<%# Convert.ToInt32(Eval("pontos_ganhos")) >= 0 ? "text-success fw-bold" : "text-danger fw-bold" %>'>
                                        <%# Convert.ToInt32(Eval("pontos_ganhos")) >= 0 ? "+" : "" %><%# Eval("pontos_ganhos") %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>

                <!-- TAB CUPÕES -->
                <div class="tab-pane fade" id="tab-cupoes">
                    <span class="label-secao">Benefícios</span>
                    <h4 class="mb-0">Os Meus Cupões</h4>
                    <div class="linha-verde-esq"></div>

                    <!-- Cupões Disponíveis -->
                    <div class="mb-5">
                        <h6 class="text-muted small text-uppercase mb-3" style="letter-spacing:0.05em;">
                            <i class="bi bi-star-fill me-1" style="color: var(--cor-destaque);"></i>Cupões Disponíveis
                        </h6>
                        <asp:Repeater ID="rpt_cupoes_disponiveis" runat="server">
                            <ItemTemplate>
                                <div class="mb-3 p-4" style="background: linear-gradient(135deg, var(--cor-fundo) 0%, rgba(74, 124, 47, 0.05) 100%); border: 2px dashed var(--cor-primaria); border-radius: 8px; position: relative;">
                                    <div class="row align-items-center">
                                        <div class="col-md-8">
                                            <div class="d-flex align-items-center mb-2">
                                                <span class="badge px-3 py-2 me-2" style="background-color: var(--cor-destaque); color: white; font-size: 0.95rem; letter-spacing: 0.1em; font-weight: 600;">
                                                    <%# Eval("codigo") %>
                                                </span>
                                                <span class="text-muted small">Código do cupão</span>
                                            </div>
                                            <p class="mb-1" style="color: var(--cor-primaria); font-size: 1.1rem; font-weight: 600;">
                                                <%# Eval("descricao_desconto") %>
                                            </p>
                                            <p class="text-muted small mb-0">
                                                <i class="bi bi-calendar3 me-1"></i>Válido até <%# Convert.ToDateTime(Eval("validade")).ToString("dd/MM/yyyy") %>
                                            </p>
                                        </div>
                                        <div class="col-md-4 text-md-end mt-3 mt-md-0">
                                            <a href="~/Pages/reserva.aspx" runat="server" class="btn-quinta-outline btn-sm">
                                                Usar Agora <i class="bi bi-arrow-right ms-1"></i>
                                            </a>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:Label ID="lbl_sem_cupoes" runat="server" CssClass="text-muted fst-italic d-block text-center py-4" Visible="false"
                            Text="Ainda não tem cupões disponíveis. Continue a acumular pontos!" />
                    </div>

                    <!-- Histórico de Cupões Usados -->
                    <div>
                        <h6 class="text-muted small text-uppercase mb-3" style="letter-spacing:0.05em;">
                            <i class="bi bi-clock-history me-1"></i>Histórico de Cupões
                        </h6>
                        <asp:GridView ID="gv_cupoes_usados" runat="server"
                            CssClass="table table-hover"
                            AutoGenerateColumns="false"
                            GridLines="None"
                            EmptyDataText="Ainda não utilizou nenhum cupão.">
                            <Columns>
                                <asp:BoundField DataField="codigo" HeaderText="Código" />
                                <asp:BoundField DataField="descricao_desconto" HeaderText="Desconto" />
                                <asp:BoundField DataField="data_utilizacao" HeaderText="Data de Uso" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:BoundField DataField="num_reserva" HeaderText="Nº Reserva" />
                                <asp:TemplateField HeaderText="Estado">
                                    <ItemTemplate>
                                        <span class="badge bg-secondary">Utilizado</span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>

                <!-- TAB OFERTAS B2B (só visível para segmento B2B) -->
                    <div class="tab-pane fade" id="tab-ofertas-b2b">
                        <asp:Panel ID="pnl_conteudo_ofertas_b2b" runat="server" Visible="false">
                        <span class="label-secao">Exclusivo B2B</span>
                        <h4 class="mb-0">Ofertas Especiais para Empresas</h4>
                        <div class="linha-verde-esq"></div>

                        <div class="alert mb-4" style="background-color: rgba(74, 124, 47, 0.1); border-left: 3px solid var(--cor-primaria); border-radius: 0;">
                            <i class="bi bi-info-circle me-2" style="color: var(--cor-primaria);"></i>
                            <strong>Cliente B2B:</strong> Aproveite descontos exclusivos nas experiências selecionadas abaixo.
                        </div>

                        <div class="row">
                            <asp:Repeater ID="rpt_ofertas_b2b" runat="server">
                                <ItemTemplate>
                                    <div class="col-md-6 mb-4">
                                        <div class="h-100" style="border: 1px solid var(--cor-neutro); border-radius: 4px; overflow: hidden; background-color: white;">
                                            <!-- Imagem -->
                                            <asp:Image ID="img_experiencia" runat="server"
                                                ImageUrl='<%# Eval("imagem_url") %>'
                                                CssClass="w-100"
                                                style="height: 200px; object-fit: cover;"
                                                AlternateText='<%# Eval("nome") %>' />
                                            
                                            <!-- Conteúdo -->
                                            <div class="p-4">
                                                <div class="d-flex justify-content-between align-items-start mb-2">
                                                    <h5 class="mb-0" style="color: var(--cor-texto);"><%# Eval("nome") %></h5>
                                                    <span class="badge px-2 py-1" style="background-color: var(--cor-destaque); color: white; font-size: 0.85rem;">
                                                        -<%# Convert.ToDecimal(Eval("desconto_b2b")).ToString("0") %>%
                                                    </span>
                                                </div>
                                                
                                                <p class="text-muted small mb-3"><%# Eval("descricao") %></p>
                                                
                                                <div class="d-flex justify-content-between align-items-center">
                                                    <div>
                                                        <span class="text-muted small text-decoration-line-through d-block">
                                                            <%# Convert.ToDecimal(Eval("preco_por_pessoa")).ToString("0.00") %> €
                                                        </span>
                                                        <span class="fw-bold" style="color: var(--cor-primaria); font-size: 1.2rem;">
                                                            <%# Convert.ToDecimal(Eval("preco_final_b2b")).ToString("0.00") %> €
                                                        </span>
                                                        <span class="text-muted small"> /pessoa</span>
                                                    </div>
                                                    <a href='<%# "~/Pages/reserva.aspx?id=" + Eval("id_experiencia") %>' runat="server" class="btn-quinta-outline btn-sm">
                                                        Reservar
                                                    </a>
                                                </div>

                                                <div class="mt-3 pt-3 border-top">
                                                    <small class="text-muted">
                                                        <i class="bi bi-clock me-1"></i><%# Eval("duracao_horas") %> horas
                                                        <span class="mx-2">·</span>
                                                        <i class="bi bi-people me-1"></i>Até <%# Eval("capacidade_max") %> pessoas
                                                    </small>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>

                        <asp:Label ID="lbl_sem_ofertas_b2b" runat="server" CssClass="text-muted fst-italic d-block text-center py-4" Visible="false"
                            Text="De momento não há ofertas B2B disponíveis. Contacte-nos para propostas personalizadas." />
                        </asp:Panel>
                    </div>

                <!-- TAB FAVORITOS -->
                <div class="tab-pane fade" id="tab-favoritos">
                    <span class="label-secao">A Minha Garrafeira</span>
                    <h4 class="mb-0">Vinhos Favoritos</h4>
                    <div class="linha-verde-esq"></div>
                    <div class="row mt-3">
                        <asp:Repeater ID="rpt_favoritos" runat="server" OnItemCommand="rpt_favoritos_ItemCommand">
                            <ItemTemplate>
                                <div class="col-md-4 mb-3">
                                    <div class="p-3" style="border: 1px solid var(--cor-neutro); background-color: var(--cor-fundo); position:relative;">
                                        <asp:LinkButton ID="btn_remover_favorito" runat="server"
                                            CommandName="RemoverFavorito"
                                            CommandArgument='<%# Eval("id_vinho") %>'
                                            style="position:absolute; top:6px; right:6px; background:none; border:none; padding:0; line-height:1; color:var(--cor-primaria); font-size:1.1rem;"
                                            CausesValidation="false">
                                            <i class="bi bi-x-lg"></i>
                                        </asp:LinkButton>
                                        <strong style="color: var(--cor-texto);"><%# Eval("nome") %></strong>
                                        <p class="text-muted small mb-1"><%# Eval("tipo") %> · <%# Eval("casta") %></p>
                                        <small class="text-muted">Favorito desde <%# Eval("data_adicao", "{0:dd/MM/yyyy}") %></small>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <asp:Label ID="lbl_sem_favoritos" runat="server" CssClass="text-muted fst-italic" Visible="false"
                        Text="Ainda não marcou nenhum vinho como favorito. Explore o nosso catálogo de vinhos." />
                </div>

                <!-- TAB PERFIL -->
                <div class="tab-pane fade" id="tab-perfil">
                <asp:Label ID="lbl_mensagem_perfil" runat="server" CssClass="d-block mb-4" Visible="false" />

                <div class="row">
                    <div class="col-md-6">
                        <span class="label-secao">Dados de Contacto</span>
                        <h4 class="mb-0">O Meu Perfil</h4>
                        <div class="linha-verde-esq"></div>

                        <div class="mb-3">
                            <p class="text-muted small text-uppercase mb-1" style="letter-spacing:0.05em;">Email</p>
                            <p class="fw-bold" style="color: var(--cor-texto);"><asp:Literal ID="lit_email" runat="server" /></p>
                        </div>

                        <div class="mb-3">
                            <label class="text-muted small text-uppercase mb-1" style="letter-spacing:0.05em;">Telefone</label>
                            <asp:TextBox ID="tb_telefone" runat="server" CssClass="form-control" />
                        </div>

                        <div class="mb-3">
                            <label class="text-muted small text-uppercase mb-1" style="letter-spacing:0.05em;">Alergias / Restrições</label>
                            <asp:TextBox ID="tb_alergias" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                        </div>

                        <div class="mb-4">
                            <label class="text-muted small text-uppercase mb-1" style="letter-spacing:0.05em;">Preferências de Vinho</label>
                            <asp:TextBox ID="tb_pref_vinhos" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                        </div>

                        <asp:Button ID="btn_guardar_perfil" runat="server"
                            Text="Guardar Alterações"
                            CssClass="btn-quinta"
                            OnClick="btn_guardar_perfil_Click"
                            CausesValidation="false" />
                    </div>

                    <div class="col-md-6">
                        <span class="label-secao">Segurança</span>
                        <h4 class="mb-0">Alterar Password</h4>
                        <div class="linha-verde-esq"></div>

                        <%--BOTAO 2FA VERIFICAR--%>
                    <div class="mt-3 p-3 border rounded bg-light">
                        <h6>Autenticação em Dois Passos (2FA)</h6>
                        <p class="small text-muted">Aumente a segurança da sua conta exigindo um código enviado por email ao entrar.</p>
                        <asp:LinkButton ID="btn_toggle_2fa" runat="server" OnClick="btn_toggle_2fa_Click" CssClass="btn btn-sm" />
                    </div>

                        <asp:Label ID="lbl_mensagem_password" runat="server" CssClass="d-block mb-3" Visible="false" />

                        <div class="mb-3">
                            <label class="text-muted small text-uppercase mb-1" style="letter-spacing:0.05em;">Password Actual *</label>
                            <asp:TextBox ID="tb_pw_actual" runat="server" CssClass="form-control" TextMode="Password" />
                        </div>

                        <div class="mb-3">
                            <label class="text-muted small text-uppercase mb-1" style="letter-spacing:0.05em;">Nova Password *</label>
                            <asp:TextBox ID="tb_pw_nova" runat="server" CssClass="form-control" TextMode="Password" />
                        </div>

                        <div class="mb-4">
                            <label class="text-muted small text-uppercase mb-1" style="letter-spacing:0.05em;">Confirmar Nova Password *</label>
                            <asp:TextBox ID="tb_pw_confirmar" runat="server" CssClass="form-control" TextMode="Password" />
                        </div>

                        <asp:Button ID="btn_alterar_password" runat="server"
                            Text="Alterar Password"
                            CssClass="btn-quinta-outline"
                            OnClick="btn_alterar_password_Click"
                            CausesValidation="false" />

                        <div class="mt-4 p-3" style="background-color: var(--cor-fundo); border: 1px solid var(--cor-neutro);">
                            <p class="small text-muted mb-0">
                                <i class="bi bi-info-circle me-1"></i>
                                Se criou a sua conta através de uma reserva, a password temporária atribuída foi <strong>Raizes2026!</strong> - altere-a aqui para uma password pessoal.
                            </p>
                        </div>
                    </div>
                </div>
            </div>

            </div>

            <!-- TERMINAR SESSÃO -->
            <div class="mt-5 pt-4" style="border-top: 1px solid var(--cor-neutro);">
                <asp:LinkButton ID="btn_sair" runat="server"
                    CssClass="btn-quinta-outline"
                    OnClick="btn_sair_Click"
                    CausesValidation="false">
                    Terminar Sessão
                </asp:LinkButton>
            </div>

        </div>
    </main>

    <!-- ═══════════════════════════════════════════════════
         MODAL — Deixar Testemunho
         ═══════════════════════════════════════════════════ -->
    <div class="modal fade" id="modal_testemunho" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header" style="background-color: var(--cor-primaria); border:none;">
                    <h5 class="modal-title text-white">Deixar Testemunho</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <asp:Label ID="lbl_mensagem_testemunho" runat="server" CssClass="d-block mb-3" Visible="false" />

                    <input type="hidden" id="hf_id_reserva_testemunho" runat="server" />
                    <input type="hidden" id="hf_id_experiencia_testemunho" runat="server" />
                    <asp:HiddenField ID="hf_tab_activa" runat="server" Value="" />

                    <div class="mb-3">
                        <p class="text-muted small mb-2" style="color: var(--cor-texto-leve);">Avalie a sua experiência:</p>
                        <div id="estrelas_input" class="d-flex gap-1">
                            <span class="faixa-estrela" data-valor="1" onclick="selecionarEstrela(1)" style="cursor:pointer; font-size:2rem; color: var(--cor-neutro);">&#9733;</span>
                            <span class="faixa-estrela" data-valor="2" onclick="selecionarEstrela(2)" style="cursor:pointer; font-size:2rem; color: var(--cor-neutro);">&#9733;</span>
                            <span class="faixa-estrela" data-valor="3" onclick="selecionarEstrela(3)" style="cursor:pointer; font-size:2rem; color: var(--cor-neutro);">&#9733;</span>
                            <span class="faixa-estrela" data-valor="4" onclick="selecionarEstrela(4)" style="cursor:pointer; font-size:2rem; color: var(--cor-neutro);">&#9733;</span>
                            <span class="faixa-estrela" data-valor="5" onclick="selecionarEstrela(5)" style="cursor:pointer; font-size:2rem; color: var(--cor-neutro);">&#9733;</span>
                        </div>
                        <input type="hidden" id="hf_estrelas" runat="server" value="0" />
                    </div>

                    <div class="mb-3">
                        <label class="text-muted small text-uppercase mb-1" style="letter-spacing:0.05em;">Deixe um comentário (opcional, máx. 500 caracteres)</label>
                        <asp:TextBox ID="tb_comentario_testemunho" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" MaxLength="500" />
                    </div>
                </div>
                <div class="modal-footer" style="border:none; padding-top:0;">
                    <asp:Button ID="btn_enviar_testemunho" runat="server"
                        Text="Enviar Testemunho"
                        CssClass="btn-quinta"
                        OnClick="btn_enviar_testemunho_Click"
                        CausesValidation="false" />
                </div>
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        // ── Activar tab após postback ─────────────────────────────
        // O code-behind escreve o ID da tab em hf_tab_activa.
        // Este script corre depois do Bootstrap estar carregado.
        (function () {
            var tabAlvo = document.getElementById('<%= hf_tab_activa.ClientID %>').value;
            if (tabAlvo) {
                var btn = document.querySelector('[data-bs-target="' + tabAlvo + '"]');
                if (btn) bootstrap.Tab.getOrCreateInstance(btn).show();
            }
        })();

        // ── Abrir modal de testemunho após postback ───────────────
        (function () {
            var abrirModal = '<%= ViewState["abrirModalTestemunho"] ?? "" %>';
            if (abrirModal === '1') {
                var el = document.getElementById('modal_testemunho');
                if (el) bootstrap.Modal.getOrCreateInstance(el).show();
            }
        })();
        // Selecionar estrelas do testemunho
        var estrelaSelecionada = 0;
        function selecionarEstrela(valor) {
            estrelaSelecionada = valor;
            document.getElementById('<%= hf_estrelas.ClientID %>').value = valor;
            var estrelas = document.querySelectorAll('.faixa-estrela');
            for (var i = 0; i < estrelas.length; i++) {
                if (i < valor) {
                    estrelas[i].style.color = '#c8a84b';
                } else {
                    estrelas[i].style.color = '#E8E8E4';
                }
            }
        }

        // Reset estrelas quando modal fecha
        document.getElementById('modal_testemunho').addEventListener('hidden.bs.modal', function () {
            estrelaSelecionada = 0;
            document.getElementById('<%= hf_estrelas.ClientID %>').value = '0';
        var estrelas = document.querySelectorAll('.faixa-estrela');
        estrelas.forEach(function (el) { el.style.color = '#E8E8E4'; });
        document.getElementById('<%= tb_comentario_testemunho.ClientID %>').value = '';
        });
    </script>
</asp:Content>