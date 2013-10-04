using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
    public class Files
    {
        public static CloudFilesProvider filesProvider()
        {
            string CloudIdentityUserName = (string)(HttpContext.Current.Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(HttpContext.Current.Session["CloudIdentityApiKey"]);

            RackspaceCloudIdentity identity = new RackspaceCloudIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudFilesProvider CloudFilesProvider = new net.openstack.Providers.Rackspace.CloudFilesProvider(identity);

            return CloudFilesProvider;
        }
        public static IEnumerable<Container> CF_m_ListContainers(string dcregion, bool dcsnet = false)
        {
            IEnumerable<Container> CfContainers = filesProvider().ListContainers(null, null, null, dcregion, dcsnet);

            return CfContainers;
        }
        public static IEnumerable<ContainerObject> CF_m_ListContainerObjects(string cfcontainername, string dcregion, bool dcsnet = false)
        {
            IEnumerable<ContainerObject> Cfobjects = filesProvider().ListObjects(cfcontainername, null, null, null, null, dcregion, dcsnet);

            return Cfobjects;
        }
        public static string CF_m_EnableCDNContainer(string ContainerName, long ttl, string dcregion, bool logretention = false)
        {
            var EnableCDNOnContainer = Files.filesProvider().EnableCDNOnContainer(ContainerName, ttl, logretention, dcregion);

            var EnableCDNOnContainer_SB = new StringBuilder();

            foreach (var i in EnableCDNOnContainer)
            {
                EnableCDNOnContainer_SB.AppendLine(i.Key + " " + i.Value + "<br />");
            }

            return EnableCDNOnContainer_SB.ToString();
        }
        public static string CF_m_DisableCDNContainer(string ContainerName, string dcregion, bool dcsnet = false)
        {
            var DisableCDNOnContainer = Files.filesProvider().DisableCDNOnContainer(ContainerName, dcregion);

            try
            {
                var cdnContainerHeaderResponse = Files.filesProvider().GetContainerCDNHeader(ContainerName);

                if (cdnContainerHeaderResponse.CDNEnabled == false)
                {
                    return "CDN has been disabled.";
                }
                else
                {
                    return "CDN has not been disabled.  Please try again.";
                }
            }
            catch
            {
                return "CDN has been disabled.";
            }
        }
        public static string getAccountHeaderResponse(string containername, string strToFind, string region, bool snet = false)
        {
            var AccountHeadersResponse = filesProvider().GetAccountHeaders(region, snet);
            string value = "";

            if (AccountHeadersResponse.ContainsKey(strToFind))
            {
                AccountHeadersResponse.TryGetValue(strToFind, out value);
                return value;
            }

            return "No value found in header response.";
        }
        public static string getContainerHeaderResponse(string containername, string strToFind, string region, bool snet = false)
        {
            var ContainerHeaderResponse = filesProvider().GetContainerHeader(containername, region, snet);

            string value = "";

            if (ContainerHeaderResponse.ContainsKey(strToFind))
            {
                ContainerHeaderResponse.TryGetValue(strToFind, out value);
                return value;
            }

            return "No value found in header response.";
        }
        public static string getObjectHeaderResponse(string containername, string objectName, string strToFind, string region, bool snet = false)
        {
            var ContainerHeaderResponse = filesProvider().GetObjectHeaders(containername, objectName, region, snet);

            string value = "";

            if (ContainerHeaderResponse.ContainsKey(strToFind))
            {
                ContainerHeaderResponse.TryGetValue(strToFind, out value);
                return value;
            }

            return "No value found in header response.";
        }
        public static string CF_m_AccountDetails(string dcregion, bool dcsnet = false)
        {
            var AccountHeadersResponse = filesProvider().GetAccountHeaders(dcregion, dcsnet);
            var bytesused = CloudFilesProvider.AccountBytesUsed;
            string value = "";

            var AccountHeadersRespond_SB = new StringBuilder();

            if (AccountHeadersResponse.ContainsKey(bytesused))
            {
                AccountHeadersResponse.TryGetValue(bytesused, out value);
                string FinalByteSize = Main.CF_m_ConvertByteSize(double.Parse(value));
                AccountHeadersRespond_SB.AppendLine("Overall account size is : " + FinalByteSize + "<br /><br />");
            }

            foreach (var i in AccountHeadersResponse)
            {
                AccountHeadersRespond_SB.AppendLine(i.Key + " " + i.Value + "<br />");
            }

            return AccountHeadersRespond_SB.ToString();
        }
        
        public static string CF_m_ContainerDetails(string containername, string dcregion, bool dcsnet = false)
        {
            try
            {
                var ContainerHeaderResponse = filesProvider().GetContainerHeader(containername, dcregion, dcsnet);
                
                var bytesused = CloudFilesProvider.ContainerBytesUsed;
                string value = "";

                var ContainerHeaderRespond_SB = new StringBuilder();

                if (ContainerHeaderResponse.ContainsKey(bytesused))
                {
                    ContainerHeaderResponse.TryGetValue(bytesused, out value);
                    string FinalByteSize = Main.CF_m_ConvertByteSize(double.Parse(value));
                    ContainerHeaderRespond_SB.AppendLine("Container size is : " + FinalByteSize + "<br /><br />");
                }


                foreach (var i in ContainerHeaderResponse)
                {
                    ContainerHeaderRespond_SB.AppendLine(i.Key + " " + i.Value + "<br />");
                }

                return ContainerHeaderRespond_SB.ToString();
            }
            catch (Exception ex)
            {
                return "There are no containers in this datacenter.";
            }
        }
        public static string CF_m_ContainerCDNDetails(string ContainerName, string dcregion)
        {
            try
            {
                var ContainerHeaderResponse = filesProvider().GetContainerCDNHeader(ContainerName, dcregion);

                string Name = ContainerHeaderResponse.Name;
                string CDNUri = ContainerHeaderResponse.CDNUri;
                string CDNSslUri = ContainerHeaderResponse.CDNSslUri;
                string CDNStreamingUri = ContainerHeaderResponse.CDNStreamingUri;
                string CDNIosUri = ContainerHeaderResponse.CDNIosUri;

                string CDNEnabled = ContainerHeaderResponse.CDNEnabled.ToString();
                bool LogRetention = ContainerHeaderResponse.LogRetention;

                long Ttl = ContainerHeaderResponse.Ttl;

                return " CDN Enabled : " + CDNEnabled + "<br />" + " CDN Container Name : " + Name + "<br />" + " CDN URI : " + CDNUri + "<br />" + " CDN SSL URI : " + CDNSslUri + "<br />" + " CDN Streaming URI : " + CDNStreamingUri + "<br />" + " CDN IOS URI : " + CDNIosUri + "<br />" + " TTL : " + Ttl.ToString() + "<br />";
            }
            catch (Exception ex)
            {
                return " CDN is not enabled.";
            }
        }
        public static string CF_m_ContainerObjectDetails(string ContainerName, string ContainerObjectName, string dcregion, bool dcsnet = false)
        {
            var ContainerHeaderResponse = filesProvider().GetObjectHeaders(ContainerName, ContainerObjectName, dcregion, dcsnet);
            var ContainerHeaderRespond_SB = new StringBuilder();

            foreach (var i in ContainerHeaderResponse)
            {
                ContainerHeaderRespond_SB.AppendLine(i.Key + " " + i.Value + "<br />");
            }

            return ContainerHeaderRespond_SB.ToString();
        }
        public static void CF_m_CreateContainer(string cfcreatecontainername, string dcregion, bool dcsnet = false)
        {
            filesProvider().CreateContainer(cfcreatecontainername, null, dcregion, dcsnet);
        }
        public static void CF_m_DeleteContainer(string cfdeletecontainername, string dcregion, bool deleteAllObjs = false, bool dcsnet = false)
        {
            filesProvider().DeleteContainer(cfdeletecontainername, deleteAllObjs, dcregion, dcsnet);
        }
        public static void CF_m_CopyObj(string cfcontainername, string cfcopyobj, string cfdestobjcontainer, string destcopyobjname, string dcregion, bool dcsnet = false)
        {
            filesProvider().CopyObject(cfcontainername, cfcopyobj, cfdestobjcontainer, destcopyobjname, null, null, dcregion, dcsnet);
        }
        public static void CF_m_MoveObj(string cfcontainername, string cfcopyobj, string cfdestobjcontainer, string destobjname, string dcregion, bool dcsnet = false)
        {
            filesProvider().MoveObject(cfcontainername, cfcopyobj, cfdestobjcontainer, destobjname, null, null, dcregion, dcsnet);
        }
        public static void CF_m_GetObjectSaveToFile(string cfcontainername, string saveDirectory, string objname, string fileName, int CF_m_GetObjectSaveToFilechunksize, string dcregion, bool verifyEtag = false, bool dcsnet = false)
        {
            filesProvider().GetObjectSaveToFile(cfcontainername, saveDirectory, objname, fileName, CF_m_GetObjectSaveToFilechunksize, null, dcregion, verifyEtag, null, dcsnet);
        }
        public static void CF_m_DeleteContainerObject(string cfcontainername, string cfdeletecontainerobject, string dcregion, bool deleteSegments = false, bool dcsnet = false)
        {
            filesProvider().DeleteObject(cfcontainername, cfdeletecontainerobject, null, deleteSegments, dcregion, dcsnet);
        }
        public static void CF_m_BulkDelete(string cfcontainername, string dcregion, bool deleteSegments = false, bool dcsnet = false)
        {
            filesProvider().LargeFileBatchThreshold = 81920;
            var listOfObjToDelete = filesProvider().ListObjects(cfcontainername, null, null, null, null, dcregion, dcsnet).Select(o => new KeyValuePair<string, string>(cfcontainername, o.Name));
            filesProvider().BulkDelete(listOfObjToDelete, null, dcregion, dcsnet);
        }
        public static void CF_m_CreateObjectFromFile(string cfcontainername, string cfcreateobjfilepath, string cfcreateobjfilename, string contenttype, int cfcreateobjchunksize, string dcregion, bool dcsnet = false)
        {
            filesProvider().CreateObjectFromFile(cfcontainername, cfcreateobjfilepath, cfcreateobjfilename, contenttype, cfcreateobjchunksize, null, dcregion, null, dcsnet);
        }
    }
}