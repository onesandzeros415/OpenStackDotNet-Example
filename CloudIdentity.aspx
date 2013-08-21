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
        <div class="span3">
            <br />
            <asp:Label ID="CI_lbl_Info" EnableViewState="false" runat="server"></asp:Label>
            <br />
            <asp:Label ID="CI_lbl_Error" EnableViewState="false" ForeColor="Red" runat="server"></asp:Label>
            <br />
        </div>
        <div class="span3">
            Users:
            <br />
            <asp:DropDownList ID="CI_ddl_ListUsers" runat="server"></asp:DropDownList>
            <br />
            Account/DDI #:
            <asp:DropDownList ID="CI_ddl_ListTenants" runat="server"></asp:DropDownList>
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
        <div class="span3">
        </div>
    </div>
</asp:Content>
