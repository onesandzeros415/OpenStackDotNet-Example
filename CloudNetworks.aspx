<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CloudNetworks.aspx.cs" Inherits="OpenStackDotNet_Test.CloudNetworks" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">
    Openstack.NET Cloud Networks
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ProjectName" runat="server">
    Openstack.NET Cloud Networks
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Body_Content" runat="server">
    <div class="row">
        <div class="span9">
            <h1>Instructions:</h1>
            <p>
                Please login with your Username and APIKey, to use <a href="http://www.rackspace.com/blog/cloud-networks-the-next-chapter-in-the-open-cloud/" target="_blank">Cloud Networks</a>.
                <br />
                <br />
                Before using be sure to select the region (DFW, ORD, or SYD) you would like to perform operations on.
                <br />
                <br />
                <a href="http://docs.rackspace.com/networks/api/v2/cn-devguide/content/ch_preface.html" target="_blank">For Cloud Networks API Documentation Click Here</a>
                <br />
                <br />
            </p>
        </div>
        <div class="span3"></div>
        <div class="span3"></div>
    </div>
    <div class="row">
        <div class="span4">
            <div>
                Please Select Region:
                <br />
                <asp:DropDownList ID="CN_ddl_Region" OnSelectedIndexChanged="CN_ddl_Region_SelectChange" AutoPostBack="true" runat="server">
                    <asp:ListItem Text="DFW" Value="dfw"></asp:ListItem>
                    <asp:ListItem Text="ORD" Value="ord"></asp:ListItem>
                    <asp:ListItem Text="SYD" Value="syd"></asp:ListItem>
                    <asp:ListItem Text="IAD" Value="iad"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <br />
            <br />
            <asp:Label ID="CN_lbl_Error" ForeColor="Red" runat="server"></asp:Label>
            <br />
            <asp:Label ID="CN_lbl_Info" runat="server"></asp:Label>
            <br />
        </div>
        <div class="span4">
            Create A Network:
            <br />
            <asp:TextBox ID="CN_txt_CreateNetworkName" Text="net-sdk-test-network" runat="server"></asp:TextBox>
            <asp:TextBox ID="CN_txt_CIDRRange" Text="192.0.2.0/24" runat="server"></asp:TextBox>
            <asp:Button ID="CN_btn_CreateNetwork" runat="server" CssClass="btn-primary" OnClick="CN_btn_CreateNetwork_OnClick" Text="Create Network" />
        </div>
        <div class="span4">
            List of Networks:
            <br />
            <asp:DropDownList ID="CN_ddl_ListNetworks" OnSelectedIndexChanged="CN_ddl_ListNetworks_SelectChange" AutoPostBack="true" runat="server"></asp:DropDownList>
            <asp:Button ID="CN_btn_DeleteNetwork" runat="server" CssClass="btn-primary" OnClick="CN_btn_DeleteNetwork_OnClick" Text="Delete Network" />
            <br />
            <br />
            <asp:Label ID="CN_lbl_networkinfo" runat="server"></asp:Label>
            <br />
            <br />
            <asp:GridView ID="CN_grid_Results" runat="server"></asp:GridView>
            <br />
        </div>
    </div>
</asp:Content>
