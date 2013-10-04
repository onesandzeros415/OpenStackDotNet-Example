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
                <br />
                <asp:Label runat="server" ID="lblinfo"></asp:Label>

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
                        </div>
                        <div>
                            <asp:Label ID="CF_lbl_Info" EnableViewState="false" runat="server"></asp:Label>
                            <br />
                            <br />
                            <asp:Label ID="CF_lbl_AccountContainerInfo" EnableViewState="false" runat="server"></asp:Label>
                            <br />
                            <asp:Label ID="CF_lbl_ContainerObjectInfo" EnableViewState="false" runat="server"></asp:Label>
                            <br />
                            <asp:Label ID="CF_lbl_Error" EnableViewState="false" ForeColor="Red" runat="server"></asp:Label>
                            <br />
                            <asp:Label ID="CF_lbl_TimeClock" EnableViewState="false" runat="server"></asp:Label>
                            <br />
                            <asp:Label ID="CF_lbl_EnableDisableCDN" EnableViewState="false" runat="server"></asp:Label>
                            <br />
                        </div>
                    </div>
                    <div class="span4">
                        Container To Upload To:
                        <br />
                        <asp:DropDownList ID="CF_ddl_ListContainer" runat="server" OnSelectedIndexChanged="CF_ddl_ListContainer_SelectChange" AutoPostBack="true"></asp:DropDownList>
                        <br />
                        <asp:Button ID="CF_btn_DeleteContainer" runat="server" CssClass="btn-primary" OnClick="CF_btn_DeleteContainer_OnClick" OnClientClick="javascript:return confirmAction(this.name);" Text="Delete Container" />
                        <br />
                        Remove all objects in container?
                        <asp:CheckBox ID="CF_chk_deleteallobjects" runat="server" />
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
                        <asp:DropDownList ID="CF_ddl_ListContainerContents" OnSelectedIndexChanged="CF_ddl_ListContainerContents_SelectChange" AutoPostBack="true" runat="server"></asp:DropDownList>
                        <br />
                        <asp:Label ID="lblClicked" runat="server" />
                        <br />
                        <asp:Button ID="CF_btn_DeleteContainerObject" runat="server" CssClass="btn-primary" OnClick="CF_btn_DeleteContainerObject_OnClick" OnClientClick="javascript:return confirmAction(this.name);" Text="Delete Object" />
                        <br />
                        <br />
                        <asp:Button ID="CF_btn_DeleteMultipleContainerObjects" runat="server" CssClass="btn-primary" OnClick="CF_btn_BulkDelete_Click" OnClientClick="javascript:return confirmAction(this.name);" Text="Bulk Delete" />
                        <br />
                        <br />
                        <asp:Button ID="CF_btn_GetObjectSaveToFileBrowserPrompt" runat="server" CssClass="btn-primary" OnClick="CF_btn_GetObjectSaveToFileBrowserPrompt_OnClick" Text="Download Object" />
                        <br />
                        <br />
                        Container to Copy To: 
                        <br />
                        <asp:DropDownList ID="CF_ddl_CopyListContainer" runat="server"></asp:DropDownList>
                        <br />
                        <asp:Button ID="CF_btn_CopyOBJ" runat="server" CssClass="btn-primary" OnClick="CF_btn_CopyObj_OnClick" OnClientClick="javascript:return confirmAction(this.name);" Text="Copy Object" />
                        <br />
                        <br />
                        <asp:Button ID="CF_btn_MoveOBJ" runat="server" CssClass="btn-primary" OnClick="CF_btn_MoveObj_OnClick" OnClientClick="javascript:return confirmAction(this.name);" Text="Move Object" />
                    </div>
                    <div class="span4">
                        New Container Name:
                        <br />
                        <asp:TextBox ID="CF_txt_CreateContainer" runat="server"></asp:TextBox>
                        <asp:Button ID="CF_btn_CreateContainer" runat="server" CssClass="btn-primary" OnClick="CF_btn_CreateContainer_OnClick" OnClientClick="javascript:return confirmAction(this.name);" Text="Create Container" />
                        <br />
                        <br />
                        <asp:FileUpload ID="CF_fu_FileUpload" multiple="true" runat="server" />
                        <asp:Button ID="CF_btn_CloudFilesUpload" runat="server" CssClass="btn-primary" OnClick="CF_btn_CloudFilesUpload_OnClick" OnClientClick="javascript:return confirmAction(this.name);" Text="Upload to Cloud Files" />
                        <br />
                    </div>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="CF_btn_DeleteContainer" />
                <asp:AsyncPostBackTrigger ControlID="CF_btn_EnableCDNOnContainer" />
                <asp:AsyncPostBackTrigger ControlID="CF_btn_DisableCDNOnContainer" />
                <asp:AsyncPostBackTrigger ControlID="CF_btn_DeleteContainerObject" />
                <asp:AsyncPostBackTrigger ControlID="CF_btn_DeleteMultipleContainerObjects" />
                <asp:AsyncPostBackTrigger ControlID="CF_btn_GetObjectSaveToFileBrowserPrompt" />
                <asp:AsyncPostBackTrigger ControlID="CF_btn_CopyOBJ" />
                <asp:AsyncPostBackTrigger ControlID="CF_btn_MoveOBJ" />
                <asp:AsyncPostBackTrigger ControlID="CF_btn_CreateContainer" />
                <asp:AsyncPostBackTrigger ControlID="CF_ddl_Region" />
                <asp:AsyncPostBackTrigger ControlID="CF_ddl_ListContainer" />
                <asp:AsyncPostBackTrigger ControlID="CF_ddl_ListContainerContents" />
                <asp:PostBackTrigger ControlID="CF_btn_CloudFilesUpload" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
    <div class="row">
        <div class="span6">
            CDN Info : 
            <br />
            <br />
            <asp:Label ID="CF_lbl_ContainerCDNDetails" EnableViewState="false" runat="server"></asp:Label>
            <br />
            <br />
            <asp:GridView ID="CF_grid_Results" BorderWidth="0" runat="server"></asp:GridView>
        </div>
        <div class="span6">
            <p>
                <asp:Label ID="CF_lbl_FileList" runat="server"></asp:Label>
            </p>
            <p>
                <asp:Label ID="CF_lbl_UploadStatus" runat="server"></asp:Label>
            </p>
            <p>
                <asp:Label ID="CF_lbl_FailedStatus" runat="server"></asp:Label>
            </p>
        </div>
    </div>
    <script>
        $('#btUpload').click(function () { if (fileUpload.value.length == 0) { alert('No files selected.'); return false; } });
    </script>
    <div id="dialogContent">
        <p>Click ok to accept</p>
    </div>
</asp:Content>
