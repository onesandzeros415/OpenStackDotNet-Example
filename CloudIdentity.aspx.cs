using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            System.Net.ServicePointManager.DefaultConnectionLimit = 50;

            Page.GetPostBackEventReference(CI_btn_NewUser);

            HttpContext.Current.Session["CIListUserDDL"] = HttpUtility.HtmlEncode(CI_ddl_ListUsers.SelectedItem);

            string CiListUsersSession = (string)(HttpContext.Current.Session["CIListUserDDL"]);

            try
            {
                if (Page.IsPostBack)
                {

                }
                else
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
                        object ListUsers = Identity.CI_m_ListUsers();
                        bindListUsersDDL(ListUsers, "Username", "Id", CiListUsersSession);
                        bindGridResults(ListUsers);

                        object ListRoles = Identity.CI_m_ListRoles();
                        bindListRolesDDL(ListRoles, "Name", "Id");

                        object ListTenants = Identity.CI_m_ListTenants();
                        bindGridResults3(ListTenants);

                        object ListUserRole = Identity.CI_m_ListUserRoles(CI_ddl_ListUsers.SelectedValue);
                        bindListRolesUserInDDL(ListUserRole, "Key", "Value");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageCatchException(ex.ToString());
            }
        }
        protected void CI_ddl_ListUsers_SelectChange(object sender, EventArgs e)
        {
            string CiListUsersSession = (string)(HttpContext.Current.Session["CIListUserDDL"]);

            try
            {
                object ListUsers = Identity.CI_m_ListUsers();
                bindListUsersDDL(ListUsers, "Username", "Id", CiListUsersSession);
                bindGridResults(ListUsers);

                object ListRoles = Identity.CI_m_ListRoles();
                bindListRolesDDL(ListRoles, "Name", "Id");

                object ListTenants = Identity.CI_m_ListTenants();
                bindGridResults3(ListTenants);

                object ListUserRole = Identity.CI_m_ListUserRoles(CI_ddl_ListUsers.SelectedValue);
                bindListRolesUserInDDL(ListUserRole, "Key", "Value");
            }
            catch (Exception ex)
            {
                MessageCatchException(ex.ToString());
            }
        }
        protected void CI_btn_NewUser_OnClick(object sender, EventArgs e)
        {
            bool enabled = true;
            string CiListUsersSession = (string)(HttpContext.Current.Session["CIListUserDDL"]);

            try
            {
                NewUser user = Identity.CI_m_NewUser(CI_txt_NewUser.Text, CI_txt_Passwd.Text, CI_txt_Email.Text, enabled);

                object ListUsers = Identity.CI_m_ListUsers();
                bindListUsersDDL(ListUsers, "Username", "Id", CiListUsersSession);
                bindGridResults(ListUsers);

                object ListRoles = Identity.CI_m_ListRoles();
                bindListRolesDDL(ListRoles, "Name", "Id");

                object ListTenants = Identity.CI_m_ListTenants();
                bindGridResults3(ListTenants);

                object ListUserRole = Identity.CI_m_ListUserRoles(CI_ddl_ListUsers.SelectedValue);
                bindListRolesUserInDDL(ListUserRole, "Key", "Value");


                if (user.Enabled)
                {
                    CI_lbl_Info.Text = "success";
                }
                else
                {
                    CI_lbl_Info.Text = "user add failed";
                }
            }
            catch (Exception ex)
            {
                MessageCatchException(ex.ToString());
            }
        }
        protected void CI_btn_DeleteUser_OnClick(object sender, EventArgs e)
        {
            string CiListUsersSession = (string)(HttpContext.Current.Session["CIListUserDDL"]);

            try
            {
                var response = Identity.CI_m_DeleteUser(CI_ddl_ListUsers.SelectedValue);

                object ListUsers = Identity.CI_m_ListUsers();
                bindListUsersDDL(ListUsers, "Username", "Id", CiListUsersSession);
                bindGridResults(ListUsers);

                object ListRoles = Identity.CI_m_ListRoles();
                bindListRolesDDL(ListRoles, "Name", "Id");

                object ListTenants = Identity.CI_m_ListTenants();
                bindGridResults3(ListTenants);

                object ListUserRole = Identity.CI_m_ListUserRoles(CI_ddl_ListUsers.SelectedValue);
                bindListRolesUserInDDL(ListUserRole, "Key", "Value");

                if (response == true)
                {
                    CI_lbl_Info.Text = "User : " + CI_ddl_ListUsers.SelectedValue + " has been removed successfully.";
                }
                else
                {
                    CI_lbl_Info.Text = "User : " + CI_ddl_ListUsers.SelectedValue + "has failed being remoed.  Try again.";
                }
            }
            catch (Exception ex)
            {
                MessageCatchException(ex.ToString());
            }
        }
        protected void CI_btn_AddRoleToUser_OnClick(object sender, EventArgs e)
        {
            string CiListUsersSession = (string)(HttpContext.Current.Session["CIListUserDDL"]);

            try
            {
                CI_m_ClearSessions();
                var response = Identity.CI_m_AddRoleToUser(CI_ddl_ListUsers.SelectedValue, CI_ddl_ListRoles.SelectedValue);

                object ListUsers = Identity.CI_m_ListUsers();
                bindListUsersDDL(ListUsers, "Username", "Id", CiListUsersSession);
                bindGridResults(ListUsers);

                object ListRoles = Identity.CI_m_ListRoles();
                bindListRolesDDL(ListRoles, "Name", "Id");

                object ListTenants = Identity.CI_m_ListTenants();
                bindGridResults3(ListTenants);

                object ListUserRole = Identity.CI_m_ListUserRoles(CI_ddl_ListUsers.SelectedValue);
                bindListRolesUserInDDL(ListUserRole, "Key", "Value");

                if (response == true)
                {
                    CI_lbl_Info.Text = "New role : " + CI_ddl_ListRoles.SelectedValue + " added to user " + CI_ddl_ListUsers.SelectedValue + " successfully.";
                }
                else
                {
                    CI_lbl_Info.Text = "New role : " + CI_ddl_ListRoles.SelectedValue + " has failed being added to user " + CI_ddl_ListUsers.SelectedValue + ". Try again.";
                }
            }
            catch (Exception ex)
            {
                MessageCatchException(ex.ToString());
            }
        }
        protected void CI_btn_RemoveRoleFromUser_OnClick(object sender, EventArgs e)
        {
            string CiListUsersSession = (string)(HttpContext.Current.Session["CIListUserDDL"]);

            try
            {
                var response = Identity.CI_m_DeleteRoleFromUser(CI_ddl_ListUsers.SelectedValue, CI_ddl_ListRoleUserIn.SelectedValue);

                object ListUsers = Identity.CI_m_ListUsers();
                bindListUsersDDL(ListUsers, "Username", "Id", CiListUsersSession);
                bindGridResults(ListUsers);

                object ListRoles = Identity.CI_m_ListRoles();
                bindListRolesDDL(ListRoles, "Name", "Id");

                object ListTenants = Identity.CI_m_ListTenants();
                bindGridResults3(ListTenants);

                object ListUserRole = Identity.CI_m_ListUserRoles(CI_ddl_ListUsers.SelectedValue);
                bindListRolesUserInDDL(ListUserRole, "Key", "Value");

                if (response == true)
                {
                    CI_lbl_Info.Text = "Role : " + CI_ddl_ListRoles.SelectedValue + " removed from user " + CI_ddl_ListUsers.SelectedValue + " successfully.";
                }
                else
                {
                    CI_lbl_Info.Text = "Role : " + CI_ddl_ListRoles.SelectedValue + " has failed being removed from user " + CI_ddl_ListUsers.SelectedValue + ". Try again.";
                }
            }
            catch (Exception ex)
            {
                MessageCatchException(ex.ToString());
            }
        }
        protected void bindListUsersDDL(object dataSource, string dataTextField, string dataValueField, string selectedSessionIndex)
        {
            CI_ddl_ListUsers.DataSource = dataSource;
            CI_ddl_ListUsers.DataTextField = dataTextField;
            CI_ddl_ListUsers.DataValueField = dataValueField;
            CI_ddl_ListUsers.SelectedIndex = CI_ddl_ListUsers.Items.IndexOf(CI_ddl_ListUsers.Items.FindByText(selectedSessionIndex));
            CI_ddl_ListUsers.DataBind();
        }
        protected void bindGridResults(object dataSource)
        {
            CI_grid_Results1.DataSource = dataSource;
            CI_grid_Results1.DataBind();
        }
        protected void bindGridResults3(object dataSource)
        {
            CI_grid_Results3.DataSource = dataSource;
            CI_grid_Results3.DataBind();
        }
        protected void bindListRolesDDL(object dataSource, string dataTextField, string dataValueField)
        {
            CI_ddl_ListRoles.DataSource = dataSource;
            CI_ddl_ListRoles.DataTextField = dataTextField;
            CI_ddl_ListRoles.DataValueField = dataValueField;
            CI_ddl_ListRoles.DataBind();
        }
        protected void bindListRolesUserInDDL(object dataSource, string dataTextField, string dataValueField)
        {
            CI_ddl_ListRoleUserIn.DataSource = dataSource;
            CI_ddl_ListRoleUserIn.DataTextField = dataTextField;
            CI_ddl_ListRoleUserIn.DataValueField = dataValueField;
            CI_ddl_ListRoleUserIn.DataBind();
        }
        protected void CI_m_ClearSessions()
        {
            Session.Remove("CIListUserDDL");
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