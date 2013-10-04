using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            HttpContext.Current.Session["CSListServersName"] = HttpUtility.HtmlEncode(CS_ddl_ListServers.SelectedItem);
            HttpContext.Current.Session["CSListServersID"] = HttpUtility.HtmlEncode(CS_ddl_ListServers.SelectedValue);
            HttpContext.Current.Session["CSListServersRegion"] = CS_ddl_Region.Text;

            string region = CS_ddl_Region.SelectedItem.ToString();
            string cslistserverid = CS_ddl_ListServers.SelectedValue.ToString();
            string cslistserveridsession = (string)(HttpContext.Current.Session["CSListServersID"]);
            string cslistservernamesession = (string)(HttpContext.Current.Session["CSListServersName"]);
            string CloudIdentityUserName = (string)(HttpContext.Current.Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(HttpContext.Current.Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudServersProvider CloudServersProvider = new net.openstack.Providers.Rackspace.CloudServersProvider();

            Page.GetPostBackEventReference(CS_btn_RebootCloudServer);

            try
            {
                if (IsPostBack)
                {
                    HttpContext.Current.Session["PostBackListServerIdDDL"] = HttpUtility.HtmlEncode(CS_ddl_ListServers.SelectedValue);
                    HttpContext.Current.Session["PostBackListServerNameDDL"] = HttpUtility.HtmlEncode(CS_ddl_ListServers.SelectedItem);
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
                        var listimages = Servers.CS_m_ListImages();
                        bindListImagesNoIndexInDDL(listimages, "Name", "Id");

                        var listflavors = Servers.CS_m_ListFlavors();
                        bindListFlavorsNoIndexInDDL(listflavors, "Name", "Id");

                        var serverdetails = Servers.CS_m_ListServerInfo(region);
                        var firstserver = serverdetails.First();
                        bindListServersNoIndexInDDL(serverdetails, "Name", "Id");

                        CS_m_GetCloudServerDetails(firstserver.Id, region);

                        TimeClock.Stop();
                        CS_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
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
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string region = CS_ddl_Region.SelectedItem.ToString();
            string cslistserverid = CS_ddl_ListServers.SelectedValue.ToString();

            try
            {
                CS_m_ClearGrid();

                var serverdetails = Servers.CS_m_ListServerInfo(region);
                bindListServersNoIndexInDDL(serverdetails, "Name", "Id");
                var firstserver = serverdetails.First();
                CS_m_GetCloudServerDetails(firstserver.Id, region);

                TimeClock.Stop();
                CS_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CS_ddl_ListServers_SelectChange(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string region = CS_ddl_Region.SelectedItem.ToString();
            string cslistserverid = CS_ddl_ListServers.SelectedValue;
            string cslistserverregion = (string)(Session["CSListServersRegion"]);
            string cslistserveridsession = (string)(Session["PostBackListServerIdDDL"]);
            string cslistservernamesession = (string)(Session["CSListServersName"]);
            string postbackname = (string)(Session["PostBackListServerNameDDL"]);
            string postbackid = (string)(Session["PostBackListServerIdDDL"]);

            try
            {
                if (string.IsNullOrEmpty((string)(Session["CSListServersID"])))
                {
                    var serverdetails = Servers.CS_m_ListServerInfo(region);
                    var firstserver = serverdetails.First();

                    bindListServersNoIndexInDDL(serverdetails, "Name", "Id");

                    CS_m_GetCloudServerDetails(firstserver.Id, region);

                    TimeClock.Stop();
                    CS_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
                }
                else
                {
                    var serverdetails = Servers.CS_m_ListServerInfo(region);
                    bindListServersDDL(serverdetails, "Name", "Id", postbackname);
                    CS_m_GetCloudServerDetails(postbackid, region);

                    TimeClock.Stop();
                    CS_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
                }
            }
            catch (Exception ex)
            {
                CS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CS_btn_RebootServer_OnClick(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string softReboot = "soft";
            string hardReboot = "hard";

            string region = CS_ddl_Region.SelectedItem.ToString();
            string cslistserverid = CS_ddl_ListServers.SelectedValue;

            try
            {
                if (region != null & CSRebootServerDDL.Text == "soft")
                {
                    Servers.CS_m_RebootServer(region, cslistserverid, softReboot);
                    CS_m_GetCloudServerDetails(cslistserverid, region);
                    CS_m_MsgRebootServerSuccess();

                    TimeClock.Stop();
                    CS_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
                }
                else if (region != null & CSRebootServerDDL.Text == "hard")
                {
                    Servers.CS_m_RebootServer(region, cslistserverid, hardReboot);
                    CS_m_GetCloudServerDetails(cslistserverid, region);
                    CS_m_MsgRebootServerSuccess();

                    TimeClock.Stop();
                    CS_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
                }
            }
            catch (Exception ex)
            {
                CS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CS_btn_CreateServer_OnClick(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string region = CS_ddl_Region.SelectedItem.ToString();
            string cslistserverid = CS_ddl_ListServers.SelectedValue;

            string servername = CS_txt_Name.Text;
            string howmany = CS_txt_HowMany.Text.ToString();
            string listimages = CS_ddl_AvailableImage.SelectedValue;
            string listflavors = CS_ddl_AvailableFlavors.SelectedValue;

            try
            {
                var createserver = Servers.CS_m_CreateServer(servername, listimages, listflavors, howmany, region);
                Servers.CS_m_ListServerInfo(region);
                CS_m_GetCloudServerDetails(cslistserverid, region);

                CS_lbl_Info.Text = createserver.ToString();

                TimeClock.Stop();
                CS_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CS_btn_DeleteServer_OnClick(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string region = CS_ddl_Region.SelectedItem.ToString();
            string cslistserverid = CS_ddl_ListServers.SelectedValue;

            try
            {
                CS_m_ClearSessions();
                Servers.CS_m_DeleteServer(cslistserverid, region);
                Servers.CS_m_ListServerInfo(region);
                CS_m_MsgDeleteServerSuccess();

                TimeClock.Stop();
                CS_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CS_m_MsgCatchException(ex.ToString());
            }
        }

        protected void CS_btn_PasswdReset_OnClick(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string region = CS_ddl_Region.SelectedItem.ToString();
            string cslistserverid = CS_ddl_ListServers.SelectedValue;
            string passwdreset = CS_txt_PasswdReset.Text;

            try
            {
                CS_m_ClearSessions();
                Servers.CS_m_PasswdReset(cslistserverid, passwdreset, region);
                CS_m_MSgPasswdReset(passwdreset);

                TimeClock.Stop();
                CS_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CS_btn_AttachVolume_OnClick(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string region = CS_ddl_Region.SelectedItem.ToString();
            string cslistserverid = CS_ddl_ListServers.SelectedValue;
            string cslistavailablevolumeid = CS_ddl_CBSAvailableVolume.SelectedValue;

            try
            {
                CS_m_ClearSessions();

                var serverdetails = Servers.CS_m_ListServerInfo(region);
                var firstserver = serverdetails.First();

                Servers.CS_m_AttachVolume(cslistserverid, cslistavailablevolumeid, region);
                

                CS_m_GetCloudServerDetails(firstserver.Id, region);

                TimeClock.Stop();
                CS_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CS_btn_DetachVolume_OnClick(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string region = CS_ddl_Region.SelectedItem.ToString();
            string cslistserverid = CS_ddl_ListServers.SelectedValue;
            string cslistavailablevolumeid = CS_ddl_CBSAvailableVolume.SelectedValue;
            var serverdetails = Servers.CS_m_ListServerInfo(region);
            var firstserver = serverdetails.First();

            try
            {
                CS_m_ClearSessions();
                CS_ddl_CBSAttachedVolume.Items.Clear();

                Servers.CS_m_DetachVolume(cslistserverid, cslistavailablevolumeid, region);

                CS_m_GetCloudServerDetails(firstserver.Id, region);

                TimeClock.Stop();
                CS_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();

            }
            catch (Exception ex)
            {
                CS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CS_m_GetCloudServerDetails(string cslistserverid, string dcregion)
        {
            string value = "";

            try
            {
                if (string.IsNullOrEmpty((string)(Session["CSListServersID"])))
                {
                    if (string.IsNullOrEmpty(cslistserverid))
                    {
                        CS_m_ClearDdls();
                        CS_lbl_CSInfo.Text = "You currently have no servers built in the " + dcregion.ToString() + " datacenter.  Please create one to see more information about it.";
                    }
                    else
                    {
                        var serverdetails = Servers.serversProvider().GetDetails(cslistserverid, dcregion);
                        var volumedetails = Servers.blockStorageProvider().ListVolumes(dcregion);

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
                        var FlavorID = serverdetails.GetDetails().Flavor.Id;
                        var FlavorName = serverdetails.GetDetails().Flavor.Name;
                        var ImageID = serverdetails.GetDetails().Image.Id;
                        var ImageName = serverdetails.GetDetails().Image.Name;
                        var PowerState = serverdetails.GetDetails().PowerState;
                        var GetAttachedVolumes = serverdetails.GetVolumes().ToList();
                        var GetVolumeID = volumedetails.ToList();

                        var listServerVolumeList = Servers.CS_m_ListServerVolumeList(dcregion);
                        foreach (var i in listServerVolumeList )
                        {
                            addItemToDdl(i);
                        }

                        bindlblCSInfo2(Servers.CS_m_ListAddresses(CS_ddl_ListServers.SelectedValue, dcregion));

                        foreach (var i in GetAttachedVolumes)
                        {
                            ListItem ListItemDisplayName = new ListItem(i.Device, i.Id);
                            CS_ddl_CBSAttachedVolume.Items.Add(ListItemDisplayName);
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
                                         "Flavor Name : " + FlavorName + "<br />" +
                                         "Flavor ID : " + FlavorID + "<br />" +
                                         "Image Name : " + ImageName + "<br />" +
                                         "Image ID : " + ImageID + "<br />" +
                                         "Power :" + PowerState + "<br />";
                    }
                }
                else
                {
                    CS_m_ClearDdls();

                    var serverdetails = Servers.serversProvider().GetDetails(cslistserverid, dcregion);
                    var volumedetails = Servers.blockStorageProvider().ListVolumes(dcregion);

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
                    var FlavorID = serverdetails.GetDetails().Flavor.Id;
                    var FlavorName = serverdetails.GetDetails().Flavor.Name;
                    var ImageID = serverdetails.GetDetails().Image.Id;
                    var ImageName = serverdetails.GetDetails().Image.Name;
                    var PowerState = serverdetails.GetDetails().PowerState;
                    var PublicAddresses = serverdetails.Addresses.Public.ToList();
                    var PrivateAddresses = serverdetails.Addresses.Private.ToList();
                    var GetAttachedVolumes = serverdetails.GetVolumes().ToList();
                    var GetVolumeID = volumedetails.ToList();

                    var listServerVolumeList = Servers.CS_m_ListServerVolumeList(dcregion);
                    foreach (var i in listServerVolumeList)
                    {
                        addItemToDdl(i);
                    }

                    bindlblCSInfo2(Servers.CS_m_ListAddresses(CS_ddl_ListServers.SelectedValue, dcregion));


                    foreach (var i in GetAttachedVolumes)
                    {
                        ListItem ListItemDisplayName = new ListItem(i.Device, i.Id);
                        CS_ddl_CBSAttachedVolume.Items.Add(ListItemDisplayName);
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
                                         "Flavor Name : " + FlavorName + "<br />" +
                                         "Flavor ID : " + FlavorID + "<br />" +
                                         "Image Name : " + ImageName + "<br />" +
                                         "Image ID : " + ImageID + "<br />" +
                                         "Power :" + PowerState + "<br />";

                }
            }
            catch (Exception ex)
            {
                CS_lbl_Error.Text = " Server details are not available yet.";
            }
        }
        protected void bindListServersDDL(object dataSource, string dataTextField, string dataValueField, string selectedSessionIndex)
        {
            CS_ddl_ListServers.DataSource = dataSource;
            CS_ddl_ListServers.DataTextField = dataTextField;
            CS_ddl_ListServers.DataValueField = dataValueField;
            CS_ddl_ListServers.SelectedIndex = CS_ddl_ListServers.Items.IndexOf(CS_ddl_ListServers.Items.FindByText(selectedSessionIndex));
            CS_ddl_ListServers.DataBind();
        }
        protected void bindListServersNoIndexInDDL(object dataSource, string dataTextField, string dataValueField)
        {
            CS_ddl_ListServers.DataSource = dataSource;
            CS_ddl_ListServers.DataTextField = dataTextField;
            CS_ddl_ListServers.DataValueField = dataValueField;
            CS_ddl_ListServers.DataBind();
        }
        protected void bindListFlavorsNoIndexInDDL(object dataSource, string dataTextField, string dataValueField)
        {
            CS_ddl_AvailableFlavors.DataSource = dataSource;
            CS_ddl_AvailableFlavors.DataTextField = dataTextField;
            CS_ddl_AvailableFlavors.DataValueField = dataValueField;
            CS_ddl_AvailableFlavors.DataBind();
        }
        protected void bindListImagesNoIndexInDDL(object dataSource, string dataTextField, string dataValueField)
        {
            CS_ddl_AvailableImage.DataSource = dataSource;
            CS_ddl_AvailableImage.DataTextField = dataTextField;
            CS_ddl_AvailableImage.DataValueField = dataValueField;
            CS_ddl_AvailableImage.DataBind();
        }
        protected void bindlblCSInfo2(string textToBind)
        {
            CS_lbl_CSInfo2.Text = textToBind;
        }
        protected void addItemToDdl(ListItem ListItemDisplayName)
        {
            CS_ddl_CBSAvailableVolume.Items.Add(ListItemDisplayName);
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