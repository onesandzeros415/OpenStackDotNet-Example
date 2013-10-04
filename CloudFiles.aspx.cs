using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
            System.Net.ServicePointManager.DefaultConnectionLimit = 50;

            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string containername = (string)(Session["ContainerName"]);
            string region = CF_ddl_Region.SelectedItem.ToString();

            bool snet = CF_cb_SnetCheck.Checked;

            Page.GetPostBackEventReference(CF_btn_DeleteContainerObject);

            try
            {
                if (IsPostBack)
                {
                    HttpContext.Current.Session["ContainerName"] = CF_ddl_ListContainer.Text;
                    HttpContext.Current.Session["ContainerContents"] = CF_ddl_ListContainerContents.Text;
                    HttpContext.Current.Session["Region"] = CF_ddl_Region.SelectedValue;
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
                        var listContainers = Files.CF_m_ListContainers(region, snet);

                        bindListContainerDDL(listContainers, "Name");
                        bindCopyListContainerDDL(listContainers, "Name");

                        listContainerObjs(region, snet);

                        CF_lbl_ContainerCDNDetails.Text = Files.CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                        CF_lbl_AccountContainerInfo.Text = getAccountSize(CF_ddl_ListContainer.Text, region, snet) + getContainerSize(CF_ddl_ListContainer.Text, region, snet);

                        TimeClock.Stop();
                        CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                CF_m_MsgCatchException(ex.ToString());

                TimeClock.Stop();
                CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
        }
        protected void CF_ddl_Region_SelectChange(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string region = CF_ddl_Region.SelectedItem.ToString();
            string cslistserverregion = (string)(Session["CSListServersRegion"]);
            string session_containername = (string)(Session["ContainerName"]);
            string session_containercontents = (string)(Session["ContainerContents"]);
            string session_region = (string)(Session["Region"]);

            bool snet = CF_cb_SnetCheck.Checked;

            try
            {
                var listContainers = Files.CF_m_ListContainers(region, snet);

                bindListContainerDDL(listContainers, "Name");
                bindCopyListContainerDDL(listContainers, "Name");

                listContainerObjs(region, snet);

                CF_lbl_ContainerCDNDetails.Text = Files.CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                CF_lbl_AccountContainerInfo.Text = getAccountSize(CF_ddl_ListContainer.Text, region, snet) + getContainerSize(CF_ddl_ListContainer.Text, region, snet);

                TimeClock.Stop();
                CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CF_m_MsgCatchException(ex.ToString());

                TimeClock.Stop();
                CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
        }
        protected void CF_ddl_ListContainer_SelectChange(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string containername = HttpUtility.HtmlEncode(CF_ddl_ListContainer.Text);
            string region = CF_ddl_Region.SelectedItem.ToString();

            bool snet = CF_cb_SnetCheck.Checked;

            try
            {
                listContainerObjs(region, snet);

                CF_lbl_ContainerCDNDetails.Text = Files.CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                CF_lbl_AccountContainerInfo.Text = getAccountSize(CF_ddl_ListContainer.Text, region, snet) + getContainerSize(CF_ddl_ListContainer.Text, region, snet);

                TimeClock.Stop();
                CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CF_m_MsgCatchException(ex.ToString());

                TimeClock.Stop();
                CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
        }
        protected void CF_ddl_ListContainerContents_SelectChange(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string containername = HttpUtility.HtmlEncode(CF_ddl_ListContainer.Text);
            string region = CF_ddl_Region.SelectedItem.ToString();

            bool snet = CF_cb_SnetCheck.Checked;

            try
            {
                CF_lbl_AccountContainerInfo.Text = getAccountSize(CF_ddl_ListContainer.Text, region, snet) + getContainerSize(CF_ddl_ListContainer.Text, region, snet);
                CF_lbl_ContainerObjectInfo.Text = getObjectSize(CF_ddl_ListContainer.SelectedItem.ToString(), CF_ddl_ListContainerContents.SelectedItem.ToString(), region, snet);

                TimeClock.Stop();
                CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CF_m_MsgCatchException(ex.ToString());

                TimeClock.Stop();
                CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
        }
        protected void CF_btn_CreateContainer_OnClick(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string containername = HttpUtility.HtmlEncode(CF_ddl_ListContainer.Text);
            string region = CF_ddl_Region.SelectedItem.ToString();

            bool snet = CF_cb_SnetCheck.Checked;

            try
            {
                Files.CF_m_CreateContainer(CF_txt_CreateContainer.Text, region, snet);
                bindListContainerDDL(Files.CF_m_ListContainers(region, snet), "Name");
                bindCopyListContainerDDL(Files.CF_m_ListContainers(region, snet), "Name");

                listContainerObjs(region, snet);

                CF_lbl_AccountContainerInfo.Text = getAccountSize(CF_ddl_ListContainer.Text, region, snet) + getContainerSize(CF_ddl_ListContainer.Text, region, snet);
                CF_m_MsgCreateContainer(CF_txt_CreateContainer.Text);

                TimeClock.Stop();
                CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CF_m_MsgCatchException(ex.ToString());

                TimeClock.Stop();
                CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
        }
        protected void CF_btn_DeleteContainer_OnClick(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string containername = HttpUtility.HtmlEncode(CF_ddl_ListContainer.Text);
            string region = CF_ddl_Region.SelectedItem.ToString();

            bool snet = CF_cb_SnetCheck.Checked;
            bool deleteAllObjs = CF_chk_deleteallobjects.Checked;

            try
            {
                Files.CF_m_DeleteContainer(containername, region, deleteAllObjs, snet);
                CF_m_ClearSessions();
                bindListContainerDDL(Files.CF_m_ListContainers(region, snet), "Name");
                bindCopyListContainerDDL(Files.CF_m_ListContainers(region, snet), "Name");

                listContainerObjs(region, snet);

                CF_lbl_AccountContainerInfo.Text = getAccountSize(CF_ddl_ListContainer.Text, region, snet);
                CF_m_MsgDeleteContainer(containername);

                TimeClock.Stop();
                CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CF_m_MsgCatchException(ex.ToString());

                TimeClock.Stop();
                CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
        }
        protected void CF_btn_DeleteContainerObject_OnClick(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string containername = HttpUtility.HtmlEncode(CF_ddl_ListContainer.Text);

            string region = CF_ddl_Region.SelectedItem.ToString();

            bool snet = CF_cb_SnetCheck.Checked;

            try
            {
                Files.CF_m_DeleteContainerObject(containername, CF_ddl_ListContainerContents.SelectedValue, region, snet);

                listContainerObjs(region, snet);

                CF_lbl_AccountContainerInfo.Text = getAccountSize(CF_ddl_ListContainer.Text, region, snet) + getContainerSize(CF_ddl_ListContainer.Text, region, snet);
                CF_m_MsgDeleteContainerObject(CF_ddl_ListContainerContents.Text);

                TimeClock.Stop();
                CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();

            }
            catch (Exception ex)
            {
                CF_m_MsgCatchException(ex.ToString());

                TimeClock.Stop();
                CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
        }
        protected void CF_btn_BulkDelete_Click(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string containername = HttpUtility.HtmlEncode(CF_ddl_ListContainer.Text);

            string region = CF_ddl_Region.SelectedItem.ToString();

            bool snet = CF_cb_SnetCheck.Checked;

            try
            {
                Files.CF_m_BulkDelete(containername, region, snet);

                listContainerObjs(region, snet);

                CF_lbl_AccountContainerInfo.Text = getAccountSize(CF_ddl_ListContainer.Text, region, snet) + getContainerSize(CF_ddl_ListContainer.Text, region, snet);
                CF_m_MsgBulkDeleteContainerObjects(CF_ddl_ListContainer.Text);

                TimeClock.Stop();
                CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CF_m_MsgCatchException(ex.ToString());

                TimeClock.Stop();
                CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
        }
        protected void CF_btn_EnableCDNOnContainer_OnClick(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string containername = HttpUtility.HtmlEncode(CF_ddl_ListContainer.Text);
            string region = CF_ddl_Region.SelectedItem.ToString();

            long ttl = 1000;

            bool snet = CF_cb_SnetCheck.Checked;

            try
            {
                Files.CF_m_EnableCDNContainer(containername, ttl, region, false);

                CF_lbl_ContainerCDNDetails.Text = Files.CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                CF_lbl_AccountContainerInfo.Text = getAccountSize(CF_ddl_ListContainer.Text, region, snet) + getContainerSize(CF_ddl_ListContainer.Text, region, snet);

                TimeClock.Stop();
                CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CF_m_MsgCatchException(ex.ToString());

                TimeClock.Stop();
                CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
        }
        protected void CF_btn_DisableCDNOnContainer_OnClick(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string containername = HttpUtility.HtmlEncode(CF_ddl_ListContainer.Text);
            string region = CF_ddl_Region.SelectedItem.ToString();

            bool snet = CF_cb_SnetCheck.Checked;

            try
            {
                CF_lbl_EnableDisableCDN.Text = Files.CF_m_DisableCDNContainer(containername, region);

                CF_lbl_ContainerCDNDetails.Text = Files.CF_m_ContainerCDNDetails(CF_ddl_ListContainer.Text, region);
                CF_lbl_AccountContainerInfo.Text = getAccountSize(CF_ddl_ListContainer.Text, region, snet) + getContainerSize(CF_ddl_ListContainer.Text, region, snet);

                TimeClock.Stop();
                CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CF_m_MsgCatchException(ex.ToString());
            }
        }
        protected void CF_btn_GetObjectSaveToFileBrowserPrompt_OnClick(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string containername = HttpUtility.HtmlEncode(CF_ddl_ListContainer.Text);
            string fileName = HttpUtility.UrlPathEncode(CF_ddl_ListContainerContents.SelectedValue);
            string tempDir = Server.MapPath(HttpUtility.UrlPathEncode("~/temp/"));
            string filePath = Path.Combine(tempDir, fileName);

            string region = CF_ddl_Region.SelectedItem.ToString();

            bool snet = CF_cb_SnetCheck.Checked;
            bool etag = bool.Parse("false");

            try
            {
                CF_m_CreateObjectFromFileTempCheck(tempDir);

                CF_m_GetObjectSaveToFileBrowserPrompt(containername, tempDir, CF_ddl_ListContainerContents.SelectedValue, fileName, 65536, filePath, tempDir, region, etag, snet);
                CF_m_CreateObjectFromFileDeleteTempData(filePath);
                CF_m_CreateObjectFromFileDeleteTempDir(tempDir);
                CF_m_MsgGetObjectSaveToFile();

                TimeClock.Stop();
                CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
            catch (Exception ex)
            {
                CF_lbl_Error.Text = "Authentication failed or You Simply Suck! <br /> <br />" + ex.ToString();
            }
        }
        protected void CF_btn_CloudFilesUpload_OnClick(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string containername = CF_ddl_ListContainer.Text;
            string tempDir = Server.MapPath("~/temp");

            string region = CF_ddl_Region.SelectedItem.ToString();

            bool snet = CF_cb_SnetCheck.Checked;

            try
            {
                CF_m_CreateObjectFromFileTempCheck(tempDir);

                if (CF_fu_FileUpload.HasFile)     // CHECK IF ANY FILE HAS BEEN SELECTED.
                {
                    int iUploadedCnt = 0;

                    HttpFileCollection hfc = Request.Files;
                    CF_lbl_FileList.Text = "Select <b>" + hfc.Count + "</b> file(s)";

                    Parallel.For(0, hfc.Count, i =>
                    {
                        HttpPostedFile hpf = hfc[i];

                        string fileName = hpf.FileName;
                        string filePath = Path.Combine(tempDir, fileName);
                        string contenttype = Main.CF_m_GetMimeTypeByFileName(fileName);

                        if (hpf.ContentLength > 0)
                        {
                            // SAVE THE FILE IN A FOLDER.
                            hpf.SaveAs(tempDir + "\\" + Path.GetFileName(hpf.FileName));

                            CF_m_CreateObjectFromFile(containername, filePath, hpf.FileName, contenttype, 4096, region, snet);
                            CF_m_CreateObjectFromFileDeleteTempData(filePath);

                            iUploadedCnt += 1;
                        }
                    });
                    CF_m_MsgFileUploadSuccess(CF_fu_FileUpload.FileName);
                    CF_lbl_UploadStatus.Text = "<b>" + iUploadedCnt + "</b> file(s) Uploaded.";
                    CF_m_CreateObjectFromFileDeleteTempDir(tempDir);

                    TimeClock.Stop();
                    CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
                }

                listContainerObjs(region, snet);

                CF_lbl_AccountContainerInfo.Text = getAccountSize(CF_ddl_ListContainer.Text, region, snet) + getContainerSize(CF_ddl_ListContainer.Text, region, snet);
            }
            catch (Exception ex)
            {
                CF_m_MsgCatchException(ex.ToString());

                TimeClock.Stop();
                CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();
            }
        }
        protected void CF_btn_CopyObj_OnClick(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string containername = HttpUtility.HtmlEncode(CF_ddl_ListContainer.Text);
            string fileName = HttpUtility.UrlPathEncode(CF_ddl_ListContainerContents.SelectedValue);
            string tempDir = Server.MapPath(HttpUtility.UrlPathEncode("~/temp/"));
            string filePath = Path.Combine(tempDir, fileName);

            string region = CF_ddl_Region.SelectedItem.ToString();

            bool snet = CF_cb_SnetCheck.Checked;

            bool etag = bool.Parse("false");

            try
            {
                CF_m_CreateObjectFromFileTempCheck(tempDir);

                Files.CF_m_CopyObj(CF_ddl_ListContainer.SelectedItem.ToString(), CF_ddl_ListContainerContents.SelectedItem.ToString(), CF_ddl_CopyListContainer.SelectedItem.ToString(), CF_ddl_ListContainerContents.SelectedItem.ToString(), region, snet);

                TimeClock.Stop();
                CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();

                CF_lbl_Info.Text = CF_ddl_ListContainerContents.SelectedItem.ToString() + " has been copied successfully to container " + CF_ddl_CopyListContainer.SelectedItem.ToString();
            }
            catch (Exception ex)
            {
                CF_lbl_Error.Text = "Authentication failed or You Simply Suck! <br /> <br />" + ex.ToString();
            }
        }
        protected void CF_btn_MoveObj_OnClick(object sender, EventArgs e)
        {
            Stopwatch TimeClock = new Stopwatch();
            TimeClock.Start();

            string containername = HttpUtility.HtmlEncode(CF_ddl_ListContainer.Text);
            string fileName = HttpUtility.UrlPathEncode(CF_ddl_ListContainerContents.SelectedValue);
            string tempDir = Server.MapPath(HttpUtility.UrlPathEncode("~/temp/"));
            string filePath = Path.Combine(tempDir, fileName);

            string region = CF_ddl_Region.SelectedItem.ToString();

            bool snet = CF_cb_SnetCheck.Checked;

            bool etag = bool.Parse("false");

            try
            {
                CF_m_CreateObjectFromFileTempCheck(tempDir);

                Files.CF_m_MoveObj(CF_ddl_ListContainer.SelectedItem.ToString(), CF_ddl_ListContainerContents.SelectedItem.ToString(), CF_ddl_CopyListContainer.SelectedItem.ToString(), CF_ddl_ListContainerContents.SelectedItem.ToString(), region, snet);

                TimeClock.Stop();
                CF_lbl_TimeClock.Text = TimeClock.Elapsed.ToString();

                CF_lbl_Info.Text = CF_ddl_ListContainerContents.SelectedItem.ToString() + " has been moved successfully to container " + CF_ddl_CopyListContainer.SelectedItem.ToString();
            }
            catch (Exception ex)
            {
                CF_lbl_Error.Text = "Authentication failed or You Simply Suck! <br /> <br />" + ex.ToString();
            }
        }
        protected static string getAccountSize(string containername, string region, bool snet = false)
        {
            var accountBytesUsed = CloudFilesProvider.AccountBytesUsed;
            var accountContainerCount = CloudFilesProvider.AccountContainerCount;

            var getAccountSize = Files.getAccountHeaderResponse(containername, accountBytesUsed, region, snet);
            var getAccountContainerCount = Files.getAccountHeaderResponse(containername, accountContainerCount, region, snet);

            string FinalByteSize = Main.CF_m_ConvertByteSize(double.Parse(getAccountSize));

            return "Account size is : " + FinalByteSize + "<br />" + "Account has " + getAccountContainerCount + " containers <br /><br />";
        }
        protected static string getContainerSize(string containername, string region, bool snet = false)
        {
            var containerBytesUsed = CloudFilesProvider.ContainerBytesUsed;
            var containerObjCount = CloudFilesProvider.ContainerObjectCount;

            var getContainerSize = Files.getContainerHeaderResponse(containername, containerBytesUsed, region, snet);
            var getContaainerObjCount = Files.getContainerHeaderResponse(containername, containerObjCount, region, snet);

            string FinalByteSize = Main.CF_m_ConvertByteSize(double.Parse(getContainerSize));

            return "Container size is : " + FinalByteSize + "<br />" + "Container has " + getContaainerObjCount + " objects <br />";
        }
        protected static string getObjectSize(string containername, string objectName, string region, bool snet = false)
        {
            string modifiedObj = objectName.Replace(@"\", "/");

            var getContentLength = Files.getObjectHeaderResponse(containername, modifiedObj, "Content-Length", region, snet);

            string FinalByteSize = Main.CF_m_ConvertByteSize(double.Parse(getContentLength));

            return "Object size is : " + FinalByteSize + "<br />";
        }
        protected CloudFilesProvider filesProvider()
        {
            string CloudIdentityUserName = (string)(Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(Session["CloudIdentityApiKey"]);

            RackspaceCloudIdentity identity = new RackspaceCloudIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudFilesProvider CloudFilesProvider = new net.openstack.Providers.Rackspace.CloudFilesProvider(identity);

            return CloudFilesProvider;
        }
        protected void CF_m_CreateObjectFromFile(string cfcontainername, string cfcreateobjfilepath, string cfcreateobjfilename, string contenttype, int cfcreateobjchunksize, string dcregion, bool dcsnet = false)
        {
            filesProvider().CreateObjectFromFile(cfcontainername, cfcreateobjfilepath, cfcreateobjfilename, contenttype, cfcreateobjchunksize, null, dcregion, null, dcsnet);
        }
        protected void CF_m_GetObjectSaveToFileBrowserPrompt(string cfcontainername, string saveDirectory, string objname, string fileName, int CF_m_GetObjectSaveToFilechunksize, string filetodelete, string tempDir, string dcregion, bool verifyEtag = false, bool dcsnet = false)
        {
            string file_name = saveDirectory + objname;

            Files.CF_m_GetObjectSaveToFile(cfcontainername, saveDirectory, objname, fileName, CF_m_GetObjectSaveToFilechunksize, dcregion, verifyEtag, dcsnet);

            // the file name to get
            string CFfileName = file_name;
            // get the file bytes to download to the browser
            byte[] fileBytes = System.IO.File.ReadAllBytes(CFfileName);
            // NOTE: You could also read the file bytes from a database as well.
            // download this file to the browser
            CF_m_StreamFileToBrowser(objname, fileBytes, filetodelete, tempDir);
        }
        public void CF_m_StreamFileToBrowser(string sFileName, byte[] fileBytes, string FileToDelete, string tempDir)
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            context.Response.Clear();
            context.Response.ClearHeaders();
            context.Response.ClearContent();
            context.Response.AppendHeader("content-length", fileBytes.Length.ToString());
            context.Response.ContentType = Main.CF_m_GetMimeTypeByFileName(sFileName);
            context.Response.AppendHeader("content-disposition", "attachment; filename=" + sFileName);
            context.Response.BinaryWrite(fileBytes);

            CF_m_CreateObjectFromFileDeleteTempData(FileToDelete);
            CF_m_CreateObjectFromFileDeleteTempDir(tempDir);

            // use this instead of response.end to avoid thread aborted exception (known issue):
            // http://support.microsoft.com/kb/312629/EN-US
            context.ApplicationInstance.CompleteRequest();
        }
        protected void listContainerObjs(string region, bool snet)
        {
            var listContainerObjs = Files.CF_m_ListContainerObjects(CF_ddl_ListContainer.SelectedValue, region, snet);
            bindListContainerContentsDDL(listContainerObjs, "Name");
            grid(listContainerObjs);

            try
            {
                if (listContainerObjs.Count() <= 0)
                {
                    CF_lbl_Info.Text = "No objects in this container.";
                }
                else
                {
                    CF_lbl_ContainerObjectInfo.Text = getObjectSize(CF_ddl_ListContainer.SelectedItem.ToString(), CF_ddl_ListContainerContents.SelectedItem.ToString(), region, snet);
                }
            }
            catch(Exception ex)
            {
                CF_lbl_ContainerObjectInfo.Text = ex.ToString();
            }
        }
        protected void grid(object containeritems)
        {
            CF_grid_Results.DataSource = containeritems;
            CF_grid_Results.DataBind();

            if (CF_grid_Results.Columns.Count > 0)
                CF_grid_Results.Columns[0].Visible = false;
            else
            {
                CF_grid_Results.HeaderRow.Cells[0].Visible = false;
                CF_grid_Results.HeaderRow.Cells[1].Visible = false;
                CF_grid_Results.HeaderRow.Cells[2].Visible = false;
                CF_grid_Results.HeaderRow.Cells[3].Visible = false;
                CF_grid_Results.HeaderRow.Cells[4].Visible = false;

                foreach (GridViewRow gvr in CF_grid_Results.Rows)
                {
                    gvr.Cells[0].Visible = true;
                    gvr.Cells[1].Visible = false;
                    gvr.Cells[2].Visible = false;
                    gvr.Cells[3].Visible = false;
                    gvr.Cells[4].Visible = false;
                }
            }
        }
        protected void bindListContainerDDL(object dataSource, string dataTextField)
        {
            CF_ddl_ListContainer.DataSource = dataSource;
            CF_ddl_ListContainer.DataTextField = dataTextField;
            CF_ddl_ListContainer.DataBind();
        }
        protected void bindListContainerContentsDDL(object dataSource, string dataTextField)
        {
            CF_ddl_ListContainerContents.DataSource = dataSource;
            CF_ddl_ListContainerContents.DataTextField = dataTextField;
            CF_ddl_ListContainerContents.DataBind();
        }
        protected void bindCopyListContainerDDL(object dataSource, string dataTextField)
        {
            CF_ddl_CopyListContainer.DataSource = dataSource;
            CF_ddl_CopyListContainer.DataTextField = dataTextField;
            CF_ddl_CopyListContainer.DataBind();
        }
        protected void CF_m_CreateObjectFromFileDeleteTempData(string FileToDelete)
        {
            File.Delete(FileToDelete);
        }
        protected void CF_m_CreateObjectFromFileDeleteTempDir(string tempDir)
        {
            Directory.Delete(tempDir);
        }
        protected void CF_m_CreateObjectFromFileTempCheck(string tempDir)
        {
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
        }
        protected void CF_m_ClearSessions()
        {
            Session.Remove("ContainerName");
            Session.Remove("ContainerContents");
            Session.Remove("Regions");
        }
        protected void CF_m_ClearDDLs()
        {
            CF_ddl_ListContainer.Items.Clear();
            CF_ddl_ListContainerContents.Items.Clear();
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