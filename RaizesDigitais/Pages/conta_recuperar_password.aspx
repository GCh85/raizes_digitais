<%@ Page Title="Recuperar Password | Quinta da Azenha" Language="C#" MasterPageFile="~/MasterSite.Master" AutoEventWireup="true" CodeBehind="conta_recuperar_password.aspx.cs" Inherits="RaizesDigitais.Pages.conta_recuperar_password" %>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="text-center py-5" style="margin-top: 56px; background-color: var(--cor-texto); border-bottom: 2px solid var(--cor-destaque);">
        <div class="container">
            <h1 class="fw-normal text-white">Recuperar Password</h1>
        </div>
    </div>
    <main class="py-5">
        <div class="container">
            <div class="row justify-content-center">
                <div class="col-md-5">
                    <span class="label-secao">Área Pessoal</span>
                    <h2 class="mb-0">Esqueceu a password?</h2>
                    <div class="linha-verde-esq"></div>
                    <p class="text-muted mt-3">Introduza o seu email e enviaremos um link para redefinir a password.</p>

                    <asp:Label ID="lbl_mensagem" runat="server" CssClass="d-block mb-3" Visible="false" />

                    <div class="mb-3">
                        <label class="form-label fw-bold small text-uppercase" style="letter-spacing:0.05em;">Email *</label>
                        <asp:TextBox ID="tb_email" runat="server" CssClass="form-control" TextMode="Email" />
                        <asp:RequiredFieldValidator
                            ID="rfv_email" runat="server"
                            ControlToValidate="tb_email"
                            ErrorMessage="Introduza o seu email."
                            CssClass="text-danger small"
                            Display="Dynamic" />
                        <asp:RegularExpressionValidator
                            ID="rev_email" runat="server"
                            ControlToValidate="tb_email"
                            ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                            ErrorMessage="Formato de email inválido."
                            CssClass="text-danger small"
                            Display="Dynamic" />
                    </div>

                    <asp:Button ID="btn_enviar" runat="server" Text="Enviar Link"
                        CssClass="btn-quinta w-100 py-2"
                        OnClick="btn_enviar_Click" />

                    <p class="text-center mt-3">
                        <a href="conta_login.aspx" style="color: var(--cor-primaria);">Voltar ao login</a>
                    </p>
                </div>
            </div>
        </div>
    </main>
</asp:Content>
