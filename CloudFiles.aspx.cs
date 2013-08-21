using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
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
    public partial class CloudFiles : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string containername = (string)(Session["ContainerName"]);

            string region = CF_ddl_Region.SelectedItem.ToString();

            bool snetTrue = bool.Parse(CF_cb_SnetCheck.Text);
            bool snetFalse = bool.Parse("false");

            try
            {
                if (IsPostBack)
                {
                    HttpContext.Current.Session["ContainerName"] = CF_ddl_ListContainer.Text;
                    HttpContext.Current.Session["ContainerContents"] = CF_ddl_ListContainerContents.Text;
                    HttpContext.Current.Session["Region"] = CF_ddl_Region.SelectedValue;

                    ClientScript.RegisterClientScriptBlock(GetType(), "IsPostBack", "var isPostBack = true;", true);
                }
                else
                {
                    if (string.IsNullOrEmpty((string)(Session["CloudIdentityUserName"])) & string.IsNullOrEmpty((string)(Session["CloudIdentityApiKey"])))
                    {
                        ClientScript.RegisterClientScriptBlock(GetType(), "IsPostBack", "var isPostBack = true;", true);

                        CF_lbl_Error.Text = "Before continuing please login and enter Cloud Username and API Key.";
                    }
                    else if (string.IsNullOrEmpty((string)(Session["CloudIdentityUserName"])))
                    {
                        ClientScript.RegisterClientScriptBlock(GetType(), "IsPostBack", "var isPostBack = true;", true);

                        CF_lbl_Error.Text = "Before continuing please login and please enter Cloud Username.";
                    }
                    else if (string.IsNullOrEmpty((string)(Session["CloudIdentityApiKey"])))
                    {
                        ClientScript.RegisterClientScriptBlock(GetType(), "IsPostBack", "var isPostBack = true;", true);

                        CF_lbl_Error.Text = "Before continuing please login and please enter API Key.";
                    }
                    else
                    {
                        if (CF_ddl_ListContainer.SelectedItem == null & CF_cb_SnetCheck.Checked)
                        {
                            CF_m_ListContainers(region, snetTrue);
                            CF_m_AccountDetails(CF_ddl_ListContainer.Text, region, snetTrue);
                            CF_m_ContainerDetails(CF_ddl_ListContainer.Text, region, snetTrue);
                            CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                        }
                        else if (CF_ddl_ListContainer.SelectedItem == null)
                        {
                            CF_m_ListContainers(region, snetFalse);
                            CF_m_AccountDetails(CF_ddl_ListContainer.SelectedValue, region, snetFalse);
                            CF_m_ContainerDetails(CF_ddl_ListContainer.Text, region, snetFalse);
                            CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                        }
                        else if (CF_ddl_ListContainer.SelectedItem != null & CF_cb_SnetCheck.Checked)
                        {
                            CF_m_ListContainers(region, snetTrue);
                            CF_m_ListContainerObjects(containername, region, snetTrue);
                            CF_m_AccountDetails(CF_ddl_ListContainer.Text, region, snetTrue);
                            CF_m_ContainerDetails(CF_ddl_ListContainer.Text, region, snetTrue);
                            CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                        }
                        else
                        {
                            CF_m_ListContainers(region, snetFalse);
                            CF_m_AccountDetails(CF_ddl_ListContainer.Text, region, snetFalse);
                            CF_m_ContainerDetails(CF_ddl_ListContainer.Text, region, snetFalse);
                            CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                CF_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CF_ddl_ListContainer_SelectChange(object sender, EventArgs e)
        {
            string containername = HttpUtility.HtmlEncode(CF_ddl_ListContainer.Text);
            string region = CF_ddl_Region.SelectedItem.ToString();

            bool snetTrue = bool.Parse(CF_cb_SnetCheck.Text);
            bool snetFalse = bool.Parse("false");

            try
            {
                if (IsPostBack)
                {
                    if (CF_ddl_ListContainer.SelectedItem == null & CF_cb_SnetCheck.Checked)
                    {
                        CF_m_ListContainerObjects(containername, region, snetTrue);
                        CF_m_AccountDetails(CF_ddl_ListContainer.Text, region, snetTrue);
                        CF_m_ContainerDetails(CF_ddl_ListContainer.Text, region, snetTrue);
                        CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                    }
                    else if (CF_ddl_ListContainer.SelectedItem == null)
                    {
                        CF_m_ListContainerObjects(containername, region, snetFalse);
                        CF_m_AccountDetails(CF_ddl_ListContainer.Text, region, snetFalse);
                        CF_m_ContainerDetails(CF_ddl_ListContainer.Text, region, snetFalse);
                        CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                    }
                    else if (CF_ddl_ListContainer.SelectedItem != null & CF_cb_SnetCheck.Checked)
                    {
                        CF_m_ListContainerObjects(containername, region, snetTrue);
                        CF_m_AccountDetails(CF_ddl_ListContainer.Text, region, snetTrue);
                        CF_m_ContainerDetails(CF_ddl_ListContainer.Text, region, snetTrue);
                        CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                    }
                    else
                    {
                        CF_m_ListContainerObjects(containername, region, snetFalse);
                        CF_m_AccountDetails(CF_ddl_ListContainer.Text, region, snetFalse);
                        CF_m_ContainerDetails(CF_ddl_ListContainer.Text, region, snetFalse);
                        CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                    }
                }
            }
            catch (Exception ex)
            {
                CF_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CF_ddl_Region_SelectChange(object sender, EventArgs e)
        {
            string region = CF_ddl_Region.SelectedItem.ToString();

            string cslistserverregion = (string)(Session["CSListServersRegion"]);
            string session_containername = (string)(Session["ContainerName"]);
            string session_containercontents = (string)(Session["ContainerContents"]);
            string session_region = (string)(Session["Region"]);

            bool snetTrue = bool.Parse(CF_cb_SnetCheck.Text);
            bool snetFalse = bool.Parse("false");

            try
            {
                if (string.IsNullOrEmpty(session_containername))
                {
                    if (CF_ddl_ListContainer.SelectedItem == null & CF_cb_SnetCheck.Checked)
                    {
                        CF_m_ListContainers(region, snetTrue);
                        CF_m_AccountDetails(CF_ddl_ListContainer.Text, region, snetTrue);
                        CF_m_ContainerDetails(CF_ddl_ListContainer.Text, region, snetTrue);
                        CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                    }
                    else if (CF_ddl_ListContainer.SelectedItem == null)
                    {
                        CF_m_ListContainers(region, snetFalse);
                        CF_m_AccountDetails(CF_ddl_ListContainer.Text, region, snetFalse);
                        CF_m_ContainerDetails(CF_ddl_ListContainer.Text, region, snetFalse);
                        CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                    }
                    else if (CF_ddl_ListContainer.SelectedItem != null & CF_cb_SnetCheck.Checked)
                    {
                        CF_m_ListContainers(region, snetTrue);
                        CF_m_ListContainerObjects(session_containername, region, snetTrue);
                        CF_m_AccountDetails(CF_ddl_ListContainer.Text, region, snetTrue);
                        CF_m_ContainerDetails(CF_ddl_ListContainer.Text, region, snetTrue);
                        CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                    }
                    else
                    {
                        CF_m_ListContainers(region, snetFalse);
                        CF_m_AccountDetails(CF_ddl_ListContainer.Text, region, snetFalse);
                        CF_m_ContainerDetails(CF_ddl_ListContainer.Text, region, snetFalse);
                        CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                    }
                }
                else
                {
                    if (CF_ddl_ListContainer.SelectedItem == null & CF_cb_SnetCheck.Checked)
                    {
                        CF_m_ListContainers(region, snetTrue);
                        CF_m_AccountDetails(CF_ddl_ListContainer.Text, region, snetTrue);
                        CF_m_ContainerDetails(CF_ddl_ListContainer.Text, region, snetTrue);
                        CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                    }
                    else if (CF_ddl_ListContainer.SelectedItem == null)
                    {
                        CF_m_ListContainers(region, snetFalse);
                        CF_m_AccountDetails(CF_ddl_ListContainer.Text, region, snetFalse);
                        CF_m_ContainerDetails(CF_ddl_ListContainer.Text, region, snetFalse);
                        CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                    }
                    else if (CF_ddl_ListContainer.SelectedItem != null & CF_cb_SnetCheck.Checked)
                    {
                        CF_m_ListContainers(region, snetTrue);
                        CF_m_ListContainerObjects(session_containername, region, snetTrue);
                        CF_m_AccountDetails(CF_ddl_ListContainer.Text, region, snetTrue);
                        CF_m_ContainerDetails(CF_ddl_ListContainer.Text, region, snetTrue);
                        CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                    }
                    else
                    {
                        CF_m_MsgClearDDLs();
                        CF_m_MsgClearSessions();
                        CF_m_ListContainers(region, snetFalse);
                        CF_m_AccountDetails(CF_ddl_ListContainer.Text, region, snetFalse);
                        CF_m_ContainerDetails(CF_ddl_ListContainer.Text, region, snetFalse);
                        CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                    }
                }
            }
            catch (Exception ex)
            {
                CF_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CF_btn_CreateContainer_OnClick(object sender, EventArgs e)
        {
            string containername = HttpUtility.HtmlEncode(CF_ddl_ListContainer.Text);
            string region = CF_ddl_Region.SelectedItem.ToString();

            bool snetTrue = bool.Parse(CF_cb_SnetCheck.Text);
            bool snetFalse = bool.Parse("false");

            try
            {
                if (CF_ddl_ListContainer.SelectedItem == null & CF_cb_SnetCheck.Checked)
                {
                    CF_m_CreateContainer(CF_txt_CreateContainer.Text, region, snetTrue);
                    CF_m_ListContainers(region, snetTrue);
                    CF_m_MsgCreateContainer(CF_txt_CreateContainer.Text);
                }
                else if (CF_ddl_ListContainer.SelectedItem == null)
                {
                    CF_m_CreateContainer(CF_txt_CreateContainer.Text, region, snetFalse);
                    CF_m_ListContainers(region, snetFalse);
                    CF_m_MsgCreateContainer(CF_txt_CreateContainer.Text);
                }
                else if (CF_ddl_ListContainer.SelectedItem != null & CF_cb_SnetCheck.Checked)
                {
                    CF_m_CreateContainer(CF_txt_CreateContainer.Text, region, snetTrue);
                    CF_m_ListContainers(region, snetTrue);
                    CF_m_MsgCreateContainer(CF_txt_CreateContainer.Text);
                }
                else
                {
                    CF_m_CreateContainer(CF_txt_CreateContainer.Text, region, snetFalse);
                    CF_m_ListContainers(region, snetFalse);
                    CF_m_MsgCreateContainer(CF_txt_CreateContainer.Text);
                }
            }
            catch (Exception ex)
            {
                CF_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CF_btn_DeleteContainer_OnClick(object sender, EventArgs e)
        {
            string containername = HttpUtility.HtmlEncode(CF_ddl_ListContainer.Text);
            string region = CF_ddl_Region.SelectedItem.ToString();

            bool snetTrue = bool.Parse(CF_cb_SnetCheck.Text);
            bool snetFalse = bool.Parse("false");

            try
            {
                if (CF_ddl_ListContainer.SelectedItem == null & CF_cb_SnetCheck.Checked)
                {
                    CF_m_DeleteContainer(containername, region, snetTrue);
                    CF_m_MsgClearSessions();
                    CF_m_ListContainers(region, snetTrue);
                    CF_m_MsgDeleteContainer(containername);
                }
                else if (CF_ddl_ListContainer.SelectedItem == null)
                {
                    CF_m_DeleteContainer(containername, region, snetFalse);
                    CF_m_MsgClearSessions();
                    CF_m_ListContainers(region, snetFalse);
                    CF_m_MsgDeleteContainer(containername);
                }
                else if (CF_ddl_ListContainer.SelectedItem != null & CF_cb_SnetCheck.Checked)
                {
                    CF_m_DeleteContainer(containername, region, snetTrue);
                    CF_m_MsgClearSessions();
                    CF_m_ListContainers(region, snetTrue);
                    CF_m_MsgDeleteContainer(containername);
                }
                else
                {
                    CF_m_DeleteContainer(containername, region, snetFalse);
                    CF_m_MsgClearSessions();
                    CF_m_ListContainers(region, snetFalse);
                    CF_m_MsgDeleteContainer(containername);
                }
            }
            catch (Exception ex)
            {
                CF_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CF_btn_DeleteContainerObject_OnClick(object sender, EventArgs e)
        {

            string containername = HttpUtility.HtmlEncode(CF_ddl_ListContainer.Text);

            string region = CF_ddl_Region.SelectedItem.ToString();

            bool snetTrue = bool.Parse(CF_cb_SnetCheck.Text);
            bool snetFalse = bool.Parse("false");

            try
            {
                if (CF_ddl_ListContainer.SelectedItem == null & CF_cb_SnetCheck.Checked)
                {
                    CF_m_DeleteContainerObject(containername, CF_ddl_ListContainerContents.SelectedValue, region, snetTrue);
                    CF_m_ListContainerObjects(containername, region, snetTrue);
                    CF_m_MsgDeleteContainerObject(CF_ddl_ListContainerContents.Text);
                }
                else if (CF_ddl_ListContainer.SelectedItem == null)
                {
                    CF_m_DeleteContainerObject(containername, CF_ddl_ListContainerContents.SelectedValue, region, snetFalse);
                    CF_m_ListContainerObjects(containername, region, snetFalse);
                    CF_m_MsgDeleteContainerObject(CF_ddl_ListContainerContents.Text);
                }
                else if (CF_ddl_ListContainer.SelectedItem != null & CF_cb_SnetCheck.Checked)
                {
                    CF_m_DeleteContainerObject(containername, CF_ddl_ListContainerContents.SelectedValue, region, snetTrue);
                    CF_m_ListContainerObjects(containername, region, snetTrue);
                    CF_m_MsgDeleteContainerObject(CF_ddl_ListContainerContents.Text);
                }
                else
                {
                    CF_m_DeleteContainerObject(containername, CF_ddl_ListContainerContents.SelectedValue, region, snetFalse);
                    CF_m_ListContainerObjects(containername, region, snetFalse);
                    CF_m_MsgDeleteContainerObject(CF_ddl_ListContainerContents.Text);
                }
            }
            catch (Exception ex)
            {
                CF_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CF_btn_DeleteMultipleContainerObjects_Click(object sender, EventArgs e)
        {

            string containername = HttpUtility.HtmlEncode(CF_ddl_ListContainer.Text);

            string region = CF_ddl_Region.SelectedItem.ToString();

            bool snetTrue = bool.Parse(CF_cb_SnetCheck.Text);
            bool snetFalse = bool.Parse("false");

            try
            {
                if (CF_ddl_ListContainer.SelectedItem == null & CF_cb_SnetCheck.Checked)
                {
                    CF_m_DeleteMultipleContainerObjects(containername, region, snetTrue);
                    CF_m_ListContainerObjects(containername, region, snetTrue);
                    CF_m_MsgBulkDeleteContainerObjects(CF_ddl_ListContainer.Text);
                }
                else if (CF_ddl_ListContainer.SelectedItem == null)
                {
                    CF_m_DeleteMultipleContainerObjects(containername, region, snetFalse);
                    CF_m_ListContainerObjects(containername, region, snetFalse);
                    CF_m_MsgBulkDeleteContainerObjects(CF_ddl_ListContainer.Text);
                }
                else if (CF_ddl_ListContainer.SelectedItem != null & CF_cb_SnetCheck.Checked)
                {
                    CF_m_DeleteMultipleContainerObjects(containername, region, snetTrue);
                    CF_m_ListContainerObjects(containername, region, snetTrue);
                    CF_m_MsgBulkDeleteContainerObjects(CF_ddl_ListContainer.Text);
                }
                else
                {
                    CF_m_DeleteMultipleContainerObjects(containername, region, snetFalse);
                    CF_m_ListContainerObjects(containername, region, snetFalse);
                    CF_m_MsgBulkDeleteContainerObjects(CF_ddl_ListContainer.Text);
                }
            }
            catch (Exception ex)
            {
                CF_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CF_btn_EnableCDNOnContainer_OnClick(object sender, EventArgs e)
        {
            string containername = HttpUtility.HtmlEncode(CF_ddl_ListContainer.Text);
            string region = CF_ddl_Region.SelectedItem.ToString();

            long ttl = 1000;

            bool snetTrue = bool.Parse(CF_cb_SnetCheck.Text);
            bool snetFalse = bool.Parse("false");

            try
            {
                if (CF_ddl_ListContainer.SelectedItem == null & CF_cb_SnetCheck.Checked)
                {
                    CF_m_EnableCDNContainer(containername, ttl, region, false);
                    CF_m_ListContainers(region, snetTrue);
                    CF_m_AccountDetails(CF_ddl_ListContainer.Text, region, snetTrue);
                    CF_m_ContainerDetails(CF_ddl_ListContainer.Text, region, snetTrue);
                    CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                }
                else if (CF_ddl_ListContainer.SelectedItem == null)
                {
                    CF_m_EnableCDNContainer(containername, ttl, region, false);
                    CF_m_ListContainers(region, snetFalse);
                    CF_m_AccountDetails(CF_ddl_ListContainer.Text, region, snetFalse);
                    CF_m_ContainerDetails(CF_ddl_ListContainer.Text, region, snetFalse);
                    CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                }
                else if (CF_ddl_ListContainer.SelectedItem != null & CF_cb_SnetCheck.Checked)
                {
                    CF_m_EnableCDNContainer(containername, ttl, region, false);
                    CF_m_ListContainers(region, snetTrue);
                    CF_m_AccountDetails(CF_ddl_ListContainer.Text, region, snetTrue);
                    CF_m_ContainerDetails(CF_ddl_ListContainer.Text, region, snetTrue);
                    CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                }
                else
                {
                    CF_m_EnableCDNContainer(containername, ttl, region, false);
                    CF_m_ListContainers(region, snetFalse);
                    CF_m_AccountDetails(CF_ddl_ListContainer.Text, region, snetFalse);
                    CF_m_ContainerDetails(CF_ddl_ListContainer.Text, region, snetFalse);
                    CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                }
            }
            catch (Exception ex)
            {
                CF_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CF_btn_DisableCDNOnContainer_OnClick(object sender, EventArgs e)
        {
            string containername = HttpUtility.HtmlEncode(CF_ddl_ListContainer.Text);
            string region = CF_ddl_Region.SelectedItem.ToString();

            bool snetTrue = bool.Parse(CF_cb_SnetCheck.Text);
            bool snetFalse = bool.Parse("false");

            try
            {
                if (CF_ddl_ListContainer.SelectedItem == null & CF_cb_SnetCheck.Checked)
                {
                    CF_m_DisableCDNContainer(containername, region);
                }
                else if (CF_ddl_ListContainer.SelectedItem == null)
                {
                    CF_m_DisableCDNContainer(containername, region);
                }
                else if (CF_ddl_ListContainer.SelectedItem != null & CF_cb_SnetCheck.Checked)
                {
                    CF_m_DisableCDNContainer(containername, region);
                }
                else
                {
                    CF_m_DisableCDNContainer(containername, region);
                }
            }
            catch (Exception ex)
            {
                CF_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CF_btn_GetObjectSaveToFile_Click(object sender, EventArgs e)
        {
            string containername = HttpUtility.HtmlEncode(CF_ddl_ListContainer.Text);
            string fileName = HttpUtility.UrlPathEncode(CF_fu_FileUpload.FileName);
            string tempDir = Server.MapPath(HttpUtility.UrlPathEncode("~/temp"));
            string filePath = Path.Combine(tempDir, fileName);

            string region = CF_ddl_Region.SelectedItem.ToString();

            bool snetTrue = bool.Parse(CF_cb_SnetCheck.Text);
            bool snetFalse = bool.Parse("false");
            bool etag = bool.Parse("false");

            try
            {
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }
                if (CF_ddl_ListContainer.SelectedItem == null & CF_cb_SnetCheck.Checked)
                {
                    CF_m_GetObjectSaveToFile(containername, tempDir, CF_ddl_ListContainerContents.SelectedValue, fileName, 65536, region, etag, snetTrue);
                    CF_m_MsgGetObjectSaveToFile();
                }
                else if (CF_ddl_ListContainer.SelectedItem == null)
                {
                    CF_m_GetObjectSaveToFile(containername, tempDir, CF_ddl_ListContainerContents.SelectedValue, fileName, 65536, region, etag, snetFalse);
                    CF_m_MsgGetObjectSaveToFile();
                }
                else if (CF_ddl_ListContainer.SelectedItem != null & CF_cb_SnetCheck.Checked)
                {
                    CF_m_GetObjectSaveToFile(containername, tempDir, CF_ddl_ListContainerContents.SelectedValue, fileName, 65536, region, etag, snetTrue);
                    CF_m_MsgGetObjectSaveToFile();
                }
                else
                {
                    CF_m_GetObjectSaveToFile(containername, tempDir, CF_ddl_ListContainerContents.SelectedValue, fileName, 65536, region, etag, snetFalse);
                    CF_m_MsgGetObjectSaveToFile();
                }
            }
            catch (Exception ex)
            {
                CF_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CF_btn_GetObjectSaveToFileBrowserPrompt_OnClick(object sender, EventArgs e)
        {
            string containername = HttpUtility.HtmlEncode(CF_ddl_ListContainer.Text);
            string fileName = HttpUtility.UrlPathEncode(CF_ddl_ListContainerContents.SelectedValue);
            string tempDir = Server.MapPath(HttpUtility.UrlPathEncode("~/temp/"));
            string filePath = Path.Combine(tempDir, fileName);

            string region = CF_ddl_Region.SelectedItem.ToString();

            bool snetTrue = bool.Parse(CF_cb_SnetCheck.Text);
            bool snetFalse = bool.Parse("false");
            bool etag = bool.Parse("false");

            try
            {
                CF_m_CreateObjectFromFileTempCheck(tempDir);


                if (CF_ddl_ListContainer.SelectedItem == null & CF_cb_SnetCheck.Checked)
                {
                    CF_m_GetObjectSaveToFileBrowserPrompt(containername, tempDir, CF_ddl_ListContainerContents.SelectedValue, fileName, 65536, filePath, tempDir, region, etag, snetTrue);
                    CF_m_CreateObjectFromFileDeleteTempData(filePath, tempDir);
                    CF_m_MsgGetObjectSaveToFile();
                }
                else if (CF_ddl_ListContainer.SelectedItem == null)
                {
                    CF_m_GetObjectSaveToFileBrowserPrompt(containername, tempDir, CF_ddl_ListContainerContents.SelectedValue, fileName, 65536, filePath, tempDir, region, etag, snetFalse);
                    CF_m_CreateObjectFromFileDeleteTempData(filePath, tempDir);
                    CF_m_MsgGetObjectSaveToFile();
                }
                else if (CF_ddl_ListContainer.SelectedItem != null & CF_cb_SnetCheck.Checked)
                {
                    CF_m_GetObjectSaveToFileBrowserPrompt(containername, tempDir, CF_ddl_ListContainerContents.SelectedValue, fileName, 65536, filePath, tempDir, region, etag, snetTrue);
                    CF_m_CreateObjectFromFileDeleteTempData(filePath, tempDir);
                    CF_m_MsgGetObjectSaveToFile();
                }
                else
                {
                    CF_m_GetObjectSaveToFileBrowserPrompt(containername, tempDir, CF_ddl_ListContainerContents.SelectedValue, fileName, 65536, filePath, tempDir, region, etag, snetFalse);
                    CF_m_CreateObjectFromFileDeleteTempData(filePath, tempDir);
                    CF_m_MsgGetObjectSaveToFile();
                }
            }
            catch (Exception ex)
            {
                CF_lbl_Error.Text = "Authentication failed or You Simply Suck! <br /> <br />" + ex.ToString();
            }
        }

        protected void CF_btn_CloudFilesUpload_OnClick(object sender, EventArgs e)
        {
            string containername = CF_ddl_ListContainer.Text;
            string tempDir = Server.MapPath("~/temp");

            string region = CF_ddl_Region.SelectedItem.ToString();

            bool snetTrue = bool.Parse(CF_cb_SnetCheck.Text);
            bool snetFalse = bool.Parse("false");
            try
            {
                if (CF_fu_FileUpload.HasFile)     // CHECK IF ANY FILE HAS BEEN SELECTED.
                {
                    int iUploadedCnt = 0;
                    int iFailedCnt = 0;
                    HttpFileCollection hfc = Request.Files;
                    CF_lbl_FileList.Text = "Select <b>" + hfc.Count + "</b> file(s)";

                    if (hfc.Count <= 10)    // 10 FILES RESTRICTION.
                    {
                        for (int i = 0; i <= hfc.Count - 1; i++)
                        {
                            HttpPostedFile hpf = hfc[i];

                            string fileName = hpf.FileName;
                            string filePath = Path.Combine(tempDir, fileName);
                            if (hpf.ContentLength > 0)
                            {
                                CF_m_CreateObjectFromFileTempCheck(tempDir);

                                if (!File.Exists(tempDir + "\\" + Path.GetFileName(hpf.FileName)))
                                {
                                    DirectoryInfo objDir = new DirectoryInfo(tempDir + "\\");
                                    string sFileName = Path.GetFileName(hpf.FileName);
                                    string sFileExt = Path.GetExtension(hpf.FileName);

                                    // CHECK FOR DUPLICATE FILES.
                                    FileInfo[] objFI = objDir.GetFiles(sFileName.Replace(sFileExt, "") + ".*");

                                    if (objFI.Length > 0)
                                    {
                                        // CHECK IF FILE WITH THE SAME NAME EXISTS (IGNORING THE EXTENTIONS).
                                        foreach (FileInfo file in objFI)
                                        {
                                            string sFileName1 = objFI[0].Name;
                                            string sFileExt1 = Path.GetExtension(objFI[0].Name);

                                            if (sFileName1.Replace(sFileExt1, "") == sFileName.Replace(sFileExt, ""))
                                            {
                                                iFailedCnt += 1;        // NOT ALLOWING DUPLICATE.
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // SAVE THE FILE IN A FOLDER.
                                        hpf.SaveAs(tempDir + "\\" + Path.GetFileName(hpf.FileName));

                                        if (CF_ddl_ListContainer.SelectedItem == null & CF_cb_SnetCheck.Checked)
                                        {
                                            CF_m_CreateObjectFromFile(containername, filePath, hpf.FileName, 4096, region, snetTrue);
                                            CF_m_CreateObjectFromFileDeleteTempData(filePath, tempDir);
                                            CF_m_ListContainerObjects(containername, region, snetTrue);
                                            iUploadedCnt += 1;
                                            CF_m_MsgFileUploadSuccess(CF_fu_FileUpload.FileName);
                                        }
                                        else if (CF_ddl_ListContainer.SelectedItem == null)
                                        {
                                            CF_m_CreateObjectFromFile(containername, filePath, hpf.FileName, 4096, region, snetFalse);
                                            CF_m_CreateObjectFromFileDeleteTempData(filePath, tempDir);
                                            CF_m_ListContainerObjects(containername, region, snetFalse);
                                            iUploadedCnt += 1;
                                            CF_m_MsgFileUploadSuccess(CF_fu_FileUpload.FileName);
                                        }
                                        else if (CF_ddl_ListContainer.SelectedItem != null & CF_cb_SnetCheck.Checked)
                                        {
                                            CF_m_CreateObjectFromFile(containername, filePath, hpf.FileName, 4096, region, snetTrue);
                                            CF_m_CreateObjectFromFileDeleteTempData(filePath, tempDir);
                                            CF_m_ListContainerObjects(containername, region, snetTrue);
                                            iUploadedCnt += 1;
                                            CF_m_MsgFileUploadSuccess(CF_fu_FileUpload.FileName);
                                        }
                                        else
                                        {
                                            CF_m_CreateObjectFromFile(containername, filePath, hpf.FileName, 4096, region, snetFalse);
                                            CF_m_CreateObjectFromFileDeleteTempData(filePath, tempDir);
                                            CF_m_ListContainerObjects(containername, region, snetFalse);
                                            iUploadedCnt += 1;
                                            CF_m_MsgFileUploadSuccess(CF_fu_FileUpload.FileName);
                                        }
                                    }
                                }
                            }
                        }
                        CF_lbl_UploadStatus.Text = "<b>" + iUploadedCnt + "</b> file(s) Uploaded.";
                        CF_lbl_UploadStatus.Text = "<b>" + iFailedCnt + "</b> duplicate file(s) could not be uploaded.";
                    }
                    else CF_lbl_UploadStatus.Text = "Max. 10 files allowed.";
                }
                else CF_lbl_UploadStatus.Text = "No files selected.";
            }
            catch (Exception ex)
            {
                CF_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CF_m_ListContainers(string dcregion, bool dcsnet = false)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);
            CloudFilesProvider CloudFilesProvider = new net.openstack.Providers.Rackspace.CloudFilesProvider(identity);

            if (string.IsNullOrEmpty((string)(Session["ContainerName"])))
            {
                var CfContainers = CloudFilesProvider.ListContainers(null, null, null, dcregion, dcsnet);

                CF_ddl_ListContainer.DataSource = CfContainers;
                CF_ddl_ListContainer.DataTextField = "Name";
                CF_ddl_ListContainer.DataBind();

                if (string.IsNullOrEmpty(CF_ddl_ListContainer.SelectedValue))
                {

                }
                else
                {
                    try
                    {
                        CF_m_ListContainerObjects(CF_ddl_ListContainer.SelectedValue, dcregion, dcsnet);
                    }
                    catch (Exception ex)
                    {
                        CF_lbl_Error.Text = "There are no objects in this container.";
                    }
                }
            }
            else
            {
                var CfContainers = CloudFilesProvider.ListContainers(null, null, null, dcregion, dcsnet);

                CF_ddl_ListContainer.DataSource = CfContainers;
                CF_ddl_ListContainer.DataTextField = "Name";
                CF_ddl_ListContainer.DataBind();

                if (CF_ddl_ListContainer.SelectedValue != null)
                {
                    CF_m_ListContainerObjects((string)(Session["ContainerName"]), dcregion, dcsnet);
                }
            }
        }
        protected void CF_m_ListContainerObjects(string cfcontainername, string dcregion, bool dcsnet = false)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new CloudIdentityProvider(identity);
            CloudFilesProvider CloudFilesProvider = new CloudFilesProvider(identity);

            var Cfobjects = CloudFilesProvider.ListObjects(cfcontainername, null, null, null, null, dcregion, dcsnet);

            CF_ddl_ListContainerContents.DataSource = Cfobjects;
            CF_ddl_ListContainerContents.DataTextField = "Name";
            CF_ddl_ListContainerContents.DataBind();

            try
            {
                CF_m_ContainerObjectDetails(CF_ddl_ListContainer.SelectedValue, CF_ddl_ListContainerContents.SelectedItem.ToString(), dcregion, dcsnet);
            }
            catch (Exception ex)
            {
                CF_lbl_Error.Text = "There are no objects in this container.";
            }
        }
        protected void CF_m_GetObjectSaveToFile(string cfcontainername, string saveDirectory, string objname, string fileName, int CF_m_GetObjectSaveToFilechunksize, string dcregion, bool verifyEtag = false, bool dcsnet = false)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new CloudIdentityProvider(identity);
            CloudFilesProvider CloudFilesProvider = new CloudFilesProvider(identity);

            CloudFilesProvider.GetObjectSaveToFile(cfcontainername, saveDirectory, objname, fileName, CF_m_GetObjectSaveToFilechunksize, null, "dfw", verifyEtag, null, dcsnet);
        }
        protected void CF_m_GetObjectSaveToFileBrowserPrompt(string cfcontainername, string saveDirectory, string objname, string fileName, int CF_m_GetObjectSaveToFilechunksize, string filetodelete, string tempDir, string dcregion, bool verifyEtag = false, bool dcsnet = false)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);
            string file_name = saveDirectory + objname;

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new CloudIdentityProvider(identity);
            CloudFilesProvider CloudFilesProvider = new CloudFilesProvider(identity);

            CloudFilesProvider.GetObjectSaveToFile(cfcontainername, saveDirectory, objname, fileName, CF_m_GetObjectSaveToFilechunksize, null, dcregion, verifyEtag, null, dcsnet);

            // the file name to get
            string CFfileName = file_name;
            // get the file bytes to download to the browser
            byte[] fileBytes = System.IO.File.ReadAllBytes(CFfileName);
            // NOTE: You could also read the file bytes from a database as well.
            // download this file to the browser
            CF_m_StreamFileToBrowser(objname, fileBytes, filetodelete, tempDir);
        }
        protected void CF_m_CreateContainer(string cfcreatecontainername, string dcregion, bool dcsnet = false)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new CloudIdentityProvider(identity);
            CloudFilesProvider CloudFilesProvider = new CloudFilesProvider(identity);

            var CfCreateContainer = CloudFilesProvider.CreateContainer(cfcreatecontainername, dcregion, dcsnet);
        }
        protected void CF_m_DeleteContainer(string cfdeletecontainername, string dcregion, bool deleteObjects = false, bool dcsnet = false)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new CloudIdentityProvider(identity);
            CloudFilesProvider CloudFilesProvider = new CloudFilesProvider(identity);

            var Cfdeletecontainer = CloudFilesProvider.DeleteContainer(cfdeletecontainername, deleteObjects, dcregion, dcsnet);
        }
        protected void CF_m_DeleteContainerObject(string cfcontainername, string cfdeletecontainerobject, string dcregion, bool deleteSegments = false, bool dcsnet = false)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new CloudIdentityProvider(identity);
            CloudFilesProvider CloudFilesProvider = new CloudFilesProvider(identity);

            var Cfdeletecontainerobject = CloudFilesProvider.DeleteObject(cfcontainername, cfdeletecontainerobject, null, deleteSegments, dcregion, dcsnet);
        }
        protected void CF_m_DeleteMultipleContainerObjects(string cfcontainername, string dcregion, bool deleteSegments = false, bool dcsnet = false)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new CloudIdentityProvider(identity);
            CloudFilesProvider CloudFilesProvider = new CloudFilesProvider(identity);

            var objects = CloudFilesProvider.ListObjects(cfcontainername, null, null, null, null, dcregion, dcsnet).ToArray();

            CloudFilesProvider.BulkDelete(objects.Select(o => string.Format("{0}/{1}", cfcontainername, o.Name)), null, dcregion, dcsnet);
        }
        protected void CF_m_CreateObjectFromFile(string cfcontainername, string cfcreateobjfilepath, string cfcreateobjfilename, int cfcreateobjchunksize, string dcregion, bool dcsnet = false)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new CloudIdentityProvider(identity);
            CloudFilesProvider CloudFilesProvider = new CloudFilesProvider(identity);

            CloudFilesProvider.CreateObjectFromFile(cfcontainername, cfcreateobjfilepath, cfcreateobjfilename, cfcreateobjchunksize, null, dcregion, null, dcsnet);
        }
        protected void CF_m_AccountDetails(string containername, string dcregion, bool dcsnet = false)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new CloudIdentityProvider(identity);
            CloudFilesProvider CloudFilesProvider = new CloudFilesProvider(identity);

            var AccountHeadersResponse = CloudFilesProvider.GetAccountHeaders(dcregion, dcsnet);

            var AccountHeadersRespond_SB = new StringBuilder();

            foreach (var i in AccountHeadersResponse)
            {
                AccountHeadersRespond_SB.AppendLine(i.Key + " " + i.Value + "<br />");
            }

            CF_lbl_AccountDetails.Text = AccountHeadersRespond_SB.ToString();
        }
        protected void CF_m_ContainerDetails(string containername, string dcregion, bool dcsnet = false)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new CloudIdentityProvider(identity);
            CloudFilesProvider CloudFilesProvider = new CloudFilesProvider(identity);

            try
            {
                var ContainerHeaderResponse = CloudFilesProvider.GetContainerHeader(containername, dcregion, dcsnet);
                var ContainerHeaderRespond_SB = new StringBuilder();

                foreach (var i in ContainerHeaderResponse)
                {
                    ContainerHeaderRespond_SB.AppendLine(i.Key + " " + i.Value + "<br />");
                }

                CF_lbl_ContainerDetails.Text = ContainerHeaderRespond_SB.ToString();
            }
            catch (Exception ex)
            {
                CF_lbl_Error.Text = "There are no containers in this datacenter.";
            }
        }
        protected void CF_m_ContainerObjectDetails(string ContainerName, string ContainerObjectName, string dcregion, bool dcsnet = false)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new CloudIdentityProvider(identity);
            CloudFilesProvider CloudFilesProvider = new CloudFilesProvider(identity);

            var ContainerHeaderResponse = CloudFilesProvider.GetObjectHeaders(ContainerName, ContainerObjectName, dcregion, dcsnet);
            var ContainerHeaderRespond_SB = new StringBuilder();

            foreach (var i in ContainerHeaderResponse)
            {
                ContainerHeaderRespond_SB.AppendLine(i.Key + " " + i.Value + "<br />");
            }

            CF_lbl_ContainerObjectDetails.Text = ContainerHeaderRespond_SB.ToString();
        }
        protected void CF_m_ContainerCDNDetails(string ContainerName, string dcregion)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new CloudIdentityProvider(identity);
            CloudFilesProvider CloudFilesProvider = new CloudFilesProvider(identity);

            try
            {
                var ContainerHeaderResponse = CloudFilesProvider.GetContainerCDNHeader(ContainerName, dcregion);

                string Name = ContainerHeaderResponse.Name;
                string CDNUri = ContainerHeaderResponse.CDNUri;
                string CDNSslUri = ContainerHeaderResponse.CDNSslUri;
                string CDNStreamingUri = ContainerHeaderResponse.CDNStreamingUri;
                string CDNIosUri = ContainerHeaderResponse.CDNIosUri;

                bool CDNEnabled = ContainerHeaderResponse.CDNEnabled;
                bool LogRetention = ContainerHeaderResponse.LogRetention;

                long Ttl = ContainerHeaderResponse.Ttl;

                CF_lbl_ContainerCDNDetails.Text = " CDN Container Name : " + Name + "<br />" + " CDN URI : " + CDNUri + "<br />" + " CDN SSL URI : " + CDNSslUri + "<br />" + " CDN Streaming URI : " + CDNStreamingUri + "<br />" + " CDN IOS URI : " + CDNIosUri + "<br />" + " TTL : " + Ttl.ToString() + "<br />";
            }
            catch (Exception ex)
            {
                CF_lbl_ContainerCDNDetails.Text = " CDN is not enabled.";
            }
        }
        protected void CF_m_EnableCDNContainer(string ContainerName, long ttl, string dcregion, bool logretention = false)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new CloudIdentityProvider(identity);
            CloudFilesProvider CloudFilesProvider = new CloudFilesProvider();

            var EnableCDNOnContainer = CloudFilesProvider.EnableCDNOnContainer(ContainerName, ttl, logretention, dcregion, identity);

            var EnableCDNOnContainer_SB = new StringBuilder();

            foreach (var i in EnableCDNOnContainer)
            {
                EnableCDNOnContainer_SB.AppendLine(i.Key + " " + i.Value + "<br />");
            }

            CF_lbl_EnableDisableCDN.Text = EnableCDNOnContainer_SB.ToString();
        }
        protected void CF_m_DisableCDNContainer(string ContainerName, string dcregion, bool dcsnet = false)
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            var identity = new RackspaceImpersonationIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new CloudIdentityProvider(identity);
            CloudFilesProvider CloudFilesProvider = new CloudFilesProvider(identity);

            var DisableCDNOnContainer = CloudFilesProvider.DisableCDNOnContainer(ContainerName, dcregion);

            CF_lbl_EnableDisableCDN.Text = "CDN has been disabled.";
        }
        protected void CF_m_CreateObjectFromFileDeleteTempData(string FileToDelete, string tempDir)
        {
            File.Delete(FileToDelete);

            Directory.Delete(tempDir);
        }
        protected void CF_m_CreateObjectFromFileTempCheck(string tempDir)
        {
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
        }
        protected void CF_m_MsgClearSessions()
        {
            Session.Remove("ContainerName");
            Session.Remove("ContainerContents");
            Session.Remove("Regions");
        }
        protected void CF_m_MsgClearDDLs()
        {
            CF_ddl_ListContainer.Items.Clear();
            CF_ddl_ListContainerContents.Items.Clear();
        }
        public void CF_m_StreamFileToBrowser(string sFileName, byte[] fileBytes, string FileToDelete, string tempDir)
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            context.Response.Clear();
            context.Response.ClearHeaders();
            context.Response.ClearContent();
            context.Response.AppendHeader("content-length", fileBytes.Length.ToString());
            context.Response.ContentType = CF_m_GetMimeTypeByFileName(sFileName);
            context.Response.AppendHeader("content-disposition", "attachment; filename=" + sFileName);
            context.Response.BinaryWrite(fileBytes);

            CF_m_CreateObjectFromFileDeleteTempData(FileToDelete, tempDir);

            // use this instead of response.end to avoid thread aborted exception (known issue):
            // http://support.microsoft.com/kb/312629/EN-US
            context.ApplicationInstance.CompleteRequest();
        }
        public string CF_m_GetMimeTypeByFileName(string sFileName)
        {
            string sMime = "application/octet-stream";

            string sExtension = System.IO.Path.GetExtension(sFileName);
            if (!string.IsNullOrEmpty(sExtension))
            {
                sExtension = sExtension.Replace(".", "");
                sExtension = sExtension.ToLower();

                if (sExtension == "xls" || sExtension == "xlsx")
                {
                    sMime = "application/ms-excel";
                }
                else if (sExtension == "doc" || sExtension == "docx")
                {
                    sMime = "application/msword";
                }
                else if (sExtension == "ppt" || sExtension == "pptx")
                {
                    sMime = "application/ms-powerpoint";
                }
                else if (sExtension == "rtf")
                {
                    sMime = "application/rtf";
                }
                else if (sExtension == "zip")
                {
                    sMime = "application/zip";
                }
                else if (sExtension == "mp3")
                {
                    sMime = "audio/mpeg";
                }
                else if (sExtension == "bmp")
                {
                    sMime = "image/bmp";
                }
                else if (sExtension == "gif")
                {
                    sMime = "image/gif";
                }
                else if (sExtension == "jpg" || sExtension == "jpeg")
                {
                    sMime = "image/jpeg";
                }
                else if (sExtension == "png")
                {
                    sMime = "image/png";
                }
                else if (sExtension == "tiff" || sExtension == "tif")
                {
                    sMime = "image/tiff";
                }
                else if (sExtension == "txt")
                {
                    sMime = "text/plain";
                }
            }

            return sMime;
        }
        protected void CF_m_MsgFileUploadSuccess(string FileUploadFileName)
        {
            CF_lbl_Info.Text = FileUploadFileName + " Has Been Uploaded Successfully";
        }
        protected void CF_m_MsgPleaseSeleactRegion()
        {
            CF_lbl_Info.Text = "Please select a region: DFW, or ORD but not both.";
        }
        protected void CF_m_MsgCatchException(string exception)
        {
            CF_lbl_Error.Text = "Something went terribly wrong! See below for more info. <br /> <br />" + exception;
        }
        protected void CF_m_MsgListContainerInfo()
        {
            CF_lbl_Info.Text = " Container info retrieved successfully.";
        }
        protected void CF_m_MsgDeleteContainer(string containername)
        {
            CF_lbl_Info.Text = containername + " - container has been successfully deleted.";
        }
        protected void CF_m_MsgDeleteContainerObject(string containerobject)
        {
            CF_lbl_Info.Text = containerobject + " - container object has been succesully deleted.";
        }
        protected void CF_m_MsgBulkDeleteContainerObjects(string container)
        {
            CF_lbl_Info.Text = "All objects have been succesully deleted from : " + container;
        }
        protected void CF_m_MsgCreateContainer(string containername)
        {
            CF_lbl_Info.Text = containername + " - container has been created successfully.";
        }
        protected void CF_m_MsgGetObjectSaveToFile()
        {
            CF_lbl_Info.Text = "Successfully downloaded object from Cloud Files.";
        }
    }
}