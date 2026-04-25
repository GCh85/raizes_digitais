 <%@ Page Title="" Language="C#" MasterPageFile="~/MasterPublico.Master" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="RaizesDigitais.login" %>

<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolder3" runat="server">

    <div class="card-auth">

        <h4>Quinta da Azenha</h4>
        <p class="subtitulo">Área de gestão - iniciar sessão</p>

        <!-- Utilizador -->
        <div class="mb-3">
            <label class="form-label" for="tb_utilizador">Utilizador</label>
            <asp:TextBox ID="tb_utilizador" runat="server"
                CssClass="form-control" placeholder="nome de utilizador" />
            <asp:RequiredFieldValidator ID="rfv_utilizador" runat="server"
                ControlToValidate="tb_utilizador"
                ErrorMessage="Introduza o utilizador."
                CssClass="lbl-erro d-block mt-1"
                Display="Dynamic"
                ValidationGroup="vg_login"
                EnableClientScript="false" />
        </div>

        <!-- Password -->
        <div class="mb-3">
            <label class="form-label" for="tb_pw">Palavra-passe</label>
            <asp:TextBox ID="tb_pw" runat="server"
                CssClass="form-control" placeholder="••••••••"
                TextMode="Password" />
            <asp:RequiredFieldValidator ID="rfv_pw" runat="server"
                ControlToValidate="tb_pw"
                ErrorMessage="Introduza a palavra-passe."
                CssClass="lbl-erro d-block mt-1"
                Display="Dynamic"
                ValidationGroup="vg_login"
                EnableClientScript="false" />
        </div>

        <!-- Mensagem de erro -->
        <asp:Label ID="lbl_erro" runat="server" CssClass="lbl-erro d-block mb-3" />

        <!-- Botão entrar -->
        <asp:Button ID="btn_entrar" runat="server"
            Text="Entrar"
            CssClass="btn-primario btn mb-2"
            OnClick="btn_entrar_Click"
            ValidationGroup="vg_login"
            EnableClientScript="false" />

        <!-- Separador -->
        <div class="separador">ou</div>

        <!-- Botão Google OAuth -->
        <asp:LinkButton ID="btn_google" runat="server"
            CssClass="btn-google mb-3"
            OnClick="btn_google_Click"
            CausesValidation="false">
            <img src="https://developers.google.com/identity/images/g-logo.png" 
            alt="Google" style="width:20px; height:20px; margin-right:10px;" />
            Entrar com Google
        </asp:LinkButton>

        <!-- Links secundários -->
        <a href="recuperar_password.aspx" class="link-secundario">
            Esqueceu a palavra-passe?
        </a>
        <a href="registo.aspx" class="link-secundario mt-1">
            Criar conta
        </a>

    </div>

</asp:Content>