<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CloudBlockStorage.aspx.cs" Inherits="OpenStackDotNet_Test.CloudBlockStorage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">
    Openstack.NET Cloud Block Storage
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ProjectName" runat="server">
    Openstack.NET Cloud Block Storage
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Body_Content" runat="server">
    <div class="row">
        <div class="span9">
            <h1>Instructions:</h1>
            <p>
                Please login with your Username and APIKey, to use <a href="http://www.rackspace.com/cloud/block-storage/" target="_blank">Cloud Block Storage</a>.
                <br />
                <br />
                Before using be sure to select the region (DFW, ORD, or SYD) you would like to perform operations on.
                <br />
                <br />
                <a href="http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/overview.html" target="_blank">For Cloud Block Storage API Documentation Click Here</a>
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
                <asp:DropDownList ID="CBS_ddl_Region" OnSelectedIndexChanged="CBS_ddl_Region_SelectChange" AutoPostBack="true" runat="server">
                    <asp:ListItem Text="DFW" Value="dfw"></asp:ListItem>
                    <asp:ListItem Text="ORD" Value="ord"></asp:ListItem>
                    <asp:ListItem Text="SYD" Value="syd"></asp:ListItem>
                    <asp:ListItem Text="IAD" Value="iad"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <br />
            <br />
            <asp:Label ID="CBS_lbl_Info" EnableViewState="false" runat="server"></asp:Label>
            <br />
            <asp:Label ID="CBS_lbl_Error" EnableViewState="false" ForeColor="Red" runat="server"></asp:Label>
            <br />
        </div>
        <div class="span4">
            Available Volumes:
            <br />
            <asp:DropDownList ID="CBS_ddl_ListVolumes" runat="server"></asp:DropDownList>
            <br />
            <asp:Button ID="CBS_btn_DeleteVolume" runat="server" CssClass="btn-primary" OnClick="CBS_btn_DeleteVolume_OnClick" Text="Delete Volume" />
        </div>
        <div class="span4">
            Available Snapshots:
            <br />
            <asp:DropDownList ID="CBS_ddl_ListSnapShots" runat="server"></asp:DropDownList>
            <br />
            <asp:Button ID="CBS_btn_DeleteSnapShot" runat="server" CssClass="btn-primary" OnClick="CBS_btn_DeleteSnapShot_Click" Text="Delete SnapShot" />
            <br />
        </div>
    </div>
    <br />
    <div class="row">
        <div class="span6">
            <h1>Create New Volume</h1>
            <br />
            <br />
            New Volume Name:
            <br />
            <asp:TextBox ID="CBS_txt_CreateVolumeName" runat="server"></asp:TextBox>
            <br />
            New Volume Description:
            <br />
            <asp:TextBox ID="CBS_txt_CreateVolumeDescription" runat="server"></asp:TextBox>
            <br />
            New Volume Display Name:
            <br />
            <asp:TextBox ID="CBS_txt_CreateVolumeDisplayname" runat="server"></asp:TextBox>
            <br />
            New Volume Disk Size in GigaBytes:
            <br />
            <asp:DropDownList ID="CBSCreateVolumeSizeDDL" runat="server">
                <asp:ListItem Text="100 GB" Value="100"></asp:ListItem>
                <asp:ListItem Text="200 GB" Value="200"></asp:ListItem>
                <asp:ListItem Text="300 GB" Value="300"></asp:ListItem>
                <asp:ListItem Text="400 GB" Value="400"></asp:ListItem>
                <asp:ListItem Text="500 GB" Value="500"></asp:ListItem>
                <asp:ListItem Text="600 GB" Value="600"></asp:ListItem>
                <asp:ListItem Text="700 GB" Value="700"></asp:ListItem>
                <asp:ListItem Text="800 GB" Value="800"></asp:ListItem>
                <asp:ListItem Text="900 GB" Value="900"></asp:ListItem>
            </asp:DropDownList>
            <br />
            New Volume Type:
            <br />
            <asp:DropDownList ID="CBS_ddl_CreateVolumeType" runat="server">
                <asp:ListItem Text="SATA" Value="sata"></asp:ListItem>
                <asp:ListItem Text="SSD" Value="ssd"></asp:ListItem>
            </asp:DropDownList>
            <asp:Button ID="CBS_btn_CreateVolume" runat="server" CssClass="btn-primary" OnClick="CBS_btn_CreateVolume_OnClick" Text="Create Volume" />
            <br />
        </div>
        <div class="span2"></div>
        <div class="span6">
            <h1>Create new SnapShot</h1>
            <br />
            <br />
            New Display Name:
            <br />
            <asp:TextBox ID="CBS_Txt_SnapshotDisplayName" runat="server"></asp:TextBox>
            <br />
            New Description:
            <br />
            <asp:TextBox ID="CBS_txt_SnapShotDescription" runat="server"></asp:TextBox>
            <asp:Button ID="CBS_btn_CreateSnapShot" runat="server" CssClass="btn-primary" OnClick="CBS_btn_CreateSnapShot_OnClick" Text="Create SnapShot" />
            <br />
        </div>
    </div>
    <div class="row">
        <div class="span12">
            <asp:GridView ID="CBS_grid_Results" runat="server"></asp:GridView>
            <br />
            <asp:GridView ID="CBS_grid_Results2" runat="server"></asp:GridView>
            <br />
        </div>
    </div>
</asp:Content>
