<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CloudFiles.aspx.cs" Inherits="OpenStackDotNet_Test.CloudFiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">
    Openstack.NET Cloud Files
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ProjectName" runat="server">
    Openstack.NET Cloud Files
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Body_Content" runat="server">
    <div class="row">
        <div class="span9">
            <h1>Instructions:</h1>
            <p>
                Please login with your Username and APIKey, to use <a href="http://www.rackspace.com/cloud/files/" target="_blank">Cloud Files</a>.
                <br />
                <br />
                Before using be sure to select true or false to make use of ServiceNet as well as be sure to select the region (DFW, ORD, or SYD) you would like to perform operations on.
                <br />
                <br />
                When uploading a file it's temporarily stored in the temp directory so be sure to add your impersonation to your web.config so the file system can upload the file.
                <br />
                <br />
                <a href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/Overview-d1e70.html" target="_blank">For Cloud Files API Documentation Click Here</a>
                <br />
                <br />
            </p>
        </div>
        <div class="span3"></div>
        <div class="span3"></div>
    </div>

    <div>
        <div id="CFMainContent" class="row">
            <div class="span4">
                <div>
                    Please Select Region:
                    <br />
                    <asp:DropDownList ID="CF_ddl_Region" OnSelectedIndexChanged="CF_ddl_Region_SelectChange" AutoPostBack="true" runat="server">
                        <asp:ListItem Text="DFW" Value="dfw"></asp:ListItem>
                        <asp:ListItem Text="ORD" Value="ord"></asp:ListItem>
                        <asp:ListItem Text="SYD" Value="syd"></asp:ListItem>
                        <asp:ListItem Text="IAD" Value="iad"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div>
                    Use service net?:  
                <asp:CheckBox ID="CF_cb_SnetCheck" Text="true" runat="server" />
                    <br />
                    <br />
                </div>
                <div>
                    <br />
                    <asp:Label ID="CF_lbl_Info" EnableViewState="false" runat="server"></asp:Label>
                    <br />
                    <asp:Label ID="CF_lbl_Error" EnableViewState="false" ForeColor="Red" runat="server"></asp:Label>
                    <br />
                </div>
            </div>
            <div class="span4">
                Container To Upload To:
                <br />
                <asp:DropDownList ID="CF_ddl_ListContainer" runat="server" OnSelectedIndexChanged="CF_ddl_ListContainer_SelectChange" AutoPostBack="true"></asp:DropDownList>
                <br />
                <asp:Button ID="CF_btn_DeleteContainer" runat="server" CssClass="btn-primary" OnClick="CF_btn_DeleteContainer_OnClick" Text="Delete Container" />
                <br />
                <br />
                <asp:Button ID="CF_btn_EnableCDNOnContainer" runat="server" CssClass="btn-primary" OnClick="CF_btn_EnableCDNOnContainer_OnClick" Text="Enable CDN" />
                <br />
                <br />
                <asp:Button ID="CF_btn_DisableCDNOnContainer" runat="server" CssClass="btn-primary" OnClick="CF_btn_DisableCDNOnContainer_OnClick" Text="Disable CDN" />
                <br />
                <br />
                Container Contents:
                <br />
                <asp:DropDownList ID="CF_ddl_ListContainerContents" runat="server"></asp:DropDownList>
                <br />
                <asp:Button ID="CF_btn_DeleteContainerObject" runat="server" CssClass="btn-primary" OnClick="CF_btn_DeleteContainerObject_OnClick" Text="Delete Object" />
                <br />
                <br />
                <asp:Button ID="CF_btn_DeleteMultipleContainerObjects" runat="server" CssClass="btn-primary" OnClick="CF_btn_DeleteMultipleContainerObjects_Click" Text="Bulk Delete" />
                <br />
                <br />
                <asp:Button ID="CF_btn_GetObjectSaveToFileBrowserPrompt" runat="server" CssClass="btn-primary" OnClick="CF_btn_GetObjectSaveToFileBrowserPrompt_OnClick" Text="Download" />
            </div>
            <div class="span4">
                New Container Name:
            <br />
                <asp:TextBox ID="CF_txt_CreateContainer" runat="server"></asp:TextBox>
                <asp:Button ID="CF_btn_CreateContainer" runat="server" CssClass="btn-primary" OnClick="CF_btn_CreateContainer_OnClick" Text="Create Container" />
                <br />
                <br />
                <asp:FileUpload ID="CF_fu_FileUpload" multiple="true" runat="server" />
                <asp:Button ID="CF_btn_CloudFilesUpload" runat="server" CssClass="btn-primary" OnClick="CF_btn_CloudFilesUpload_OnClick" Text="Upload to Cloud Files" />
                <br />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="span6">
            <br />
            Account Details : 
            <br />
            <br />
            <asp:Label ID="CF_lbl_AccountDetails" EnableViewState="false" runat="server"></asp:Label>
            <br />
            Container Details : 
            <br />
            <br />
            <asp:Label ID="CF_lbl_ContainerDetails" EnableViewState="false" runat="server"></asp:Label>
            <br />
            <asp:GridView ID="CF_grid_Results" runat="server"></asp:GridView>
            <p>
                <asp:Label ID="CF_lbl_FileList" runat="server"></asp:Label>
            </p>
            <p>
                <asp:Label ID="CF_lbl_UploadStatus" runat="server"></asp:Label>
            </p>
            <p>
                <asp:Label ID="CF_lbl_FailedStatus" runat="server"></asp:Label>
            </p>
            <br />
        </div>
        <div class="span6">
            <br />
            Container Object Details : 
            <br />
            <br />
            <asp:Label ID="CF_lbl_ContainerObjectDetails" EnableViewState="false" runat="server"></asp:Label>
            <br />
            <br />
            CDN Info : 
            <br />
            <br />
            <asp:Label ID="CF_lbl_ContainerCDNDetails" EnableViewState="false" runat="server"></asp:Label>
            <br />
            <br />
            <asp:Label ID="CF_lbl_EnableDisableCDN" EnableViewState="false" runat="server"></asp:Label>
            <br />
            <br />
        </div>
    </div>
    <script>
        $('#btUpload').click(function () { if (fileUpload.value.length == 0) { alert('No files selected.'); return false; } });
    </script>
</asp:Content>
