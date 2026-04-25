<%@ Page Title="Redefinir Password | Quinta da Azenha" Language="C#" MasterPageFile="~/MasterSite.Master" AutoEventWireup="true" CodeBehind="conta_reset_password.aspx.cs" Inherits="RaizesDigitais.Pages.conta_reset_password" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="text-center py-5" style="margin-top: 56px; background-color: var(--cor-texto); border-bottom: 2px solid var(--cor-destaque);">
        <div class="container">
            <h1 class="fw-normal text-white">Redefinir Password</h1>
        </div>
    </div>
    <main class="py-5">
        <div class="container">
            <div class="row justify-content-center">
                <div class="col-md-5">
                    <asp:Label ID="lbl_mensagem" runat="server" CssClass="d-block mb-3" Visible="false" />

                    <asp:Panel ID="pnl_form" runat="server">
                        <span class="label-secao">Área Pessoal</span>
                        <h2 class="mb-0">Nova Password</h2>
                        <div class="linha-verde-esq"></div>

                        <div class="mb-3 mt-3">
                            <label class="form-label fw-bold small text-uppercase" style="letter-spacing:0.05em;">Nova Password *</label>
                            <asp:TextBox ID="tb_pw_nova" runat="server" CssClass="form-control" TextMode="Password" />
                            <asp:RequiredFieldValidator
                                ID="rfv_pw_nova" runat="server"
                                ControlToValidate="tb_pw_nova"
                                ErrorMessage="Introduza a nova password."
                                CssClass="text-danger small"
                                Display="Dynamic" />
                            <asp:RegularExpressionValidator
                                ID="rev_pw_nova" runat="server"
                                ControlToValidate="tb_pw_nova"
                                ValidationExpression=".{8,}"
                                ErrorMessage="A password deve ter pelo menos 8 caracteres."
                                CssClass="text-danger small"
                                Display="Dynamic" />
                        </div>
                        <div class="mb-4">
                            <label class="form-label fw-bold small text-uppercase" style="letter-spacing:0.05em;">Confirmar Password *</label>
                            <asp:TextBox ID="tb_pw_confirmar" runat="server" CssClass="form-control" TextMode="Password" />
                            <asp:RequiredFieldValidator
                                ID="rfv_pw_confirmar" runat="server"
                                ControlToValidate="tb_pw_confirmar"
                                ErrorMessage="Confirme a nova password."
                                CssClass="text-danger small"
                                Display="Dynamic" />
                        </div>

                        <asp:Button ID="btn_redefinir" runat="server" Text="Redefinir Password"
                            CssClass="btn-quinta w-100 py-2"
                            OnClick="btn_redefinir_Click" />
                    </asp:Panel>

                    <p class="text-center mt-3">
                        <a href="conta_login.aspx" style="color: var(--cor-primaria);">Voltar ao login</a>
                    </p>
                </div>
            </div>
        </div>
    </main>
</asp:Content>
