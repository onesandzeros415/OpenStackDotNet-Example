<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="True" CodeBehind="Default.aspx.cs" Inherits="OpenStackDotNet_Test.Default" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">
    Rackspace/Openstack Web Console
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ProjectName" runat="server">
    Rackspace/Openstack Web Console
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Body_Content" runat="server">
    <div class="row">
        <div class="span3"></div>
        <div class="span6">
            <h1>Instructions:</h1>
            <p>
                This is a Web Forms example of how to use the OpenStack.NET bindings
                <br />
                <br />
                Please be sure to login with your Rackspace API credentials here first in order to perform any operations. 
                <br />
                <br />
                Click one of the urls above to view how this demo works or open up Visual studio or a text editor of your choice and start review the code. I hope my example helps!
                <br />
                <br />
                When uploading a file it's temporarily stored in the temp directory so be sure to add your impersonation to your web.config so the file system can upload the file.
                <br />
                <br />
                This web console uses the <a href="https://github.com/rackspace/openstack.net" target="_blank">OpenStack .net</a> library which makes following Rackspace API's: 
                <br />
                <a href="http://docs.rackspace.com/files/api/v1/cf-devguide/content/Overview-d1e70.html" target="_blank">Cloud Files</a>, <a href="http://docs.rackspace.com/cbs/api/v1.0/cbs-devguide/content/overview.html" target="_blank">Cloud Block Storage</a>, <a href="http://docs.rackspace.com/servers/api/v2/cs-devguide/content/ch_preface.html" target="_blank">Cloud Servers</a>, <a href="http://docs.rackspace.com/auth/api/v2.0/auth-client-devguide/content/Overview-d1e65.html" target="_blank">Cloud Identity</a>
                <br />
                <br />
                For more information on the OpenStack .net library visit the <a href="http://openstacknetsdk.org/" target="_blank">wiki here</a>
                <br />
                <br />
                Rackspace Documentation can be found <a href="http://docs.rackspace.com/" target="_blank">Here!</a>
            </p>
            <br />
        </div>
        <div class="span3"></div>
    </div>
    <div class="row">
        <div class="span4"></div>
        <div class="span4">
            
            <asp:Label ID="LblInfo" EnableViewState="false" runat="server"></asp:Label>
            <br />
            <asp:Label ID="LblDefaultRegionPrefab" runat="server" Text="Default Region is:<br />" />
            <br />
            <asp:Label ID="Lbl_DefaultRegion" runat="server" />
            <br />
            <asp:Label ID="Error" EnableViewState="false" ForeColor="Red" runat="server"></asp:Label>
            <br />
        </div>
        <div class="span4"></div>
    </div>
</asp:Content>
