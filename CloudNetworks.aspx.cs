using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string region = CN_ddl_Region.SelectedItem.ToString();

            HttpContext.Current.Session["CNListNetworks"] = HttpUtility.HtmlEncode(CN_ddl_ListNetworks.SelectedItem);
            HttpContext.Current.Session["CNListNetworksID"] = HttpUtility.HtmlEncode(CN_ddl_ListNetworks.SelectedValue);
            
            Page.GetPostBackEventReference(CN_btn_CreateNetwork);

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
                        bindListNetworksDDL(Networks.CN_m_ListNetworks(region), "Label", "Id");

                        CN_lbl_networkinfo.Text = Networks.CN_m_ShowNetworkInfo(CN_ddl_ListNetworks.SelectedValue, region);

                        TimeClock.Stop();
                        CN_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
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
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string region = CN_ddl_Region.SelectedItem.ToString();

            try
            {
                bindListNetworksDDL(Networks.CN_m_ListNetworks(region), "Label", "Id");

                CN_lbl_networkinfo.Text = Networks.CN_m_ShowNetworkInfo(CN_ddl_ListNetworks.SelectedValue, region);

                TimeClock.Stop();
                CN_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CN_m_MSGCatchException(ex.ToString());
            }
        }
        protected void CN_ddl_ListNetworks_SelectChange(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string region = CN_ddl_Region.SelectedItem.ToString();

            try
            {
                CN_lbl_networkinfo.Text = Networks.CN_m_ShowNetworkInfo(CN_ddl_ListNetworks.SelectedValue, region);

                TimeClock.Stop();
                CN_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CN_m_MSGCatchException(ex.ToString());
            }
        }
        protected void CN_btn_CreateNetwork_OnClick(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string cidrrange = CN_txt_CIDRRange.Text;
            string networkname = CN_txt_CreateNetworkName.Text;
            string region = CN_ddl_Region.SelectedItem.ToString();

            try
            {
                Networks.CN_m_CreateNetwork(cidrrange, networkname, CN_txt_CreateNetworkName.Text, region);
                bindListNetworksDDL(Networks.CN_m_ListNetworks(region), "Label", "Id");
                CN_lbl_networkinfo.Text = Networks.CN_m_ShowNetworkInfo(CN_ddl_ListNetworks.SelectedValue, region);
                CN_m_MSGCreateNetwork();

                TimeClock.Stop();
                CN_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CN_m_MSGCatchException(ex.ToString());
            }
        }
        protected void CN_btn_DeleteNetwork_OnClick(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string region = CN_ddl_Region.SelectedItem.ToString();
            string networkid = CN_ddl_ListNetworks.SelectedValue.ToString();

            try
            {
                Networks.CN_m_DeleteNetwork(networkid, region);
                bindListNetworksDDL(Networks.CN_m_ListNetworks(region), "Label", "Id");
                CN_lbl_networkinfo.Text = Networks.CN_m_ShowNetworkInfo(CN_ddl_ListNetworks.SelectedValue, region);
                CN_m_MSGDeleteNetwork();

                TimeClock.Stop();
                CN_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CN_m_MSGCatchException(ex.ToString());
            }
        }
        protected static string cnlistnetworkssession() { return (string)(HttpContext.Current.Session["CNListNetworks"]); }
        protected static string cnlistnetworkssessionid() { return (string)(HttpContext.Current.Session["CNListNetworksID"]); }
        protected void bindListNetworksDDL(object dataSource, string dataTextField, string dataValueField)
        {
            CN_ddl_ListNetworks.DataSource = dataSource;
            CN_ddl_ListNetworks.DataTextField = dataTextField;
            CN_ddl_ListNetworks.DataValueField = dataValueField;
            CN_ddl_ListNetworks.DataBind();
        }
        protected void bindGridResults(object dataSource)
        {
            CN_grid_Results.DataSource = dataSource;
            CN_grid_Results.DataBind();
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