<%@ Page Title="Experiências | Raízes Digitais" Language="C#" MasterPageFile="~/MasterBackoffice.Master" AutoEventWireup="true" CodeBehind="gerir_experiencias.aspx.cs" Inherits="RaizesDigitais.Backoffice.gerir_experiencias" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder3" runat="server">

    <div class="content-header">
        <div class="container-fluid">
            <div class="row mb-2">
                <div class="col-sm-6">
                    <h1 class="m-0">Gestão de Experiências</h1>
                </div>
                <div class="col-sm-6">
                    <ol class="breadcrumb float-sm-right">
                        <li class="breadcrumb-item">
                            <a href="dashboard.aspx">Início</a>
                        </li>
                        <li class="breadcrumb-item active">Gestão de Experiências</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>

    <div class="content">
        <div class="container-fluid">

            <!-- LISTA EXPERIÊNCIAS -->
            <div class="card">
                <div class="card-header">
                    <h3 class="card-title">Experiências</h3>
                    <div class="card-tools">
                        
                        <%-- Botão Nova Experiência (só para Admin) --%>
                        <asp:Button ID="btn_nova_experiencia" runat="server"
                            Text="+ Nova Experiência"
                            CssClass="btn btn-success btn-sm"
                            OnClick="btn_nova_experiencia_Click"
                            CausesValidation="false" />
                    </div>
                </div>
                <div class="card-body p-0">
                    <asp:GridView ID="gv_experiencias" runat="server"
                        CssClass="table table-hover m-0"
                        AutoGenerateColumns="false"
                        GridLines="None"
                        OnRowCommand="gv_experiencias_RowCommand"
                        AllowSorting="True"
                        OnSorting="gv_experiencias_Sorting">
                        <Columns>
                            <asp:BoundField DataField="nome" HeaderText="Nome" SortExpression="nome" />
                            <asp:BoundField DataField="tipo" HeaderText="Tipo" SortExpression="tipo" />
                            <asp:BoundField DataField="preco_por_pessoa" HeaderText="Preço/Pessoa" DataFormatString="{0:0.00} €" SortExpression="preco_por_pessoa" />
                            <asp:BoundField DataField="duracao_horas" HeaderText="Duração (h)" SortExpression="duracao_horas" />
                            <asp:BoundField DataField="capacidade_max" HeaderText="Cap. Máx." SortExpression="capacidade_max" />

                            <asp:TemplateField HeaderText="Estado" SortExpression="activo">
                                <ItemTemplate>
                                    <span class='badge <%# Convert.ToBoolean(Eval("activo")) ? "badge-success" : "badge-secondary" %>'>
                                        <%# Convert.ToBoolean(Eval("activo")) ? "Activo" : "Inactivo" %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Destaque">
                                <ItemTemplate>
                                    <span runat="server" visible='<%# Convert.ToBoolean(Eval("destaque")) %>' class="badge badge-destaque">
                                        <i class="fas fa-star mr-1"></i>Destaque
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btn_editar" runat="server"
                                        CommandName="EditarExp"
                                        CommandArgument='<%# Eval("id_experiencia") %>'
                                        CssClass="btn btn-sm btn-outline-primary mr-1"
                                        CausesValidation="false">
                                        <i class="fas fa-edit"></i> Editar
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
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

    <!-- ═══════════════════════════════════════════════════════
          NOVA / EDITAR EXPERIÊNCIA
         ═══════════════════════════════════════════════════════ -->
    <div class="modal fade" id="modalExperiencia" tabindex="-1" role="dialog"
         aria-labelledby="modalExperienciaLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-scrollable" role="document" style="max-width:1100px;">
            <div class="modal-content">

                <!-- CABEÇALHO -->
                <div class="modal-header bg-primary">
                    <h5 class="modal-title text-white" id="modalExperienciaLabel">
                        <i class="fas fa-leaf mr-2"></i>
                        <asp:Literal ID="lit_titulo_form" runat="server" Text="Nova Experiência" />
                    </h5>
                    <asp:Button ID="btn_cancelar_edicao" runat="server" Text="✕"
                        CssClass="close text-white" OnClick="btn_cancelar_edicao_Click"
                        data-dismiss="modal" CausesValidation="false"
                        style="opacity:0.8; font-size:1.4rem; background:none; border:none;" />
                </div>

                <div class="modal-body p-0">

                    <!-- TABS DE NAVEGAÇÃO -->
                    <ul class="nav nav-tabs nav-fill px-3 pt-3" id="tabsExperiencia" role="tablist">
                        <li class="nav-item">
                            <a class="nav-link active" id="tab-dados-link" data-toggle="tab"
                               href="#tab-dados" role="tab">
                                <i class="fas fa-info-circle mr-1"></i> Dados
                            </a>
                        </li>
                        <li class="nav-item" id="li_tab_sessoes">
                            <a class="nav-link" id="tab-sessoes-link" data-toggle="tab"
                               href="#tab-sessoes" role="tab">
                                <i class="fas fa-calendar-alt mr-1"></i> Sessões
                                <asp:Label ID="lbl_badge_sessoes" runat="server"
                                    CssClass="badge badge-warning ml-1" Text="" Visible="false" />
                            </a>
                        </li>
                    </ul>

                    <div class="tab-content px-3 pt-3 pb-2">

                        <!-- ── TAB 1: DADOS DA EXPERIÊNCIA ── -->
                        <div class="tab-pane fade show active" id="tab-dados" role="tabpanel">

                            <asp:HiddenField ID="hf_id_experiencia" runat="server" Value="0" />

                            <div class="row">
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <label>Nome <span class="text-danger">*</span></label>
                                        <asp:TextBox ID="tb_nome" runat="server" CssClass="form-control" />
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <label>Tipo</label>
                                        <asp:DropDownList ID="ddl_tipo" runat="server" CssClass="form-control select2">
                                            <asp:ListItem Value="Prova">Prova</asp:ListItem>
                                            <asp:ListItem Value="Visita">Visita</asp:ListItem>
                                            <asp:ListItem Value="Workshop">Workshop</asp:ListItem>
                                            <asp:ListItem Value="Jantar">Jantar</asp:ListItem>
                                            <asp:ListItem Value="Estadia">Estadia</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <label>Preço/Pessoa (€) <span class="text-danger">*</span></label>
                                        <asp:TextBox ID="tb_preco" runat="server" CssClass="form-control" placeholder="ex: 45.00" />
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <label>Duração (h)</label>
                                        <asp:TextBox ID="tb_duracao" runat="server" CssClass="form-control" placeholder="ex: 2.5" />
                                    </div>
                                </div>
                                <div class="col-md-1">
                                    <div class="form-group">
                                        <label>Cap. Máx.</label>
                                        <asp:TextBox ID="tb_capacidade" runat="server" CssClass="form-control" TextMode="Number" />
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-8">
                                    <div class="form-group">
                                        <label>Descrição</label>
                                        <asp:TextBox ID="tb_descricao" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <label>URL Imagem</label>
                                        <asp:TextBox ID="tb_imagem_url" runat="server" CssClass="form-control" />
                                    </div>
                                </div>
                            </div>

                            <!-- Checkboxes -->
                            <div class="row mt-1 mb-3">
                                <div class="col-md-12 d-flex" style="gap:1.5rem; align-items:center;">
                                    <div class="d-flex align-items-center" style="gap:0.4rem;">
                                        <asp:CheckBox ID="cb_activo" runat="server" Checked="true" />
                                        <label for="ContentPlaceHolder3_cb_activo" style="margin:0; font-weight:400; cursor:pointer;">Activo</label>
                                    </div>
                                    <div class="d-flex align-items-center" style="gap:0.4rem;">
                                        <asp:CheckBox ID="cb_destaque" runat="server" />
                                        <label for="ContentPlaceHolder3_cb_destaque" style="margin:0; font-weight:400; cursor:pointer;">Destacar na Homepage</label>
                                    </div>
                                </div>
                            </div>

                            <hr class="mt-0 mb-2" />

                            <!-- Auditoria -->
                            <p class="text-muted small mb-0">
                                <i class="fas fa-history mr-1"></i>
                                <strong>Última actualização:</strong>
                                <asp:Literal ID="lit_ultima_actualizacao_experiencia" runat="server" Text="Por actualizar" />
                            </p>

                        </div>
                        <%-- /#tab-dados --%>

                        <!-- ── TAB 2: SESSÕES / DISPONIBILIDADES ── -->
                        <div class="tab-pane fade" id="tab-sessoes" role="tabpanel">

                            <%-- Aviso quando é nova experiência (sem ID ainda) --%>
                            <asp:Panel ID="pnl_aviso_nova" runat="server" Visible="false">
                                <div class="alert alert-info">
                                    <i class="fas fa-info-circle mr-1"></i>
                                    Guarde primeiro os dados da experiência para poder adicionar sessões.
                                </div>
                            </asp:Panel>

                            <%-- Conteúdo de sessões (só quando está a editar) --%>
                            <asp:Panel ID="pnl_disponibilidades" runat="server" Visible="false">

                                <!-- Formulário nova sessão -->
                                <div class="card card-outline card-warning mb-3">
                                    <div class="card-header">
                                        <h3 class="card-title">
                                            <i class="fas fa-plus-circle mr-1"></i> Adicionar Sessão —
                                            <asp:Literal ID="lit_nome_exp_disp" runat="server" />
                                        </h3>
                                    </div>
                                    <div class="card-body">
                                        <div class="row">
                                            <div class="col-md-5">
                                                <div class="form-group mb-0">
                                                    <label>Data e Hora <span class="text-danger">*</span></label>
                                                    <asp:TextBox ID="tb_disp_data" runat="server" CssClass="form-control"
                                                        placeholder="dd/MM/yyyy HH:mm" />
                                                    <small class="text-muted">Formato: 25/06/2025 10:00</small>
                                                </div>
                                            </div>
                                            <div class="col-md-4">
                                                <div class="form-group mb-0">
                                                    <label>Vagas Totais <span class="text-danger">*</span></label>
                                                    <asp:TextBox ID="tb_disp_vagas" runat="server" CssClass="form-control"
                                                        TextMode="Number" placeholder="ex: 20" />
                                                    <small class="text-muted">&nbsp;</small>
                                                </div>
                                            </div>
                                            <div class="col-md-3">
                                                <label>&nbsp;</label>
                                                <asp:Button ID="btn_adicionar_sessao" runat="server"
                                                    Text="+ Adicionar Sessão"
                                                    CssClass="btn btn-warning btn-block d-block"
                                                    OnClick="btn_adicionar_sessao_Click"
                                                    CausesValidation="false" />
                                                <small class="text-muted">&nbsp;</small>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <!-- Lista sessões existentes -->
                                <asp:GridView ID="gv_disponibilidades" runat="server"
                                    CssClass="table table-hover m-0"
                                    AutoGenerateColumns="false"
                                    GridLines="None"
                                    OnRowCommand="gv_disponibilidades_RowCommand">
                                    <Columns>
                                        <asp:BoundField DataField="data_hora" HeaderText="Data e Hora"
                                            DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                                        <asp:BoundField DataField="vagas_total" HeaderText="Vagas Totais" />
                                        <asp:BoundField DataField="vagas_disponiveis" HeaderText="Vagas Disponíveis" />
                                        <asp:TemplateField HeaderText="">
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server"
                                                    CommandName="RemoverSessao"
                                                    CommandArgument='<%# Eval("id_disponibilidade") %>'
                                                    CssClass="btn btn-sm btn-outline-danger"
                                                    CausesValidation="false"
                                                    OnClientClick="return confirm('Remover esta sessão?');">
                                                    <i class="fas fa-trash"></i> Remover
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <div class="alert alert-info m-3">
                                            <i class="fas fa-calendar-times mr-1"></i>
                                            Ainda não há sessões para esta experiência. Adicione a primeira acima.
                                        </div>
                                    </EmptyDataTemplate>
                                </asp:GridView>

                            </asp:Panel>
                            <%-- /#pnl_disponibilidades --%>

                        </div>
                        <%-- /#tab-sessoes --%>

                    </div>
                    <%-- /.tab-content --%>

                </div>
                <%-- /.modal-body --%>

                <!-- RODAPÉ -->
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">
                        Cancelar
                    </button>
                    <asp:Button ID="btn_guardar" runat="server" Text="Guardar Experiência"
                        CssClass="btn btn-primary" OnClick="btn_guardar_Click"
                        CausesValidation="false" />
                </div>

            </div>
        </div>
    </div>
    <%-- /#modalExperiencia --%>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        // Quando o modal fecha, repõe para a tab Dados
        $('#modalExperiencia').on('hidden.bs.modal', function () {
            $('#tab-dados-link').tab('show');
        });
    </script>
</asp:Content>
