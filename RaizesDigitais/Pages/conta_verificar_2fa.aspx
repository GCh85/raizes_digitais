<%@ Page Title="Verificação de Segurança | Quinta da Azenha" Language="C#" MasterPageFile="~/MasterSite.Master" AutoEventWireup="true" CodeBehind="conta_verificar_2fa.aspx.cs" Inherits="RaizesDigitais.Pages.conta_verificar_2fa" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="text-center py-5" style="margin-top: 56px; background-color: var(--cor-texto); border-bottom: 2px solid var(--cor-destaque);">
        <div class="container">
            <h1 class="fw-normal text-white">Segurança</h1>
            <p class="fst-italic" style="color: var(--cor-destaque);">Confirme a sua identidade</p>
        </div>
    </div>

    <main class="py-5">
        <div class="container">
            <div class="row justify-content-center">
                <div class="col-md-5">
                    <span class="label-secao">Autenticação</span>
                    <h2 class="mb-0">Verificar Código</h2>
                    <div class="linha-verde-esq"></div>

                    <p class="text-muted small mb-4">
                        Introduza o código de 6 dígitos enviado para o seu email.
                    </p>

                    <asp:Label ID="lbl_erro" runat="server" CssClass="lbl-erro d-block mb-3" Visible="false" />

                    <div class="mb-4">
                        <label class="form-label fw-bold small text-uppercase" style="letter-spacing:0.05em;">Código de Verificação *</label>
                        <asp:TextBox ID="tb_codigo" runat="server" CssClass="form-control text-center fs-4" MaxLength="6" placeholder="000000" />
                        <asp:RequiredFieldValidator
                            ID="rfv_codigo" runat="server"
                            ControlToValidate="tb_codigo"
                            ErrorMessage="Introduza o código de verificação."
                            CssClass="text-danger small"
                            Display="Dynamic" />
                        <asp:RegularExpressionValidator
                            ID="rev_codigo" runat="server"
                            ControlToValidate="tb_codigo"
                            ValidationExpression="^\d{6}$"
                            ErrorMessage="O código deve ter exactamente 6 dígitos."
                            CssClass="text-danger small"
                            Display="Dynamic" />
                    </div>

                    <asp:Button ID="btn_verificar" runat="server" Text="Verificar e Entrar"
                        CssClass="btn-quinta w-100 py-2"
                        OnClick="btn_verificar_Click" />

                    <p class="text-center mt-4">
                        <asp:LinkButton ID="btn_voltar" runat="server" CssClass="text-muted small text-decoration-none" OnClick="btn_voltar_Click" CausesValidation="false">
                            <i class="bi bi-arrow-left"></i> Voltar ao Login
                        </asp:LinkButton>
                    </p>
                </div>
            </div>
        </div>
    </main>
</asp:Content>
