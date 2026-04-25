<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPublico.Master" AutoEventWireup="true" CodeBehind="reset_password.aspx.cs" Inherits="RaizesDigitais.reset_password" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder3" runat="server">

    <div class="card-auth">
        <h4>Quinta da Azenha</h4>
        <p class="subtitulo">Redefinir palavra-passe</p>

        <asp:Label ID="lbl_erro" runat="server" CssClass="lbl-erro d-block mb-3" />
        <asp:Label ID="lbl_sucesso" runat="server" CssClass="lbl-sucesso d-block mb-3" />

        <asp:Panel ID="pnl_form" runat="server">

            <div class="mb-3">
                <label class="form-label">Nova palavra-passe</label>
                <asp:TextBox ID="tb_pw_nova" runat="server"
                    CssClass="form-control" TextMode="Password" />
                <asp:RequiredFieldValidator ID="rfv_pw_nova" runat="server"
                    ControlToValidate="tb_pw_nova"
                    ErrorMessage="Introduza a nova palavra-passe."
                    CssClass="lbl-erro d-block mt-1"
                    Display="Dynamic"
                    ValidationGroup="vg_reset"
                    EnableClientScript="false" />
            </div>

            <div class="mb-3">
                <label class="form-label">Confirmar palavra-passe</label>
                <asp:TextBox ID="tb_pw_confirmar" runat="server"
                    CssClass="form-control" TextMode="Password" />
                <asp:RequiredFieldValidator ID="rfv_pw_confirmar" runat="server"
                    ControlToValidate="tb_pw_confirmar"
                    ErrorMessage="Confirme a palavra-passe."
                    CssClass="lbl-erro d-block mt-1"
                    Display="Dynamic"
                    ValidationGroup="vg_reset"
                    EnableClientScript="false" />
                <asp:CompareValidator ID="cv_pw" runat="server"
                    ControlToValidate="tb_pw_confirmar"
                    ControlToCompare="tb_pw_nova"
                    ErrorMessage="As palavras-passe não coincidem."
                    CssClass="lbl-erro d-block mt-1"
                    Display="Dynamic"
                    ValidationGroup="vg_reset"
                    EnableClientScript="false" />
            </div>

            <asp:Button ID="btn_redefinir" runat="server"
                Text="Redefinir palavra-passe"
                CssClass="btn-primario btn mb-2"
                OnClick="btn_redefinir_Click"
                ValidationGroup="vg_reset" />

        </asp:Panel>

        <a href="login.aspx" class="link-secundario mt-1">
            Voltar ao login
        </a>
    </div>

</asp:Content>

