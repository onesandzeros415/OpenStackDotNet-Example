using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using net.openstack.Providers;
using net.openstack.Core;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace;
using net.openstack.Providers.Rackspace.Objects;


namespace OpenStackDotNet_Test
{
    public class Identity
    {
        public static CloudIdentityProvider identityProvider()
        {
            string CloudIdentityUserName = (string)(HttpContext.Current.Session["CloudIdentityUserName"]);
            string CloudIdentityApiKey = (string)(HttpContext.Current.Session["CloudIdentityApiKey"]);

            RackspaceCloudIdentity identity = new RackspaceCloudIdentity() { Username = CloudIdentityUserName, APIKey = CloudIdentityApiKey };

            CloudIdentityProvider identityProvider = new net.openstack.Providers.Rackspace.CloudIdentityProvider(identity);

            return identityProvider;
        }
        public static IEnumerable<User> CI_m_ListUsers()
        {
            CloudIdentityProvider cloudIdentityProvider = identityProvider();

            IEnumerable<User> ListUsers = cloudIdentityProvider.ListUsers(cloudIdentityProvider.DefaultIdentity);

            return ListUsers;
        }
        public static IEnumerable<Tenant> CI_m_ListTenants()
        {
            CloudIdentityProvider cloudIdentityProvider = identityProvider();
            IEnumerable<Tenant> ListTenants = cloudIdentityProvider.ListTenants(cloudIdentityProvider.DefaultIdentity);

            return ListTenants;
        }
        protected void CI_m_ListTenantsTest()
        {
            CloudIdentityProvider cloudIdentityProvider = identityProvider();

            IEnumerable<Tenant> ListTenants = cloudIdentityProvider.ListTenants(cloudIdentityProvider.DefaultIdentity);

            StringBuilder AccountId_SB = new StringBuilder();

            foreach (Tenant i in ListTenants)
            {
                AccountId_SB.Append(i.Id);
            }
        }
        public static IEnumerable<Role> CI_m_ListRoles()
        {
            CloudIdentityProvider cloudIdentityProvider = identityProvider();

            IEnumerable<Role> ListRoles = cloudIdentityProvider.ListRoles(null, null, null);

            return ListRoles;
        }
        public static List<KeyValuePair<string, string>> CI_m_ListUserRoles(string userId)
        {
            CloudIdentityProvider cloudIdentityProvider = identityProvider();

            IEnumerable<Role> ListRoles = cloudIdentityProvider.GetRolesByUser(userId, cloudIdentityProvider.DefaultIdentity);

            List<KeyValuePair<string, string>> _ListRoles = new List<KeyValuePair<string, string>>();

            foreach (Role role in ListRoles)
            {
                _ListRoles.Add(new KeyValuePair<string, string>(role.Name, role.Id));
            }

            return _ListRoles;
        }
        public static NewUser CI_m_NewUser(string username, string passwd, string email, bool enabled = true)
        {
            CloudIdentityProvider cloudIdentityProvider = identityProvider();

            NewUser newUser = new NewUser(username, email, passwd, enabled);

            NewUser user = cloudIdentityProvider.AddUser(newUser, cloudIdentityProvider.DefaultIdentity);

            return user;
        }
        public static bool CI_m_DeleteUser(string userid)
        {
            CloudIdentityProvider cloudIdentityProvider = identityProvider();

            Boolean deleteUser = cloudIdentityProvider.DeleteUser(userid, cloudIdentityProvider.DefaultIdentity);

            return deleteUser;
        }
        public static bool CI_m_AddRoleToUser(string userid, string roleid)
        {
            CloudIdentityProvider cloudIdentityProvider = identityProvider();

            Boolean addRoleToUser = cloudIdentityProvider.AddRoleToUser(userid, roleid, cloudIdentityProvider.DefaultIdentity);

            return addRoleToUser;
        }
        public static bool CI_m_DeleteRoleFromUser(string userid, string roleid)
        {
            CloudIdentityProvider cloudIdentityProvider = identityProvider();

            Boolean deleteRoleFromuser = cloudIdentityProvider.DeleteRoleFromUser(userid, roleid, cloudIdentityProvider.DefaultIdentity);

            return deleteRoleFromuser; 
        }
    }
}