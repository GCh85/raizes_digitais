<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPublico.Master" AutoEventWireup="true" CodeBehind="recuperar_password.aspx.cs" Inherits="RaizesDigitais.recuperar_password" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder3" runat="server">

    <div class="card-auth">
        <h4>Quinta da Azenha</h4>
        <p class="subtitulo">Recuperar palavra-passe</p>

        <p style="color:#555; font-size:14px; margin-bottom:24px;">
            Introduza o seu nome de utilizador. Receberá um email com um link para redefinir a sua palavra-passe.
        </p>

        <asp:Label ID="lbl_erro" runat="server" CssClass="lbl-erro d-block mb-3" />
        <asp:Label ID="lbl_sucesso" runat="server" CssClass="lbl-sucesso d-block mb-3" />

        <div class="mb-3">
            <label class="form-label">Utilizador</label>
            <asp:TextBox ID="tb_utilizador" runat="server"
                CssClass="form-control" placeholder="nome de utilizador" />
            <asp:RequiredFieldValidator ID="rfv_utilizador" runat="server"
                ControlToValidate="tb_utilizador"
                ErrorMessage="Introduza o nome de utilizador."
                CssClass="lbl-erro d-block mt-1"
                Display="Dynamic"
                ValidationGroup="vg_recuperar"
                EnableClientScript="false" />
        </div>

        <asp:Button ID="btn_enviar" runat="server"
            Text="Enviar link"
            CssClass="btn-primario btn mb-2"
            OnClick="btn_enviar_Click"
            ValidationGroup="vg_recuperar" />

        <a href="login.aspx" class="link-secundario mt-1">
            Voltar ao login
        </a>
    </div>

</asp:Content>
