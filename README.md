# 🍇 Raizes Digitais
## Plataforma de Enoturismo Digital da Quinta da Azenha

[![ASP.NET](https://img.shields.io/badge/ASP.NET-Web%20Forms-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-Express-CC2927?style=flat&logo=microsoftsqlserver)](https://www.microsoft.com/sql-server/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=flat&logo=bootstrap)](https://getbootstrap.com/)
[![Kotlin](https://img.shields.io/badge/Kotlin-Android-7F52FF?style=flat&logo=kotlin)](https://kotlinlang.org/)
[![License](https://img.shields.io/badge/License-Academic-blue?style=flat)](LICENSE)

> **Projeto Final ATEC TPSI-CAS-0725**
>
> Sistema de gestão de enoturismo da Quinta da Azenha, propriedade vitivinícola familiar em Bucelas, produtora de vinho Arinto DOC. Caso real aprovado pelo professor António Pacheco (Termo de Abertura, Março 2026).

---

## 📋 Índice

- [Funcionalidades](#-funcionalidades)
- [Arquitetura](#-arquitetura)
- [Stack-Tecnológico](#️-stack-tecnológico)
- [Base de Dados](#-base-de-dados)
- [Website Público](#-website-público)
- [Backoffice](#-backoffice)
- [App Android](#-app-android)
- [Gamificação](#-gamificação)
- [Inteligência Artificial](#-inteligência-artificial)
- [Setup Local](#-setup-local)
- [Desenvolvimento](#-desenvolvimento)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Contribuição](#-contribuição)
- [Licença](#-licença)

---

## ✨ Funcionalidades

### 🌐 Website Público

- **Catálogo de Experiências** — Prova de Vinhos, Visita à Vinha, Almoço Rural, Estadia
- **Sistema de Reservas** — Escolha de data/pessoas, cálculo automático de preço
- **Autenticação** — Registo, login, 2FA, recuperação de password
- **Área Pessoal** — Histórico de reservas, favoritos, pontos de fidelização
- **Email + PDF** — Confirmação automática com anexo iTextSharp

### 🖥️ Backoffice

- **Dashboard** — KPIs do dia, reservas, receita, alertas de stock
- **Gestão de Reservas** — Criar, editar, alterar estado, cancelar
- **CRM** — Ficha completa do cliente (alergias, preferências, histórico)
- **Gestão de Vinhos** — Catálogo com stock, perfil sensorial
- **Gestão de Experiências** — Criar, editar, disponibilidade
- **Programa de Fidelização** — Pontos, níveis, cupões de desconto
- **Avaliações** — Aprovar testemunhos de experiências

### 📱 App Android

- **Login** — Autenticação com o sistema do site
- **Catálogo de Vinhos** — Lista com filtro por tipo
- **QR Scanner** — Ler QR Codes físicos na quinta
- **Avaliações** — 1-5 estrelas + comentário
- **Favoritos** — Guardar vinhos da visita
- **Reservas** — Criar reserva diretamente na app

### 🏆 Sistema de Gamificação

- **Pontuação por Ação** — Reserva confirmada, QR lido, avaliação
- **Níveis de Fidelização** — Visitante → Conhecedor → Sommelier → Embaixador
- **Pontos Resgatáveis** — Troca por cupões de desconto
- **Narrativa IA** — Adapta-se ao nível do cliente

### 🤖 Inteligência Artificial

- **Narrativa Personalizada** — IA gera texto de acordo com o nível de fidelização
- **OpenRouter API** — Integração com modelos de linguagem

---

## 🏗️ Arquitetura

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        WEBSITE PÚBLICO                                      │
│                   Quinta da Azenha (ASP.NET Web Forms)                      │
│                  Bootstrap 5 + JavaScript + jQuery                          │
└───────────────────────────────────┬─────────────────────────────────────────┘
                                    │ HTTP/Post
                                    ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                    SERVIDOR (IIS Express)                                   │
│  ┌─────────────────────────────────────────────────────────────┐            │
│  │                APRESENTAÇÃO (Code-Behind C#)                │            │
│  │  • ASPX Pages + Master Pages                                │            │
│  │  • Code-Behind (.aspx.cs)                                   │            │
│  │  • Handlers (.ashx) → JSON para API                         │            │
│  └─────────────────────────────────────────────────────────────┘            │
│                                    │                                        │
│                                    ▼                                        │
│  ┌─────────────────────────────────────────────────────────────┐            │
│  │              LÓGICA DE NEGÓCIO (App_Code)                   │            │
│  │  • Seguranca.cs (SHA-256 + Salt)                            │            │
│  │  • Email.cs (SmtpClient)                                    │            │
│  │  • GeradorPDF.cs (iTextSharp)                               │            │
│  │  • Startup.cs                                               │            │
│  └─────────────────────────────────────────────────────────────┘            │
│                                    │                                        │
│                                    ▼                                        │
│  ┌─────────────────────────────────────────────────────────────┐            │
│  │              PERSISTÊNCIA (Stored Procedures)               │            │
│  │  • SqlCommand + SqlParameter                                │            │
│  │  • CommandType.StoredProcedure                              │            │
│  │  • Transações quando necessário                             │            │
│  └─────────────────────────────────────────────────────────────┘            │
└───────────────────────────────┬─────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                       SQL SERVER EXPRESS                                    │
│  ┌──────────────────────┐   ┌───────────────┐   ┌───────────────┐           │
│  │   13 Tabelas         │   │  88 SPs       │   │     Views     │           │
│  │                      │   │               │   │               │           │
│  │ • clientes           │   │ • sp_login    │   │ • v_kpis      │           │
│  │ • reservas           │   │ • sp_inserir_ │   │               │           │
│  │ • experiencias       │   │   reserva     │   │               │           │
│  │ • vinhos             │   │ • sp_listar_  │   │               │           │
│  │ • favoritos          │   │  vinhos       │   │               │           │
│  │ • pontos_            │   │ • sp_obter_   │   │               │           │
│  │   fideliz.           │   │   cliente     │   │               │           │
│  └──────────────────────┘   └───────────────┘   └───────────────┘           │
└─────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│                        APP ANDROID (KOTLIN)                                 │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐         │
│  │   Login     │  │  Catalogo   │  │  QR Scan    │  │  Favoritos  │         │
│  │             │  │   Vinhos    │  │             │  │             │         │
│  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────┘         │
│                                                                             │
│  │ HTTP/JSON                                                                │
│  ▼                                                                          │
│  API REST via Handlers .ashx                                                │
└─────────────────────────────────────────────────────────────────────────────┘
```

### Arquitetura de 3 Camadas

| Camada | Tecnologia | Responsabilidade |
|--------|-----------|---------------|
| **Apresentação** | ASPX Pages + Code-Behind | UI, validação JS, postback |
| **Lógica de Negócio** | Classes C# (App_Code) | Seguranca, Email, PDF |
| **Persistência** | Stored Procedures | Acesso a dados, transações |


---

## 🛠️ Stack Tecnológico

| Tecnologia | Versão | Finalidade |
|------------|--------|------------|
| **ASP.NET Web Forms** | .NET Framework 4.8 | Framework web do servidor |
| **C#** | 12 | Linguagem server-side |
| **SQL Server Express** | 2022 | Base de dados relacional |
| **ADO.NET** | - | Acesso a dados |
| **Stored Procedures** | - | Lógica de dados |
| **Bootstrap** | 5.3 | Framework CSS frontend |
| **JavaScript/jQuery** | 3.x | Interatividade frontend |
| **SHA-256 + Salt** | .NET native | Hash de passwords |
| **iTextSharp** | 7.x | Geração de PDF |
| **SmtpClient** | .NET native | Envio de emails |
| **Kotlin** | 1.9.x | linguagem app Android |
| **Android SDK** | API 34 | Framework app mobile |
| **Chart.js** | 4.x | Gráficos no dashboard |
| **OSMDroid** | 6.x | Mapas na app |
| **ZXing** | 3.x | Leitor QR Code |
| **OpenRouter API** | - | Narrativa IA |

---

## 🗄️ Base de Dados

### 13 Tabelas

| # | Tabela | Descrição |
|---|-------|-----------|
| 1 | `perfis` | Perfis de acesso (Administrador, Gestor) |
| 2 | `utilizadores` | Contas do backoffice |
| 3 | `clientes` | Visitantes registados |
| 4 | `experiências` | Catálogo de experiências |
| 5 | `disponibilidade` | Calendário de sessões |
| 6 | `reservas` | Reservas efetuadas |
| 7 | `vinhos` | Catálogo de vinhos |
| 8 | `favoritos` | Vinhos favoritos por cliente |
| 9 | `avaliações_vinhos` | Avaliações de vinhos |
| 10 | `avaliações_experiências` | Testemunhos de experiências |
| 11 | `pontos_fidelização` | Histórico de pontos |
| 12 | `cupões` | Cupões de desconto |
| 13 | `qr_codigos` | QR Codes físicos na quinta |

### Stored Procedures

- **88 Stored Procedures** documentadas
- Todas as operações de leitura/escrita passam por SP
- Parâmetros OUTPUT com `.Size` obrigatório (regra das aulas)
- Transações para operações críticas (reservas)

### Estrutura de Reservas

```
reservas
├── num_reserva (UNIQUE: RD-AAAAMMDD-XXXX)
├── id_cliente → clientes
├── id_disponibilidade → disponibilidade
├── num_pessoas
├── preco_total (guardado no momento)
├── estado (Pendente/Confirmada/Cancelada/Concluída)
└── data_reserva
```

---

## 🌐 Website Público

### Páginas Principais

| Página | Descrição | Master |
|-------|-----------|--------|
| `index.aspx` | Homepage com catálogo | Site.Master |
| `login.aspx` | Login do Backoffice | Site.Master |
| `registo.aspx` | Registo de utilizador | Site.Master |
| `recuperar_password.aspx` | Recuperação | Site.Master |
| `experiencias.aspx` | Catálogo completo | Site.Master |
| `reserva.aspx` | Formulário de reserva | Site.Master |
| `confirmacao.aspx` | Página de confirmação | Site.Master |

### Área Pessoal (Site.Master)

| Página | Descrição |
|-------|-----------|
| `conta_login.aspx` | Login de cliente |
| `conta_registo.aspx` | Registo de cliente |
| `conta_area.aspx` | Dashboard pessoal |
| `conta_reservas.aspx` | Histórico |
| `conta_favoritos.aspx` | Vinhos favoritos |
| `conta_pontos.aspx` | Pontos e níveis |

### Funcionalidades do Site

- **Catálogo Dinâmico** — Experiências ativas com imagens
- **Calendário de Disponibilidade** — Seleção de data/pessoas
- **Processo de Reserva em 3 Passos** — Escolha → Dados → Confirmação
- **Autenticação SHA-256 + Salt** — Nunca MD5 (explicar ao professor)
- **2FA por Email** — Código de 6 dígitos
- **Google OAuth** — Login com Google
- **Email de Confirmação** — Com PDF em anexo (iTextSharp)
- **Area Pessoal** — Histórico, favoritos, pontos

---

## 🖥️ Backoffice

### Páginas (AdminLTE)

| Página | Descrição | Master |
|-------|-----------|--------|
| `login.aspx` | Login administrativo | Site.Master |
| `dashboard.aspx` | KPIs e alertas | Backoffice.Master |
| `gerir_reservas.aspx` | Gestão de reservas | Backoffice.Master |
| `gerir_clientes.aspx` | CRM completo | Backoffice.Master |
| `gerir_vinhos.aspx` | Catálogo de vinhos | Backoffice.Master |
| `gerir_experiencias.aspx` | Gestão de experiências | Backoffice.Master |
| `gerir_cupoes.aspx` | Criar cupões | Backoffice.Master |
| `gerir_utilizadores.aspx` | Gestão de staff | Backoffice.Master |
| `gerir_testemunhos.aspx` | Aprovar avaliações | Backoffice.Master |
| `gerir_ofertas_b2b.aspx` | Ofertas empresariais | Backoffice.Master |

### Dashboard KPIs

- **Reservas do dia** — Total e por estado
- **Receita do mês** — Gráfico Chart.js
- **Alertas de Stock** — Vinhos abaixo do mínimo
- **Clientes VIP** — Top 5 por pontos
- **Próximas chegadas** — Hoje e amanhã

### Funcionalidades do Backoffice

- **CRUD Completo** — Todas as entidades
- **Pesquisa e Filtro** — GridView com paginação
- **Segmentação CRM** — VIP, Regular, Inativo, B2B
- **Programa de Fidelização** — Atribuir pontos manualmente
- **Cupões de Desconto** — Percentagem ou valor fijo
- **Gestão de Stock** — Alertas visuais
- **Auditoria** — Última alteração por quem

---

## 📱 App Android

### 4 Ecrãs Principais

| Ecrã | Descrição |
|------|-----------|
| **Login** | Autenticação com o sistema do site |
| **Catálogo** | Lista de vinhos filtrável |
| **QR Scanner** | Ler QR Codes físicos |
| **Favoritos** | Vinhos guardados |

### Funcionalidades da App

- **Login** — Integra com `sp_login_cliente`
- **Lista de Vinhos** — GET via handler .ashx
- **Avaliação** — 1-5 estrelas (ganha 10 pontos)
- **QR Reader** — ZXing (ganha 30 pontos)
- **Favoritos** — Sincroniza com site
- **Reservas** — Criar reserva direta
- **Mapa** — OSMDroid com localização

### Comunicação com Servidor

```
App Android
      │
      │ HTTP/JSON
      ▼
Handlers .ashx
      │
      ├─ api/vinhos.ashx
      ├─ api/login.ashx
      ├─ api/reservas.ashx
      ├─ api/avaliacoes.ashx
      └─ api/favoritos.ashx
            │
            ▼
        Stored Procedures
            │
            ▼
      SQL Server
```

---

## 🏆 Gamificação

### Pontuação por Ação

| Origem | Ação | Pontos | Notas |
|--------|------|--------|-------|
| App mobile | Avaliar vinho | **10 pts** | 1x por vinho |
| App mobile | Ler QR Code | **30 pts** | Por código |
| Ambos | Reserva confirmada | **50 pts** | - |
| Ambos | Reserva (1€ = 1 pt) | **FLOOR(preço/10)** | - |
| Backoffice | Resgate de cupão | **-pontos** | Valor negativo |

### Níveis de Fidelização

| Nível | Pontos | Narrativa IA |
|-------|--------|--------------|
| Visitante | 0–99 | Acolhedora, introdutória |
| Conhecedor | 100–299 | Detalhada |
| Sommelier | 300–599 | Técnica |
| Embaixador | 600+ | Exclusiva |

### Fonte de Verdade

Todos os pontos são registados na tabela `pontos_fidelização`:

```sql
SELECT SUM(pontos_ganhos) 
FROM pontos_fidelizacao 
WHERE id_cliente = @id
```

---

## 🤖 Inteligência Artificial

### Narrativa Personalizada

A IA gera texto de acordo com o nível de fidelização do cliente:

- **Visitante** → Texto acolhedor, introdutório
- **Conhecedor** → Detalhado, com contexto
- **Sommelier** → Técnico, especializado
- **Embaixador** → Exclusivo, como o produtor

### Integração

- **OpenRouter API** — Modelos LLM
- **Prompt personalizado** — Com base nos pontos
- **Cache de resposta** — Evitar chamadas repetidas

---

## 🚀 Setup Local

### Pré-requisitos

- **Visual Studio 2022** (17.8+)
- **SQL Server Express** 2022
- **SQL Server Management Studio** (SSMS)
- **Android Studio** (para a app Kotlin)
- **Git**

### Configuração da Base de Dados

```sql
-- 1. Criar base de dados
CREATE DATABASE raizes_digitais_azenha
GO

-- 2. Executar scripts na ordem:
--    Context_versao FINAL/BD_Raizes_Digitais_Completo.md
--    (13 tabelas + 88 SPs + dados de teste)
```

### Configuração do Visual Studio

```bash
# 1. Abrir solução
RaizesDigitais.sln

# 2. Verificar Web.config
<connectionStrings>
  <add name="RaizesDB" 
       connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=raizes_digitais_azenha;Integrated Security=True;TrustServerCertificate=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>

# 3. Pressionar F5 para executar
```

### Criar Utilizador admin

```csharp
// Via código Seguranca.cs
string salt = Seguranca.GerarSalt();
string hash = Seguranca.HashPassword("Admin123!", salt);

// Executar SP
sp_inserir_utilizador 
  @utilizador = "admin"
  @email = "admin@quintaazenha.pt"
  @hash = [hash gerado]
  @salt = [salt gerado]
  @id_perfil = 1  -- Administrador
```

---

## 💻 Desenvolvimento

### Regras de Código

```
✔ Naming Controls: tb_nome, btn_guardar, lbl_erro, ddl_perfil, gv_lista
✔ Session Keys: Session["perfil"], Session["cliente_id"], Session["utilizador"]
✔ Pages: login.aspx, gerir_utilizadores.aspx
✔ Stored Procedure: CommandType.StoredProcedure
✔ Parameters: definir .Size em OUTPUT varchar
✔ Connection: ConfigurationManager.ConnectionStrings
✔ Logout: Session.Abandon() + Redirect
```

### Padrão de Conexão

```csharp
using System.Data.SqlClient;

SqlConnection myConn = new SqlConnection(
    ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString
);
SqlCommand myCommand = new SqlCommand("nome_sp", myConn);
myCommand.CommandType = CommandType.StoredProcedure;

myCommand.Parameters.AddWithValue("@param", valor);

SqlParameter retorno = new SqlParameter();
retorno.ParameterName = "@retorno";
retorno.Direction = ParameterDirection.Output;
retorno.SqlDbType = SqlDbType.Int;
myCommand.Parameters.Add(retorno);

myConn.Open();
myCommand.ExecuteNonQuery();
int resultado = Convert.ToInt32(myCommand.Parameters["@retorno"].Value);
myConn.Close();
```


## 📁 Estrutura do Projeto

```
RaizesDigitais/
│
├── RaizesDigitais.sln
│
└── RaizesDigitais/
    ├── Web.config                    # Connection string + SMTP
    ├── RaizesDigitais.csproj
    │
    ├── App_Code/
    │   ├── Seguranca.cs         # SHA-256 + Salt
    │   ├── Email.cs            # SmtpClient
    │   ├── GeradorPDF.cs      # iTextSharp
    │   └── Startup.cs
    │
    ├── MasterSite.Master         # Website público
    ├── MasterPublico.Master
    │
    ├── MasterBackoffice.Master      # AdminLTE
    │
    ├── Content/
    │   ├── raizes.css        # Estilos do site
    │   ├── site.css          # CSS base
    │   └── backoffice.css    # AdminLTE
    │
    ├── Images/
    │   ├── *.png            # Imagens
    │   └── vindima.jpg
    │
    ├── Template/
    │   └── confirmacao_reserva_template.pdf
    │
    ├── Pages/                  # Website público
    │   ├── index.aspx
    │   ├── experiencias.aspx
    │   ├── reserva.aspx
    │   ├── confirmacao.aspx
    │   ├── login.aspx
    │   ├── registo.aspx
    │   ├── recuperar_password.aspx
    │   └── conta/           # Área pessoal
    │       ├── area.aspx
    │       ├── reservas.aspx
    │       └── pontos.aspx
    │
    ├── Backoffice/             # Área administrativa
    │   ├── dashboard.aspx
    │   ├── gerir_reservas.aspx
    │   ├── gerir_clientes.aspx
    │   ├── gerir_vinhos.aspx
    │   ├── gerir_experiencias.aspx
    │   ├── gerir_cupoes.aspx
    │   ├── gerir_utilizadores.aspx
    │   ├── gerir_testemunhos.aspx
    │   └── gerir_ofertas_b2b.aspx
    │
    ├── Handlers/             # API REST
    │   ├── api/
    │   │   ├── vinhos.ashx
    │   │   ├── login.ashx
    │   │   ├── reservas.ashx
    │   │   ├── avaliacoes.ashx
    │   │   └── favoritos.ashx
    │
    └── Properties/
        └── AssemblyInfo.cs
```

---



## 🗓️ Roadmap

| Sprint | Período | Foco | Estado |
|--------|---------|------|--------|
| 0 | 17 Fev - 3 Mar | Setup + proposta | ✅ Concluído |
| 1 | 4 - 17 Mar | BD + Auth + estrutura | ✅ Concluído |
| 2 | 18 - 31 Mar | Website + reservas | ✅ Concluído |
| 3 | 1 - 14 Abr | Backoffice + CRM | ✅ Concluído |
| 4 | 15 - 28 Abr | App Android + API | ✅ Concluído |
| 5 | 29 Abr - 8 Mai | IA +Login/Gamificação | ⚠️ Em progresso |


---

## 🤝 Contribuição

1. Fork o repositório
2. Criar branch (`git checkout -b feature/nova-funcionalidade`)
3. Commit das alterações (`git commit -m 'Adicionar nova funcionalidade'`)
4. Push para o branch (`git push origin feature/nova-funcionalidade`)
5. Abrir Pull Request

### Convenções de Código

- Seguir [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Usar **Async/Await** em operações de I/O
- Validar inputs nos code-behind
- Documentar com comentários em português

---

## 📄 Licença

Projeto académico desenvolvido para **ATEC TPSI-CAS-0725** — Curso de Especialização Tecnológica em Técnicas e Programação de Sistemas de Informação.

---

<div align="center">

**Made with 🍇 for Quinta da Azenha**

[![ASP.NET](https://img.shields.io/badge/ASP.NET-Web%20Forms-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=flat&logo=bootstrap)](https://getbootstrap.com/)
[![Kotlin](https://img.shields.io/badge/Kotlin-Android-7F52FF?style=flat&logo=kotlin)](https://kotlinlang.org/)

</div>