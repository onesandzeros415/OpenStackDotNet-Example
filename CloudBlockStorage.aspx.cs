using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static string CBSListVolumesSession() { return (string)(HttpContext.Current.Session["CBSListVolumes"]); }

        protected void Page_Load(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string region = CBS_ddl_Region.SelectedItem.ToString();

            HttpContext.Current.Session["CBSListVolumes"] = HttpUtility.HtmlEncode(CBS_ddl_ListVolumes.SelectedItem);
            HttpContext.Current.Session["CBSListSnapshots"] = CBS_ddl_ListSnapShots.Text;
            

            Page.GetPostBackEventReference(CBS_btn_CreateVolume);

            try
            {
                if (Page.IsPostBack)
                {

                }
                else
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
                        StringBuilder listVolumesSB = new StringBuilder();
                        var listVolumes = BlockStorage.CBS_m_ListVolumes(region);
                        bindListNetworksDDL(listVolumes, "DisplayName", "Id");
                        foreach (var volume in listVolumes) { listVolumesSB.Append(volume.Status); }
                        
                        CBS_lbl_VolumeStatus.Text = "Volume Status : " + listVolumesSB.ToString();

                        StringBuilder listSnapshotsSB = new StringBuilder();
                        var listSnapshots = BlockStorage.CBS_m_ListSnapShots(region);
                        bindListSnapShotsDDL(listSnapshots, "DisplayName", "Id");
                        foreach (var snapshot in listSnapshots) { listSnapshotsSB.Append(snapshot); }
                        
                        CBS_lbl_SnapshotStatus.Text = "Snapshot Status : " + listSnapshotsSB.ToString();

                        CBS_lbl_Info.Text = CBS_m_GetVolumeinfo(BlockStorage.CBS_m_ListVolumes(region));

                        bindListVolumeTypeDDL(BlockStorage.CBS_m_ListVolumesType(region), "Name");
                        
                        TimeClock.Stop();
                        CBS_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                CBS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CBS_ddl_Region_SelectChange(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string region = CBS_ddl_Region.SelectedItem.ToString();

            try
            {
                StringBuilder listVolumesSB = new StringBuilder();
                var listVolumes = BlockStorage.CBS_m_ListVolumes(region);
                bindListNetworksDDL(listVolumes, "DisplayName", "Id");
                foreach (var volume in listVolumes) { listVolumesSB.Append(volume.Status); }

                CBS_lbl_VolumeStatus.Text = "Volume Status : " + listVolumesSB.ToString();

                StringBuilder listSnapshotsSB = new StringBuilder();
                var listSnapshots = BlockStorage.CBS_m_ListSnapShots(region);
                bindListSnapShotsDDL(listSnapshots, "DisplayName", "Id");
                foreach (var snapshot in listSnapshots) { listSnapshotsSB.Append(snapshot.Status); }

                CBS_lbl_SnapshotStatus.Text = "Snapshot Status : " + listSnapshotsSB.ToString();

                CBS_lbl_Info.Text = CBS_m_GetVolumeinfo(BlockStorage.CBS_m_ListVolumes(region));

                bindListVolumeTypeDDL(BlockStorage.CBS_m_ListVolumesType(region), "Name");

                TimeClock.Stop();
                CBS_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CBS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CBS_ddl_ListVolumes_SelectChange(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string region = CBS_ddl_Region.SelectedItem.ToString();

            try
            {
                bindListNetworksSessionDDL(BlockStorage.CBS_m_ListVolumes(region), "DisplayName", "Id", CBSListVolumesSession());

                CBS_lbl_Info.Text = CBS_m_GetVolumeinfo(BlockStorage.CBS_m_ListVolumes(region));

                bindListSnapShotsDDL(BlockStorage.CBS_m_ListSnapShots(region), "DisplayName", "Id");
                bindListVolumeTypeDDL(BlockStorage.CBS_m_ListVolumesType(region), "Name");

                TimeClock.Stop();
                CBS_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CBS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CBS_btn_CreateVolume_OnClick(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string CBSCreateVolumeName = HttpUtility.HtmlEncode(CBS_txt_CreateVolumeName.Text);

            string region = CBS_ddl_Region.SelectedItem.ToString();

            int size = int.Parse(CBSCreateVolumeSizeDDL.SelectedValue);
            string description = CBS_txt_CreateVolumeDescription.Text;
            string displayname = CBS_txt_CreateVolumeDisplayname.Text;
            string volumetype = CBS_ddl_ListVolumeType.SelectedItem.ToString();

            try
            {
                BlockStorage.CBS_m_CreateVolume(size, description, displayname, volumetype, region);
                CBS_m_MsgCreateVolumeSuccess();
                bindListSnapShotsDDL(BlockStorage.CBS_m_ListSnapShots(region), "DisplayName", "Id");

                TimeClock.Stop();
                CBS_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CBS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CBS_btn_DeleteVolume_OnClick(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string region = CBS_ddl_Region.SelectedItem.ToString();
            string CBSDeleteVolumeID = CBS_ddl_ListVolumes.SelectedValue;

            try
            {
                BlockStorage.CBS_m_DeleteVolume(CBSDeleteVolumeID, region);
                BlockStorage.CBS_m_ListVolumes(region);
                BlockStorage.CBS_m_ListVolumesType(region);
                CBS_m_MsgDeleteVolumeSuccess();

                TimeClock.Stop();
                CBS_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CBS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CBS_btn_ListSnapShots_OnClick(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string region = CBS_ddl_Region.SelectedItem.ToString();

            try
            {
                bindListSnapShotsDDL(BlockStorage.CBS_m_ListSnapShots(region), "DisplayName", "Id");

                TimeClock.Stop();
                CBS_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CBS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CBS_btn_CreateSnapShot_OnClick(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string region = CBS_ddl_Region.SelectedItem.ToString();
            string CBSCreateSnapShotID = CBS_ddl_ListVolumes.SelectedValue;
            string description = CBS_txt_SnapShotDescription.Text;
            string displayname = CBS_Txt_SnapshotDisplayName.Text;

            try
            {
                BlockStorage.CBS_m_CreateSnapShot(CBSCreateSnapShotID, displayname, description, region);
                bindListSnapShotsDDL(BlockStorage.CBS_m_ListSnapShots(region), "DisplayName", "Id");
                CBS_m_MsgCreateSnapShotSuccess();

                TimeClock.Stop();
                CBS_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CBS_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CBS_btn_DeleteSnapShot_Click(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string CBSDeleteSnapShotID = CBS_ddl_ListSnapShots.SelectedValue;
            string region = CBS_ddl_Region.SelectedItem.ToString();

            try
            {
                BlockStorage.CBS_m_DeleteSnapShot(CBSDeleteSnapShotID, region);
                bindListSnapShotsDDL(BlockStorage.CBS_m_ListSnapShots(region), "DisplayName", "Id");
                CBS_m_MsgDeleteSnapShotSuccess();

                TimeClock.Stop();
                CBS_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CBS_m_MsgCatchException(ex.ToString());
            }
        }
        protected CloudBlockStorageProvider blockStorageProvider()
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            RackspaceImpersonationIdentity identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudBlockStorageProvider CloudBlockStorageProvider = new net.openstack.Providers.Rackspace.CloudBlockStorageProvider(identity);

            return CloudBlockStorageProvider;
        }

        protected string CBS_m_GetVolumeinfo(IEnumerable<Volume> dataSource)
        {
            var GetVolumeID = dataSource.ToList();
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

            return GetVolumeID_SB.ToString();
        }
        protected void bindListNetworksDDL(object dataSource, string dataTextField, string dataValueField)
        {
            CBS_ddl_ListVolumes.DataSource = dataSource;
            CBS_ddl_ListVolumes.DataTextField = dataTextField;
            CBS_ddl_ListVolumes.DataValueField = dataValueField;
            CBS_ddl_ListVolumes.DataBind();
        }
        protected void bindListNetworksSessionDDL(object dataSource, string dataTextField, string dataValueField, string selectedSessionIndex)
        {
            CBS_ddl_ListVolumes.DataSource = dataSource;
            CBS_ddl_ListVolumes.DataTextField = dataTextField;
            CBS_ddl_ListVolumes.DataValueField = dataValueField;
            CBS_ddl_ListVolumes.SelectedIndex = CBS_ddl_ListVolumes.Items.IndexOf(CBS_ddl_ListVolumes.Items.FindByText(selectedSessionIndex));
            CBS_ddl_ListVolumes.DataBind();
        }
        protected void bindListSnapShotsDDL(object dataSource, string dataTextField, string dataValueField)
        {
            CBS_ddl_ListSnapShots.DataSource = dataSource;
            CBS_ddl_ListSnapShots.DataTextField = dataTextField;
            CBS_ddl_ListSnapShots.DataValueField = dataValueField;
            CBS_ddl_ListSnapShots.DataBind();
        }
        protected void bindListVolumeTypeDDL(object dataSource, string dataTextField)
        {
            CBS_ddl_ListVolumeType.DataSource = dataSource;
            CBS_ddl_ListVolumeType.DataTextField = dataTextField;
            CBS_ddl_ListVolumeType.DataBind();
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