using System;
using System.Collections.Generic;
using System.Net;

namespace Test
{
    public class Program
    {
        public static void Main(string[] args)
        {

            string ProxyHost = "http://localhost";
            int ProxyPort = 8005;
            //string ProxyUser = "";
            //string ProxyPassword = ""; 
            //string ProxyDomain = "";

            WebProxy webProxy = new WebProxy(ProxyHost,
            ProxyPort);

            //webProxy.Credentials = new NetworkCredential(
            //    ProxyUser, 
            //    ProxyPassword, 
            //    ProxyDomain);

            // WebRequest.DefaultWebProxy = WebRequest.GetSystemWebProxy();

            mediationServer.WebService ws = new mediationServer.WebService();

            ws.Proxy = webProxy;

            List<User.File> listOfFiles = new List<User.File>();
            User.File f1 = new User.File("");
            //User.File f2 = new User.File("f2", 500);
            listOfFiles.Add(f1);
            //listOfFiles.Add(f2); 

            User user = new User("admon", "admon", listOfFiles);

            Console.WriteLine(ws.RequestFiles(user.getXMLFormatToString()));
        }
    }
}