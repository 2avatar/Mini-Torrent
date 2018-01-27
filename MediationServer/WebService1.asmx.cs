using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;

namespace MediationServer
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        [WebMethod]
        public bool SignIn(string xml)
        {
            if (!validateUser(xml))
                return false;

            activateUser(xml);

            updateFiles(xml);

            //XmlNode address = xmlDoc.SelectSingleNode("//Address");
            //string ip = address.Attributes["IP"].Value;
            //string port = address.Attributes["PORT"].Value;

            return true;
        }

        private void updateFiles(string xml)
        {
            string username = getUsername(xml);

            XmlNodeList files = getFilesList(xml);

            var ds = new DataSetUsersTableAdapters.FilesTableAdapter();

            foreach (XmlNode file in files)
            {
                string name = file.Attributes["name"].Value;
                int size = int.Parse(file.Attributes["size"].Value);
                ds.Insert(username, name, size);
            }
        }

        private void activateUser(string xml)
        {
            string username = getUsername(xml);

            var ds = new DataSetUsersTableAdapters.UsersTableAdapter();
            ds.ActivateUser(username);
        }

        [WebMethod]
        public bool SignOut(string xml)
        {
            if (!validateUser(xml))
                return false;

            disableUser(xml);

            return true;
        }

        private void disableUser(string xml)
        {
            string username = getUsername(xml);

            var ds = new DataSetUsersTableAdapters.UsersTableAdapter();
            ds.DisableUser(username);
        }

        private bool validateUser(string xml)
        {

            string username = getUsername(xml);
            string password = getPassword(xml);

            var ds = new DataSetUsersTableAdapters.UsersTableAdapter();
            return ds.ValidateUser(username, password) == 1 ? true : false;
        }

        private XmlNodeList getFilesList(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            return xmlDoc.SelectNodes("//File");
        }

        private string getUsername(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode user = xmlDoc.SelectSingleNode("//User");
            return user.Attributes["Username"].Value;
        }

        private string getPassword(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode user = xmlDoc.SelectSingleNode("//User");
            return user.Attributes["Password"].Value;
        }
    }
}
