<%@ Page Title="Cupões | Raízes Digitais" Language="C#" MasterPageFile="~/MasterBackoffice.Master" AutoEventWireup="true" CodeBehind="gerir_cupoes.aspx.cs" Inherits="RaizesDigitais.Backoffice.gerir_cupoes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder3" runat="server">

    <div class="content-header">
        <div class="container-fluid">
            <div class="row mb-2">
                <div class="col-sm-6">
                    <h1 class="m-0">Gestão de Cupões</h1>
                </div>
                <div class="col-sm-6">
                    <ol class="breadcrumb float-sm-right">
                        <li class="breadcrumb-item"><a href="dashboard.aspx">Início</a></li>
                        <li class="breadcrumb-item active">Gestão de Cupões</li>
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
                        <asp:LinkButton ID="badge_validos" runat="server" CssClass="btn btn-outline-success btn-sm" OnClick="badge_validos_Click">
                            Válidos <span class="badge badge-light"><asp:Literal ID="lit_count_validos" runat="server" Text="0" /></span>
                        </asp:LinkButton>
                        <asp:LinkButton ID="badge_usados" runat="server" CssClass="btn btn-outline-secondary btn-sm" OnClick="badge_usados_Click">
                            Usados <span class="badge badge-light"><asp:Literal ID="lit_count_usados" runat="server" Text="0" /></span>
                        </asp:LinkButton>
                        <asp:LinkButton ID="badge_expirados" runat="server" CssClass="btn btn-outline-danger btn-sm" OnClick="badge_expirados_Click">
                            Expirados <span class="badge badge-light"><asp:Literal ID="lit_count_expirados" runat="server" Text="0" /></span>
                        </asp:LinkButton>
                    </div>
                </div>
            </div>

            <!-- ═══════════════════════════════════════════════════════════════
                 MELHORIA: CALLOUT PARA CUPÕES A EXPIRAR EM BREVE
                 ═══════════════════════════════════════════════════════════════ -->
            <asp:Panel ID="pnl_alerta_expirar" runat="server" Visible="false">
                <div class="alert alert-warning alert-dismissible">
                    <button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>
                    <h5><i class="icon fas fa-exclamation-triangle"></i> Cupões a Expirar</h5>
                    Existem <strong><asp:Literal ID="lit_expirar_breve" runat="server" /></strong> cupões que expiram nos próximos 7 dias.
                </div>
            </asp:Panel>

            <!-- FILTROS -->
            <div class="card mb-4 card-outline card-primary">
                <div class="card-header">
                    <h3 class="card-title">Filtrar Cupões</h3>
                    <div class="card-tools">
                        <asp:Button ID="btn_novo_cupao" runat="server" Text="+ Novo Cupão"
                            CssClass="btn btn-success btn-sm" OnClick="btn_novo_cupao_Click"
                            CausesValidation="false" />
                    </div>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-3">
                            <label>Estado</label>
                            <asp:DropDownList ID="ddl_filtro_estado" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">— Todos —</asp:ListItem>
                                <asp:ListItem Value="validos">Válidos</asp:ListItem>
                                <asp:ListItem Value="usados">Usados</asp:ListItem>
                                <asp:ListItem Value="expirados">Expirados</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-3">
                            <label>Tipo de Desconto</label>
                            <asp:DropDownList ID="ddl_filtro_tipo" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">— Todos —</asp:ListItem>
                                <asp:ListItem Value="percentagem">Percentagem</asp:ListItem>
                                <asp:ListItem Value="valor_fixo">Valor Fixo</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-3">
                            <label>Código</label>
                            <asp:TextBox ID="tb_filtro_codigo" runat="server" CssClass="form-control"
                                placeholder="Pesquisar código..."></asp:TextBox>
                        </div>
                        <div class="col-md-3 d-flex align-items-end">
                            <asp:Button ID="btn_filtrar" runat="server" Text="Filtrar"
                                CssClass="btn btn-primary mr-2" OnClick="btn_filtrar_Click" />
                            <asp:Button ID="btn_limpar" runat="server" Text="Limpar"
                                CssClass="btn btn-secondary" OnClick="btn_limpar_Click"
                                CausesValidation="false" />
                        </div>
                    </div>
                </div>
            </div>

            <!-- GRID DE CUPÕES -->
            <div class="card">
                <div class="card-header">
                    <h3 class="card-title">
                        Lista de Cupões
                        <!-- MELHORIA: Badge com total -->
                        <span class="badge badge-primary ml-2"><asp:Literal ID="lit_total_lista" runat="server" Text="0" /></span>
                    </h3>
                </div>
                <div class="card-body p-0">
                    <div style="overflow-x:auto;">
                        <asp:GridView ID="gv_cupoes" runat="server"
                            AutoGenerateColumns="False"
                            CssClass="table table-hover m-0"
                            GridLines="None"
                            DataKeyNames="id_cupao"
                            OnRowCommand="gv_cupoes_RowCommand"
                            OnRowDataBound="gv_cupoes_RowDataBound">
                            <Columns>
                                <asp:BoundField DataField="codigo" HeaderText="Código" />

                                <asp:TemplateField HeaderText="Desconto">
                                    <ItemTemplate>
                                        <span class='badge badge-<%# (string)Eval("tipo_desconto") == "percentagem" ? "primary" : "info" %>'>
                                            <%# FormatarDesconto(Eval("tipo_desconto").ToString(), Eval("valor_desconto").ToString()) %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Destinatário">
                                    <ItemTemplate>
                                        <%# FormatarDestinatario(Eval("nome_cliente"), Eval("segmento_crm")) %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Validade">
                                    <ItemTemplate>
                                        <asp:Literal ID="lit_validade" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Estado">
                                    <ItemTemplate>
                                        <%# GetEstadoBadge(
                                            Convert.ToBoolean(Eval("utilizado")),
                                            Convert.ToDateTime(Eval("validade"))) %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="data_utilizacao" HeaderText="Data Uso"
                                    DataFormatString="{0:dd/MM/yyyy}" NullDisplayText="—" />

                                <asp:TemplateField HeaderText="">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btn_apagar" runat="server"
                                            CommandName="ApagarCupao"
                                            CommandArgument='<%# Eval("id_cupao") %>'
                                            CssClass="btn btn-sm btn-outline-danger"
                                            OnClientClick="return confirm('Apagar este cupão?');"
                                            Visible='<%# !Convert.ToBoolean(Eval("utilizado")) %>'>
                                            <i class="fas fa-trash"></i>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="alert alert-info m-3">Nenhum cupão encontrado com os filtros seleccionados.</div>
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

    <!-- MODAL: CRIAR CUPÃO -->
    <div class="modal fade" id="modalCriarCupao" tabindex="-1" role="dialog" aria-labelledby="modalCriarCupaoLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header bg-success text-white">
                    <h5 class="modal-title" id="modalCriarCupaoLabel">
                        <i class="fas fa-ticket-alt mr-2"></i>Criar Novo Cupão
                    </h5>
                    <button type="button" class="close text-white" data-dismiss="modal" aria-label="Fechar">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">

                    <div class="form-group">
                        <label>Código <span class="text-danger">*</span></label>
                        <div class="input-group">
                            <asp:TextBox ID="tb_codigo" runat="server" CssClass="form-control text-uppercase"
                                placeholder="Ex: VERAO10" MaxLength="20"></asp:TextBox>
                            <div class="input-group-append">
                                <asp:Button ID="btn_gerar_codigo" runat="server" Text="Gerar"
                                    CssClass="btn btn-outline-secondary" OnClick="btn_gerar_codigo_Click"
                                    CausesValidation="false" />
                            </div>
                        </div>
                        <small class="text-muted">Máx. 20 caracteres. Deve ser único.</small>
                    </div>

                    <div class="form-group">
                        <label>Tipo de Desconto <span class="text-danger">*</span></label>
                        <asp:DropDownList ID="ddl_tipo_desconto" runat="server" CssClass="form-control">
                            <asp:ListItem Value="percentagem">Percentagem (%)</asp:ListItem>
                            <asp:ListItem Value="valor_fixo">Valor Fixo (€)</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="form-group">
                        <label>Valor do Desconto <span class="text-danger">*</span></label>
                        <asp:TextBox ID="tb_desconto" runat="server" CssClass="form-control"
                            placeholder="Ex: 10 (para 10% ou 10€)" TextMode="Number"></asp:TextBox>
                    </div>

                    <!-- MELHORIA: Callout de preview do desconto -->
                    <asp:Panel ID="pnl_preview_desconto" runat="server" Visible="false" CssClass="callout callout-info mb-3">
                        <h5><i class="fas fa-calculator mr-1"></i> Preview</h5>
                        <p class="mb-0" id="preview_text">Reserva de 50€ → <strong id="preview_final">45€</strong> com este desconto.</p>
                    </asp:Panel>

                    <div class="form-group">
                        <label>Validade <span class="text-danger">*</span></label>
                        <asp:TextBox ID="tb_validade" runat="server" CssClass="form-control"
                            TextMode="Date"></asp:TextBox>
                    </div>

                    <div class="form-group">
                        <label>Cliente (opcional)</label>
                        <asp:DropDownList ID="ddl_cupao_cliente" runat="server" CssClass="form-control select2">
                            <asp:ListItem Value="">— Genérico (todos os clientes) —</asp:ListItem>
                        </asp:DropDownList>
                        <small class="text-muted">Se não selecionar, o cupão fica disponível para todos.</small>
                    </div>

                    <div class="form-group">
                        <label>Segmento CRM (opcional)</label>
                        <asp:DropDownList ID="ddl_cupao_segmento" runat="server" CssClass="form-control select2">
                            <asp:ListItem Value="">— Todos os segmentos —</asp:ListItem>
                            <asp:ListItem Value="VIP">VIP</asp:ListItem>
                            <asp:ListItem Value="Regular">Regular</asp:ListItem>
                            <asp:ListItem Value="B2B">B2B</asp:ListItem>
                            <asp:ListItem Value="Inactivo">Inactivo</asp:ListItem>
                        </asp:DropDownList>
                        <small class="text-muted">Se não selecionar, o cupão aplica-se a todos os segmentos.</small>
                    </div>

                    <asp:Label ID="lbl_erro_modal" runat="server" CssClass="text-danger d-block" Visible="false" />

                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancelar</button>
                    <asp:Button ID="btn_criar_cupao" runat="server" Text="Criar Cupão"
                        CssClass="btn btn-success" OnClick="btn_criar_cupao_Click" />
                </div>
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        // Forçar uppercase no código (vanilla JS - sem jQuery)
        document.getElementById('<%= tb_codigo.ClientID %>').addEventListener('input', function () {
            this.value = this.value.toUpperCase();
        });
    </script>
</asp:Content>