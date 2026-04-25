<%@ Page Title="Dashboard | Raízes Digitais" Language="C#" MasterPageFile="~/MasterBackoffice.Master" AutoEventWireup="true" CodeBehind="dashboard.aspx.cs" Inherits="RaizesDigitais.Backoffice.dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <style>
    .content .container-fluid .row {
        margin-bottom: 1rem !important;
    }
    .content .container-fluid .row:last-child {
        margin-bottom: 0 !important;
    }
    .content .container-fluid .card {
        margin-bottom: 0 !important;
    }
    .content .container-fluid .small-box {
        margin-bottom: 1rem !important;
    }
    .chegadas-table th,
    .chegadas-table td {
        width: auto !important;
    }
    .chart-container {
        min-height: 280px;
    }
    @media (max-width: 768px) {
        .chart-container {
            min-height: 240px !important;
        }
        .small-box .inner h3 {
            font-size: 1.5rem !important;
        }
        .small-box .inner p {
            font-size: 0.8rem !important;
        }
    }
    /* Calendário */
    #calendario_reservas {
        padding: 1rem;
        min-height: 400px;
    }
    #calendario_reservas .fc-event {
        cursor: pointer;
    }
    /* ═══════════════════════════════════════════════════════════════
       MELHORIAS CSS PURO
       ═══════════════════════════════════════════════════════════════ */
    /* Progress bar customizada */
    .progress-custom {
        height: 8px;
        background-color: #f8f9fa;
        border-radius: 4px;
    }
    .progress-custom .progress-bar {
        border-radius: 4px;
    }
    /* KPI clicável */
    .small-box-clickable {
        text-decoration: none;
        color: inherit;
        display: block;
    }
    .small-box-clickable:hover {
        opacity: 0.9;
    }
    .small-box-clickable:hover .small-box-footer {
        background-color: rgba(0,0,0,0.15);
    }
</style>
</asp:Content>


<asp:Content ID="Content_Head" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <!-- CSS do FullCalendar -->
    <link rel="stylesheet" href="<%= ResolveUrl("~/AdminLTE-3.2.0/plugins/fullcalendar/main.css") %>" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder3" runat="server">

    <div class="content-header">
        <div class="container-fluid">
            <h1 class="m-0">Painel de Controlo</h1>
        </div>
    </div>

    <div class="content">
        <div class="container-fluid">

            <!-- ═══════════════════════════════════════════════════════════════
                 CALLOUT DE ALERTA: RESERVAS PENDENTES (SÓ ADMIN)
                 ═══════════════════════════════════════════════════════════════ -->
            <asp:Panel ID="pnl_alerta_pendentes" runat="server" Visible="false">
                <div class="alert alert-warning alert-dismissible">
                    <button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>
                    <h5><i class="icon fas fa-exclamation-triangle"></i> Atenção!</h5>
                    Existem <strong><asp:Literal ID="lit_pendentes_alerta" runat="server" /></strong> reservas pendentes de confirmação.
                    <a href="gerir_reservas.aspx?estado=Pendente" class="alert-link">Ver reservas pendentes</a>
                </div>
            </asp:Panel>

            <!-- ═══════════════════════════════════════════════════════════════
                 KPIs CLICÁVEIS (SÓ ADMIN)
                 ═══════════════════════════════════════════════════════════════ -->
            <div style='<%= IsGestor ? "display:none;" : "" %>'>
            <div class="row">

            <!-- KPI 1: Chegadas Hoje -->
            <div class="col-6 col-lg-2">
                <a href="gerir_reservas.aspx?data=<%= DateTime.Today.ToString("yyyy-MM-dd") %>" class="small-box-clickable">
                    <div class="small-box <%= ChegadasCss %>">
                        <div class="inner">
                            <h3><asp:Literal ID="lit_reservas_hoje" runat="server" Text="0" /></h3>
                            <p>Chegadas Hoje
                                <small class="d-block" style="font-size:0.75rem; opacity:0.85;">reservas confirmadas</small>
                            </p>
                        </div>
                        <div class="icon"><i class="fas fa-calendar-day"></i></div>
                        <div class="small-box-footer">Ver Agenda <i class="fas fa-arrow-circle-right"></i></div>
                    </div>
                </a>
            </div>

            <!-- KPI 2: Por Confirmar -->
            <div class="col-6 col-lg-2">
                <a href="gerir_reservas.aspx?estado=Pendente" class="small-box-clickable">
                    <div class="small-box <%= PendentsCss %>">
                        <div class="inner">
                            <h3><asp:Literal ID="lit_reservas_pendentes" runat="server" Text="0" /></h3>
                            <p>Por Confirmar
                                <small class="d-block" style="font-size:0.75rem; opacity:0.85;">aguardam confirmação</small>
                            </p>
                        </div>
                        <div class="icon"><i class="fas fa-clock"></i></div>
                        <div class="small-box-footer">Ver Reservas <i class="fas fa-arrow-circle-right"></i></div>
                    </div>
                </a>
            </div>

            <!-- KPI 3: Receita do Mês -->
            <div class="col-6 col-lg-2">
                <a href="gerir_reservas.aspx" class="small-box-clickable">
                    <div class="small-box <%= ReceitaCss %>">
                        <div class="inner">
                            <h3><asp:Literal ID="lit_receita_mes" runat="server" Text="0" /> €</h3>
                            <p>Receita do Mês
                                <small class="d-block" style="font-size:0.75rem; opacity:0.85;">
                                    Ant: <asp:Literal ID="lit_receita_anterior" runat="server" Text="0" /> €
                                </small>
                            </p>
                        </div>
                        <div class="icon"><i class="fas fa-euro-sign"></i></div>
                        <div class="small-box-footer">Ver Reservas <i class="fas fa-arrow-circle-right"></i></div>
                    </div>
                </a>
            </div>

            <!-- KPI 4: Clientes -->
            <div class="col-6 col-lg-2">
                <a href="gerir_clientes.aspx" class="small-box-clickable">
                    <div class="small-box <%= ClientesCss %>">
                        <div class="inner">
                            <h3><asp:Literal ID="lit_total_clientes" runat="server" Text="0" /></h3>
                            <p>Clientes
                                <small class="d-block" style="font-size:0.75rem; opacity:0.85;">
                                    +<asp:Literal ID="lit_clientes_novos" runat="server" Text="0" /> este mês
                                </small>
                            </p>
                        </div>
                        <div class="icon"><i class="fas fa-users"></i></div>
                        <div class="small-box-footer">Ver CRM <i class="fas fa-arrow-circle-right"></i></div>
                    </div>
                </a>
            </div>

            <!-- KPI 5: Cupões Activos -->
            <div class="col-6 col-lg-2">
                <a href="gerir_cupoes.aspx" class="small-box-clickable">
                    <div class="small-box <%= CupoesActivosCss %>">
                        <div class="inner">
                            <h3><asp:Literal ID="lit_total_cupoes_activos" runat="server" Text="0" /></h3>
                            <p>Cupões Activos
                                <small class="d-block" style="font-size:0.75rem; opacity:0.85;">disponíveis para uso</small>
                            </p>
                        </div>
                        <div class="icon"><i class="fas fa-ticket-alt"></i></div>
                        <div class="small-box-footer">Ver Cupões <i class="fas fa-arrow-circle-right"></i></div>
                    </div>
                </a>
            </div>

            <!-- KPI 6: Clientes B2B -->
            <div class="col-6 col-lg-2">
                <a href="gerir_clientes.aspx?segmento=B2B" class="small-box-clickable">
                    <div class="small-box <%= ClientesB2BCss %>">
                        <div class="inner">
                            <h3><asp:Literal ID="lit_total_clientes_b2b" runat="server" Text="0" /></h3>
                            <p>Clientes B2B
                                <small class="d-block" style="font-size:0.75rem; opacity:0.85;">agentes de viagens</small>
                            </p>
                        </div>
                        <div class="icon"><i class="fas fa-users-cog"></i></div>
                        <div class="small-box-footer">Ver B2B <i class="fas fa-arrow-circle-right"></i></div>
                    </div>
                </a>
            </div>

            <!-- KPI 7: Ofertas B2B -->
            <div class="col-6 col-lg-2">
                <a href="gerir_ofertas_b2b.aspx" class="small-box-clickable">
                    <div class="small-box <%= OfertasB2BAtivasCss %>">
                        <div class="inner">
                            <h3><asp:Literal ID="lit_total_ofertas_b2b_activas" runat="server" Text="0" /></h3>
                            <p>Ofertas B2B
                                <small class="d-block" style="font-size:0.75rem; opacity:0.85;">experiências com desconto</small>
                            </p>
                        </div>
                        <div class="icon"><i class="fas fa-percent"></i></div>
                        <div class="small-box-footer">Ver Ofertas <i class="fas fa-arrow-circle-right"></i></div>
                    </div>
                </a>
            </div>

            <!-- KPI 8: Alertas Stock -->
            <div class="col-6 col-lg-2">
                <a href="gerir_vinhos.aspx?filtro=stock" class="small-box-clickable">
                    <div class="small-box <%= AlertasStockCss %>">
                        <div class="inner">
                            <h3><asp:Literal ID="lit_alertas_stock" runat="server" Text="0" /></h3>
                            <p>Alertas Stock
                                <small class="d-block" style="font-size:0.75rem; opacity:0.85;">vinhos abaixo do mínimo</small>
                            </p>
                        </div>
                        <div class="icon"><i class="fas fa-exclamation-triangle"></i></div>
                        <div class="small-box-footer">Ver Vinhos <i class="fas fa-arrow-circle-right"></i></div>
                    </div>
                </a>
            </div>

        </div>

            <!-- ═══════════════════════════════════════════════════════════════
                 PROGRESS BARS: OCUPAÇÃO DE EXPERIÊNCIAS (SÓ ADMIN)
                 ═══════════════════════════════════════════════════════════════ -->
            <asp:Panel ID="pnl_ocupacao" runat="server" Visible="false">
            <div class="row">
                <div class="col-12">
                    <div class="card card-outline card-info">
                        <div class="card-header">
                            <h3 class="card-title"><i class="fas fa-chart-line mr-1"></i> Ocupação de Experiências — Próximos 7 dias</h3>
                        </div>
                        <div class="card-body">
                            <asp:Repeater ID="rpt_ocupacao" runat="server">
                                <ItemTemplate>
                                    <div class="mb-3">
                                        <div class="d-flex justify-content-between mb-1">
                                            <span><%# Eval("Experiencia") %></span>
                                            <span class="text-<%# GetOcupacaoClass(Eval("Ocupacao")) %>"><%# Eval("Ocupacao") %>%</span>
                                        </div>
                                        <div class="progress progress-custom">
                                            <div class="progress-bar bg-<%# GetOcupacaoClass(Eval("Ocupacao")) %>"
                                                 style="width: <%# Eval("Ocupacao") %>%"></div>
                                        </div>
                                        <small class="text-muted"><%# Eval("VagasUsadas") %> / <%# Eval("VagasTotal") %> vagas</small>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>
            </div>
            </asp:Panel>

            </div><!-- Fim dos KPIs -->

            <!-- ═══════════════════════════════════════════════════════════════
                 TABELA DE CHEGADAS (ADMIN E GESTOR)
                 ═══════════════════════════════════════════════════════════════ -->
            <div class="row">
                <div class="col-12">
                    <div class="card card-outline card-success">
                        <div class="card-header">
                            <h3 class="card-title">
                                <i class="fas fa-clock mr-1"></i> Próximas Chegadas — Hoje
                                <span class="badge badge-success ml-2"><asp:Literal ID="lit_total_chegadas" runat="server" Text="0" /></span>
                            </h3>
                        </div>
                        <div class="card-body p-0">
                            <asp:GridView ID="gv_proximas_chegadas" runat="server"
                                CssClass="table table-hover table-striped m-0 chegadas-table"
                                AutoGenerateColumns="false"
                                GridLines="None"
                                ShowHeaderWhenEmpty="true">
                                <Columns>
                                    <asp:BoundField DataField="hora" HeaderText="Hora" ItemStyle-Font-Bold="true" />
                                    <asp:BoundField DataField="cliente" HeaderText="Cliente" />
                                    <asp:BoundField DataField="pax" HeaderText="Pessoas" ItemStyle-CssClass="text-center" />
                                </Columns>
                                <EmptyDataTemplate>
                                    <div class="p-3 text-center text-muted">Não há chegadas previstas para hoje.</div>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                        <div class="card-footer text-center">
                            <a href="gerir_reservas.aspx">Ver Agenda Completa</a>
                        </div>
                    </div>
                </div>
            </div>

            <!-- ═══════════════════════════════════════════════════════════════
                 CALENDÁRIO DE RESERVAS (ADMIN E GESTOR)
                 ═══════════════════════════════════════════════════════════════ -->
            <div class="row">
                <div class="col-12">
                    <div class="card card-primary">
                        <div class="card-header">
                            <h3 class="card-title">
                                <i class="fas fa-calendar-alt mr-2"></i>Reservas confirmadas — próximos 30 dias
                            </h3>
                        </div>
                        <div class="card-body p-0">
                            <div id="calendario_reservas"></div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- ═══════════════════════════════════════════════════════════════
                 GRÁFICOS (SÓ ADMIN)
                 ═══════════════════════════════════════════════════════════════ -->
            <div style='<%= IsGestor ? "display:none;" : "" %>'>

            <!-- LINHA 2: Receita e Reservas -->
            <div class="row">
                <div class="col-12 col-md-6">
                    <div class="card card-outline card-success">
                        <div class="card-header">
                            <h3 class="card-title"><i class="fas fa-euro-sign mr-1"></i> Receita por Mês</h3>
                        </div>
                        <div class="card-body chart-container" style="height:300px;">
                            <canvas id="chartReceita"></canvas>
                        </div>
                    </div>
                </div>
                <div class="col-12 col-md-6">
                    <div class="card card-outline card-warning">
                        <div class="card-header">
                            <h3 class="card-title"><i class="fas fa-calendar-check mr-1"></i> Reservas por Mês</h3>
                        </div>
                        <div class="card-body chart-container" style="height:300px;">
                            <canvas id="chartReservas"></canvas>
                        </div>
                    </div>
                </div>
            </div>

            <!-- LINHA 3: Doughnut + Top Experiências -->
            <div class="row">
                <div class="col-12 col-md-6">
                    <div class="card card-outline card-primary h-100">
                        <div class="card-header">
                            <h3 class="card-title"><i class="fas fa-chart-pie mr-1"></i> Estado das Reservas</h3>
                        </div>
                        <div class="card-body d-flex align-items-center justify-content-center" style="height:320px;">
                            <canvas id="chartEstados"></canvas>
                        </div>
                    </div>
                </div>
                <div class="col-12 col-md-6">
                    <div class="card card-outline card-primary h-100">
                        <div class="card-header">
                            <h3 class="card-title"><i class="fas fa-chart-bar mr-1"></i> Top Experiências</h3>
                        </div>
                        <div class="card-body" style="height:320px;">
                            <canvas id="chartExperiencias"></canvas>
                        </div>
                    </div>
                </div>
            </div>

            </div><!-- Fim dos Gráficos -->

        </div>
    </div>

    <script>
        document.addEventListener('DOMContentLoaded', function () {
            var isGestor = <%= IsGestor ? "true" : "false" %>;

            // ═══════════════════════════════════════════════════════════════
            // GRÁFICOS (SÓ ADMIN)
            // ═══════════════════════════════════════════════════════════════
            if (!isGestor) {
                // GRÁFICO 1: Receita
                new Chart(document.getElementById('chartReceita').getContext('2d'), {
                    type: 'line',
                    data: {
                        labels: [<%= LabelsGrafico %>],
                    datasets: [{
                        label: 'Receita (€)',
                        data: [<%= ReceitaGrafico %>],
                        borderColor: '#28a745',
                        backgroundColor: 'rgba(40,167,69,0.15)',
                        borderWidth: 3,
                        pointBackgroundColor: '#28a745',
                        pointRadius: 6,
                        pointHoverRadius: 8,
                        tension: 0.4,
                        fill: true
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: { legend: { display: false } },
                    scales: { y: { beginAtZero: true, ticks: { callback: function (value) { return value + ' €'; } } } }
                }
            });

                // GRÁFICO 2: Reservas
                new Chart(document.getElementById('chartReservas').getContext('2d'), {
                    type: 'bar',
                    data: {
                        labels: [<%= LabelsGrafico %>],
                    datasets: [{
                        label: 'Reservas',
                        data: [<%= DadosGrafico %>],
                        backgroundColor: '#ffc107',
                        borderRadius: 6,
                        borderWidth: 0
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: { legend: { display: false } },
                    scales: { y: { beginAtZero: true, ticks: { stepSize: 1 } } }
                }
            });

                // GRÁFICO 3: Donut - Estados
                new Chart(document.getElementById('chartEstados').getContext('2d'), {
                    type: 'doughnut',
                    data: {
                        labels: [<%= LabelsEstados %>],
                    datasets: [{
                        data: [<%= DadosEstados %>],
                        backgroundColor: ['#dc3545', '#6c757d', '#28a745', '#ffc107'],
                        borderWidth: 2
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: { legend: { position: 'bottom' } }
                }
            });

                // GRÁFICO 4: Top Experiências
                new Chart(document.getElementById('chartExperiencias').getContext('2d'), {
                    type: 'bar',
                    data: {
                        labels: [<%= LabelsExperiencias %>],
                    datasets: [{
                        label: 'Reservas',
                        data: [<%= DadosExperiencias %>],
                        backgroundColor: ['#28a745', '#17a2b8', '#fd7e14', '#6f42c1', '#ffc107'],
                        borderRadius: 4
                    }]
                },
                options: {
                    indexAxis: 'y',
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: { legend: { display: false } },
                    scales: { x: { beginAtZero: true, ticks: { stepSize: 1 } } }
                }
            });
            }

            // ═══════════════════════════════════════════════════════════════
            // CALENDÁRIO DE RESERVAS (ADMIN E GESTOR)
            // ═══════════════════════════════════════════════════════════════
            var calendarEl = document.getElementById('calendario_reservas');
            var calendar = new FullCalendar.Calendar(calendarEl, {
                headerToolbar: {
                    left: 'prev,next today',
                    center: 'title',
                    right: 'dayGridMonth,timeGridWeek'
                },
                locale: 'pt',
                initialView: 'dayGridMonth',
                height: 'auto',
                editable: false,
                eventColor: '#4A7C2F',
                events: <%= EventosCalendario %>,
                eventDidMount: function (info) {
                    info.el.setAttribute('title', info.event.title);
                }
            });
            calendar.render();

            // DataTables na GridView de próximas chegadas
            $('#<%= gv_proximas_chegadas.ClientID %>').DataTable({
                "language": {
                    "lengthMenu": "Mostrar _MENU_ registos",
                    "zeroRecords": "Nenhuma chegada encontrada",
                    "info": "Página _PAGE_ de _PAGES_",
                    "infoEmpty": "Sem chegadas",
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
                "order": [],
                "autoWidth": false
            });
        });
    </script>

</asp:Content>


<asp:Content ID="Content_Scripts" ContentPlaceHolderID="scripts" runat="server">
    <!-- Script do FullCalendar -->
    <script src="<%= ResolveUrl("~/AdminLTE-3.2.0/plugins/fullcalendar/main.js") %>"></script>
    <script src="<%= ResolveUrl("~/AdminLTE-3.2.0/plugins/fullcalendar/locales/pt.js") %>"></script>
</asp:Content>