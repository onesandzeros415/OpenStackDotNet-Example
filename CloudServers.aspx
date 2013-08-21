<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CloudServers.aspx.cs" Inherits="OpenStackDotNet_Test.CloudServers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">
    Openstack.NET Cloud Servers
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ProjectName" runat="server">
    Openstack.NET Cloud Servers
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Body_Content" runat="server">
    <div class="row">
        <div class="span9">
            <h1>Instructions:</h1>
            <p>
                Please login with your Username and APIKey, to use <a href="http://www.rackspace.com/cloud/servers/overview_b/" target="_blank">Cloud Servers</a>.
                <br />
                <br />
                Before using be sure to select the region (DFW, ORD, or SYD) you would like to perform operations on.
                <br />
                <br />
                <a href="http://docs.rackspace.com/servers/api/v2/cs-devguide/content/ch_preface.html" target="_blank">For Cloud Servers API Documentation Click Here</a>
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
                <asp:DropDownList ID="CS_ddl_Region" OnSelectedIndexChanged="CS_ddl_Region_SelectChange" AutoPostBack="true" runat="server">
                    <asp:ListItem Text="DFW" Value="dfw"></asp:ListItem>
                    <asp:ListItem Text="ORD" Value="ord"></asp:ListItem>
                    <asp:ListItem Text="SYD" Value="syd"></asp:ListItem>
                    <asp:ListItem Text="IAD" Value="iad"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <br />
            <br />
            <asp:Label ID="CS_lbl_Error" ForeColor="Red" runat="server"></asp:Label>
            <br />
            <asp:Label ID="CS_lbl_Info" runat="server"></asp:Label>
            <br />
            <asp:Label ID="CS_lbl_Passwd" runat="server"></asp:Label>
            <br />
        </div>
        <div class="span4">
            Available Cloud Servers :
            <br />
            <asp:DropDownList ID="CS_ddl_ListServers" runat="server" OnSelectedIndexChanged="CS_ddl_ListServers_SelectChange" AutoPostBack="true"></asp:DropDownList>
            <br />
            Reboot Cloud Server :
            <br />
            <asp:DropDownList ID="CSRebootServerDDL" runat="server">
                <asp:ListItem Value="soft">Soft</asp:ListItem>
                <asp:ListItem Value="hard">Hard</asp:ListItem>
            </asp:DropDownList>
            <br />
            <asp:Button ID="CS_btn_RebootCloudServer" runat="server" CssClass="btn-primary" OnClick="CS_btn_RebootServer_OnClick" Text="Reboot CloudServer" />
            <br />
            <br />
            <asp:Button ID="CS_btn_DeleteCloudServer" runat="server" CssClass="btn-primary" OnClick="CS_btn_DeleteServer_OnClick" Text="Delete CloudServer" />
            <br />
            <br />
            <asp:TextBox ID="CS_txt_PasswdReset" runat="server"></asp:TextBox>
            <br />
            <asp:Button ID="CS_btn_PasswdReset" runat="server" CssClass="btn-primary" OnClick="CS_btn_PasswdReset_OnClick" Text="Reset Passwd" />
            <br />
            <br />
            Attached Volumes : 
            <br />
            <asp:DropDownList ID="CS_ddl_CBSAttachedVolume" runat="server">
            </asp:DropDownList>
            <br />
            <br />
            Available Volumes : 
            <br />
            <asp:DropDownList ID="CS_ddl_CBSAvailableVolume" runat="server">
            </asp:DropDownList>
            <br />
            <asp:Button ID="CS_btn_AttachVolume" runat="server" CssClass="btn-primary" OnClick="CS_btn_AttachVolume_OnClick" Text="Attach Volume" />
            <br />
            <br />
            <asp:Button ID="CS_btn_DetachVolume" runat="server" CssClass="btn-primary" OnClick="CS_btn_DetachVolume_OnClick" Text="Detatch Volume" />
            <br />
        </div>
        <div class="span4">
            Cloud Server Info :
            <br />
            <asp:Label ID="CS_lbl_CSInfo" runat="server"></asp:Label>
            <br />
            <asp:Label ID="CS_lbl_CSInfo2" runat="server"></asp:Label>
            <br />
            <asp:GridView ID="CS_grid_ServerInfo" runat="server"></asp:GridView>
            <br />
            <asp:GridView ID="CS_grid_ServerInfo2" runat="server"></asp:GridView>
            <br />
            <asp:GridView ID="CS_grid_ServerInfo3" runat="server"></asp:GridView>
        </div>
    </div>
    <div class="row">
        <div class="span4">
            <h1>Create CloudServer</h1>
            <br />
            Server Name :
            <br />
            <asp:TextBox ID="CS_txt_Name" runat="server"></asp:TextBox>
            <br />
            List of available Flavors :
            <br />
            <asp:DropDownList ID="CS_ddl_AvailableFlavors" runat="server"></asp:DropDownList>
            <br />
            List of available Images :
            <br />
            <asp:DropDownList ID="CS_ddl_AvailableImage" runat="server"></asp:DropDownList>
            <br />
            How many :
            <br />
            <asp:TextBox ID="CS_txt_HowMany" runat="server"></asp:TextBox>
            <br />
            <asp:Button ID="CS_btn_CreateCloudServer" runat="server" CssClass="btn-primary" OnClick="CS_btn_CreateServer_OnClick" Text="Create Cloud Server" />
            <br />
        </div>
    </div>
    <br />
    <br />
    <asp:GridView ID="CS_grid_Results" runat="server"></asp:GridView>
</asp:Content>
