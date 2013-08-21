using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions.Response;
using net.openstack.Providers.Rackspace;
using net.openstack.Providers.Rackspace.Exceptions;
using net.openstack.Providers.Rackspace.Objects;

namespace OpenStackDotNet_Test
{
    public partial class CloudServers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string region = CS_ddl_Region.SelectedItem.ToString();
            string cslistserverid = CS_ddl_ListServers.SelectedValue.ToString();
            string cslistserveridsession = (string)(Session["CSListServersID"]);
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            HttpContext.Current.Session["CSListServersName"] = HttpUtility.HtmlEncode(CS_ddl_ListServers.SelectedItem);
            HttpContext.Current.Session["CSListServersID"] = HttpUtility.HtmlEncode(CS_ddl_ListServers.SelectedValue);
            HttpContext.Current.Session["CSListServersRegion"] = CS_ddl_Region.Text;

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudServersProvider CloudServersProvider = new net.openstack.Providers.Rackspace.CloudServersProvider();

            try
            {
                if (IsPostBack)
                {
                    HttpContext.Current.Session["PostBackListServerDDL"] = HttpUtility.HtmlEncode(CS_ddl_ListServers.Text);

                    ClientScript.RegisterClientScriptBlock(GetType(), "IsPostBack", "var isPostBack = true;", true);
                }
                else
                {
                    if (string.IsNullOrEmpty((string)(Session["CloudIdentityUserName"])) & string.IsNullOrEmpty((string)(Session["CloudIdentityApiKey"])))
                    {
                        CS_lbl_Error.Text = "Before continuing please login and enter Cloud Username and API Key.";
                    }
                    else if (string.IsNullOrEmpty((string)(Session["CloudIdentityUserName"])))
                    {
                        CS_lbl_Error.Text = "Before continuing please login and please enter Cloud Username.";
                    }
                    else if (string.IsNullOrEmpty((string)(Session["CloudIdentityApiKey"])))
                    {
                        CS_lbl_Error.Text = "Before continuing please login and please enter API Key.";
                    }
                    else
                    {
                        CS_m_ListImages();
                        CS_m_ListFlavors();

                        if (string.IsNullOrEmpty((string)(Session["CSListServersID"])))
                        {
                            CS_m_ListServerInfo(region);
                            CS_m_GetCloudServerDetails(cslistserverid, region);
                        }
                        else
                        {
                            CS_m_ListServerInfo(region);
                            CS_m_GetCloudServerDetails(cslistserveridsession, region);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CS_ddl_Region_SelectChange(object sender, EventArgs e)
        {
            string region = CS_ddl_Region.SelectedItem.ToString();
            string cslistserverid = CS_ddl_ListServers.SelectedValue.ToString();
            string cslistserveridsession = (string)(Session["CSListServersID"]);
            string cslistserverregion = (string)(Session["CSListServersRegion"]);

            try
            {
                CS_m_ClearGrid();

                if (string.IsNullOrEmpty((string)(Session["CSListServersID"])))
                {
                    CS_m_ListServerInfo(region);
                    CS_m_GetCloudServerDetails(cslistserverid, region);
                }
                else
                {
                    CS_m_ClearSessions();
                    CS_m_ListServerInfo(region);
                    CS_m_GetCloudServerDetails(cslistserveridsession, region);
                }
            }
            catch (Exception ex)
            {
                CS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CS_ddl_ListServers_SelectChange(object sender, EventArgs e)
        {
            string region = CS_ddl_Region.SelectedItem.ToString();
            string cslistserverid = CS_ddl_ListServers.SelectedValue;

            try
            {
                CS_m_ListServerInfo(region);
                CS_m_GetCloudServerDetails(cslistserverid, region);
            }
            catch (Exception ex)
            {
                CS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CS_btn_RebootServer_OnClick(object sender, EventArgs e)
        {
            string softReboot = "soft";
            string hardReboot = "hard";

            string region = CS_ddl_Region.SelectedItem.ToString();
            string cslistserverid = CS_ddl_ListServers.SelectedValue;

            try
            {
                if (region != null & CSRebootServerDDL.Text == "soft")
                {
                    CS_m_RebootServer(region, softReboot);
                    CS_m_GetCloudServerDetails(cslistserverid, region);
                    CS_m_MsgRebootServerSuccess();
                }
                else if (region != null & CSRebootServerDDL.Text == "hard")
                {
                    CS_m_RebootServer(region, hardReboot);
                    CS_m_GetCloudServerDetails(cslistserverid, region);
                    CS_m_MsgRebootServerSuccess();
                }
            }
            catch (Exception ex)
            {
                CS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CS_btn_CreateServer_OnClick(object sender, EventArgs e)
        {
            string region = CS_ddl_Region.SelectedItem.ToString();
            string cslistserverid = CS_ddl_ListServers.SelectedValue;

            try
            {
                CS_m_CreateServer(region);
                CS_m_ListServerInfo(region);
                CS_m_GetCloudServerDetails(cslistserverid, region);
            }
            catch (Exception ex)
            {
                CS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CS_btn_DeleteServer_OnClick(object sender, EventArgs e)
        {
            string region = CS_ddl_Region.SelectedItem.ToString();
            string cslistserverid = CS_ddl_ListServers.SelectedValue;

            try
            {
                CS_m_ClearSessions();
                CS_m_DeleteServer(region);
                CS_m_ListServerInfo(region);
                CS_m_MsgDeleteServerSuccess();
            }
            catch (Exception ex)
            {
                CS_m_MsgCatchException(ex.ToString());
            }
        }

        protected void CS_btn_PasswdReset_OnClick(object sender, EventArgs e)
        {
            string region = CS_ddl_Region.SelectedItem.ToString();
            string cslistserverid = CS_ddl_ListServers.SelectedValue;
            string passwdreset = CS_txt_PasswdReset.Text;

            try
            {
                CS_m_ClearSessions();
                CS_m_PasswdReset(region);
                CS_m_MSgPasswdReset(passwdreset);
            }
            catch (Exception ex)
            {
                CS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CS_btn_AttachVolume_OnClick(object sender, EventArgs e)
        {
            string region = CS_ddl_Region.SelectedItem.ToString();
            string cslistserverid = CS_ddl_ListServers.SelectedValue;

            try
            {
                CS_m_ClearSessions();
                CS_m_AttachVolume(region);
            }
            catch (Exception ex)
            {
                CS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CS_btn_DetachVolume_OnClick(object sender, EventArgs e)
        {
            string region = CS_ddl_Region.SelectedItem.ToString();
            string cslistserverid = CS_ddl_ListServers.SelectedValue;

            try
            {
                CS_m_ClearSessions();
                CS_m_DetachVolume(region);

            }
            catch (Exception ex)
            {
                CS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CS_m_ListServerInfo(string dcregion)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);
            string cslistserveridsession = HttpUtility.HtmlEncode((string)(Session["CSListServersID"]));
            string cslistservernamesession = HttpUtility.HtmlEncode((string)(Session["CSListServersName"]));

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudServersProvider CloudServersProvider = new net.openstack.Providers.Rackspace.CloudServersProvider();

            var serverdetails = CloudServersProvider.ListServersWithDetails(null, null, null, null, null, null, null, dcregion, identity);

            CS_ddl_ListServers.DataSource = serverdetails;
            CS_ddl_ListServers.DataTextField = "Name";
            CS_ddl_ListServers.DataValueField = "Id";
            CS_ddl_ListServers.SelectedIndex = CS_ddl_ListServers.Items.IndexOf(CS_ddl_ListServers.Items.FindByText(cslistservernamesession));
            CS_ddl_ListServers.DataBind();
        }
        protected void CS_m_GetCloudServerDetails(string cslistserverid, string dcregion)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);
            string CSListServersSession = CS_ddl_ListServers.SelectedValue;

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudServersProvider CloudServersProvider = new net.openstack.Providers.Rackspace.CloudServersProvider();
            CloudBlockStorageProvider CloudBlockStorageProvider = new net.openstack.Providers.Rackspace.CloudBlockStorageProvider(identity);

            try
            {
                if (string.IsNullOrEmpty((string)(Session["CSListServersID"])))
                {
                    if (string.IsNullOrEmpty(CSListServersSession))
                    {
                        CS_m_ClearDdls();
                        CS_lbl_CSInfo.Text = "You currently have no servers built in the " + dcregion.ToString() + " datacenter.  Please create one to see more information about it.";
                    }
                    else
                    {
                        var serverdetails = CloudServersProvider.GetDetails(CSListServersSession, dcregion, identity);
                        var volumedetails = CloudBlockStorageProvider.ListVolumes(dcregion);

                        var Status = serverdetails.GetDetails().Status;
                        var TaskState = serverdetails.GetDetails().TaskState;
                        var VMState = serverdetails.GetDetails().VMState;
                        var UserID = serverdetails.GetDetails().UserId;
                        var HostId = serverdetails.GetDetails().HostId;
                        var ServerId = serverdetails.GetDetails().Id;
                        var TenantId = serverdetails.GetDetails().TenantId;
                        var Created = serverdetails.GetDetails().Created;
                        var LastUpdated = serverdetails.GetDetails().Updated;
                        var DiskConfig = serverdetails.GetDetails().DiskConfig;
                        var Bandwidth = serverdetails.Bandwidth;
                        var FlavorID = serverdetails.GetDetails().Flavor.Id;
                        var FlavorName = serverdetails.GetDetails().Flavor.Name;
                        var ImageID = serverdetails.GetDetails().Image.Id;
                        var ImageName = serverdetails.GetDetails().Image.Name;
                        var PowerState = serverdetails.GetDetails().PowerState;
                        var PublicAddresses = serverdetails.Addresses.Public.ToList();
                        var PrivateAddresses = serverdetails.Addresses.Private.ToList();
                        var GetAttachedVolumes = serverdetails.GetVolumes().ToList();
                        var GetVolumeID = volumedetails.ToList();

                        CS_m_ListServerVolumeList(dcregion);

                        foreach (var i in GetAttachedVolumes)
                        {
                            ListItem ListItemDisplayName = new ListItem(i.Device, i.Id);
                            CS_ddl_CBSAttachedVolume.Items.Add(ListItemDisplayName);
                        }

                        var PublicAddresses_SB = new StringBuilder();

                        foreach (var i in PublicAddresses)
                        {
                            PublicAddresses_SB.AppendLine(i.Address + "<br />");
                        }

                        var PrivateAddresses_SB = new StringBuilder();

                        foreach (var i in PrivateAddresses)
                        {
                            PrivateAddresses_SB.AppendLine(i.Address + "<br />");
                        }

                        CS_lbl_CSInfo.Text = "Status : " + Status + "<br />" +
                                         "Task State : " + TaskState + "<br />" +
                                         "VM State : " + VMState + "<br />" +
                                         "User ID : " + UserID + "<br />" +
                                         "Host ID : " + HostId + "<br />" +
                                         "Server ID : " + ServerId + "<br />" +
                                         "Tenant ID : " + TenantId + "<br />" +
                                         "Created: " + Created + "<br />" +
                                         "Last Updated : " + LastUpdated + "<br />" +
                                         "Disk Config : " + DiskConfig + "<br />" +
                                         "Bandwidth : " + Bandwidth + "<br />" +
                                         "Flavor Name : " + FlavorName + "<br />" +
                                         "Flavor ID : " + FlavorID + "<br />" +
                                         "Image Name : " + ImageName + "<br />" +
                                         "Image ID : " + ImageID + "<br />" +
                                         "Power :" + PowerState + "<br />" +
                                         "Public Address : " + PublicAddresses_SB + "<br />" +
                                         "Private Address : " + PrivateAddresses_SB + "<br />";
                    }
                }
                else
                {
                    var serverdetails = CloudServersProvider.GetDetails(cslistserverid, dcregion, identity);
                    var volumedetails = CloudBlockStorageProvider.ListVolumes(dcregion);

                    var Status = serverdetails.GetDetails().Status;
                    var TaskState = serverdetails.GetDetails().TaskState;
                    var VMState = serverdetails.GetDetails().VMState;
                    var UserID = serverdetails.GetDetails().UserId;
                    var HostId = serverdetails.GetDetails().HostId;
                    var ServerId = serverdetails.GetDetails().Id;
                    var TenantId = serverdetails.GetDetails().TenantId;
                    var Created = serverdetails.GetDetails().Created;
                    var LastUpdated = serverdetails.GetDetails().Updated;
                    var DiskConfig = serverdetails.GetDetails().DiskConfig;
                    var Bandwidth = serverdetails.Bandwidth;
                    var FlavorID = serverdetails.GetDetails().Flavor.Id;
                    var FlavorName = serverdetails.GetDetails().Flavor.Name;
                    var ImageID = serverdetails.GetDetails().Image.Id;
                    var ImageName = serverdetails.GetDetails().Image.Name;
                    var PowerState = serverdetails.GetDetails().PowerState;
                    var PublicAddresses = serverdetails.Addresses.Public.ToList();
                    var PrivateAddresses = serverdetails.Addresses.Private.ToList();
                    var GetAttachedVolumes = serverdetails.GetVolumes().ToList();
                    var GetVolumeID = volumedetails.ToList();

                    CS_m_ListServerVolumeList(dcregion);

                    foreach (var i in GetAttachedVolumes)
                    {
                        ListItem ListItemDisplayName = new ListItem(i.Device, i.Id);
                        CS_ddl_CBSAttachedVolume.Items.Add(ListItemDisplayName);
                    }

                    var PublicAddresses_SB = new StringBuilder();

                    foreach (var i in PublicAddresses)
                    {
                        PublicAddresses_SB.AppendLine(i.Address + "<br />");
                    }

                    var PrivateAddresses_SB = new StringBuilder();

                    foreach (var i in PrivateAddresses)
                    {
                        PrivateAddresses_SB.AppendLine(i.Address + "<br />");
                    }

                    CS_lbl_CSInfo.Text = "Status : " + Status + "<br />" +
                                         "Task State : " + TaskState + "<br />" +
                                         "VM State : " + VMState + "<br />" +
                                         "User ID : " + UserID + "<br />" +
                                         "Host ID : " + HostId + "<br />" +
                                         "Server ID : " + ServerId + "<br />" +
                                         "Tenant ID : " + TenantId + "<br />" +
                                         "Created: " + Created + "<br />" +
                                         "Last Updated : " + LastUpdated + "<br />" +
                                         "Disk Config : " + DiskConfig + "<br />" +
                                         "Bandwidth : " + Bandwidth + "<br />" +
                                         "Flavor Name : " + FlavorName + "<br />" +
                                         "Flavor ID : " + FlavorID + "<br />" +
                                         "Image Name : " + ImageName + "<br />" +
                                         "Image ID : " + ImageID + "<br />" +
                                         "Power :" + PowerState + "<br />" +
                                         "Public Address : " + PublicAddresses_SB + "<br />" +
                                         "Private Address : " + PrivateAddresses_SB + "<br />";
                }
            }
            catch (Exception ex)
            {
                CS_lbl_Error.Text = " Server details are not available yet.";
            }
        }
        protected void CS_m_ListServerVolumeList(string dcregion)
        {
            CS_m_ClearDdls();

            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);
            string CSListServersSession = CS_ddl_ListServers.SelectedValue;

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudServersProvider CloudServersProvider = new net.openstack.Providers.Rackspace.CloudServersProvider();
            CloudBlockStorageProvider CloudBlockStorageProvider = new net.openstack.Providers.Rackspace.CloudBlockStorageProvider(identity);

            var volumedetails = CloudBlockStorageProvider.ListVolumes(dcregion);

            var GetVolumeID = volumedetails.ToList();

            foreach (var i in GetVolumeID)
            {
                var attachments_var = i.Attachments;
                if (attachments_var.Count() <= 0)
                {
                    ListItem ListItemDisplayName = new ListItem(i.DisplayName, i.Id);
                    CS_ddl_CBSAvailableVolume.Items.Add(ListItemDisplayName);
                }
                else
                {
                    foreach (var item in attachments_var)
                    {
                        ListItem ListItemDisplayName = new ListItem(i.DisplayName, i.Id);
                        CS_ddl_CBSAvailableVolume.Items.Add(ListItemDisplayName);
                    }
                }
            }
        }
        protected void CS_m_AttachVolume(string dcregion)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);
            string CSListServersSession = CS_ddl_ListServers.SelectedValue;

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudServersProvider CloudServersProvider = new net.openstack.Providers.Rackspace.CloudServersProvider();
            CloudBlockStorageProvider CloudBlockStorageProvider = new net.openstack.Providers.Rackspace.CloudBlockStorageProvider(identity);

            var volumedetails = CloudServersProvider.AttachServerVolume(CS_ddl_ListServers.SelectedValue, CS_ddl_CBSAvailableVolume.SelectedValue, null, dcregion);
        }
        protected void CS_m_DetachVolume(string dcregion)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);
            string CSListServersSession = CS_ddl_ListServers.SelectedValue;

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudServersProvider CloudServersProvider = new net.openstack.Providers.Rackspace.CloudServersProvider();
            CloudBlockStorageProvider CloudBlockStorageProvider = new net.openstack.Providers.Rackspace.CloudBlockStorageProvider(identity);

            var volumedetails = CloudServersProvider.DetachServerVolume(CS_ddl_ListServers.SelectedValue, CS_ddl_CBSAttachedVolume.SelectedValue, dcregion);
        }
        protected void CS_m_ResizeServer(string dcregion)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);
            string CSListServersSession = CS_ddl_ListServers.SelectedValue;

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudServersProvider CloudServersProvider = new net.openstack.Providers.Rackspace.CloudServersProvider();
            CloudBlockStorageProvider CloudBlockStorageProvider = new net.openstack.Providers.Rackspace.CloudBlockStorageProvider(identity);


        }
        protected void CS_m_RebootServer(string dcregion, string reboottype)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudServersProvider CloudServersProvider = new net.openstack.Providers.Rackspace.CloudServersProvider();

            if (reboottype.ToString() == "soft")
            {
                CloudServersProvider.RebootServer(CS_ddl_ListServers.SelectedValue, RebootType.SOFT, dcregion, identity);
            }
            else if (reboottype.ToString() == "hard")
            {
                CloudServersProvider.RebootServer(CS_ddl_ListServers.SelectedValue, RebootType.HARD, dcregion, identity);
            }
        }
        protected void CS_m_ListFlavors()
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudServersProvider CloudServersProvider = new net.openstack.Providers.Rackspace.CloudServersProvider(identity);

            var flavordetails = CloudServersProvider.ListFlavors();

            CS_ddl_AvailableFlavors.DataSource = flavordetails;
            CS_ddl_AvailableFlavors.DataTextField = "Name";
            CS_ddl_AvailableFlavors.DataValueField = "Id";
            CS_ddl_AvailableFlavors.DataBind();
        }
        protected void CS_m_ListImages()
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudServersProvider CloudServersProvider = new net.openstack.Providers.Rackspace.CloudServersProvider(identity);

            var images = CloudServersProvider.ListImages();

            CS_ddl_AvailableImage.DataSource = images;
            CS_ddl_AvailableImage.DataTextField = "Name";
            CS_ddl_AvailableImage.DataValueField = "Id";
            CS_ddl_AvailableImage.DataBind();
        }
        protected void CS_m_CreateServer(string dcregion)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);
            StringBuilder CS_sb_CloudServerPasswd = new StringBuilder();

            int InputHowMany = int.Parse(CS_txt_HowMany.Text.ToString());

            bool attachtoservicenetwork = bool.Parse("true");
            bool attachtopublicnetwork = bool.Parse("true");

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudServersProvider CloudServersProvider = new net.openstack.Providers.Rackspace.CloudServersProvider(identity);

            if (InputHowMany <= 10)
            {
                if (InputHowMany >= 1)
                {
                    int count = 1;
                    int countid = 1;
                    while (count <= InputHowMany)
                    {
                        count = count + 1;
                        countid = count - 1;
                        var server = CloudServersProvider.CreateServer(CS_txt_Name.Text + "_" + countid, CS_ddl_AvailableImage.SelectedValue, CS_ddl_AvailableFlavors.SelectedValue, null, null, null, attachtoservicenetwork, attachtopublicnetwork, null, dcregion, identity);
                        var serverpasswd = server.AdminPassword.ToString();
                        CS_sb_CloudServerPasswd.Append(CS_txt_Name.Text + "_" + countid + " " + serverpasswd.ToString() + "\r\n");
                    }

                    CS_lbl_Passwd.Text = "Servers have been created successfully.  Your admin password is: \r\n" + CS_sb_CloudServerPasswd;
                }
                else
                {
                    var server = CloudServersProvider.CreateServer(CS_txt_Name.Text, CS_ddl_AvailableImage.SelectedValue, CS_ddl_AvailableFlavors.SelectedValue, null, null, null, attachtoservicenetwork, attachtopublicnetwork, null, dcregion, identity);
                    var serverpasswd = server.AdminPassword.ToString();

                    CS_lbl_Passwd.Text = CS_txt_Name + " created successfully.  Your admin password is: " + serverpasswd;
                }
            }
            else
            {
                CS_lbl_Error.Text = "You can only create 10 servers at a time.";
            }
        }
        protected void CS_m_DeleteServer(string dcregion)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);
            string CSListServersSession = CS_ddl_ListServers.SelectedValue;

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudServersProvider CloudServersProvider = new net.openstack.Providers.Rackspace.CloudServersProvider();

            CloudServersProvider.DeleteServer(CS_ddl_ListServers.SelectedValue, dcregion, identity);
        }
        protected void CS_m_PasswdReset(string dcregion)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);
            string CSListServersSession = CS_ddl_ListServers.SelectedValue;

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudServersProvider CloudServersProvider = new net.openstack.Providers.Rackspace.CloudServersProvider();

            CloudServersProvider.ChangeAdministratorPassword(CS_ddl_ListServers.SelectedValue, CS_txt_PasswdReset.Text, dcregion, identity);
        }
        protected void CS_m_ClearSessions()
        {
            Session.Remove("CSListServersName");
            Session.Remove("CSListServersID");
        }
        protected void CS_m_ClearDdls()
        {
            CS_ddl_CBSAvailableVolume.Items.Clear();
            CS_ddl_CBSAttachedVolume.Items.Clear();
        }
        protected void CS_m_ClearGrid()
        {
            CS_grid_Results.DataSource = null;
            CS_grid_Results.DataBind();
            CS_grid_ServerInfo.DataSource = null;
            CS_grid_ServerInfo.DataBind();
            CS_grid_ServerInfo2.DataSource = null;
            CS_grid_ServerInfo2.DataBind();
            CS_grid_ServerInfo3.DataSource = null;
            CS_grid_ServerInfo3.DataBind();
        }
        protected void CS_m_MSgPleaseSeleactRegion()
        {
            CS_lbl_Info.Text = "Please select a region: DFW, or ORD but not both.";
        }
        protected void CS_m_MSgPasswdReset(string passwd)
        {
            CS_lbl_Info.Text = "Admin Passwd has been reset succesfully.  New passwd is : " + passwd;
        }
        protected void CS_m_MsgRebootServerSuccess()
        {
            CS_lbl_Info.Text = CS_ddl_ListServers.SelectedItem + " has been successfully rebooted.";
        }
        protected void CS_m_MsgDeleteServerSuccess()
        {
            CS_lbl_Info.Text = CS_ddl_ListServers.SelectedItem + " has been successfully deleted.";
        }
        protected void CS_m_MsgCatchException(string exception)
        {
            CS_lbl_Error.Text = "Something went terribly wrong! See below for more info. <br /> <br />" + exception;
        }
    }
}