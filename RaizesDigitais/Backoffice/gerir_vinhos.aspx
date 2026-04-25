<%@ Page Title="Vinhos | Raízes Digitais" Language="C#" MasterPageFile="~/MasterBackoffice.Master" AutoEventWireup="true" CodeBehind="gerir_vinhos.aspx.cs" Inherits="RaizesDigitais.Backoffice.gerir_vinhos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder3" runat="server">

    <div class="content-header">
        <div class="container-fluid">
            <div class="row mb-2">
                <div class="col-sm-6">
                    <h1 class="m-0">Gestão de Vinhos</h1>
                </div>
                <div class="col-sm-6">
                    <ol class="breadcrumb float-sm-right">
                        <li class="breadcrumb-item">
                            <a href="dashboard.aspx">Início</a>
                        </li>
                        <li class="breadcrumb-item active">Gestão de Vinhos</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>

    <div class="content">
        <div class="container-fluid">

            <!-- LISTA DE VINHOS -->
            <div class="card">
                <div class="card-header">
                    <h3 class="card-title">Catálogo de Vinhos</h3>
                    <div class="card-tools">

                        <!-- Botão Novo Vinho no header — só Admin -->
                        <asp:Button ID="btn_novo_vinho" runat="server"
                            Text="+ Novo Vinho"
                            CssClass="btn btn-success btn-sm"
                            OnClick="btn_novo_vinho_Click"
                            CausesValidation="false" />
                    </div>
                </div>
                <div class="card-body p-0">
                    <div style="overflow-x:auto;">
                        <asp:GridView ID="gv_vinhos" runat="server"
                            CssClass="table table-hover m-0"
                            AutoGenerateColumns="false"
                            GridLines="None"
                            OnRowCommand="gv_vinhos_RowCommand"
                            OnRowDataBound="gv_vinhos_RowDataBound"
                            AllowSorting="True"
                            OnSorting="gv_vinhos_Sorting">
                            <Columns>
                                <asp:BoundField DataField="nome" HeaderText="Nome" SortExpression="nome" />
                                <asp:BoundField DataField="tipo" HeaderText="Tipo" SortExpression="tipo" />
                                <asp:BoundField DataField="casta" HeaderText="Casta" SortExpression="casta" />
                                <asp:BoundField DataField="ano" HeaderText="Ano" SortExpression="ano" />
                                <asp:BoundField DataField="preco" HeaderText="Preço" DataFormatString="{0:0.00} €" SortExpression="preco" />
                                <asp:BoundField DataField="stock_actual" HeaderText="Stock" SortExpression="stock_actual" />
                                <asp:BoundField DataField="stock_minimo" HeaderText="Mínimo" SortExpression="stock_minimo" />
                                <asp:TemplateField HeaderText="Estado" SortExpression="activo">
                                    <ItemTemplate>
                                        <span class='badge <%# Convert.ToBoolean(Eval("activo")) ? "badge-success" : "badge-secondary" %>'>
                                            <%# Convert.ToBoolean(Eval("activo")) ? "Activo" : "Inactivo" %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btn_editar" runat="server"
                                            CommandName="EditarVinho"
                                            CommandArgument='<%# Eval("id_vinho") %>'
                                            CssClass="btn btn-sm btn-outline-primary mr-1"
                                            CausesValidation="false">
                                            <i class="fas fa-edit"></i> Editar
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btn_toggle" runat="server"
                                            CommandName="ToggleActivo"
                                            CommandArgument='<%# Eval("id_vinho") + "|" + Eval("activo") %>'
                                            CssClass="btn btn-sm btn-outline-secondary"
                                            CausesValidation="false"
                                            OnClientClick="return confirmSwal(this, 'Alterar estado', 'Confirma a alteração do estado?', 'Sim, alterar');">
                                            <%# Convert.ToBoolean(Eval("activo")) ? "Desactivar" : "Activar" %>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="alert alert-info m-3">Nenhum vinho encontrado.</div>
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
         Criar/Editar Vinho
         ═══════════════════════════════════════════ -->
    <div class="modal fade" id="modalVinho" tabindex="-1" role="dialog"
         aria-labelledby="modalVinhoLabel" aria-hidden="true">
        <div class="modal-dialog modal-xl modal-dialog-scrollable" role="document">
            <div class="modal-content">

                <!-- CABEÇALHO -->
                <div class="modal-header bg-success text-white">
                    <h5 class="modal-title" id="modalVinhoLabel">
                        <i class="fas fa-wine-bottle mr-2"></i>
                        <asp:Literal ID="lit_titulo_form" runat="server" Text="Novo Vinho" />
                    </h5>
                    <asp:Button ID="btn_fechar_modal" runat="server" Text="✕"
                        CssClass="close text-white" OnClick="btn_fechar_modal_Click"
                        data-dismiss="modal" CausesValidation="false"
                        style="opacity:0.8; font-size:1.4rem; background:none; border:none;" />
                </div>

                <div class="modal-body">

                    <asp:HiddenField ID="hf_id_vinho" runat="server" Value="0" />

                    <div class="row">
                        <div class="col-md-4">
                            <div class="form-group">
                                <label>Nome <span class="text-danger">*</span></label>
                                <asp:TextBox ID="tb_nome" runat="server" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <label>Casta</label>
                                <asp:TextBox ID="tb_casta" runat="server" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="col-md-2">
                            <div class="form-group">
                                <label>Ano</label>
                                <asp:TextBox ID="tb_ano" runat="server" CssClass="form-control" TextMode="Number" />
                            </div>
                        </div>
                        <div class="col-md-2">
                            <div class="form-group">
                                <label>Tipo</label>
                                <asp:DropDownList ID="ddl_tipo" runat="server" CssClass="form-control select2">
                                    <asp:ListItem Value="Branco">Branco</asp:ListItem>
                                    <asp:ListItem Value="Tinto">Tinto</asp:ListItem>
                                    <asp:ListItem Value="Espumante">Espumante</asp:ListItem>
                                    <asp:ListItem Value="Rosé">Rosé</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Preço (€) <span class="text-danger">*</span></label>
                                <asp:TextBox ID="tb_preco" runat="server" CssClass="form-control" TextMode="Number" />
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Stock Actual <span class="text-danger">*</span></label>
                                <asp:TextBox ID="tb_stock_actual" runat="server" CssClass="form-control" TextMode="Number" />
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Stock Mínimo <span class="text-danger">*</span></label>
                                <asp:TextBox ID="tb_stock_minimo" runat="server" CssClass="form-control" TextMode="Number" />
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>Descrição</label>
                                <asp:TextBox ID="tb_descricao" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>URL da Imagem</label>
                                <asp:TextBox ID="tb_imagem_url" runat="server" CssClass="form-control" placeholder="~/Images/vinho.png" />
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Doçura (0-100)</label>
                                <asp:TextBox ID="tb_docura" runat="server" CssClass="form-control"
                                             TextMode="Number" Min="0" Max="100" Text="0" />
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Acidez (0-100)</label>
                                <asp:TextBox ID="tb_acidez" runat="server" CssClass="form-control"
                                             TextMode="Number" Min="0" Max="100" Text="0" />
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Corpo (0-100)</label>
                                <asp:TextBox ID="tb_corpo" runat="server" CssClass="form-control"
                                             TextMode="Number" Min="0" Max="100" Text="0" />
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Harmonização</label>
                                <asp:TextBox ID="tb_harmonizacao" runat="server" CssClass="form-control"
                                             placeholder="Ex: Carnes vermelhas, queijos..." />
                            </div>
                        </div>
                    </div>

                    <div class="row mt-2 mb-2">
                        <div class="col-md-12">
                            <div class="d-flex align-items-center">
                                <asp:CheckBox ID="cb_activo" runat="server" Text="Visível no website" />
                            </div>
                        </div>
                    </div>

                    <hr />

                    <p class="text-muted mb-0" style="font-size:0.85rem;">
                        <i class="fas fa-history mr-1"></i>
                        <strong>Última actualização:</strong>
                        <asp:Literal ID="lit_ultima_actualizacao_vinho" runat="server" Text="Por actualizar" />
                    </p>

                </div>

                <!-- RODAPÉ DO MODAL -->
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancelar</button>
                    <asp:Button ID="btn_guardar" runat="server" Text="Guardar Vinho"
                        CssClass="btn btn-success"
                        OnClick="btn_guardar_Click"
                        CausesValidation="false" />
                </div>

            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
