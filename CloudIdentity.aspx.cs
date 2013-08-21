using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using net.openstack.Providers;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace;

namespace OpenStackDotNet_Test
{
    public partial class CloudIdentity : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty((string)(Session["CloudIdentityUserName"])) & string.IsNullOrEmpty((string)(Session["CloudIdentityApiKey"])))
                {
                    CI_lbl_Error.Text = "Before continuing please login and enter Cloud Username and API Key.";
                }
                else if (string.IsNullOrEmpty((string)(Session["CloudIdentityUserName"])))
                {
                    CI_lbl_Error.Text = "Before continuing please login and please enter Cloud Username.";
                }
                else if (string.IsNullOrEmpty((string)(Session["CloudIdentityApiKey"])))
                {
                    CI_lbl_Error.Text = "Before continuing please login and please enter API Key.";
                }
                else
                {
                    CI_m_ListUsers();
                    CI_m_ListRoles();
                    CI_m_ListTenants();
                }
            }
            catch (Exception ex)
            {
                MessageCatchException(ex.ToString());
            }
        }
        protected void CI_m_ListUsers()
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);

            IEnumerable<User> ListUsers = identityProvider.ListUsers(identity);
            IEnumerable<Tenant> ListTenants = identityProvider.ListTenants(identity);

            CI_ddl_ListUsers.DataSource = ListUsers;
            CI_ddl_ListUsers.DataTextField = "Username";
            CI_ddl_ListUsers.DataBind();

            CI_grid_Results1.DataSource = ListUsers;
            CI_grid_Results1.DataBind();

            CI_grid_Results3.DataSource = ListTenants;
            CI_grid_Results3.DataBind();
        }
        protected void CI_m_ListRoles()
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);

            IEnumerable<Role> ListRoles = identityProvider.ListRoles(null,null,null,identity);

            CI_grid_Results2.DataSource = ListRoles;
            CI_grid_Results2.DataBind();
        }
        protected void CI_m_ListTenants()
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);

            IEnumerable<Tenant> ListTenants = identityProvider.ListTenants(identity);

            CI_ddl_ListTenants.DataSource = ListTenants;
            CI_ddl_ListTenants.DataTextField = "Id";
            CI_ddl_ListTenants.DataBind();
        }
        protected void MessagePleaseSeleactRegion()
        {
            CI_lbl_Info.Text = "Please select a region: DFW, or ORD but not both.";
        }
        protected void MessageCatchException(string exception)
        {
            CI_lbl_Error.Text = "Something went terribly wrong! See below for more info. <br /> <br />" + exception;
        }
    }
}