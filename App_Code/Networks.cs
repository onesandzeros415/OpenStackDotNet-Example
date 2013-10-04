using System;
using System.Collections.Generic;
using System.Web;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions.Response;
using net.openstack.Providers.Rackspace;
using net.openstack.Providers.Rackspace.Exceptions;
using net.openstack.Providers.Rackspace.Objects;

namespace OpenStackDotNet_Test
{
    public class Networks
    {
        public static CloudNetworksProvider networksProvider()
        {
            string CloudIdentityUserName = (string)(HttpContext.Current.Session["CloudIdentityUserName"]); ;
            string CloudIdentityApiKey = (string)(HttpContext.Current.Session["CloudIdentityApiKey"]);

            RackspaceCloudIdentity identity = new RackspaceCloudIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudNetworksProvider CloudNetworksProvider = new net.openstack.Providers.Rackspace.CloudNetworksProvider(identity);

            return CloudNetworksProvider;
        }
        public static IEnumerable<CloudNetwork> CN_m_ListNetworks(string dcregion)
        {
            IEnumerable<CloudNetwork> CloudNetworksListNetworks = networksProvider().ListNetworks(dcregion);

            return CloudNetworksListNetworks;
        }
        public static string CN_m_ShowNetworkInfo(string listNetworksSelectedValue, string dcregion)
        {
            if (listNetworksSelectedValue == "00000000-0000-0000-0000-000000000000")
            {
                return "No Details for public network";
            }
            else if (listNetworksSelectedValue == "11111111-1111-1111-1111-111111111111")
            {
                return "No Details for private network";
            }
            else
            {
                var CloudNetworksListNetworks = networksProvider().ShowNetwork(listNetworksSelectedValue, dcregion);

                if (string.IsNullOrEmpty(CloudNetworksListNetworks.Cidr))
                {
                    return "Network Name : " + CloudNetworksListNetworks.Label + "<br />" + "Network ID : " + CloudNetworksListNetworks.Id + "<br />";
                }
                else
                {
                    return "Network Name : " + CloudNetworksListNetworks.Label + "<br />" + "CIDR : " + CloudNetworksListNetworks.Cidr + "<br />" + "Network ID : " + CloudNetworksListNetworks.Id + "<br />";
                }
            }
        }
        public static void CN_m_CreateNetwork(string cidr, string networkname, string createNetworkName, string dcregion)
        {
            var CloudNetworksCreateNetwork = networksProvider().CreateNetwork("192.0.2.0/24", createNetworkName, dcregion);
        }
        public static void CN_m_DeleteNetwork(string networkid, string dcregion)
        {
            var CloudNetworksListNetworks = networksProvider().DeleteNetwork(networkid, dcregion);
        }
    }
}