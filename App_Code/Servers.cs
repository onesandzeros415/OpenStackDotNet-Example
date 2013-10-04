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
    public class Servers
    {
        public static CloudServersProvider serversProvider()
        {
            string CloudIdentityUserName = (string)(HttpContext.Current.Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(HttpContext.Current.Session["CloudIdentityApiKey"]);

            RackspaceCloudIdentity identity = new RackspaceCloudIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudServersProvider CloudServersProvider = new net.openstack.Providers.Rackspace.CloudServersProvider(identity);

            return CloudServersProvider;
        }
        public static CloudBlockStorageProvider blockStorageProvider()
        {
            string CloudIdentityUserName = (string)(HttpContext.Current.Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(HttpContext.Current.Session["CloudIdentityApiKey"]);

            RackspaceCloudIdentity identity = new RackspaceCloudIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudBlockStorageProvider CloudBlockStorageProvider = new net.openstack.Providers.Rackspace.CloudBlockStorageProvider(identity);

            return CloudBlockStorageProvider;
        }
        public static IEnumerable<SimpleServer> CS_m_ListServerInfo(string dcregion)
        {
            IEnumerable<SimpleServer> serverdetails = Servers.serversProvider().ListServers(null, null, null, null, null, null, null, dcregion);

            return serverdetails;
        }
        public static string CS_m_ListAddresses(string listServersSelectedValue, string dcregion)
        {
            ServerAddresses serverAddresses = Servers.serversProvider().ListAddresses(listServersSelectedValue, dcregion);

            var IpAddresses_SB = new StringBuilder();

            bool foundAddress = false;

            foreach (KeyValuePair<string, IPAddressList> addresses in serverAddresses)
            {
                IpAddresses_SB.Append("Network: " + addresses.Key + "<br />");

                foreach (IPAddress address in addresses.Value)
                {
                    foundAddress = true;
                    IpAddresses_SB.Append(address + "<br />");
                }
            }

            return IpAddresses_SB.ToString();
        }
        public static List<ListItem> CS_m_ListServerVolumeList(string dcregion)
        {
            var volumedetails = Servers.blockStorageProvider().ListVolumes(dcregion);

            var GetVolumeID = volumedetails.ToList();
            List<ListItem> volumelist = new List<ListItem>();

            foreach (var i in GetVolumeID)
            {
                var attachments_var = i.Attachments;
                if (attachments_var.Count() <= 0)
                {
                    ListItem ListItemDisplayName = new ListItem(i.DisplayName, i.Id);
                    volumelist.Add(ListItemDisplayName);
                }
                else
                {
                    foreach (var item in attachments_var)
                    {
                        ListItem ListItemDisplayName = new ListItem(i.DisplayName, i.Id);
                        volumelist.Add(ListItemDisplayName);
                    }
                }
            }
            return volumelist;
        }
        public static void CS_m_AttachVolume(string listserverid, string availablevolumeid, string dcregion)
        {
            var attachServerVolume = Servers.serversProvider().AttachServerVolume(listserverid, availablevolumeid, null, dcregion);

            var volumeIsInList = false;
            var count = 0;
            do
            {
                var serverdetails = Servers.serversProvider().GetDetails(listserverid, dcregion);
                var GetAttachedVolumes = serverdetails.GetVolumes().ToList();

                if (GetAttachedVolumes != null)
                    volumeIsInList = GetAttachedVolumes.Any(v => v.Id == attachServerVolume.Id);

                count += 1;

                Thread.Sleep(2400);
            } while (!volumeIsInList && count < 600);
        }
        public static void CS_m_DetachVolume(string listserverid, string availablevolumeid, string dcregion)
        {
            var volumedetails = Servers.serversProvider().DetachServerVolume(listserverid, availablevolumeid, dcregion);

            var volumeIsInList = false;
            var count = 0;

            do
            {
                var serverdetails = Servers.serversProvider().GetDetails(listserverid, dcregion);
                var volumes = serverdetails.GetVolumes();

                if (volumes != null)
                    volumeIsInList = volumes.Any(v => v.Id == availablevolumeid);

                count += 1;
            } while (volumeIsInList && count < 600);
        }
        public static void CS_m_RebootServer(string dcregion, string listserverid, string reboottype)
        {
            if (reboottype.ToString() == "soft")
            {
                Servers.serversProvider().RebootServer(listserverid, RebootType.Soft, dcregion);
            }
            else if (reboottype.ToString() == "hard")
            {
                Servers.serversProvider().RebootServer(listserverid, RebootType.Hard, dcregion);
            }
        }
        public static void CS_m_DeleteServer(string listserversid, string dcregion)
        {
            Servers.serversProvider().DeleteServer(listserversid, dcregion);
        }
        public static IEnumerable<Flavor> CS_m_ListFlavors()
        {
            IEnumerable<Flavor> flavordetails = Servers.serversProvider().ListFlavors();

            return flavordetails;
        }
        public static IEnumerable<SimpleServerImage> CS_m_ListImages()
        {
            IEnumerable<SimpleServerImage> images = Servers.serversProvider().ListImages();
            return images;
        }
        public static void CS_m_PasswdReset (string listserverid, string newpassword, string dcregion)
        {
            Servers.serversProvider().ChangeAdministratorPassword(listserverid, newpassword, dcregion);
        }
        public static string CS_m_CreateServer(string servername, string listimageid, string listflavorsid, string howmanyserverstocreate,string dcregion)
        {
            StringBuilder CS_sb_CloudServerPasswd = new StringBuilder();

            int InputHowMany = int.Parse(howmanyserverstocreate);

            bool attachtoservicenetwork = bool.Parse("true");
            bool attachtopublicnetwork = bool.Parse("true");

            Servers.serversProvider();

            if (InputHowMany <= 10)
            {
                if (InputHowMany >= 1)
                {
                    int count = 1;
                    int countid = 1;
                    while (count <= InputHowMany)
                    {
                        count = count + 1;
                        countid = count - 1;
                        var server = Servers.serversProvider().CreateServer(servername + "_" + countid, listimageid, listflavorsid, null, null, null, attachtoservicenetwork, attachtopublicnetwork, null, dcregion);
                        var serverpasswd = server.AdminPassword.ToString();
                        CS_sb_CloudServerPasswd.Append(servername + "_" + countid + " " + serverpasswd.ToString() + "\r\n");
                    }

                    return "Servers have been created successfully.  Your admin password is: \r\n" + CS_sb_CloudServerPasswd;
                }
                else
                {
                    var server = Servers.serversProvider().CreateServer(servername, listimageid, listflavorsid, null, null, null, attachtoservicenetwork, attachtopublicnetwork, null, dcregion);
                    var serverpasswd = server.AdminPassword.ToString();

                    return servername + " created successfully.  Your admin password is: " + serverpasswd;
                }
            }
            else
            {
                return "You can only create 10 servers at a time.";
            }
        }
    }
}