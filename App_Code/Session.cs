using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Main
{
    public class Main
    {
        public static void CloudIdentityUserNameSession(string CIUsername)
        {
            HttpContext.Current.Session["CloudIdentityUserName"] = CIUsername;
            string CloudIdentityUserName = (string)(HttpContext.Current.Session["CloudIdentityUserName"]);

            return;
        }
        public static void CloudIdentityApiKeySession(string CIApiKey)
        {
            HttpContext.Current.Session["CloudIdentityApiKey"] = CIApiKey;
            string CloudIdentityApiKey = (string)(HttpContext.Current.Session["CloudIdentityApiKey"]);

            return;
        }
    }
}