<%@ Page Title="Clientes | Raízes Digitais" Language="C#" MasterPageFile="~/MasterBackoffice.Master" AutoEventWireup="true" CodeBehind="gerir_clientes.aspx.cs" Inherits="RaizesDigitais.Backoffice.gerir_clientes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder3" runat="server">

    <div class="content-header">
        <div class="container-fluid">
            <div class="row mb-2">
                <div class="col-sm-6">
                    <h1 class="m-0">Gestão de Clientes</h1>
                </div>
                <div class="col-sm-6">
                    <ol class="breadcrumb float-sm-right">
                        <li class="breadcrumb-item">
                            <a href="dashboard.aspx">Início</a>
                        </li>
                        <li class="breadcrumb-item active">Gestão de Clientes</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>

    <div class="content">
        <div class="container-fluid">

            <!-- ═══════════════════════════════════════════════════════════════
                 MELHORIA: KPIs RÁPIDOS NO TOPO
                 ═══════════════════════════════════════════════════════════════ -->
            <div class="row mb-3">
                <div class="col-lg-3 col-6">
                    <div class="small-box bg-info">
                        <div class="inner">
                            <h3><asp:Literal ID="lit_total_clientes" runat="server" Text="0" /></h3>
                            <p>Total Clientes</p>
                        </div>
                        <div class="icon"><i class="fas fa-users"></i></div>
                    </div>
                </div>
                <div class="col-lg-3 col-6">
                    <div class="small-box bg-success">
                        <div class="inner">
                            <h3><asp:Literal ID="lit_clientes_vip" runat="server" Text="0" /></h3>
                            <p>Clientes VIP</p>
                        </div>
                        <div class="icon"><i class="fas fa-star"></i></div>
                    </div>
                </div>
                <div class="col-lg-3 col-6">
                    <div class="small-box bg-primary">
                        <div class="inner">
                            <h3><asp:Literal ID="lit_clientes_b2b" runat="server" Text="0" /></h3>
                            <p>Clientes B2B</p>
                        </div>
                        <div class="icon"><i class="fas fa-building"></i></div>
                    </div>
                </div>
                <div class="col-lg-3 col-6">
                    <div class="small-box bg-warning">
                        <div class="inner">
                            <h3><asp:Literal ID="lit_clientes_novos_mes" runat="server" Text="0" /></h3>
                            <p>Novos Este Mês</p>
                        </div>
                        <div class="icon"><i class="fas fa-user-plus"></i></div>
                    </div>
                </div>
            </div>

            <!-- FILTROS DE PESQUISA -->
            <div class="card mb-4 card-outline card-primary">
                <div class="card-header">
                    <h3 class="card-title">Pesquisar Clientes</h3>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-4">
                            <label>Nome ou Email</label>
                            <asp:TextBox ID="tb_pesquisa" runat="server" CssClass="form-control"
                                         placeholder="Insira o nome ou email..."></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            <label>Segmento CRM</label>
                            <asp:DropDownList ID="ddl_segmento_filtro" runat="server" CssClass="form-control select2">
                                <asp:ListItem Value="">— Todos —</asp:ListItem>
                                <asp:ListItem Value="VIP">VIP</asp:ListItem>
                                <asp:ListItem Value="Regular">Regular</asp:ListItem>
                                <asp:ListItem Value="Inactivo">Inactivo</asp:ListItem>
                                <asp:ListItem Value="B2B">B2B</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-5 d-flex align-items-end">
                            <asp:Button ID="btn_pesquisar" runat="server" Text="Pesquisar"
                                        CssClass="btn btn-primary mr-2" OnClick="btn_pesquisar_Click" />
                            <asp:Button ID="btn_limpar" runat="server" Text="Limpar"
                                        CssClass="btn btn-secondary mr-2" OnClick="btn_limpar_Click" />
                            <asp:LinkButton ID="btn_exportar_emails" runat="server"
                                        CssClass="btn btn-outline-success d-inline-flex align-items-center"
                                        OnClick="btn_exportar_emails_Click">
                                <i class="fas fa-envelope"></i> Exportar Emails
                            </asp:LinkButton>
                        </div>
                    </div>

                    <!-- Painel de emails exportados -->
                    <asp:Panel ID="pnl_emails_export" runat="server" Visible="false" CssClass="mt-3 pt-3 border-top">
                        <div class="form-group mb-0">
                            <label class="font-weight-bold text-muted small">Emails para campanha:</label>
                            <asp:TextBox ID="tb_emails_export" runat="server" TextMode="MultiLine"
                                        CssClass="form-control form-control-sm" Rows="3" ReadOnly="true"
                                        style="font-family:monospace; font-size:12px;"></asp:TextBox>
                        </div>
                    </asp:Panel>

                </div>
            </div>

            <!-- GRIDVIEW DE CLIENTES -->
            <div class="card">
                <div class="card-header">
                    <h3 class="card-title">
                        Lista de Clientes
                        <!-- MELHORIA: Badge com total -->
                        <span class="badge badge-primary ml-2"><asp:Literal ID="lit_total_lista" runat="server" Text="0" /></span>
                    </h3>
                </div>
                <div class="card-body p-0">
                    <div style="overflow-x:auto;">
                    <asp:GridView ID="gv_clientes" runat="server"
                                  AutoGenerateColumns="False"
                                  CssClass="table table-hover m-0"
                                  GridLines="None"
                                  OnRowCommand="gv_clientes_RowCommand"
                                  DataKeyNames="id_cliente"
                                  AllowSorting="True"
                                  OnSorting="gv_clientes_Sorting">
                        <Columns>
                            <asp:BoundField DataField="nome" HeaderText="Nome" SortExpression="nome" />
                            <asp:BoundField DataField="email" HeaderText="Email" SortExpression="email" />
                            <asp:BoundField DataField="telefone" HeaderText="Telefone" SortExpression="telefone" />
                            <asp:BoundField DataField="data_registo" HeaderText="Registo" DataFormatString="{0:dd/MM/yyyy}" SortExpression="data_registo" />

                            <asp:TemplateField HeaderText="Segmento CRM">
                                <ItemTemplate>
                                    <span class='badge badge-<%# GetSegmentoBadgeClass(Eval("segmento_crm").ToString()) %>'>
                                        <%# Eval("segmento_crm") %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="total_reservas" HeaderText="Total Reservas" />

                            <asp:TemplateField HeaderText="Pontos">
                                <ItemTemplate>
                                    <span class="badge badge-success"><%# Eval("total_pontos") %> pts</span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btn_ficha" runat="server"
                                                CommandName="VerFicha"
                                                CommandArgument='<%# Eval("id_cliente") %>'
                                                CssClass="btn btn-sm btn-outline-primary">
                                        <i class="fas fa-eye"></i> Detalhe
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="alert alert-info m-3">
                                Nenhum cliente encontrado. Tente outros critérios de pesquisa.
                            </div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                    </div>
                </div>
                <div class="card-footer">
                    <div class="row align-items-center">
                        <div class="col-md-4">
                            <asp:Label ID="lbl_total" runat="server" CssClass="text-muted" />
                        </div>
                        <div class="col-md-4 text-center">
                            <asp:Button ID="btn_anterior" runat="server"
                                Text="&laquo; Anterior"
                                CssClass="btn btn-sm btn-outline-secondary mr-1"
                                OnClick="btn_anterior_Click"
                                CausesValidation="false" />
                            <asp:Label ID="lbl_pagina" runat="server" CssClass="mx-2" />
                            <asp:Button ID="btn_seguinte" runat="server"
                                Text="Seguinte &raquo;"
                                CssClass="btn btn-sm btn-outline-secondary ml-1"
                                OnClick="btn_seguinte_Click"
                                CausesValidation="false" />
                        </div>
                        <div class="col-md-4 text-right">
                            <label class="mr-2">Por página:</label>
                            <asp:DropDownList ID="ddl_por_pagina" runat="server"
                                CssClass="form-control-sm"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddl_por_pagina_Changed">
                                <asp:ListItem Value="15" Text="15" />
                                <asp:ListItem Value="30" Text="30" />
                                <asp:ListItem Value="100" Text="100" />
                                <asp:ListItem Value="0" Text="Todos" />
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>

    <!-- ═══════════════════════════════════════════
         MODAL: FICHA DE CLIENTE (COM MELHORIAS)
         ═══════════════════════════════════════════ -->
    <asp:Panel ID="pnl_detalhe" runat="server" Visible="true">

    <div class="modal fade" id="modalFichaCliente" tabindex="-1" role="dialog"
         aria-labelledby="modalFichaClienteLabel" aria-hidden="true">
        <div class="modal-dialog modal-xl modal-dialog-scrollable" role="document">
            <div class="modal-content">

                <!-- CABEÇALHO -->
                <div class="modal-header bg-success">
                    <h5 class="modal-title text-white" id="modalFichaClienteLabel">
                        <i class="fas fa-address-card mr-2"></i>Ficha de Cliente —
                        <asp:Literal ID="lit_nome" runat="server" />
                    </h5>
                    <asp:Button ID="btn_fechar_detalhe" runat="server" Text="✕"
                        CssClass="close text-white" OnClick="btn_fechar_detalhe_Click"
                        data-dismiss="modal" CausesValidation="false"
                        style="opacity:0.8; font-size:1.4rem; background:none; border:none;" />
                </div>

                <div class="modal-body">

                    <!-- ══════════════════════════════════════════════════════
                         MELHORIA: CALLOUT COM INFO RÁPIDA
                         ══════════════════════════════════════════════════════ -->
                    <div class="callout callout-info mb-3">
                        <div class="row">
                            <div class="col-md-3">
                                <small class="text-muted">Total Reservas</small>
                                <h4 class="mb-0"><asp:Literal ID="lit_total_reservas_cliente" runat="server" Text="0" /></h4>
                            </div>
                            <div class="col-md-3">
                                <small class="text-muted">Pontos</small>
                                <h4 class="mb-0">
                                    <span class="badge badge-success">
                                        <asp:Literal ID="lit_total_pontos" runat="server" Text="0" /> pts
                                    </span>
                                </h4>
                            </div>
                            <div class="col-md-3">
                                <small class="text-muted">Cliente desde</small>
                                <h4 class="mb-0"><asp:Literal ID="lit_data_registo" runat="server" /></h4>
                            </div>
                            <div class="col-md-3">
                                <small class="text-muted">Segmento</small>
                                <h4 class="mb-0">
                                    <span class='badge badge-<%= GetSegmentoBadgeClass(lit_segmento_actual.Text) %>'>
                                        <asp:Literal ID="lit_segmento_actual" runat="server" />
                                    </span>
                                </h4>
                            </div>
                        </div>
                    </div>

                    <!-- SECÇÃO 1: DADOS DO CLIENTE -->
                    <div class="card card-outline card-success mb-3">
                        <div class="card-header">
                            <h3 class="card-title">
                                <i class="fas fa-user mr-2"></i>Dados Pessoais
                            </h3>
                        </div>
                        <div class="card-body">
                            <!-- Info estática -->
                            <div class="row mb-3">
                                <div class="col-md-6">
                                    <p class="mb-1"><strong>Email:</strong> <asp:Literal ID="lit_email" runat="server" /></p>
                                </div>
                            </div>
                            <!-- Campos editáveis -->
                            <div class="row">
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <label>Nome</label>
                                        <asp:TextBox ID="tb_nome" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <label>Telefone</label>
                                        <asp:TextBox ID="tb_telefone" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <label>Segmento CRM</label>
                                        <asp:DropDownList ID="ddl_segmento" runat="server" CssClass="form-control select2">
                                            <asp:ListItem Value="Regular">Regular</asp:ListItem>
                                            <asp:ListItem Value="VIP">VIP</asp:ListItem>
                                            <asp:ListItem Value="Inactivo">Inactivo</asp:ListItem>
                                            <asp:ListItem Value="B2B">B2B</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label>Alergias / Notas (visíveis no site)</label>
                                        <asp:TextBox ID="tb_alergias" runat="server" CssClass="form-control"
                                            TextMode="MultiLine" Rows="2"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label>Preferências de Vinho</label>
                                        <asp:TextBox ID="tb_preferencias" runat="server" CssClass="form-control"
                                            TextMode="MultiLine" Rows="2"></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                            <!-- ══════════════════════════════════════════════════════
                                 MELHORIA: NOTAS PRIVADAS (SÓ BACKOFFICE)
                                 ══════════════════════════════════════════════════════ -->
                            <div class="row" style='<%= IsGestor ? "display:none;" : "" %>'>
                                <div class="col-12">
                                    <div class="form-group">
                                        <label>
                                            <i class="fas fa-lock text-muted mr-1"></i>
                                            Notas Internas <small class="text-muted">(só visíveis no backoffice)</small>
                                        </label>
                                        <asp:TextBox ID="tb_notas_backoffice" runat="server" CssClass="form-control"
                                            TextMode="MultiLine" Rows="2"
                                            placeholder="Notas privadas para uso interno..."></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                            <asp:Button ID="btn_guardar" runat="server" Text="Guardar Alterações"
                                        CssClass="btn btn-success" OnClick="btn_guardar_Click" />
                            <hr class="mt-3 mb-2" />
                            <p class="text-muted mb-0" style="font-size:0.85rem;">
                                <i class="fas fa-history mr-1"></i>
                                <strong>Última actualização:</strong>
                                <asp:Literal ID="lit_ultima_actualizacao" runat="server" />
                            </p>
                        </div>
                    </div>

                    <!-- ══════════════════════════════════════════════════════
                         MELHORIA: TIMELINE DE ACTIVIDADE (CSS PURO)
                         ══════════════════════════════════════════════════════ -->
                    <div class="card card-outline card-info mb-3">
                        <div class="card-header">
                            <h3 class="card-title">
                                <i class="fas fa-history mr-2"></i>Actividade Recente
                            </h3>
                        </div>
                        <div class="card-body" style="max-height: 300px; overflow-y: auto;">
                            <asp:Repeater ID="rpt_timeline" runat="server">
                                <ItemTemplate>
                                    <div class="timeline">
                                        <div class="time-label">
                                            <span class='bg-<%# Eval("cor_data") %>'><%# Eval("data_formatada") %></span>
                                        </div>
                                        <div>
                                            <i class='fas <%# Eval("icone") %> bg-<%# Eval("cor_icone") %>'></i>
                                            <div class="timeline-item">
                                                <span class="time"><i class="fas fa-clock"></i> <%# Eval("hora") %></span>
                                                <h3 class="timeline-header no-border"><%# Eval("titulo") %></h3>
                                                <div class="timeline-body"><%# Eval("descricao") %></div>
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Label ID="lbl_sem_actividade" runat="server" Visible="false"
                                CssClass="text-muted" Text="Sem actividade recente registada." />
                        </div>
                    </div>

                    <!-- SECÇÃO 2: HISTÓRICO DE RESERVAS -->
                    <div class="card mb-3">
                        <div class="card-header">
                            <h3 class="card-title">
                                <i class="fas fa-calendar-check mr-2"></i>Histórico de Reservas

                            </h3>
                        </div>
                        <div class="card-body p-0">
                            <asp:GridView ID="gv_reservas" runat="server"
                                          AutoGenerateColumns="False"
                                          CssClass="table table-hover m-0"
                                          GridLines="None">
                                <Columns>
                                    <asp:BoundField DataField="num_reserva" HeaderText="Nº Reserva" />
                                    <asp:BoundField DataField="experiencia" HeaderText="Experiência" />
                                    <asp:BoundField DataField="data_hora" HeaderText="Data/Hora"
                                        DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                                    <asp:BoundField DataField="num_pessoas" HeaderText="Pessoas" />
                                    <asp:BoundField DataField="preco_total" HeaderText="Preço"
                                        DataFormatString="{0:N2} €" />
                                    <asp:TemplateField HeaderText="Estado">
                                        <ItemTemplate>
                                            <span class='badge badge-<%# GetEstadoBadgeClass(Eval("estado").ToString()) %>'>
                                                <%# Eval("estado") %>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <div class="alert alert-info m-3">Este cliente ainda não tem reservas.</div>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                    </div>

                    <!-- SECÇÃO 3: VINHOS FAVORITOS -->
                    <div class="card mb-0">
                        <div class="card-header">
                            <h3 class="card-title">
                                <i class="fas fa-heart mr-2 text-dark"></i>Vinhos Favoritos
                            </h3>
                        </div>
                        <div class="card-body p-0">
                            <asp:Label ID="lbl_sem_favoritos" runat="server" Visible="false"
                                CssClass="d-block alert alert-info m-3"
                                Text="Este cliente não tem vinhos favoritos." />
                            <asp:GridView ID="gv_favoritos" runat="server"
                                          AutoGenerateColumns="False"
                                          CssClass="table table-hover m-0"
                                          GridLines="None">
                                <Columns>
                                    <asp:BoundField DataField="nome" HeaderText="Nome" />
                                    <asp:BoundField DataField="casta" HeaderText="Casta" />
                                    <asp:BoundField DataField="tipo" HeaderText="Tipo" />
                                    <asp:BoundField DataField="ano" HeaderText="Ano" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>

                </div>

                <!-- RODAPÉ DO MODAL -->
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Fechar</button>
                </div>

            </div>
        </div>
    </div>

    </asp:Panel>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>