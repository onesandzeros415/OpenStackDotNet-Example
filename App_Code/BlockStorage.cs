using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Core.Exceptions.Response;
using net.openstack.Providers.Rackspace;
using net.openstack.Providers.Rackspace.Exceptions;
using net.openstack.Providers.Rackspace.Objects;

namespace OpenStackDotNet_Test
{
    public class BlockStorage
    {
        public static CloudBlockStorageProvider blockStorageProvider()
        {
            string CloudIdentityUserName = (string)(HttpContext.Current.Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(HttpContext.Current.Session["CloudIdentityApiKey"]);

            RackspaceCloudIdentity identity = new RackspaceCloudIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudBlockStorageProvider CloudBlockStorageProvider = new net.openstack.Providers.Rackspace.CloudBlockStorageProvider(identity);

            return CloudBlockStorageProvider;
        }
        public static string CBS_m_ListAttachedVolumes(string dcregion)
        {
            IEnumerable<Volume> ListVolumes = BlockStorage.blockStorageProvider().ListVolumes(dcregion);

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

            return GetVolumeID_SB.ToString();
        }
        public static void CBS_m_CreateSnapShot(string snapshotid, string displayName, string displayDescription, string dcregion, bool force = false)
        {
            var CreateSnapShot = blockStorageProvider().CreateSnapshot(snapshotid, force, displayName, displayDescription, dcregion);
        }
        public static void CBS_m_DeleteSnapShot(string snapshotid, string dcregion)
        {
            var DeleteSnapShot = blockStorageProvider().DeleteSnapshot(snapshotid, dcregion);
        }
        public static void CBS_m_CreateVolume(int size, string display_description, string displayname, string volumetype, string dcregion)
        {
            var CBSCreateVolume = blockStorageProvider().CreateVolume(size, display_description, displayname, null, volumetype, dcregion);
        }
        public static void CBS_m_DeleteVolume(string volumeid, string dcregion)
        {
            var CBSDeleteVolume = blockStorageProvider().DeleteVolume(volumeid, dcregion);
        }
        public static IEnumerable<VolumeType> CBS_m_ListVolumesType(string dcregion)
        {
            IEnumerable<VolumeType> ListVolumeTypes = blockStorageProvider().ListVolumeTypes(dcregion);

            return ListVolumeTypes;
        }
        public static IEnumerable<Snapshot> CBS_m_ListSnapShots(string dcregion)
        {
            IEnumerable<Snapshot> ListSnapshots = blockStorageProvider().ListSnapshots(dcregion);

            return ListSnapshots;
        }
        public static IEnumerable<Volume> CBS_m_ListVolumes(string dcregion)
        {
            IEnumerable<Volume> ListVolumes = blockStorageProvider().ListVolumes(dcregion);

            return ListVolumes;
        }
    }
}