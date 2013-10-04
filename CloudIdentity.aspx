<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CloudIdentity.aspx.cs" Inherits="OpenStackDotNet_Test.CloudIdentity" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">
    Openstack.NET Cloud Identity
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ProjectName" runat="server">
    Openstack.NET Cloud Identity
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Body_Content" runat="server">
    <div class="row">
        <div class="span9">
            <h1>Instructions:</h1>
            <p>
                Please login with your Username and APIKey, to use <a href="http://www.rackspace.com/cloud/load-balancing/" target="_blank">Cloud Load Balancers</a>.
                <br />
                <br />
                Before using be sure to select the region (DFW, ORD, or SYD) you would like to perform operations on.
                <br />
                <br />
                <a href="http://docs.rackspace.com/auth/api/v2.0/auth-client-devguide/content/Overview-d1e65.html" target="_blank">For Cloud Identity API Documentation Click Here</a>
                <br />
                <br />
            </p>
        </div>
        <div class="span3"></div>
        <div class="span3"></div>
    </div>
    <div class="row">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                    <ProgressTemplate>
                        <center>
                        <img src="assets/img/loading.gif" />
                        <br />
                        <font color=red>Processing Data...</font>
                    </center>
                    </ProgressTemplate>
                </asp:UpdateProgress>
                <div class="span4">
                    <br />
                    <asp:Label ID="CI_lbl_Info" EnableViewState="false" runat="server"></asp:Label>
                    <br />
                    <asp:Label ID="CI_lbl_Error" EnableViewState="false" ForeColor="Red" runat="server"></asp:Label>
                    <br />
                </div>
                <div class="span4">
                    <h1>Existing Users</h1>
                    <br />
                    Users :
                    <br />
                    <asp:DropDownList ID="CI_ddl_ListUsers" OnSelectedIndexChanged="CI_ddl_ListUsers_SelectChange" AutoPostBack="true" runat="server"></asp:DropDownList>
                    <br />
                    <asp:Button ID="CI_btn_DeleteUser" CssClass="btn-primary" runat="server" OnClick="CI_btn_DeleteUser_OnClick" OnClientClick="javascript:return confirmAction(this.name);" Text="Delete User" />
                    <br />
                    <br />
                    Roles Assigned To User : 
                    <asp:DropDownList ID="CI_ddl_ListRoleUserIn" runat="server"></asp:DropDownList>
                    <br />
                    <asp:Button ID="CI_btn_RemoveRoleFromUser" CssClass="btn-primary" runat="server" OnClick="CI_btn_RemoveRoleFromUser_OnClick" OnClientClick="javascript:return confirmAction(this.name);" Text="Remove Role From User" />
                    <br />
                    <br />
                    Available Roles :
                    <asp:DropDownList ID="CI_ddl_ListRoles" runat="server"></asp:DropDownList>
                    <br />
                    <asp:Button ID="CI_btn_AddRoleToUser" CssClass="btn-primary" runat="server" OnClick="CI_btn_AddRoleToUser_OnClick" OnClientClick="javascript:return confirmAction(this.name);" Text="Add Role To User" />
                    <br />
                    <br />
                    List Users:
                    <br />
                    <asp:GridView ID="CI_grid_Results1" runat="server"></asp:GridView>
                    <br />
                    List Roles:
                    <br />
                    <asp:GridView ID="CI_grid_Results2" runat="server"></asp:GridView>
                    <br />
                    List Tenants:
                    <br />
                    <asp:GridView ID="CI_grid_Results3" runat="server"></asp:GridView>
                    <br />
                </div>
                <div class="span4">
                    <h1>Create New User</h1>
                    <br />
                    Username : 
                    <br />
                    <asp:TextBox ID="CI_txt_NewUser" runat="server"></asp:TextBox>
                    <br />
                    Password : 
                    <br />
                    <asp:TextBox ID="CI_txt_Passwd" runat="server"></asp:TextBox>
                    <br />
                    Email : 
                    <br />
                    <asp:TextBox ID="CI_txt_Email" runat="server"></asp:TextBox>
                    <br />
                    <asp:Button ID="CI_btn_NewUser" CssClass="btn-primary" runat="server" OnClick="CI_btn_NewUser_OnClick" OnClientClick="javascript:return confirmAction(this.name);" Text="Create User" />
                    <br />
                    <br />
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="CI_ddl_ListUsers" />
                <asp:AsyncPostBackTrigger ControlID="CI_btn_DeleteUser" />
                <asp:AsyncPostBackTrigger ControlID="CI_btn_RemoveRoleFromUser" />
                <asp:AsyncPostBackTrigger ControlID="CI_btn_AddRoleToUser" />
                <asp:AsyncPostBackTrigger ControlID="CI_btn_NewUser" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
    <div id="dialogContent">
        <p>Click ok to accept</p>
    </div>
</asp:Content>
