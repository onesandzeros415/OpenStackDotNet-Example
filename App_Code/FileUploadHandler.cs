using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace OpenStackDotNet_Test
{
    public class FileUploadHandler : IHttpHandler
    {
        #region IHttpHandler Members
        public bool IsReusable
        {
            get { return false; }
        }
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                //Uploaded File Deletion
                if (context.Request.QueryString.Count > 0)
                {
                    string filePath = @"c:\DownloadedFiles\" + context.Request.QueryString[0].ToString();
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
                //File Upload
                else
                {
                    string fileName = Path.GetFileName(context.Request.Files[0].FileName);
                    string location = @"c:\DownloadedFiles\" + fileName;
                    context.Request.Files[0].SaveAs(location);
                }
            }
            catch
            {
            }
        }
        #endregion
    }
}