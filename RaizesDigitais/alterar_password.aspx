<%@ Page Title="" Language="C#" MasterPageFile="~/MasterBackoffice.Master" AutoEventWireup="true" CodeBehind="alterar_password.aspx.cs" Inherits="RaizesDigitais.alterar_password" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder3" runat="server">

    <div class="content-header">
        <div class="container-fluid">
            <div class="row mb-2">
                <div class="col-sm-6">
                    <h1 class="m-0">Alterar Password</h1>
                </div>
                <div class="col-sm-6">
                    <ol class="breadcrumb float-sm-right">
                        <li class="breadcrumb-item">
                            <a href="/Backoffice/dashboard.aspx">Início</a>
                        </li>
                        <li class="breadcrumb-item active">Alterar Password</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>

    <div class="content">
        <div class="container-fluid">
            <div class="row justify-content-center">
                <div class="col-md-6">
                    <div class="card card-primary">
                        <div class="card-header">
                            <h3 class="card-title">Alterar palavra-passe</h3>
                        </div>
                        <div class="card-body">

                            <asp:Label ID="lbl_erro" runat="server" 
                                CssClass="alert alert-danger d-block mb-3" Visible="false" />
                            <asp:Label ID="lbl_sucesso" runat="server" 
                                CssClass="alert alert-success d-block mb-3" Visible="false" />

                            <div class="form-group">
                                <label>Password actual</label>
                                <asp:TextBox ID="tb_pw_actual" runat="server"
                                    CssClass="form-control" TextMode="Password" />
                                <asp:RequiredFieldValidator ID="rfv_pw_actual" runat="server"
                                    ControlToValidate="tb_pw_actual"
                                    ErrorMessage="Introduza a password actual."
                                    CssClass="text-danger" Display="Dynamic"
                                    ValidationGroup="vg_alterar"
                                    EnableClientScript="false" />
                            </div>

                            <div class="form-group">
                                <label>Nova password</label>
                                <asp:TextBox ID="tb_pw_nova" runat="server"
                                    CssClass="form-control" TextMode="Password" />
                                <asp:RequiredFieldValidator ID="rfv_pw_nova" runat="server"
                                    ControlToValidate="tb_pw_nova"
                                    ErrorMessage="Introduza a nova password."
                                    CssClass="text-danger" Display="Dynamic"
                                    ValidationGroup="vg_alterar"
                                    EnableClientScript="false" />
                            </div>

                            <div class="form-group">
                                <label>Confirmar nova password</label>
                                <asp:TextBox ID="tb_pw_confirmar" runat="server"
                                    CssClass="form-control" TextMode="Password" />
                                <asp:RequiredFieldValidator ID="rfv_pw_confirmar" runat="server"
                                    ControlToValidate="tb_pw_confirmar"
                                    ErrorMessage="Confirme a nova password."
                                    CssClass="text-danger" Display="Dynamic"
                                    ValidationGroup="vg_alterar"
                                    EnableClientScript="false" />
                                <asp:CompareValidator ID="cv_pw" runat="server"
                                    ControlToValidate="tb_pw_confirmar"
                                    ControlToCompare="tb_pw_nova"
                                    ErrorMessage="As passwords não coincidem."
                                    CssClass="text-danger" Display="Dynamic"
                                    ValidationGroup="vg_alterar"
                                    EnableClientScript="false" />
                            </div>

                            <asp:Button ID="btn_alterar" runat="server"
                                Text="Alterar password"
                                CssClass="btn btn-primary"
                                OnClick="btn_alterar_Click"
                                ValidationGroup="vg_alterar" />

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

