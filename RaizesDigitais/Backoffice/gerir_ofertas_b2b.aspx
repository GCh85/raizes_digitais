<%@ Page Title="Ofertas B2B | Raízes Digitais" Language="C#" MasterPageFile="~/MasterBackoffice.Master" AutoEventWireup="true" CodeBehind="gerir_ofertas_b2b.aspx.cs" Inherits="RaizesDigitais.Backoffice.gerir_ofertas_b2b" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder3" runat="server">

    <div class="content-header">
        <div class="container-fluid">
            <div class="row mb-2">
                <div class="col-sm-6">
                    <h1 class="m-0">Ofertas Exclusivas B2B</h1>
                </div>
                <div class="col-sm-6">
                    <ol class="breadcrumb float-sm-right">
                        <li class="breadcrumb-item"><a href="dashboard.aspx">Início</a></li>
                        <li class="breadcrumb-item active">Ofertas B2B</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>

    <div class="content">
        <div class="container-fluid">

            <!-- NOTA INFORMATIVA -->
            <div class="alert alert-info alert-dismissible">
                <button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>
                <h5><i class="icon fas fa-info-circle"></i> Como funciona</h5>
                Marque as experiências que pretende disponibilizar como oferta exclusiva para clientes B2B.
                Os clientes com segmento <strong>B2B</strong> verão estas experiências na tab "Ofertas Exclusivas"
                da área pessoal, com o desconto indicado já aplicado.
            </div>

            <!-- FILTROS -->
            <div class="card mb-4 card-outline card-primary">
                <div class="card-header">
                    <h3 class="card-title">Filtrar Experiências</h3>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-4">
                            <label>Estado B2B</label>
                            <asp:DropDownList ID="ddl_filtro_b2b" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">— Todas —</asp:ListItem>
                                <asp:ListItem Value="1">Só com oferta B2B activa</asp:ListItem>
                                <asp:ListItem Value="0">Só sem oferta B2B</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-4">
                            <label>Tipo</label>
                            <asp:DropDownList ID="ddl_filtro_tipo" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">— Todos —</asp:ListItem>
                                <asp:ListItem Value="Prova">Prova</asp:ListItem>
                                <asp:ListItem Value="Visita">Visita</asp:ListItem>
                                <asp:ListItem Value="Workshop">Workshop</asp:ListItem>
                                <asp:ListItem Value="Jantar">Jantar</asp:ListItem>
                                <asp:ListItem Value="Estadia">Estadia</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-4 d-flex align-items-end">
                            <asp:Button ID="btn_filtrar" runat="server" Text="Filtrar"
                                CssClass="btn btn-primary mr-2" OnClick="btn_filtrar_Click" />
                            <asp:Button ID="btn_limpar" runat="server" Text="Limpar"
                                CssClass="btn btn-secondary" OnClick="btn_limpar_Click"
                                CausesValidation="false" />
                        </div>
                    </div>
                </div>
            </div>

            <!-- GRID DE EXPERIÊNCIAS -->
            <div class="card">
                <div class="card-header">
                    <h3 class="card-title">Experiências</h3>
                </div>
                <div class="card-body p-0">
                    <div style="overflow-x:auto;">
                        <asp:GridView ID="gv_experiencias" runat="server"
                            AutoGenerateColumns="False"
                            CssClass="table table-hover m-0"
                            GridLines="None"
                            DataKeyNames="id_experiencia"
                            OnRowCommand="gv_experiencias_RowCommand">
                            <Columns>

                                <asp:BoundField DataField="nome" HeaderText="Experiência" />
                                <asp:BoundField DataField="tipo" HeaderText="Tipo" />
                                <asp:BoundField DataField="preco_por_pessoa" HeaderText="Preço/Pessoa"
                                    DataFormatString="{0:0.00} €" />

                                <asp:TemplateField HeaderText="Oferta B2B">
                                    <ItemTemplate>
                                        <%# Convert.ToBoolean(Eval("oferta_b2b"))
                                            ? "<span class='badge badge-warning'><i class='fas fa-star mr-1'></i>Activa</span>"
                                            : "<span class='badge badge-secondary'>Inactiva</span>" %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Desconto B2B">
                                    <ItemTemplate>
                                        <%# Eval("desconto_b2b") == DBNull.Value || !Convert.ToBoolean(Eval("oferta_b2b"))
                                            ? "<span class='text-muted'>—</span>"
                                            : "<strong>" + Convert.ToDecimal(Eval("desconto_b2b")).ToString("0.##") + "%</strong>" %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Preço Final B2B">
                                    <ItemTemplate>
                                        <%# GetPrecoBadge(Eval("oferta_b2b"), Eval("preco_por_pessoa"), Eval("desconto_b2b")) %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btn_configurar" runat="server"
                                            CommandName="Configurar"
                                            CommandArgument='<%# Eval("id_experiencia") %>'
                                            CssClass="btn btn-sm btn-outline-primary">
                                            <i class="fas fa-cog"></i> Configurar
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                            <EmptyDataTemplate>
                                <div class="alert alert-info m-3">Nenhuma experiência encontrada.</div>
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
                            <asp:Button ID="btn_anterior" runat="server" Text="&laquo; Anterior"
                                CssClass="btn btn-sm btn-outline-secondary mr-1"
                                OnClick="btn_anterior_Click" CausesValidation="false" />
                            <asp:Label ID="lbl_pagina" runat="server" CssClass="mx-2" />
                            <asp:Button ID="btn_seguinte" runat="server" Text="Seguinte &raquo;"
                                CssClass="btn btn-sm btn-outline-secondary ml-1"
                                OnClick="btn_seguinte_Click" CausesValidation="false" />
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

    <!-- CONFIGURAR OFERTA B2B -->
    <div class="modal fade" id="modalB2B" tabindex="-1" role="dialog" aria-labelledby="modalB2BLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header bg-warning">
                    <h5 class="modal-title" id="modalB2BLabel">
                        <i class="fas fa-star mr-2"></i>Configurar Oferta B2B —
                        <asp:Literal ID="lit_nome_experiencia" runat="server" />
                    </h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">

                    <asp:HiddenField ID="hf_id_experiencia" runat="server" Value="0" />

                    <div class="form-group">
                        <label>Estado da Oferta B2B</label>
                        <div>
                            <asp:CheckBox ID="cb_oferta_b2b" runat="server" />
                            <label for="ContentPlaceHolder3_cb_oferta_b2b" style="font-weight:400; cursor:pointer; margin-left:6px;">
                                Activar como oferta exclusiva B2B
                            </label>
                        </div>
                        <small class="text-muted">
                            Quando activo, esta experiência aparece na tab "Ofertas Exclusivas" para clientes B2B.
                        </small>
                    </div>

                    <div class="form-group">
                        <label>Desconto B2B (%)</label>
                        <asp:TextBox ID="tb_desconto_b2b" runat="server" CssClass="form-control"
                            placeholder="Ex: 15 (para 15% de desconto)" TextMode="Number" />
                        <small class="text-muted">Obrigatório se a oferta B2B estiver activa. Valor entre 1 e 100.</small>
                    </div>

                    <asp:Label ID="lbl_erro_modal" runat="server" CssClass="text-danger d-block" Visible="false" />

                    <!-- Pré-visualização do preço -->
                    <asp:Panel ID="pnl_preview" runat="server" Visible="false">
                        <div class="alert alert-success mt-2 mb-0">
                            <i class="fas fa-tag mr-1"></i>
                            Preço original: <strong><asp:Literal ID="lit_preco_original" runat="server" /> €</strong>
                            &rarr; Preço B2B: <strong><asp:Literal ID="lit_preco_b2b" runat="server" /> €</strong>
                        </div>
                    </asp:Panel>

                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancelar</button>
                    <asp:Button ID="btn_guardar_b2b" runat="server" Text="Guardar"
                        CssClass="btn btn-warning" OnClick="btn_guardar_b2b_Click" />
                </div>
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
