<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPublico.Master" AutoEventWireup="true" CodeBehind="registo.aspx.cs" Inherits="RaizesDigitais.registo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder3" runat="server">

    <div class="card-auth">

        <h4>Quinta da Azenha</h4>
        <p class="subtitulo">Criar conta de acesso</p>

        <!-- Nome de utilizador -->
        <div class="mb-3">
            <label class="form-label">Utilizador</label>
            <asp:TextBox ID="tb_utilizador" runat="server"
                CssClass="form-control" placeholder="nome de utilizador" />
            <asp:RequiredFieldValidator ID="rfv_utilizador" runat="server"
                ControlToValidate="tb_utilizador"
                ErrorMessage="Introduza o nome de utilizador."
                CssClass="lbl-erro d-block mt-1" Display="Dynamic" 
                ValidationGroup="vg_registo"
                EnableClientScript="false" />
        </div>

        <!-- Email -->
        <div class="mb-3">
            <label class="form-label">Email</label>
            <asp:TextBox ID="tb_email" runat="server"
                CssClass="form-control" placeholder="o_seu@email.com"
                TextMode="Email" />
            <asp:RequiredFieldValidator ID="rfv_email" runat="server"
                ControlToValidate="tb_email"
                ErrorMessage="Introduza o email."
                CssClass="lbl-erro d-block mt-1" Display="Dynamic"
                ValidationGroup="vg_registo"
                EnableClientScript="false" />
        </div>

        <!-- Password -->
        <div class="mb-3">
            <label class="form-label">Palavra-passe</label>
            <asp:TextBox ID="tb_pw" runat="server"
                CssClass="form-control" placeholder="••••••••"
                TextMode="Password"
                ValidationGroup="vg_registo" />
            <asp:RequiredFieldValidator ID="rfv_pw" runat="server"
                ControlToValidate="tb_pw"
                ErrorMessage="Introduza a palavra-passe."
                CssClass="lbl-erro d-block mt-1" Display="Dynamic"
                ValidationGroup="vg_registo"
                EnableClientScript="false" />
        </div>

        <!-- Confirmar password -->
        <div class="mb-3">
            <label class="form-label">Confirmar palavra-passe</label>
            <asp:TextBox ID="tb_pw_confirmar" runat="server"
                CssClass="form-control" placeholder="••••••••"
                TextMode="Password" />
            <asp:RequiredFieldValidator ID="rfv_pw_confirmar" runat="server"
                ControlToValidate="tb_pw_confirmar"
                ErrorMessage="Confirme a palavra-passe."
                CssClass="lbl-erro d-block mt-1" Display="Dynamic"
                ValidationGroup="vg_registo"
                EnableClientScript="false" />
            <asp:CompareValidator ID="cv_pw" runat="server"
                ControlToValidate="tb_pw_confirmar"
                ControlToCompare="tb_pw"
                ErrorMessage="As palavras-passe não coincidem."
                CssClass="lbl-erro d-block mt-1" Display="Dynamic"
                ValidationGroup="vg_registo"
                EnableClientScript="false" />
        </div>

        <!-- Mensagem feedback -->
        <asp:Label ID="lbl_erro" runat="server" CssClass="lbl-erro d-block mb-3" />
        <asp:Label ID="lbl_sucesso" runat="server" CssClass="lbl-sucesso d-block mb-3" />

        <!-- Botão registar -->
        <asp:Button ID="btn_registar" runat="server"
            Text="Criar conta"
            CssClass="btn-primario btn mb-2"
            OnClick="btn_registar_Click"
            ValidationGroup="vg_registo"
            EnableClientScript="false" />

        <!-- Link para login -->
        <a href="login.aspx" class="link-secundario mt-1">
            Já tem conta? Iniciar sessão
        </a>

    </div>

</asp:Content>