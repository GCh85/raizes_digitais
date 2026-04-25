<%@ Page Title="Vinhos | Quinta da Azenha" Language="C#" MasterPageFile="~/MasterSite.Master" AutoEventWireup="true" CodeBehind="vinhos.aspx.cs" Inherits="RaizesDigitais.Pages.vinhos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="text-center py-5" style="margin-top: 56px; background-color: var(--cor-texto); border-bottom: 2px solid var(--cor-destaque);">
        <div class="container">
            <h1 class="fw-normal text-white">Os Nossos Vinhos</h1>
            <p class="fst-italic" style="color: var(--cor-destaque);">Arinto DOC Bucelas — a casta que define uma região</p>
        </div>
    </div>

    <main>

        <section class="py-5">
            <div class="container">
                <span class="label-secao text-center d-block">Catálogo Completo</span>
                <h2 class="text-center mb-0">A Nossa Gama</h2>
                <div class="linha-verde"></div>

                <div class="d-flex gap-2 justify-content-center flex-wrap mb-4">
                    <button class="btn btn-sm btn-filtro active" data-filtro="todos">Todos</button>
                    <button class="btn btn-sm btn-filtro" data-filtro="Branco">Branco</button>
                    <button class="btn btn-sm btn-filtro" data-filtro="Tinto">Tinto</button>
                    <button class="btn btn-sm btn-filtro" data-filtro="Espumante">Espumante</button>
                </div>

                <div class="row g-4">
                    <asp:Repeater ID="rpt_vinhos" runat="server" OnItemCommand="rpt_vinhos_ItemCommand">

                        <ItemTemplate>
                        <div class="col-md-6 col-lg-3 card-vinho" data-tipo='<%# Eval("tipo") %>'>
                            <div class="card h-100 shadow-sm border-0" style="position:relative;">

                                <!-- Botão favoritar -->
                                <div style="position:absolute; top:10px; right:10px; z-index:1;">
                                    <asp:LinkButton ID="btn_favoritar" runat="server"
                                        Visible='<%# Session["cliente_id"] != null && !(bool)Eval("is_favorito") %>'
                                        CommandName="ToggleFavorito"
                                        CommandArgument='<%# Eval("id_vinho") %>'
                                        CausesValidation="false"
                                        style="background:none; border:none; padding:0; line-height:1;">
                                        <i class="bi bi-heart" style="color:var(--cor-primaria);font-size:1.2rem;"></i>
                                    </asp:LinkButton>

                                        <%-- Coração cheio: logado + já é favorito --%>
                                    <asp:LinkButton ID="btn_desfavoritar" runat="server"
                                        Visible='<%# Session["cliente_id"] != null && (bool)Eval("is_favorito") %>'
                                        CommandName="ToggleFavorito"
                                        CommandArgument='<%# Eval("id_vinho") %>'
                                        CausesValidation="false"
                                        style="background:none; border:none; padding:0; line-height:1;">
                                        <i class="bi bi-heart-fill" style="color:var(--cor-primaria);font-size:1.2rem;"></i>
                                    </asp:LinkButton>

                                    <asp:HyperLink ID="lnk_login_favorito" runat="server"
                                        Visible='<%# Session["cliente_id"] == null %>'
                                        NavigateUrl="~/Pages/conta_login.aspx"
                                        ToolTip="Inicie sessão para guardar favoritos"
                                        style="color: var(--cor-primaria); font-size:1.2rem; text-decoration:none;">
                                        <i class="bi bi-heart"></i>
                                    </asp:HyperLink>

                                </div>

                                <div class="card-body d-flex flex-column">

                                    <div class="mb-2">
                                        <span class="badge rounded-pill"
                                              style='<%# GetBadgeStyle(Eval("tipo").ToString()) %>'>
                                            <%# Eval("tipo") %>
                                        </span>
                                    </div>

                                    <div class="mb-3" style="height: 220px; display: flex; align-items: center; justify-content: center;">
                                        <img src='<%# GetVinhoImagem(Eval("imagem_url"), Eval("nome")) %>'
                                             alt='<%# Eval("nome") %>'
                                             style="max-height: 100%; width: auto; object-fit: contain;"
                                             onerror='this.src="<%= ResolveUrl("~/Images/QtaAzenha.png")%>"' />
                                    </div>

                                    <h5 class="card-title"><%# Eval("nome") %></h5>
                                    <p class="card-text text-muted small"><%# Eval("descricao") %></p>

                                    <div class="mt-auto">
                                        <div class="mt-2">
                                            <small class="text-muted">Doçura</small>
                                            <div class="progress mb-1" style="height: 4px;">
                                                <div class="progress-bar"
                                                     style='width: <%# Eval("docura") %>%; background-color: var(--cor-destaque);'></div>
                                            </div>
                                            <small class="text-muted">Acidez</small>
                                            <div class="progress mb-1" style="height: 4px;">
                                                <div class="progress-bar"
                                                     style='width: <%# Eval("acidez") %>%; background-color: var(--cor-destaque);'></div>
                                            </div>
                                            <small class="text-muted">Corpo</small>
                                            <div class="progress" style="height: 4px;">
                                                <div class="progress-bar"
                                                     style='width: <%# Eval("corpo") %>%; background-color: var(--cor-destaque);'></div>
                                            </div>
                                        </div>

                                        <div class="mt-2">
                                            <small class="text-muted">
                                                <i class="bi bi-egg-fried me-1"></i><%# Eval("harmonizacao") %>
                                            </small>
                                        </div>

                                        <small class="text-muted mt-2 d-block">
                                            <%# Eval("casta") %> · <%# Eval("ano") %>
                                        </small>

                                        <div class="mt-3">
                                            <%# GetEstrelas(Eval("media_estrelas"), Eval("total_avaliacoes")) %>
                                        </div>

                                        <a href="~/Pages/reserva.aspx" runat="server"
                                           class="btn-quinta-outline mt-3 text-center d-block">
                                            Descobrir na Quinta
                                        </a>
                                    </div>

                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </section>

        <section class="py-5 bg-white">
            <div class="container">
                <div class="row justify-content-center">
                    <div class="col-lg-6 text-center">
                        <i class="bi bi-bag-x fs-1 mb-3 d-block" style="color: var(--cor-primaria);"></i>
                        <h4>Não fazemos venda online</h4>
                        <p class="text-muted">Os vinhos da Quinta da Azenha estão disponíveis exclusivamente durante as visitas. Cada garrafa merece ser descoberta pessoalmente.</p>
                        <a href="~/Pages/reserva.aspx" runat="server" class="btn-quinta">Reservar uma Visita</a>
                    </div>
                </div>
            </div>
        </section>

    </main>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
   
    <script>
        // window.onload garante que o Repeater já renderizou tudo no browser
        window.onload = function () {
            var botoesFiltro = document.querySelectorAll(".btn-filtro");
            var cards = document.querySelectorAll(".card-vinho");

            botoesFiltro.forEach(function (btn) {
                btn.addEventListener("click", function (e) {
                    e.preventDefault(); // Impede qualquer postback acidental

                    // Atualizar estado visual dos botões
                    botoesFiltro.forEach(b => b.classList.remove("active"));
                    this.classList.add("active");

                    var filtro = this.getAttribute("data-filtro").toLowerCase().trim();

                    cards.forEach(function (card) {
                        
                        var tipoCard = card.getAttribute("data-tipo").toLowerCase().trim();

                        if (filtro === "todos" || tipoCard === filtro) {
                            card.style.setProperty("display", "block", "important");
                        } else {
                            card.style.setProperty("display", "none", "important");
                        }
                    });
                });
            });
        };
    </script>
</asp:Content>
