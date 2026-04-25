<%@ Page Title="Reservas | Raízes Digitais" Language="C#" MasterPageFile="~/MasterBackoffice.Master" AutoEventWireup="true" CodeBehind="gerir_reservas.aspx.cs" Inherits="RaizesDigitais.Backoffice.gerir_reservas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder3" runat="server">

    <div class="content-header">
        <div class="container-fluid">
            <div class="row mb-2">
                <div class="col-sm-6">
                    <h1 class="m-0">Reservas</h1>
                </div>
                <div class="col-sm-6">
                    <ol class="breadcrumb float-sm-right">
                        <li class="breadcrumb-item">
                            <a href="dashboard.aspx">Início</a>
                        </li>
                        <li class="breadcrumb-item active">Reservas</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>

    <div class="content">
        <div class="container-fluid">

            <!-- ═══════════════════════════════════════════════════════════════
                 MELHORIA: CALLOUT DE ALERTA (PENDENTES)
                 ═══════════════════════════════════════════════════════════════ -->
            <asp:Panel ID="pnl_alerta_pendentes" runat="server" Visible="false">
                <div class="alert alert-warning alert-dismissible">
                    <button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>
                    <h5><i class="icon fas fa-exclamation-triangle"></i> Reservas Pendentes</h5>
                    Existem <strong><asp:Literal ID="lit_pendentes_alerta" runat="server" /></strong> reservas à espera de confirmação.
                    <asp:LinkButton ID="btn_ver_pendentes" runat="server" CssClass="alert-link" OnClick="btn_ver_pendentes_Click">Ver reservas pendentes</asp:LinkButton>
                </div>
            </asp:Panel>

            <!-- ═══════════════════════════════════════════════════════════════
                 MELHORIA: BADGES DE CONTAGEM RÁPIDA
                 ═══════════════════════════════════════════════════════════════ -->
            <div class="row mb-3" style='<%= IsGestor ? "display:none;" : "" %>'>
                <div class="col-12">
                    <div class="d-flex flex-wrap gap-2">
                        <asp:LinkButton ID="badge_todas" runat="server" CssClass="btn btn-outline-secondary btn-sm" OnClick="badge_todas_Click">
                            Todas <span class="badge badge-light"><asp:Literal ID="lit_count_todas" runat="server" Text="0" /></span>
                        </asp:LinkButton>
                        <asp:LinkButton ID="badge_pendentes" runat="server" CssClass="btn btn-outline-warning btn-sm" OnClick="badge_pendentes_Click">
                            Pendentes <span class="badge badge-light"><asp:Literal ID="lit_count_pendentes" runat="server" Text="0" /></span>
                        </asp:LinkButton>
                        <asp:LinkButton ID="badge_confirmadas" runat="server" CssClass="btn btn-outline-success btn-sm" OnClick="badge_confirmadas_Click">
                            Confirmadas <span class="badge badge-light"><asp:Literal ID="lit_count_confirmadas" runat="server" Text="0" /></span>
                        </asp:LinkButton>
                        <asp:LinkButton ID="badge_canceladas" runat="server" CssClass="btn btn-outline-danger btn-sm" OnClick="badge_canceladas_Click">
                            Canceladas <span class="badge badge-light"><asp:Literal ID="lit_count_canceladas" runat="server" Text="0" /></span>
                        </asp:LinkButton>
                        <asp:LinkButton ID="badge_concluidas" runat="server" CssClass="btn btn-outline-secondary btn-sm" OnClick="badge_concluidas_Click">
                            Concluídas <span class="badge badge-light"><asp:Literal ID="lit_count_concluidas" runat="server" Text="0" /></span>
                        </asp:LinkButton>
                    </div>
                </div>
            </div>

            <!-- ═══════════════════════════════════════════════════════════════
                 FILTROS
                 ═══════════════════════════════════════════════════════════════ -->
            <div class="card card-outline card-primary">
                <div class="card-header">
                    <h3 class="card-title">Filtrar</h3>
                    <div class="card-tools">
                        <!-- MELHORIA: Botão Exportar CSV -->
                        <asp:Button ID="btn_exportar" runat="server"
                            Text="Exportar CSV"
                            CssClass="btn btn-outline-success btn-sm"
                            OnClick="btn_exportar_Click"
                            CausesValidation="false" />
                    </div>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-2">
                            <div class="form-group">
                                <label>Estado</label>
                                <asp:DropDownList ID="ddl_estado" runat="server" CssClass="form-control select2">
                                    <asp:ListItem Value="">— Todos —</asp:ListItem>
                                    <asp:ListItem Value="Pendente">Pendente</asp:ListItem>
                                    <asp:ListItem Value="Confirmada">Confirmada</asp:ListItem>
                                    <asp:ListItem Value="Cancelada">Cancelada</asp:ListItem>
                                    <asp:ListItem Value="Concluida">Concluída</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <!-- MELHORIA: Filtro por Experiência -->
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Experiência</label>
                                <asp:DropDownList ID="ddl_experiencia" runat="server" CssClass="form-control select2">
                                    <asp:ListItem Value="">— Todas —</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <label>Período</label>
                                <div class="row">
                                    <div class="col-6 pr-1">
                                        <asp:TextBox ID="tb_data_inicio" runat="server" CssClass="form-control" TextMode="Date" />
                                    </div>
                                    <div class="col-6 pl-1">
                                        <asp:TextBox ID="tb_data_fim" runat="server" CssClass="form-control" TextMode="Date" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>&nbsp;</label>
                                <div>
                                    <asp:Button ID="btn_filtrar" runat="server" Text="Filtrar"
                                        CssClass="btn btn-primary mr-2"
                                        OnClick="btn_filtrar_Click"
                                        CausesValidation="false" />
                                    <asp:Button ID="btn_limpar" runat="server" Text="Limpar"
                                        CssClass="btn btn-default"
                                        OnClick="btn_limpar_Click"
                                        CausesValidation="false" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- ═══════════════════════════════════════════════════════════════
                 LISTA DE RESERVAS
                 ═══════════════════════════════════════════════════════════════ -->
            <div class="card">
                <div class="card-header">
                    <h3 class="card-title">
                        Reservas
                        <!-- MELHORIA: Badge com total no header -->
                        <span class="badge badge-primary ml-2"><asp:Literal ID="lit_total_registos" runat="server" Text="0" /></span>
                    </h3>
                </div>
                <div class="card-body p-0">
                    <div style="overflow-x:auto;">
                    <asp:GridView ID="gv_reservas" runat="server"
                        CssClass="table table-hover m-0"
                        AutoGenerateColumns="false"
                        GridLines="None"
                        OnRowCommand="gv_reservas_RowCommand"
                        OnRowDataBound="gv_reservas_RowDataBound"
                        AllowSorting="True"
                        OnSorting="gv_reservas_Sorting">
                        <Columns>
                            <asp:BoundField DataField="num_reserva" HeaderText="Nº Reserva" SortExpression="num_reserva" />
                            <asp:BoundField DataField="cliente" HeaderText="Cliente" SortExpression="cliente" />
                            <asp:BoundField DataField="experiencia" HeaderText="Experiência" SortExpression="experiencia" />
                            <asp:BoundField DataField="data_hora" HeaderText="Data Visita" DataFormatString="{0:dd/MM/yyyy HH:mm}" SortExpression="data_hora" />
                            <asp:BoundField DataField="num_pessoas" HeaderText="Pessoas" SortExpression="num_pessoas" />
                            <asp:BoundField DataField="preco_total" HeaderText="Total" DataFormatString="{0:0.00} €" SortExpression="preco_total" />
                            <asp:TemplateField HeaderText="Estado" SortExpression="estado">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_estado" runat="server" Text='<%# Eval("estado") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Detalhe">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btn_detalhe" runat="server"
                                        CommandName="VerDetalhe"
                                        CommandArgument='<%# Eval("id_reserva") %>'
                                        CssClass="btn btn-sm btn-outline-primary"
                                        CausesValidation="false">
                                        <i class="fas fa-eye"></i> Detalhe
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
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
         MODAL: DETALHE DA RESERVA
         ═══════════════════════════════════════════ -->
    <asp:Panel ID="pnl_detalhe" runat="server" Visible="true">

    <div class="modal fade" id="modalDetalheReserva" tabindex="-1" role="dialog"
         aria-labelledby="modalDetalheReservaLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
            <div class="modal-content">

                <!-- CABEÇALHO -->
                <div class="modal-header bg-success">
                    <h5 class="modal-title text-white" id="modalDetalheReservaLabel">
                        <i class="fas fa-calendar-check mr-2"></i>Detalhe —
                        <asp:Literal ID="lit_num_reserva" runat="server" />
                    </h5>
                    <asp:Button ID="btn_fechar_detalhe" runat="server" Text="✕"
                        CssClass="close text-white" OnClick="btn_fechar_detalhe_Click"
                        data-dismiss="modal" CausesValidation="false"
                        style="opacity:0.8; font-size:1.4rem; background:none; border:none;" />
                </div>

                <div class="modal-body">

                    <!-- DADOS DA RESERVA: 3 colunas -->
                    <div class="row mb-3">
                        <div class="col-md-4">
                            <p class="mb-1"><strong>Cliente:</strong><br /><asp:Literal ID="lit_cliente" runat="server" /></p>
                            <p class="mb-1"><strong>Email:</strong><br /><asp:Literal ID="lit_email" runat="server" /></p>
                            <p class="mb-0"><strong>Telefone:</strong><br /><asp:Literal ID="lit_telefone" runat="server" /></p>
                        </div>
                        <div class="col-md-4">
                            <p class="mb-1"><strong>Experiência:</strong><br /><asp:Literal ID="lit_experiencia" runat="server" /></p>
                            <p class="mb-1"><strong>Data da Visita:</strong><br /><asp:Literal ID="lit_data_hora" runat="server" /></p>
                            <p class="mb-0"><strong>Participantes:</strong><br /><asp:Literal ID="lit_num_pessoas" runat="server" /></p>
                        </div>
                        <div class="col-md-4">
                            <p class="mb-1"><strong>Total:</strong><br /><asp:Literal ID="lit_preco_total" runat="server" /></p>
                            <p class="mb-0"><strong>Notas:</strong><br /><asp:Literal ID="lit_notas" runat="server" /></p>
                        </div>
                    </div>

                    <hr class="mt-1 mb-3" />

                    <!-- ALTERAR ESTADO (SÓ ADMIN) -->
                    <div style='<%= IsGestor ? "display:none;" : "" %>'>
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group mb-0">
                                    <label>Alterar Estado</label>
                                    <asp:DropDownList ID="ddl_novo_estado" runat="server" CssClass="form-control">
                                        <asp:ListItem Value="Pendente">Pendente</asp:ListItem>
                                        <asp:ListItem Value="Confirmada">Confirmada</asp:ListItem>
                                        <asp:ListItem Value="Cancelada">Cancelada</asp:ListItem>
                                        <asp:ListItem Value="Concluida">Concluída</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-md-6 d-flex align-items-end">
                                <asp:Button ID="btn_actualizar_estado" runat="server" Text="Guardar Estado"
                                            CssClass="btn btn-success" OnClick="btn_actualizar_estado_Click" />
                            </div>
                        </div>
                        <hr class="mt-3 mb-2" />
                    </div>

                    <p class="text-muted small mb-0">
                        <i class="fas fa-history mr-1"></i>
                        <strong>Última actualização:</strong>
                        <asp:Literal ID="lit_ultima_actualizacao" runat="server" Text="Por actualizar" />
                    </p>

                </div>

                <!-- RODAPÉ -->
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