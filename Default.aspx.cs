using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty((string)(Session["CloudIdentityUserName"])) & string.IsNullOrEmpty((string)(Session["CloudIdentityApiKey"])))
                {
                    Lbl_DefaultRegion.Visible = false;
                    LblDefaultRegionPrefab.Visible = false;
                    Error.Text = "Before continuing please login and enter Cloud Username and API Key.";
                }
                else if (string.IsNullOrEmpty((string)(Session["CloudIdentityUserName"])))
                {
                    Lbl_DefaultRegion.Visible = false;
                    LblDefaultRegionPrefab.Visible = false;
                    Error.Text = "Before continuing please login and please enter Cloud Username.";
                }
                else if (string.IsNullOrEmpty((string)(Session["CloudIdentityApiKey"])))
                {
                    Lbl_DefaultRegion.Visible = false;
                    LblDefaultRegionPrefab.Visible = false;
                    Error.Text = "Before continuing please login and please enter API Key.";
                }
                else
                {
                    CIProviderDefaultRegion();
                }
            }
            catch (Exception ex)
            {
                MessageCatchException(ex.ToString());
            }
        }
        protected void CIProviderDefaultRegion()
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            RackspaceCloudIdentity identity = new RackspaceCloudIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);

            IEnumerable<User> ListUsers = identityProvider.ListUsers(identity);
            IEnumerable<Tenant> ListTenants = identityProvider.ListTenants(identity);
            var DefaultRegion_SB = new StringBuilder();

            foreach (var i in ListUsers)
            {
                DefaultRegion_SB.Append(Path.Combine(i.Username, i.DefaultRegion) + "<br />");
            }

            Lbl_DefaultRegion.Text = DefaultRegion_SB.ToString();

            Lbl_DefaultRegion.Visible = true;
            LblDefaultRegionPrefab.Visible = true;
        }
        protected void MessageCloudLoginSuccess()
        {
            LblInfo.Text = " You have successfully been logged in.";
        }
        protected void MessageCatchException(string exception)
        {
            Error.Text = "Something went terribly wrong! See below for more info. <br /> <br />" + exception;
        }
    }
}