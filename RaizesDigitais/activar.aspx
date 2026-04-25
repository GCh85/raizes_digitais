<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPublico.Master" AutoEventWireup="true" CodeBehind="activar.aspx.cs" Inherits="RaizesDigitais.activar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder3" runat="server">

    <div class="card-auth">
        <h4>Quinta da Azenha</h4>
        <p class="subtitulo">Activação de conta</p>

        <asp:Label ID="lbl_mensagem" runat="server" CssClass="d-block mb-3" />

        <asp:HyperLink ID="lnk_login" runat="server"
            NavigateUrl="~/login.aspx"
            CssClass="btn-primario btn mb-2"
            Visible="false">
            Iniciar sessão
        </asp:HyperLink>
    </div>

</asp:Content>
