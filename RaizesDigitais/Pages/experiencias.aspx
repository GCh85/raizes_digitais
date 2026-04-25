<%@ Page Title="Experiências | Quinta da Azenha" Language="C#" MasterPageFile="~/MasterSite.Master" AutoEventWireup="true" CodeBehind="experiencias.aspx.cs" Inherits="RaizesDigitais.Pages.experiencias" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- ═══════════════════════════════════════════════════════
         CABEÇALHO DA PÁGINA
         ═══════════════════════════════════════════════════════ -->
    <div class="text-center py-5" style="margin-top: 56px; background-color: var(--cor-texto); border-bottom: 2px solid var(--cor-destaque);">
        <div class="container">
            <h1 class="fw-normal text-white">Experiências na Quinta</h1>
            <p class="fst-italic" style="color: var(--cor-destaque);">Momentos que ficam na memória - e no palato</p>
        </div>
    </div>


    <main>

        <!-- ═══════════════════════════════════════════════════════
             CARDS DE EXPERIÊNCIAS — dados da BD
             ═══════════════════════════════════════════════════════ -->
        <section class="py-5">
            <div class="container">
                <span class="label-secao text-center d-block">O Que Fazemos</span>
                <h2 class="text-center mb-0">Escolha a Sua Experiência</h2>
                <div class="linha-verde"></div>

                <div class="row g-4">
                    <asp:Repeater ID="rpt_experiencias" runat="server">
                        <ItemTemplate>
                            <div class="col-md-6 col-lg-3">
                                <div class="card h-100 shadow-sm border-0">
                                    <img src='<%# ResolveUrl(Eval("imagem_url").ToString()) %>'
                                         class="card-img-top"
                                         alt='<%# Eval("nome") %>'
                                         onerror="this.src='<%# ResolveUrl("~/Images/QtaAzenha.png") %>'" />
                                    <div class="card-body d-flex flex-column text-center">

                                        <h5 class="card-title"><%# Eval("nome") %></h5>
                                        <p class="card-text text-muted small flex-grow-1"><%# Eval("descricao") %></p>

                                        <div class="mt-auto border-top pt-3">
                                            <p class="small text-muted mb-1">
                                                <i class="bi bi-clock me-1"></i>Duração: <%# Eval("duracao_horas") %>h
                                            </p>
                                            <p class="small text-muted mb-1">
                                                <i class="bi bi-people me-1"></i>Até <%# Eval("capacidade_max") %> pessoas
                                            </p>
                                            <p class="small mb-3" style="color: var(--cor-destaque); font-weight: 500;">
                                                <i class="bi bi-currency-euro me-1"></i><%# Eval("preco_por_pessoa", "{0:0.00}") %> / pessoa
                                            </p>
                                            <a href='<%# "~/Pages/reserva.aspx?id=" + Eval("id_experiencia") %>'
                                               runat="server"
                                               class="btn-quinta w-100 text-center d-block">Reservar</a>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </section>


        <!-- ═══════════════════════════════════════════════════════
             O QUE INCLUI
             ═══════════════════════════════════════════════════════ -->
        <section class="py-5 bg-white">
            <div class="container">
                <span class="label-secao text-center d-block">Informação</span>
                <h2 class="text-center mb-0">O Que Inclui</h2>
                <div class="linha-verde"></div>

                <div class="row g-4 text-center">
                    <div class="col-md-3">
                        <i class="bi bi-translate fs-1 mb-3 d-block" style="color: var(--cor-primaria);"></i>
                        <h5>Idiomas</h5>
                        <p class="text-muted small">Visitas disponíveis em Português e Inglês</p>
                    </div>
                    <div class="col-md-3">
                        <i class="bi bi-car-front fs-1 mb-3 d-block" style="color: var(--cor-primaria);"></i>
                        <h5>Estacionamento</h5>
                        <p class="text-muted small">Estacionamento gratuito na Quinta</p>
                    </div>
                    <div class="col-md-3">
                        <i class="bi bi-calendar-check fs-1 mb-3 d-block" style="color: var(--cor-primaria);"></i>
                        <h5>Reserva</h5>
                        <p class="text-muted small">Reserve com 48h de antecedência</p>
                    </div>
                    <div class="col-md-3">
                        <i class="bi bi-heart fs-1 mb-3 d-block" style="color: var(--cor-primaria);"></i>
                        <h5>Grupos</h5>
                        <p class="text-muted small">Programas personalizados para grupos</p>
                    </div>
                </div>
            </div>
        </section>


        <!-- ═══════════════════════════════════════════════════════
             CTA FINAL
             ═══════════════════════════════════════════════════════ -->
        <section class="py-5 text-center" style="background-color: var(--cor-texto);">
            <div class="container">
                <h2 class="mb-3 text-white">Faça já a sua reserva!</h2>
                <p class="mb-4" style="color: rgba(255,255,255,0.7);">Viva uma experiência inesquecível em Bucelas</p>
                <a href="~/Pages/reserva.aspx" runat="server" class="btn-quinta">Fazer Reserva</a>
            </div>
        </section>

    </main>

</asp:Content>
