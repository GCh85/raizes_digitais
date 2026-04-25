<%@ Page Title="" Language="C#" MasterPageFile="~/MasterBackoffice.Master" AutoEventWireup="true" CodeBehind="gerir_utilizadores.aspx.cs" Inherits="RaizesDigitais.Backoffice.gerir_utilizadores" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder3" runat="server">

    <div class="content-header">
        <div class="container-fluid">
            <div class="row mb-2">
                <div class="col-sm-6">
                    <h1 class="m-0">Utilizadores</h1>
                </div>
                <div class="col-sm-6">
                    <ol class="breadcrumb float-sm-right">
                        <li class="breadcrumb-item">
                            <a href="dashboard.aspx">Início</a>
                        </li>
                        <li class="breadcrumb-item active">Utilizadores</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>

    <div class="content">
        <div class="container-fluid">

            <asp:Label ID="lbl_sucesso" runat="server" 
                CssClass="alert alert-success d-none mb-3" Visible="false" />
            <asp:Label ID="lbl_erro" runat="server" 
                CssClass="alert alert-danger d-none mb-3" Visible="false" />

            <!-- PAINEL EDITAR -->
            <asp:Panel ID="pnl_editar" runat="server" Visible="false">
                <div class="card card-warning mb-4">
                    <div class="card-header">
                        <h3 class="card-title">
                            <asp:Literal ID="lit_titulo_form" runat="server" Text="Editar Utilizador" />
                        </h3>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label>Utilizador</label>
                                    <asp:TextBox ID="tb_utilizador" runat="server" CssClass="form-control" />
                                    <asp:RequiredFieldValidator ID="rfv_utilizador" runat="server"
                                        ControlToValidate="tb_utilizador"
                                        ErrorMessage="Campo obrigatório."
                                        CssClass="text-danger" Display="Dynamic"
                                        ValidationGroup="vg_editar"
                                        EnableClientScript="false" />
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label>Email</label>
                                    <asp:TextBox ID="tb_email" runat="server" CssClass="form-control" />
                                    <asp:RequiredFieldValidator ID="rfv_email" runat="server"
                                        ControlToValidate="tb_email"
                                        ErrorMessage="Campo obrigatório."
                                        CssClass="text-danger" Display="Dynamic"
                                        ValidationGroup="vg_editar"
                                        EnableClientScript="false" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <div class="form-group">
                                    <label>Perfil</label>
                                    <asp:DropDownList ID="ddl_perfil" runat="server" CssClass="form-control select2" />
                                </div>
                            </div>
                            <div class="col-md-2">
                                <asp:Panel ID="pnl_activo" runat="server">
                                <div class="form-group">
                                    <label>Activo</label>
                                    <asp:DropDownList ID="ddl_activo" runat="server" CssClass="form-control">
                                        <asp:ListItem Value="1" Text="Sim" />
                                        <asp:ListItem Value="0" Text="Não" />
                                    </asp:DropDownList>
                                </div>
                                </asp:Panel>
                            </div>
                        </div>
                        <asp:Panel ID="pnl_password" runat="server" Visible="false">
                            <div class="row">
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <label>Password Inicial</label>
                                        <asp:TextBox ID="tb_password_inicial" runat="server" CssClass="form-control" TextMode="Password" />
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <label>Confirmar Password</label>
                                        <asp:TextBox ID="tb_password_confirmar" runat="server" CssClass="form-control" TextMode="Password" />
                                    </div>
                                </div>
                                <div class="col-md-4 d-flex align-items-end">
                                    <p class="text-muted small mb-3">
                                        O novo utilizador fica activo de imediato e depois pode alterar a password em "Alterar Password".
                                    </p>
                                </div>
                            </div>
                        </asp:Panel>
                        <asp:HiddenField ID="hf_id_utilizador" runat="server" Value="0" />
                        <asp:Button ID="btn_guardar" runat="server"
                            Text="Guardar"
                            CssClass="btn btn-warning mr-2"
                            OnClick="btn_guardar_Click"
                            ValidationGroup="vg_editar" />
                        <asp:Button ID="btn_cancelar" runat="server"
                            Text="Cancelar"
                            CssClass="btn btn-secondary"
                            OnClick="btn_cancelar_Click"
                            CausesValidation="false" />
                    </div>
                </div>
            </asp:Panel>

            <!-- PAINEL PESQUISA + LISTA -->
            <div class="card">
                <div class="card-header">
                    <div class="row align-items-center">
                        <div class="col-md-6">
                            <h3 class="card-title">Lista de Utilizadores</h3>
                            <asp:Button ID="btn_novo_utilizador" runat="server"
                                Text="Novo Utilizador"
                                CssClass="btn btn-sm btn-primary ml-3"
                                OnClick="btn_novo_utilizador_Click"
                                CausesValidation="false" />
                        </div>
                        <div class="col-md-6">
                            <div class="input-group">
                                <asp:TextBox ID="tb_pesquisa" runat="server"
                                    CssClass="form-control" placeholder="Pesquisar..." />
                                <div class="input-group-append">
                                    <asp:Button ID="btn_pesquisar" runat="server"
                                        Text="Pesquisar"
                                        CssClass="btn btn-primary"
                                        OnClick="btn_pesquisar_Click"
                                        CausesValidation="false" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="card-body p-0">
                    <asp:GridView ID="gv_utilizadores" runat="server"
                        CssClass="table table-striped table-hover mb-0"
                        AutoGenerateColumns="false"
                        DataKeyNames="id_utilizador"
                        OnRowCommand="gv_utilizadores_RowCommand"
                        GridLines="None">
                        <Columns>
                            <asp:BoundField DataField="utilizador" HeaderText="Utilizador" />
                            <asp:BoundField DataField="email" HeaderText="Email" />
                            <asp:BoundField DataField="perfil" HeaderText="Perfil" />
                            <asp:BoundField DataField="activo" HeaderText="Activo" />
                            <asp:TemplateField HeaderText="Ações">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btn_editar" runat="server"
                                        CommandName="Editar"
                                        CommandArgument='<%# Eval("id_utilizador") %>'
                                        CssClass="btn btn-sm btn-outline-warning mr-1"
                                        CausesValidation="false">
                                        <i class="fas fa-edit"></i> Editar
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btn_eliminar" runat="server"
                                        CommandName="Eliminar"
                                        CommandArgument='<%# Eval("id_utilizador") %>'
                                        CssClass="btn btn-sm btn-outline-danger"
                                        CausesValidation="false"
                                        OnClientClick="return confirmSwal(this, 'Eliminar utilizador', 'Tem a certeza que deseja eliminar este utilizador?', 'Sim, eliminar');">
                                        <i class="fas fa-trash"></i> Eliminar
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="p-3 text-muted">Nenhum utilizador encontrado.</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
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

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="scripts" runat="server">
    <script>
        $(document).ready(function() {
            $('#<%= gv_utilizadores.ClientID %>').DataTable({
                "language": {
                    "lengthMenu": "Mostrar _MENU_ registos",
                    "zeroRecords": "Nenhum utilizador encontrado",
                    "info": "Página _PAGE_ de _PAGES_",
                    "infoEmpty": "Sem utilizadores",
                    "infoFiltered": "(filtrado de _MAX_)",
                    "search": "Procurar:",
                    "paginate": {
                        "first": "Primeiro", "last": "Último",
                        "next": "Seguinte", "previous": "Anterior"
                    }
                },
                "paging": false,
                "info": false,
                "searching": false,
                "ordering": true,
                "order": [[0, "asc"]],
                "autoWidth": false
            });
        });
    </script>
</asp:Content>
