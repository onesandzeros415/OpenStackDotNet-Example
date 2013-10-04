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
using net.openstack.Providers.Rackspace.Objects;

namespace OpenStackDotNet_Test
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty((string)(Session["CloudIdentityUserName"])))
            {
                login_btn.Visible = true;
                logout_btn.Visible = false;
            }
            else
            {
                username_txt.Visible = false;
                apikey_txt.Visible = false;
                login_btn.Visible = false;
                CIUsernameText.Visible = false;
                CIApiKeyText.Visible = false;
                logout_btn.Visible = true;
            }
        }
        protected void logout_OnClick(object sender, EventArgs e)
        {
            Session.Remove("CloudIdentityUserName");
            Session.Remove("CloudIdentityApiKey");

            Response.Redirect(Request.RawUrl);
        }
        protected void CILogin_Click(object sender, EventArgs e)
        {
            string CloudIdentityUserName = CIUsernameText.Text;
            string CloudIdentityApiKey = CIApiKeyText.Text;

            try
            {
                RackspaceCloudIdentity identity = new RackspaceCloudIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

                CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);

                var userAccess = Identity.identityProvider().Authenticate(identity);

                if (userAccess.User != null)
                {
                    Session["CloudIdentityUserName"] = CIUsernameText.Text;
                    Session["CloudIdentityApiKey"] = CIApiKeyText.Text;

                    CIUsernameText.Text = string.Empty;
                    CIApiKeyText.Text = string.Empty;

                    Response.Redirect(Request.RawUrl);
                }
                else
                {
                    LblInfo.Text = "Username or Password is Incorrect.  Please try again.";
                }
            }
            catch (Exception ex)
            {
                MessageCatchException("Username or Password is Incorrect.  Please try again.");
            }
        }
        protected void MessageCatchException(string exception)
        {
            Error.Text = "Something went terribly wrong! See below for more info. <br /> <br />" + exception;
        }
    }
}