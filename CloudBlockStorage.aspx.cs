using System;
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
    public partial class CloudBlockStorage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string region = CBS_ddl_Region.SelectedItem.ToString();

            HttpContext.Current.Session["CBSListVolumes"] = CBS_ddl_ListVolumes.Text;
            HttpContext.Current.Session["CBSListSnapshots"] = CBS_ddl_ListSnapShots.Text;

            try
            {
                if (string.IsNullOrEmpty((string)(Session["CloudIdentityUserName"])) & string.IsNullOrEmpty((string)(Session["CloudIdentityApiKey"])))
                {
                    CBS_lbl_Error.Text = "Before continuing please login and enter Cloud Username and API Key.";
                }
                else if (string.IsNullOrEmpty((string)(Session["CloudIdentityUserName"])))
                {
                    CBS_lbl_Error.Text = "Before continuing please login and please enter Cloud Username.";
                }
                else if (string.IsNullOrEmpty((string)(Session["CloudIdentityApiKey"])))
                {
                    CBS_lbl_Error.Text = "Before continuing please login and please enter API Key.";
                }
                else
                {
                    CBS_m_ListVolumes(region);
                    CBS_m_ListSnapShots(region);
                }
            }
            catch (Exception ex)
            {
                CBS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CBS_ddl_Region_SelectChange(object sender, EventArgs e)
        {
            string region = CBS_ddl_Region.SelectedItem.ToString();

            try
            {
                CBS_m_ListVolumes(region);
                CBS_m_ListSnapShots(region);
            }
            catch (Exception ex)
            {
                CBS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CBS_btn_CreateVolume_OnClick(object sender, EventArgs e)
        {
            string CBSCreateVolumeName = HttpUtility.HtmlEncode(CBS_txt_CreateVolumeName.Text);

            string region = CBS_ddl_Region.SelectedItem.ToString();

            int size = int.Parse(CBSCreateVolumeSizeDDL.SelectedValue);
            string description = HttpUtility.HtmlEncode(CBS_txt_CreateVolumeDescription.Text);
            string displayname = HttpUtility.HtmlEncode(CBS_txt_CreateVolumeDisplayname.Text);
            string volumetype = HttpUtility.HtmlEncode(CBS_ddl_CreateVolumeType.SelectedItem);

            try
            {
                CBS_m_CreateVolume(size, description, displayname, volumetype, region);
                CBS_m_MsgCreateVolumeSuccess();
                CBS_m_ListSnapShots(region);
            }
            catch (Exception ex)
            {
                CBS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CBS_btn_DeleteVolume_OnClick(object sender, EventArgs e)
        {
            string region = CBS_ddl_Region.SelectedItem.ToString();

            string CBSDeleteVolumeID = HttpUtility.HtmlEncode(CBS_ddl_ListVolumes.SelectedValue);

            try
            {
                CBS_m_DeleteVolume(CBSDeleteVolumeID, region);
                CBS_m_ListVolumes(region);
                CBS_m_MsgDeleteVolumeSuccess();
            }
            catch (Exception ex)
            {
                CBS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CBS_btn_ListSnapShots_OnClick(object sender, EventArgs e)
        {
            string region = CBS_ddl_Region.SelectedItem.ToString();

            try
            {
                CBS_m_ListSnapShots(region);
            }
            catch (Exception ex)
            {
                CBS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CBS_btn_CreateSnapShot_OnClick(object sender, EventArgs e)
        {
            string region = CBS_ddl_Region.SelectedItem.ToString();

            string CBSCreateSnapShotID = HttpUtility.HtmlEncode(CBS_ddl_ListVolumes.SelectedValue);
            string description = HttpUtility.HtmlEncode(CBS_txt_SnapShotDescription.Text);
            string displayname = HttpUtility.HtmlEncode(CBS_Txt_SnapshotDisplayName.Text);

            try
            {
                CBS_m_CreateSnapShot(CBSCreateSnapShotID, displayname, description, region);
                CBS_m_ListSnapShots(region);
                CBS_m_MsgCreateSnapShotSuccess();
            }
            catch (Exception ex)
            {
                CBS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CBS_btn_DeleteSnapShot_Click(object sender, EventArgs e)
        {
            string CBSDeleteSnapShotID = HttpUtility.HtmlEncode(CBS_ddl_ListSnapShots.SelectedValue);

            string region = CBS_ddl_Region.SelectedItem.ToString();

            try
            {
                CBS_m_DeleteSnapShot(CBSDeleteSnapShotID, region);
                CBS_m_ListSnapShots(region);
                CBS_m_MsgDeleteSnapShotSuccess();
            }
            catch (Exception ex)
            {
                CBS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CBS_m_ListVolumes(string dcregion)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudBlockStorageProvider CloudBlockStorageProvider = new net.openstack.Providers.Rackspace.CloudBlockStorageProvider(identity);

            IEnumerable<Volume> ListVolumes = CloudBlockStorageProvider.ListVolumes(dcregion);

            CBS_ddl_ListVolumes.DataSource = ListVolumes;
            CBS_ddl_ListVolumes.DataTextField = "DisplayName";
            CBS_ddl_ListVolumes.DataValueField = "Id";
            CBS_ddl_ListVolumes.DataBind();

            var GetVolumeID = ListVolumes.ToList();
            var GetVolumeID_SB = new StringBuilder();

            foreach (var i in GetVolumeID)
            {
                var attachments_var = i.Attachments;

                foreach (var item in attachments_var)
                {
                    GetVolumeID_SB.AppendLine(i.DisplayName + "<br />");

                    foreach (var item2 in item.Values)
                    {
                        GetVolumeID_SB.AppendLine(item2 + "<br />");
                    }

                    GetVolumeID_SB.AppendLine("<br />");
                }
            }

            CBS_lbl_Info.Text = GetVolumeID_SB.ToString();
        }
        protected void CBS_m_ListVolumesType(string dcregion)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudBlockStorageProvider CloudBlockStorageProvider = new net.openstack.Providers.Rackspace.CloudBlockStorageProvider(identity);

            IEnumerable<VolumeType> ListVolumeTypes = CloudBlockStorageProvider.ListVolumeTypes(dcregion);

            CBS_ddl_CreateVolumeType.DataSource = ListVolumeTypes;
            CBS_ddl_CreateVolumeType.DataTextField = "name";
            CBS_ddl_CreateVolumeType.DataBind();

            CBS_grid_Results2.DataSource = ListVolumeTypes;
            CBS_grid_Results2.DataBind();
        }
        protected void CBS_m_ListSnapShots(string dcregion)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudBlockStorageProvider CloudBlockStorageProvider = new net.openstack.Providers.Rackspace.CloudBlockStorageProvider(identity);

            IEnumerable<Snapshot> ListSnapshots = CloudBlockStorageProvider.ListSnapshots(dcregion);

            CBS_ddl_ListSnapShots.DataSource = ListSnapshots;
            CBS_ddl_ListSnapShots.DataTextField = "DisplayName";
            CBS_ddl_ListSnapShots.DataValueField = "Id";
            CBS_ddl_ListSnapShots.DataBind();

            CBS_grid_Results2.DataSource = ListSnapshots;
            CBS_grid_Results2.DataBind();
        }
        protected void CBS_m_CreateSnapShot(string snapshotid, string displayName, string displayDescription, string dcregion, bool force = false)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudBlockStorageProvider CloudBlockStorageProvider = new net.openstack.Providers.Rackspace.CloudBlockStorageProvider(identity);

            var CreateSnapShot = CloudBlockStorageProvider.CreateSnapshot(snapshotid, force, displayName, displayDescription, dcregion);
        }
        protected void CBS_m_DeleteSnapShot(string snapshotid, string dcregion)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudBlockStorageProvider CloudBlockStorageProvider = new net.openstack.Providers.Rackspace.CloudBlockStorageProvider(identity);

            var DeleteSnapShot = CloudBlockStorageProvider.DeleteSnapshot(snapshotid, dcregion);
        }
        protected void CBS_m_CreateVolume(int size, string display_description, string displayname, string volumetype, string dcregion)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudBlockStorageProvider CloudBlockStorageProvider = new net.openstack.Providers.Rackspace.CloudBlockStorageProvider(identity);

            var CBSCreateVolume = CloudBlockStorageProvider.CreateVolume(size, display_description, displayname, null, volumetype, dcregion);

            CBS_m_ListVolumes(dcregion);
        }
        protected void CBS_m_DeleteVolume(string volumeid, string dcregion)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudBlockStorageProvider CloudBlockStorageProvider = new net.openstack.Providers.Rackspace.CloudBlockStorageProvider(identity);

            var CBSDeleteVolume = CloudBlockStorageProvider.DeleteVolume(volumeid, dcregion);

            CBS_m_ListVolumes(dcregion);
        }
        protected void CBS_m_MsgListVolumeSuccess()
        {
            CBS_lbl_Info.Text = "Volumes have been listed successfully";
        }
        protected void CBS_m_MsgCreateVolumeSuccess()
        {
            CBS_lbl_Info.Text = "Volume has been created successfully";
        }
        protected void CBS_m_MsgDeleteVolumeSuccess()
        {
            CBS_lbl_Info.Text = "Volume has been deleted successfully";
        }
        protected void CBS_m_MsgCreateSnapShotSuccess()
        {
            CBS_lbl_Info.Text = "SnapShot has been created successfully";
        }
        protected void CBS_m_MsgDeleteSnapShotSuccess()
        {
            CBS_lbl_Info.Text = "SnapShot has been deleted successfully";
        }
        protected void CBS_m_MsgPleaseSeleactRegion()
        {
            CBS_lbl_Info.Text = "Please select a region: DFW, or ORD but not both.";
        }
        protected void CBS_m_MsgCatchException(string exception)
        {
            CBS_lbl_Info.Text = "Something went terribly wrong! See below for more info. <br /> <br />" + exception;
        }
    }
}