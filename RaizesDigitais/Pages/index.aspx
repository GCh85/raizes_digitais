<%@ Page Title="Quinta da Azenha | Bucelas" Language="C#" MasterPageFile="~/MasterSite.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="RaizesDigitais.Pages.index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- ═══════════════════════════════════════════════════════
         HERO SECTION
         ═══════════════════════════════════════════════════════ -->
    <div class="hero-section">
        <img src="~/Images/QtaAzenha.png" class="hero-img" alt="Vinhas da Quinta da Azenha em Bucelas" runat="server" />
        <div class="hero-texto">
            <span class="label-secao" style="color: var(--cor-destaque);">Bucelas · DOC Arinto</span>
            <h1 class="display-4 fw-bold mb-2">Raízes que Falam</h1>
            <div class="linha-verde"></div>
            <p class="lead mb-4" style="color: rgba(255,255,255,0.85);">
                Uma quinta onde a terra de Bucelas se transforma em vinho com alma
            </p>
            <a href="~/Pages/experiencias.aspx" runat="server" class="btn-quinta me-2">Ver Experiências</a>
            <a href="~/Pages/vinhos.aspx" runat="server" class="btn-quinta-outline" style="color:white; border-color:white;">Descobrir Vinhos</a>
        </div>
    </div>


    <!-- ═══════════════════════════════════════════════════════
         DESCOBRIR BUCELAS
         ═══════════════════════════════════════════════════════ -->
    <section class="py-5 bg-white">
        <div class="container">
            <span class="label-secao text-center d-block">O Nosso Terroir</span>
            <h2 class="text-center mb-0">Descobrir Bucelas</h2>
            <div class="linha-verde"></div>

            <div class="row g-4 text-center">
                <div class="col-md-4">
                    <i class="bi bi-geo-alt fs-1 mb-3 d-block" style="color: var(--cor-primaria);"></i>
                    <h4>O Solo</h4>
                    <p class="text-muted">Os solos argilo-calcários de Bucelas conferem aos vinhos uma mineralidade única - aquela frescura que persiste depois de engolir.</p>
                </div>
                <div class="col-md-4">
                    <i class="bi bi-sun fs-1 mb-3 d-block" style="color: var(--cor-primaria);"></i>
                    <h4>O Clima</h4>
                    <p class="text-muted">Invernos frios e verões ventosos. A influência do Atlântico, a 30km, traz frescura às noites — o segredo para a acidez viva dos nossos brancos.</p>
                </div>
                <div class="col-md-4">
                    <i class="bi bi-flower1 fs-1 mb-3 d-block" style="color: var(--cor-primaria);"></i>
                    <h4>O Arinto</h4>
                    <p class="text-muted">A casta rainha de Bucelas. O Arinto aqui produz vinhos que envelhecem décadas sem perder frescura — uma raridade num mundo de vinhos para beber jovens.</p>
                </div>
            </div>
        </div>
    </section>


    <!-- ═══════════════════════════════════════════════════════
         SOBRE A QUINTA
         ═══════════════════════════════════════════════════════ -->
    <section class="py-5">
        <div class="container">
            <div class="row align-items-center g-5">
                <div class="col-lg-6">
                    <span class="label-secao">A Nossa História</span>
                    <h2 class="mb-0">Quinta da Azenha</h2>
                    <div class="linha-verde-esq"></div>
                    <p class="text-muted mb-3">
                        A Quinta da Azenha está localizada no coração de Bucelas e estende-se por uma área de 45 hectares, caracterizados por solos argilo-calcários únicos. A propriedade possui 37 hectares de vinhas, incluindo vinhas velhas com mais de 50 anos.
                    </p>
                    <p class="text-muted mb-4">
                        O nome "Azenha" - a antiga roda de pedra que moía o grão — é a memória viva de gerações que trabalharam esta terra. Hoje, esse mesmo engenho move a nossa paixão pelo vinho.
                    </p>
                    <div class="row text-center border-top pt-4">
                        <div class="col-4">
                            <h3 style="color: var(--cor-primaria);">45ha</h3>
                            <small class="text-muted text-uppercase" style="letter-spacing: 0.1em;">Área Total</small>
                        </div>
                        <div class="col-4">
                            <h3 style="color: var(--cor-primaria);">37ha</h3>
                            <small class="text-muted text-uppercase" style="letter-spacing: 0.1em;">Vinhas</small>
                        </div>
                        <div class="col-4">
                            <h3 style="color: var(--cor-primaria);">+50</h3>
                            <small class="text-muted text-uppercase" style="letter-spacing: 0.1em;">Anos</small>
                        </div>
                    </div>
                </div>
                <div class="col-lg-6">
                    <img src="~/Images/QtaAzenha.png" class="img-fluid rounded shadow-sm" alt="Quinta da Azenha" runat="server" />
                </div>
            </div>
        </div>
    </section>


    <!-- ═══════════════════════════════════════════════════════
         EXPERIÊNCIAS EM DESTAQUE — dados da BD
         ALTERAÇÃO 2026-04-15: subido para cima (após Sobre)
         para melhor visibilidade e conversão
         ═══════════════════════════════════════════════════════ -->
    <section class="py-5 bg-white">
        <div class="container">
            <span class="label-secao text-center d-block">O Que Fazemos</span>
            <h2 class="text-center mb-0">Experiências em Destaque</h2>
            <div class="linha-verde"></div>

            <div class="row g-4">
                <asp:Repeater ID="rpt_experiencias" runat="server">
                    <ItemTemplate>
                        <div class="col-md-4">
                            <div class="card h-100 shadow-sm border-0">
                                <img src='<%# ResolveUrl(Eval("imagem_url").ToString()) %>'
                                     class="card-img-top"
                                     alt='<%# Eval("nome") %>'
                                     onerror="this.src='<%# ResolveUrl("~/Images/QtaAzenha.png") %>'" />
                                <div class="card-body d-flex flex-column">
                                    <h5 class="card-title"><%# Eval("nome") %></h5>
                                    <p class="card-text text-muted small"><%# Eval("descricao") %></p>
                                    <div class="mt-auto pt-2">
                                        <a href="~/Pages/experiencias.aspx" runat="server" class="btn-quinta-outline">Saber Mais</a>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <div class="text-center mt-4">
                <a href="~/Pages/experiencias.aspx" runat="server" class="btn-quinta">Ver Todas as Experiências</a>
            </div>
        </div>
    </section>


    <!-- ═══════════════════════════════════════════════════════
         A SEMANA NA VINHA — Open Meteo API
         ALTERAÇÃO 2026-04-15: cards unificados com o style guide
         ───────────────────────────────────────────────────────
         ─ fundo branco/creme, bordas neutras, hover verde
         ─ 8º card elegante com apontamentos dourados
         ═══════════════════════════════════════════════════════ -->
    <section class="py-5 bg-white">
        <div class="container">
            <span class="label-secao text-center d-block">Previsão do Tempo</span>
            <h2 class="text-center mb-0">A Semana na Vinha</h2>
            <p class="text-center text-muted small mt-2 mb-0">Bucelas · previsão em tempo real via Open-Meteo</p>
            <div class="linha-verde"></div>

            <div id="weather-loading" class="text-center py-4">
                <div class="spinner-border" role="status" style="color: var(--cor-primaria);">
                    <span class="visually-hidden">A carregar...</span>
                </div>
                <p class="text-muted small mt-2">A carregar previsão para Bucelas...</p>
            </div>

            <div id="weather-erro" class="alert alert-danger text-center" style="display:none;"></div>
            <div id="weather-vazio" class="text-center text-muted" style="display:none;">Sem dados disponíveis de momento.</div>

            <div id="weather-grid" class="row g-3" style="display:none;"></div>
        </div>
    </section>

    <script>
        var weatherInfo = {
            0: { descricao: 'Céu limpo', icone: 'bi-sun' },
            1: { descricao: 'Principalmente limpo', icone: 'bi-sun' },
            2: { descricao: 'Parcialmente nublado', icone: 'bi-cloud-sun' },
            3: { descricao: 'Nublado', icone: 'bi-cloud' },
            45: { descricao: 'Nevoeiro', icone: 'bi-cloud-haze' },
            48: { descricao: 'Nevoeiro', icone: 'bi-cloud-haze' },
            51: { descricao: 'Chuvisco leve', icone: 'bi-cloud-drizzle' },
            53: { descricao: 'Chuvisco', icone: 'bi-cloud-drizzle' },
            55: { descricao: 'Chuvisco forte', icone: 'bi-cloud-drizzle' },
            61: { descricao: 'Chuva leve', icone: 'bi-cloud-rain' },
            63: { descricao: 'Chuva', icone: 'bi-cloud-rain' },
            65: { descricao: 'Chuva forte', icone: 'bi-cloud-rain-heavy' },
            71: { descricao: 'Neve leve', icone: 'bi-cloud-snow' },
            73: { descricao: 'Neve', icone: 'bi-cloud-snow' },
            75: { descricao: 'Neve forte', icone: 'bi-cloud-snow' },
            80: { descricao: 'Aguaceiros', icone: 'bi-cloud-rain' },
            81: { descricao: 'Aguaceiros', icone: 'bi-cloud-rain' },
            82: { descricao: 'Aguaceiros fortes', icone: 'bi-cloud-rain-heavy' },
            95: { descricao: 'Trovoada', icone: 'bi-cloud-lightning-rain' }
        };

        function sugerirAtividade(code, tempMax) {
            if (code === 0 && tempMax >= 20) return 'Dia ideal para visita às vinhas';
            if (code <= 2 && tempMax >= 15) return 'Bom dia para passeio pela quinta';
            if (code === 3) return 'Dia tranquilo para visitar a adega';
            if (code >= 61 && code <= 82) return 'Dia perfeito para prova de vinhos';
            if (code === 95) return 'Ideal para jantar na adega';
            return 'Venha conhecer a quinta';
        }

        function corFundo(code) {
            // ALTERAÇÃO 2026-04-15: todas as cores unificadas com o style guide
            if (code === 0 || code === 1) return '#fff';
            if (code >= 61 && code <= 82) return '#f8f9fa';
            return '#fff';
        }

        function carregarClima() {
            fetch('https://api.open-meteo.com/v1/forecast?latitude=38.99&longitude=-9.10&daily=temperature_2m_max,temperature_2m_min,precipitation_probability_max,weather_code&timezone=Europe%2FLisbon&forecast_days=7')
                .then(function (r) {
                    if (!r.ok) throw new Error('API error');
                    return r.json();
                })
                .then(function (data) {
                    document.getElementById('weather-loading').style.display = 'none';

                    var times = data.daily.time;
                    if (!times || times.length === 0) {
                        document.getElementById('weather-vazio').style.display = 'block';
                        return;
                    }

                    var grid = document.getElementById('weather-grid');
                    grid.style.display = 'flex';
                    grid.style.flexWrap = 'wrap';

                    // ── 7 cards da API ──
                    for (var i = 0; i < times.length; i++) {
                        var code = data.daily.weather_code[i];
                        var tempMax = data.daily.temperature_2m_max[i];
                        var tempMin = data.daily.temperature_2m_min[i];
                        var precip = data.daily.precipitation_probability_max[i];
                        var info = weatherInfo[code] || { descricao: 'Tempo variável', icone: 'bi-thermometer' };
                        var dataFmt = new Date(times[i] + 'T12:00:00').toLocaleDateString('pt-PT', { weekday: 'short', day: 'numeric', month: 'short' });

                        var col = document.createElement('div');
                        col.className = 'col-md-6 col-lg-3';
                        col.innerHTML =
                            '<div class="card h-100 weather-card border">' +
                            '<div class="card-body text-center d-flex flex-column">' +
                            '<p class="small text-uppercase fw-bold mb-1" style="letter-spacing:.1em;color:var(--cor-texto-leve);">' + dataFmt + '</p>' +
                            '<i class="bi ' + info.icone + ' my-2" style="font-size:2.5rem;color:var(--cor-primaria);"></i>' +
                            '<p class="small mb-2" style="color:var(--cor-texto-leve);">' + info.descricao + '</p>' +
                            '<div class="d-flex justify-content-center gap-3 mb-2">' +
                            '<span class="fw-bold" style="color:var(--cor-texto);">' + Math.round(tempMax) + '°</span>' +
                            '<span style="color:var(--cor-texto-leve);">' + Math.round(tempMin) + '°</span>' +
                            '</div>' +
                            '<div class="mb-2">' +
                            '<div class="d-flex justify-content-between mb-1">' +
                            '<small style="color:var(--cor-texto-leve);"><i class="bi bi-droplet me-1"></i>Chuva</small>' +
                            '<small style="color:var(--cor-texto-leve);">' + precip + '%</small>' +
                            '</div>' +
                            '<div class="progress" style="height:4px;">' +
                            '<div class="progress-bar" style="width:' + precip + '%;background-color:' + (parseFloat(precip) > 50 ? '#3b82f6' : 'var(--cor-primaria)') + ';"></div>' +
                            '</div></div>' +
                            '<div class="mt-auto pt-2 border-top">' +
                            '<small style="color:var(--cor-primaria);font-style:italic;"><i class="bi bi-info-circle me-1"></i>' + sugerirAtividade(code, tempMax) + '</small>' +
                            '</div></div></div>';
                        grid.appendChild(col);
                    }

                    <!-- 8º card - convite elegante para reservas -->
                    <!-- ALTERACAO 2026-04-15: sem icones, link para reservas -->
                    var colExtra = document.createElement('div');
                    colExtra.className = 'col-md-6 col-lg-3';
                    colExtra.innerHTML =
                        '<div class="card h-100 weather-extra border d-flex flex-column justify-content-center align-items-center text-center p-3"' +
                        ' onclick="window.location.href=\'reserva.aspx\'">' +
                        '<p class="small text-uppercase fw-bold mb-2" style="letter-spacing:.1em;color:var(--cor-texto-leve);">Reservar</p>' +
                        '<h6 style="color:var(--cor-texto);font-size:.95rem;margin-bottom:.6rem;">Há sempre algo a descobrir</h6>' +
                        '<p style="color:var(--cor-texto-leve);font-size:.8rem;margin-bottom:1rem;line-height:1.5;">Provas, vindimas, passeios pelas vinhas - seja qual for o tempo.</p>' +
                        '<a href="reserva.aspx"' +
                        ' style="display:inline-block;padding:.45rem 1.25rem;border:1px solid var(--cor-primaria);color:var(--cor-primaria);font-size:.78rem;letter-spacing:.08em;text-transform:uppercase;text-decoration:none;border-radius:4px;transition:all .2s;"' +
                        ' onmouseover="this.style.background=\'var(--cor-primaria)\';this.style.color=\'var(--cor-fundo)\'"' +
                        ' onmouseout="this.style.background=\'transparent\';this.style.color=\'var(--cor-primaria)\'"' +
                        ' onclick="event.stopPropagation();">' +
                        'Fazer Reserva</a>' +
                        '</div>';
                    grid.appendChild(colExtra);
                })
                .catch(function () {
                    document.getElementById('weather-loading').style.display = 'none';
                    var erro = document.getElementById('weather-erro');
                    erro.textContent = 'Não foi possível carregar a previsão do tempo.';
                    erro.style.display = 'block';
                });
        }

        document.addEventListener('DOMContentLoaded', carregarClima);
    </script>


    <!-- ═══════════════════════════════════════════════════════
         PROGRAMA DE FIDELIZAÇÃO — Jornada do Sommelier
         ALTERAÇÃO 2026-04-15: fundo branco, cards verde-água
         transparente, textos escuros (já estilizados via raizes.css)
         ═══════════════════════════════════════════════════════ -->
    <section class="py-5 bg-white" style="position: relative; overflow: hidden;">

        <!-- Padrão subtil decorativo (muito suave) -->
        <div style="position:absolute;inset:0;opacity:0.02;background-image:repeating-linear-gradient(45deg,var(--cor-primaria) 0,var(--cor-primaria) 1px,transparent 0,transparent 50%);background-size:20px 20px;pointer-events:none;"></div>

        <div class="container" style="position:relative;">

            <div class="text-center mb-5">
                <span class="label-secao text-center d-block">Para os Nossos Visitantes</span>
                <h2 class="mb-2">A Jornada do Sommelier</h2>
                <div class="linha-verde"></div>
                <p class="mx-auto text-muted" style="max-width: 520px; font-size: 0.95rem; line-height: 1.7;">
                    Cada visita à Quinta da Azenha aprofunda a sua ligação ao território. Acumule pontos, suba de nível e desbloqueie benefícios exclusivos.
                </p>
            </div>

            <!-- 4 níveis — verde água via .card-nivel-sommelier -->
            <div class="row g-3 justify-content-center mb-5">

                <div class="col-6 col-md-3">
                    <div class="card-nivel-sommelier">
                        <div class="card-nivel-icone">
                            <i class="bi bi-eye" style="color:var(--cor-primaria);font-size:1.25rem;"></i>
                        </div>
                        <p class="card-nivel-label">Nível I</p>
                        <h5 class="card-nivel-titulo">Visitante</h5>
                        <p class="card-nivel-pontos">0 – 99 pontos</p>
                        <p class="card-nivel-descricao">Bem-vindo à quinta. A sua história começa aqui.</p>
                    </div>
                </div>

                <div class="col-6 col-md-3">
                    <div class="card-nivel-sommelier">
                        <div class="card-nivel-icone">
                            <i class="bi bi-award" style="color:var(--cor-primaria);font-size:1.25rem;"></i>
                        </div>
                        <p class="card-nivel-label">Nível II</p>
                        <h5 class="card-nivel-titulo">Conhecedor</h5>
                        <p class="card-nivel-pontos">100 – 299 pontos</p>
                        <p class="card-nivel-descricao">Desconto de <strong style="color:var(--cor-primaria);">5%</strong> na próxima reserva.</p>
                    </div>
                </div>

                <div class="col-6 col-md-3">
                    <div class="card-nivel-sommelier">
                        <div class="card-nivel-icone">
                            <i class="bi bi-star-half" style="color:var(--cor-primaria);font-size:1.25rem;"></i>
                        </div>
                        <p class="card-nivel-label">Nível III</p>
                        <h5 class="card-nivel-titulo">Sommelier</h5>
                        <p class="card-nivel-pontos">300 – 599 pontos</p>
                        <p class="card-nivel-descricao">Desconto de <strong style="color:var(--cor-primaria);">10%</strong> e prioridade em sessões exclusivas.</p>
                    </div>
                </div>

                <!-- Embaixador — destaque dourado -->
                <div class="col-6 col-md-3">
                    <div class="card-nivel-sommelier card-nivel-embaixador" style="position:relative;">
                        <div style="position:absolute;top:-1px;left:50%;transform:translateX(-50%);background:var(--cor-destaque);color:var(--cor-fundo);font-size:.65rem;font-weight:700;text-transform:uppercase;letter-spacing:.1em;padding:2px 10px;border-radius:0 0 4px 4px;">
                            Topo
                        </div>
                        <div class="card-nivel-icone" style="background:rgba(200,168,75,0.20);border-color:var(--cor-destaque);margin-top:.75rem;">
                            <i class="bi bi-star-fill" style="color:var(--cor-destaque);font-size:1.25rem;"></i>
                        </div>
                        <p class="card-nivel-label">Nível IV</p>
                        <h5 class="card-nivel-titulo">Embaixador</h5>
                        <p class="card-nivel-pontos">600+ pontos</p>
                        <p class="card-nivel-descricao">Desconto de <strong style="color:var(--cor-destaque);">15%</strong> e acesso a experiências reservadas.</p>
                    </div>
                </div>

            </div>

            <!-- Como ganhar pontos -->
            <div style="border-top:1px solid var(--cor-neutro);padding-top:2rem;margin-bottom:2rem;">
                <p class="text-center" style="color:var(--cor-texto-leve);font-size:.75rem;text-transform:uppercase;letter-spacing:.12em;margin-bottom:1.25rem;">Como ganhar pontos</p>
                <div class="row g-3 justify-content-center text-center">
                    <div class="col-6 col-md-3">
                        <i class="bi bi-calendar-check d-block mb-2" style="color:var(--cor-primaria);font-size:1.4rem;"></i>
                        <p style="color:var(--cor-texto-leve);font-size:.82rem;margin:0;line-height:1.5;">1 ponto por cada<br /><strong style="color:var(--cor-texto);">10€ de reserva</strong></p>
                    </div>
                    <div class="col-6 col-md-3">
                        <i class="bi bi-patch-check d-block mb-2" style="color:var(--cor-primaria);font-size:1.4rem;"></i>
                        <p style="color:var(--cor-texto-leve);font-size:.82rem;margin:0;line-height:1.5;"><strong style="color:var(--cor-texto);">+50 pontos</strong><br />ao confirmar a visita</p>
                    </div>
                    <div class="col-6 col-md-3">
                        <i class="bi bi-qr-code-scan d-block mb-2" style="color:var(--cor-primaria);font-size:1.4rem;"></i>
                        <p style="color:var(--cor-texto-leve);font-size:.82rem;margin:0;line-height:1.5;">QR Codes<br /><strong style="color:var(--cor-texto);">pela propriedade</strong></p>
                    </div>
                    <div class="col-6 col-md-3">
                        <i class="bi bi-chat-square-text d-block mb-2" style="color:var(--cor-primaria);font-size:1.4rem;"></i>
                        <p style="color:var(--cor-texto-leve);font-size:.82rem;margin:0;line-height:1.5;">Avaliações<br /><strong style="color:var(--cor-texto);">de vinhos na app</strong></p>
                    </div>
                </div>
            </div>

            <!-- CTA -->
            <div class="text-center">
                <a href="~/Pages/conta_login.aspx" runat="server"
                   class="btn-quinta-outline">
                    Consultar os Meus Benefícios
                </a>
            </div>

        </div>
    </section>


    <!-- ═══════════════════════════════════════════════════════
         TESTEMUNHOS DE CLIENTES — dados da BD
         ALTERAÇÃO 2026-04-15: movido para após Sommelier
         ═══════════════════════════════════════════════════════ -->
    <section class="py-5" style="background-color: var(--cor-fundo);">
        <div class="container">
            <span class="label-secao text-center d-block">O Que Dizem</span>
            <h2 class="text-center mb-0">Testemunhos</h2>
            <div class="linha-verde"></div>
            <div class="row g-4">
                <asp:Repeater ID="rpt_testemunhos" runat="server">
                    <ItemTemplate>
                        <div class="col-md-4">
                            <div class="card h-100 shadow-sm border-0 p-4">
                                <div class="d-flex align-items-center mb-3">
                                    <p class="mb-0 fw-bold" style="color: var(--cor-texto);">
                                        <%# Eval("nome_cliente") %>
                                    </p>
                                    <span class="ms-auto small text-muted" style="font-size:0.75rem;">
                                        <%# Eval("data_formatada").ToString() %>
                                    </span>
                                </div>
                                <div class="mb-2" style="color: #c8a84b;">
                                    <%# Eval("estrelas_html") %>
                                </div>
                                <p class="card-text text-muted fst-italic small mb-2">
                                    <strong style="color: var(--cor-primaria);">
                                        <%# Eval("experiencia_nome") %>
                                    </strong><br />
                                    <%# Eval("comentario") %>
                                </p>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </section>


    <!-- ═══════════════════════════════════════════════════════
         FAQ — inclui programa de fidelização
         FIX 1: classe JS corrigida para "aberto" (alinhada com raizes.css)
         FIX 2: listener único — removido o duplicado que estava antes
         ═══════════════════════════════════════════════════════ -->
    <section class="py-5" style="background-color: var(--cor-fundo);">
        <div class="container">
            <span class="label-secao text-center d-block">Tem Dúvidas?</span>
            <h2 class="text-center mb-0">Perguntas Frequentes</h2>
            <div class="linha-verde"></div>

            <div class="mx-auto" style="max-width:700px;">

                <div class="faq-item border-bottom">
                    <button class="faq-pergunta btn btn-link text-decoration-none w-100 d-flex justify-content-between align-items-center py-3 px-0" style="color:var(--cor-texto);">
                        Quais são os horários de visita?
                        <span class="faq-icone fs-5">+</span>
                    </button>
                    <div class="faq-resposta">
                        <p class="text-muted pb-3">Estamos abertos de terça a domingo, das 10h às 18h. Recomendamos reserva prévia para garantir disponibilidade.</p>
                    </div>
                </div>

                <div class="faq-item border-bottom">
                    <button class="faq-pergunta btn btn-link text-decoration-none w-100 d-flex justify-content-between align-items-center py-3 px-0" style="color:var(--cor-texto);">
                        É necessário fazer reserva?
                        <span class="faq-icone fs-5">+</span>
                    </button>
                    <div class="faq-resposta">
                        <p class="text-muted pb-3">Sim. Para garantir disponibilidade e uma experiência personalizada, pedimos que reserve com pelo menos 48 horas de antecedência.</p>
                    </div>
                </div>

                <div class="faq-item border-bottom">
                    <button class="faq-pergunta btn btn-link text-decoration-none w-100 d-flex justify-content-between align-items-center py-3 px-0" style="color:var(--cor-texto);">
                        Aceitam grupos grandes?
                        <span class="faq-icone fs-5">+</span>
                    </button>
                    <div class="faq-resposta">
                        <p class="text-muted pb-3">Sim! Temos experiências especiais para grupos de 10 ou mais pessoas. Contacte-nos para um programa personalizado.</p>
                    </div>
                </div>

                <div class="faq-item border-bottom">
                    <button class="faq-pergunta btn btn-link text-decoration-none w-100 d-flex justify-content-between align-items-center py-3 px-0" style="color:var(--cor-texto);">
                        Como chegar à Quinta da Azenha?
                        <span class="faq-icone fs-5">+</span>
                    </button>
                    <div class="faq-resposta">
                        <p class="text-muted pb-3">Estamos em Bucelas, a 25km de Lisboa. Fácil acesso pela A1 (saída Bucelas) ou A8. Enviaremos instruções com a confirmação da reserva.</p>
                    </div>
                </div>

                <div class="faq-item border-bottom">
                    <button class="faq-pergunta btn btn-link text-decoration-none w-100 d-flex justify-content-between align-items-center py-3 px-0" style="color:var(--cor-texto);">
                        Os vinhos podem ser comprados na visita?
                        <span class="faq-icone fs-5">+</span>
                    </button>
                    <div class="faq-resposta">
                        <p class="text-muted pb-3">Sim! Os nossos vinhos estão disponíveis exclusivamente na quinta, durante as visitas. Não vendemos online — cada garrafa merece ser descoberta pessoalmente.</p>
                    </div>
                </div>

                <div class="faq-item border-bottom">
                    <button class="faq-pergunta btn btn-link text-decoration-none w-100 d-flex justify-content-between align-items-center py-3 px-0" style="color:var(--cor-texto);">
                        Como funciona a Jornada do Sommelier?
                        <span class="faq-icone fs-5">+</span>
                    </button>
                    <div class="faq-resposta">
                        <p class="text-muted pb-3">Cada reserva e visita acumula pontos automaticamente: 1 ponto por cada 10€ de reserva, mais 50 pontos bónus quando a visita é confirmada. Na app, pode ainda ganhar pontos ao ler QR codes pela propriedade e ao avaliar vinhos. Ao atingir 100, 300 ou 600 pontos, recebe automaticamente um cupão de 5%, 10% ou 15% de desconto na sua próxima reserva.</p>
                    </div>
                </div>

                <div class="faq-item border-bottom">
                    <button class="faq-pergunta btn btn-link text-decoration-none w-100 d-flex justify-content-between align-items-center py-3 px-0" style="color:var(--cor-texto);">
                        Como utilizo um cupão de desconto?
                        <span class="faq-icone fs-5">+</span>
                    </button>
                    <div class="faq-resposta">
                        <p class="text-muted pb-3">No segundo passo da reserva, encontrará um campo para selecionar um cupão disponível. O desconto é aplicado automaticamente ao valor total. Cada cupão é válido para uma única utilização e expira 6 meses após a emissão.</p>
                    </div>
                </div>

                <div class="faq-item border-bottom">
                    <button class="faq-pergunta btn btn-link text-decoration-none w-100 d-flex justify-content-between align-items-center py-3 px-0" style="color:var(--cor-texto);">
                        Onde consulto os meus pontos e cupões?
                        <span class="faq-icone fs-5">+</span>
                    </button>
                    <div class="faq-resposta">
                        <p class="text-muted pb-3">Na sua área pessoal (acesso pelo canto superior direito), encontra o seu nível actual — Visitante, Conhecedor, Sommelier ou Embaixador —, o saldo de pontos, os cupões disponíveis e o histórico de cupões já utilizados.</p>
                    </div>
                </div>

                <div class="faq-item border-bottom">
                    <button class="faq-pergunta btn btn-link text-decoration-none w-100 d-flex justify-content-between align-items-center py-3 px-0" style="color:var(--cor-texto);">
                        Existem benefícios especiais para empresas?
                        <span class="faq-icone fs-5">+</span>
                    </button>
                    <div class="faq-resposta">
                        <p class="text-muted pb-3">Sim. Parceiros empresariais têm acesso a ofertas exclusivas com descontos em experiências seleccionadas, ideais para eventos corporativos e incentivos. Contacte-nos para conhecer as condições de parceria B2B.</p>
                    </div>
                </div>

            </div>
        </div>

        <%-- Listener único — classe "aberto" alinhada com raizes.css --%>
        <script>
            document.addEventListener('DOMContentLoaded', function () {
                var faqItems = document.querySelectorAll('.faq-item');
                faqItems.forEach(function (item) {
                    var btn = item.querySelector('.faq-pergunta');
                    if (!btn) return;
                    btn.addEventListener('click', function (e) {
                        e.preventDefault();
                        var estaAberto = item.classList.contains('aberto');
                        // fecha todos
                        faqItems.forEach(function (i) { i.classList.remove('aberto'); });
                        // abre o clicado (se não estava aberto)
                        if (!estaAberto) item.classList.add('aberto');
                    });
                });
            });
        </script>
    </section>


    <!-- ═══════════════════════════════════════════════════════
         CTA FINAL — único bloco escuro no final da página
         ALTERAÇÃO 2026-04-15
         ═══════════════════════════════════════════════════════ -->
    <section class="py-5 text-center" style="background-color: var(--cor-texto);">
        <div class="container">
            <span class="label-secao" style="color: var(--cor-destaque);">Bucelas começa aqui</span>
            <h2 class="mb-3 text-white">Pronto para nos Visitar?</h2>
            <p class="mb-4" style="color: rgba(255,255,255,0.7); max-width: 560px; margin-left: auto; margin-right: auto;">
                Reserve a sua experiência e descubra porque Bucelas é a região de vinho branco mais especial de Portugal.
            </p>
            <a href="~/Pages/reserva.aspx" runat="server" class="btn-quinta me-2">Fazer Reserva</a>
            <a href="~/Pages/vinhos.aspx" runat="server" class="btn-quinta-outline" style="color: white; border-color: rgba(255,255,255,0.4);">Ver os Vinhos</a>
        </div>
    </section>

</asp:Content>
