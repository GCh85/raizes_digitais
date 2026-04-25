<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPublico.Master" AutoEventWireup="true" CodeBehind="verificar_2fa.aspx.cs" Inherits="RaizesDigitais.verificar_2fa" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder3" runat="server">

    <div class="card-auth">
        <h4>Quinta da Azenha</h4>
        <p class="subtitulo">Verificação em dois passos</p>

        <p style="color:#555; font-size:14px; margin-bottom:24px;">
            Introduza o código de 6 dígitos enviado para o seu email.
        </p>

        <div class="mb-3">
            <label class="form-label">Código de verificação</label>
            <asp:TextBox ID="tb_codigo" runat="server"
                CssClass="form-control" placeholder="000000"
                MaxLength="6" />
            <asp:RequiredFieldValidator ID="rfv_codigo" runat="server"
                ControlToValidate="tb_codigo"
                ErrorMessage="Introduza o código."
                CssClass="lbl-erro d-block mt-1"
                Display="Dynamic"
                ValidationGroup="vg_2fa"
                EnableClientScript="false" />
        </div>

        <asp:Label ID="lbl_erro" runat="server" CssClass="lbl-erro d-block mb-3" />

        <asp:Button ID="btn_verificar" runat="server"
            Text="Verificar"
            CssClass="btn-primario btn mb-2"
            OnClick="btn_verificar_Click"
            ValidationGroup="vg_2fa" />

        <a href="login.aspx" class="link-secundario mt-1">
            Voltar ao login
        </a>
    </div>

</asp:Content>
