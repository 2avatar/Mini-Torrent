using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;

namespace MediationServer
{
    public class MyProxy : IWebProxy
    {
        ICredentials IWebProxy.Credentials
        {
            get
            {
                string username = ConfigurationManager.AppSettings["username"];
                string password = ConfigurationManager.AppSettings["password"];
                return new NetworkCredential(username, password);
            }
            set { }
        }

        Uri IWebProxy.GetProxy(Uri destination)
        {
            string proxy = ConfigurationManager.AppSettings["proxyaddress"];
            return new Uri(proxy);
        }

        bool IWebProxy.IsBypassed(Uri host)
        {
            return false;
        }
    }
}