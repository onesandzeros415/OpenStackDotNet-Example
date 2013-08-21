using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class CloudNetworks : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string region = CN_ddl_Region.SelectedItem.ToString();

            HttpContext.Current.Session["CNListNetworks"] = HttpUtility.HtmlEncode(CN_ddl_ListNetworks.SelectedItem);
            HttpContext.Current.Session["CNListNetworksID"] = HttpUtility.HtmlEncode(CN_ddl_ListNetworks.SelectedValue);

            try
            {
                if (IsPostBack)
                {
                }
                else
                {

                    if (string.IsNullOrEmpty((string)(Session["CloudIdentityUserName"])) & string.IsNullOrEmpty((string)(Session["CloudIdentityApiKey"])))
                    {
                        CN_lbl_Error.Text = "Before continuing please login and enter Cloud Username and API Key.";
                    }
                    else if (string.IsNullOrEmpty((string)(Session["CloudIdentityUserName"])))
                    {
                        CN_lbl_Error.Text = "Before continuing please login and please enter Cloud Username.";
                    }
                    else if (string.IsNullOrEmpty((string)(Session["CloudIdentityApiKey"])))
                    {
                        CN_lbl_Error.Text = "Before continuing please login and please enter API Key.";
                    }
                    else
                    {
                        CN_m_ListNetworks(region);
                        CN_m_ShowNetworkInfo(region);
                    }
                }
            }
            catch (Exception ex)
            {
                CN_m_MSGCatchException(ex.ToString());
            }
        }
        protected void CN_ddl_Region_SelectChange(object sender, EventArgs e)
        {
            string region = CN_ddl_Region.SelectedItem.ToString();

            try
            {
                CN_m_ListNetworks(region);
            }
            catch (Exception ex)
            {
                CN_m_MSGCatchException(ex.ToString());
            }
        }
        protected void CN_ddl_ListNetworks_SelectChange(object sender, EventArgs e)
        {
            string region = CN_ddl_Region.SelectedItem.ToString();

            try
            {
                CN_m_ShowNetworkInfo(region);
            }
            catch (Exception ex)
            {
                CN_m_MSGCatchException(ex.ToString());
            }
        }
        protected void CN_btn_CreateNetwork_OnClick(object sender, EventArgs e)
        {
            string cidrrange = CN_txt_CIDRRange.Text;
            string networkname = CN_txt_CreateNetworkName.Text;
            string region = CN_ddl_Region.SelectedItem.ToString();

            try
            {
                CN_m_CreateNetwork(cidrrange, networkname, region);
                CN_m_ListNetworks(region);
                CN_m_MSGCreateNetwork();
            }
            catch (Exception ex)
            {
                CN_m_MSGCatchException(ex.ToString());
            }
        }
        protected void CN_btn_DeleteNetwork_OnClick(object sender, EventArgs e)
        {
            string region = CN_ddl_Region.SelectedItem.ToString();
            string networkid = CN_ddl_ListNetworks.SelectedValue.ToString();

            try
            {
                CN_m_DeleteNetwork(networkid, region);
                CN_m_ListNetworks(region);
                CN_m_MSGDeleteNetwork();
            }
            catch (Exception ex)
            {
                CN_m_MSGCatchException(ex.ToString());
            }
        }
        protected static string CloudIdentityUserName() { return (string)(HttpContext.Current.Session["CloudIdentityUserName"]); }
        protected static string CloudIdentityApiKey() { return (string)(HttpContext.Current.Session["CloudIdentityApiKey"]); }
        protected static string cnlistnetworkssession() { return (string)(HttpContext.Current.Session["CNListNetworks"]); }
        protected static string cnlistnetworkssessionid() { return (string)(HttpContext.Current.Session["CNListNetworksID"]); }
        protected void CN_m_ListNetworks(string dcregion)
        {
            CN_m_clearitems();

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName(), APIKey = CloudIdentityApiKey() };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudNetworksProvider CloudNetworksProvider = new net.openstack.Providers.Rackspace.CloudNetworksProvider(identity);

            var CloudNetworksListNetworks = CloudNetworksProvider.ListNetworks(dcregion);

            CN_ddl_ListNetworks.DataSource = CloudNetworksListNetworks;
            CN_ddl_ListNetworks.DataTextField = "Label";
            CN_ddl_ListNetworks.DataValueField = "Id";
            CN_ddl_ListNetworks.SelectedIndex = CN_ddl_ListNetworks.Items.IndexOf(CN_ddl_ListNetworks.Items.FindByText(cnlistnetworkssession()));
            CN_ddl_ListNetworks.DataBind();

            CN_grid_Results.DataSource = CloudNetworksListNetworks;
            CN_grid_Results.DataBind();
        }
        protected void CN_m_ShowNetworkInfo(string dcregion)
        {
            CN_m_clearitems();

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName(), APIKey = CloudIdentityApiKey() };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudNetworksProvider CloudNetworksProvider = new net.openstack.Providers.Rackspace.CloudNetworksProvider(identity);

            if (string.IsNullOrEmpty(cnlistnetworkssessionid()))
            {
                var CloudNetworksListNetworks = CloudNetworksProvider.ShowNetwork(CN_ddl_ListNetworks.SelectedValue, dcregion);

                if (string.IsNullOrEmpty(CloudNetworksListNetworks.Cidr))
                {
                    CN_lbl_networkinfo.Text = "Network Name : " + CloudNetworksListNetworks.Label + "<br />" + "Network ID : " + CloudNetworksListNetworks.Id + "<br />";
                }
                else
                {
                    CN_lbl_networkinfo.Text = "Network Name : " + CloudNetworksListNetworks.Label + "<br />" + "CIDR : " + CloudNetworksListNetworks.Cidr + "<br />" + "Network ID : " + CloudNetworksListNetworks.Id + "<br />";
                }
            }
            else
            {
                var CloudNetworksListNetworks = CloudNetworksProvider.ShowNetwork(cnlistnetworkssessionid(), dcregion);
                if (string.IsNullOrEmpty(CloudNetworksListNetworks.Cidr))
                {
                    CN_lbl_networkinfo.Text = "Network Name : " + CloudNetworksListNetworks.Label + "<br />" + "Network ID : " + CloudNetworksListNetworks.Id + "<br />";
                }
                else
                {
                    CN_lbl_networkinfo.Text = "Network Name : " + CloudNetworksListNetworks.Label + "<br />" + "CIDR : " + CloudNetworksListNetworks.Cidr + "<br />" + "Network ID : " + CloudNetworksListNetworks.Id + "<br />";
                }
            }
        }
        protected void CN_m_CreateNetwork(string cidr, string networkname, string dcregion)
        {
            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName(), APIKey = CloudIdentityApiKey() };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudNetworksProvider CloudNetworksProvider = new net.openstack.Providers.Rackspace.CloudNetworksProvider(identity);

            var CloudNetworksCreateNetwork = CloudNetworksProvider.CreateNetwork("192.0.2.0/24", CN_txt_CreateNetworkName.Text, dcregion);
        }
        protected void CN_m_DeleteNetwork(string networkid, string dcregion)
        {
            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName(), APIKey = CloudIdentityApiKey() };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudNetworksProvider CloudNetworksProvider = new net.openstack.Providers.Rackspace.CloudNetworksProvider(identity);

            var CloudNetworksListNetworks = CloudNetworksProvider.DeleteNetwork(networkid, dcregion);
        }
        protected void CN_m_clearitems()
        {
            CN_grid_Results.DataSource = null;
            CN_grid_Results.DataBind();

            CN_lbl_Error.Text = null;
            CN_lbl_Info.Text = null;
            CN_lbl_networkinfo.Text = null;
        }
        protected void CN_m_MSGPleaseSeleactRegion()
        {
            CN_lbl_Info.Text = "Please select a region: DFW, or ORD but not both.";
        }
        protected void CN_m_MSGCatchException(string exception)
        {
            CN_lbl_Error.Text = "Something went terribly wrong! See below for more info. <br /> <br />" + exception;
        }
        protected void CN_m_MSGListNetworks()
        {
            CN_lbl_Error.Text = "Networks have been listed successfully";
        }
        protected void CN_m_MSGShowNetwork()
        {
            CN_lbl_Error.Text = CN_ddl_ListNetworks.Text + " has been listed successfully";
        }
        protected void CN_m_MSGCreateNetwork()
        {
            CN_lbl_Error.Text = "Network has been created successfully";
        }
        protected void CN_m_MSGDeleteNetwork()
        {
            CN_lbl_Error.Text = "Network has been deleted successfully";
        }
    }
}