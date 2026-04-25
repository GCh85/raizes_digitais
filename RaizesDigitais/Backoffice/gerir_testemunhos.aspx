<%@ Page Title="Testemunhos | Raízes Digitais" Language="C#" MasterPageFile="~/MasterBackoffice.Master" AutoEventWireup="true" CodeBehind="gerir_testemunhos.aspx.cs" Inherits="RaizesDigitais.Backoffice.gerir_testemunhos" ContentType="text/html" ResponseEncoding="utf-8" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder3" runat="server">

    <div class="content-header">
        <div class="container-fluid">
            <div class="row mb-2">
                <div class="col-sm-6">
                    <h1 class="m-0">Testemunhos</h1>
                </div>
                <div class="col-sm-6">
                    <ol class="breadcrumb float-sm-right">
                        <li class="breadcrumb-item">
                            <a href="dashboard.aspx">Início</a>
                        </li>
                        <li class="breadcrumb-item active">Testemunhos</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>

    <div class="content">
        <div class="container-fluid">

            <!-- ═══════════════════════════════════════════════════════════════
                 MELHORIA: BADGES DE CONTAGEM RÁPIDA
                 ═══════════════════════════════════════════════════════════════ -->
            <div class="row mb-3">
                <div class="col-12">
                    <div class="d-flex flex-wrap gap-2">
                        <asp:LinkButton ID="badge_todos" runat="server" CssClass="btn btn-outline-secondary btn-sm" OnClick="badge_todos_Click">
                            Todos <span class="badge badge-light"><asp:Literal ID="lit_count_todos" runat="server" Text="0" /></span>
                        </asp:LinkButton>
                        <asp:LinkButton ID="badge_pendentes" runat="server" CssClass="btn btn-outline-warning btn-sm" OnClick="badge_pendentes_Click">
                            Pendentes <span class="badge badge-light"><asp:Literal ID="lit_count_pendentes" runat="server" Text="0" /></span>
                        </asp:LinkButton>
                        <asp:LinkButton ID="badge_publicados" runat="server" CssClass="btn btn-outline-success btn-sm" OnClick="badge_publicados_Click">
                            Publicados <span class="badge badge-light"><asp:Literal ID="lit_count_publicados" runat="server" Text="0" /></span>
                        </asp:LinkButton>
                    </div>
                </div>
            </div>

            <!-- ═══════════════════════════════════════════════════════════════
                 MELHORIA: CALLOUT PARA PENDENTES
                 ═══════════════════════════════════════════════════════════════ -->
            <asp:Panel ID="pnl_alerta_pendentes" runat="server" Visible="false">
                <div class="alert alert-warning alert-dismissible">
                    <button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>
                    <h5><i class="icon fas fa-exclamation-triangle"></i> Testemunhos Pendentes</h5>
                    Existem <strong><asp:Literal ID="lit_pendentes_alerta" runat="server" /></strong> testemunhos à espera de aprovação.
                </div>
            </asp:Panel>

            <!-- ═══════════════════════════════════════════════════════════════
                 MELHORIA: FILTROS
                 ═══════════════════════════════════════════════════════════════ -->
            <div class="card mb-4 card-outline card-primary">
                <div class="card-header">
                    <h3 class="card-title">Filtrar</h3>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-4">
                            <div class="form-group">
                                <label>Experiência</label>
                                <asp:DropDownList ID="ddl_experiencia" runat="server" CssClass="form-control select2">
                                    <asp:ListItem Value="">— Todas —</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>Classificação</label>
                                <asp:DropDownList ID="ddl_estrelas" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">— Todas —</asp:ListItem>
                                    <asp:ListItem Value="5">★★★★★ (5 estrelas)</asp:ListItem>
                                    <asp:ListItem Value="4">★★★★☆ (4 estrelas)</asp:ListItem>
                                    <asp:ListItem Value="3">★★★☆☆ (3 estrelas)</asp:ListItem>
                                    <asp:ListItem Value="2">★★☆☆☆ (2 estrelas)</asp:ListItem>
                                    <asp:ListItem Value="1">★☆☆☆☆ (1 estrela)</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-5">
                        <div class="form-group">
                            <label>&nbsp;</label>
                            <div>
                                <asp:Button ID="btn_filtrar" runat="server" Text="Filtrar"
                                    CssClass="btn btn-primary mr-2" OnClick="btn_filtrar_Click" />
                                <asp:Button ID="btn_limpar" runat="server" Text="Limpar"
                                    CssClass="btn btn-secondary" OnClick="btn_limpar_Click"
                                    CausesValidation="false" />
                            </div>
                        </div>
                    </div>
                    </div>
                </div>
            </div>

            <!-- LISTA DE TESTEMUNHOS -->
            <div class="card card-outline card-success">
                <div class="card-header">
                    <h3 class="card-title">
                        <i class="fas fa-comments mr-1"></i>
                        Testemunhos Recebidos
                        <!-- MELHORIA: Badge com total -->
                        <span class="badge badge-primary ml-2"><asp:Literal ID="lit_total_lista" runat="server" Text="0" /></span>
                    </h3>
                </div>
                <div class="card-body p-0">
                    <div style="overflow-x:auto;">
                    <asp:GridView ID="gv_testemunhos" runat="server"
                        CssClass="table table-hover"
                        AutoGenerateColumns="false"
                        GridLines="None"
                        OnRowCommand="gv_testemunhos_RowCommand"
                        OnRowDataBound="gv_testemunhos_RowDataBound"
                        EmptyDataText="Ainda não há testemunhos recebidos.">
                        <Columns>
                            <asp:BoundField DataField="nome_cliente" HeaderText="Cliente" />
                            <asp:BoundField DataField="experiencia_nome" HeaderText="Experiência" />
                            <asp:BoundField DataField="data_avaliacao" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy HH:mm}" />

                            <asp:TemplateField HeaderText="Nota">
                                <ItemTemplate>
                                    <span style="color: #c8a84b;">
                                        <%# GetEstrelasSimples(Eval("estrelas")) %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <%-- MELHORIA: Preview do comentário (CSS puro) --%>
                            <asp:TemplateField HeaderText="Comentário" ItemStyle-Width="260px">
                                <ItemTemplate>
                                    <div style="display:flex; align-items:center; gap:6px; width:100%;">
                                        <span style="overflow:hidden; text-overflow:ellipsis; white-space:nowrap; flex:1; min-width:0;">
                                            <%# Eval("comentario") %>
                                        </span>
                                        <button type="button" class="btn btn-xs btn-outline-info"
                                                style="flex-shrink:0;"
                                                onclick="togglePreview('preview_<%# Eval("id_avaliacao") %>')">
                                            <i class="fas fa-eye"></i>
                                        </button>
                                    </div>
                                    <div id="preview_<%# Eval("id_avaliacao") %>"
                                         class="mt-2 p-2 bg-light border rounded"
                                         style="display:none; white-space:normal;">
                                        <p class="mb-0 text-muted"><%# Eval("comentario") %></p>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Estado">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_estado" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btn_aprovar" runat="server"
                                        CommandName="Aprovar"
                                        CommandArgument='<%# Eval("id_avaliacao") %>'
                                        CssClass="btn btn-sm btn-success"
                                        Visible='<%# !Convert.ToBoolean(Eval("publicado")) %>'
                                        ToolTip="Aprovar para mostrar no site público">
                                        <i class="fa fa-check mr-1"></i>Aprovar
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btn_reprovar" runat="server"
                                        CommandName="Reprovar"
                                        CommandArgument='<%# Eval("id_avaliacao") %>'
                                        CssClass="btn btn-sm btn-outline-danger"
                                        Visible='<%# Convert.ToBoolean(Eval("publicado")) %>'
                                        Tooltip="Remover do site público">
                                        <i class="fa fa-times mr-1"></i>Remover
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
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        // Função global para toggle do preview do comentário
        function togglePreview(panelId) {
            var panel = document.getElementById(panelId);
            if (panel) {
                panel.style.display = panel.style.display === 'none' ? 'block' : 'none';
            }
        }
    </script>
</asp:Content>