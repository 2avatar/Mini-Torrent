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
    public class WebService : System.Web.Services.WebService
    {
        // user create element
        private static string USER_ELEMENT = "User";
        // user retrieval path
        private static string USER_PATH = "//" + USER_ELEMENT;
        // user attributes
        private static string USER_ATTRIBUTE_NAME = "Username";
        private static string USER_ATTRIBUTE_PASSWORD = "Password";
        // files create element
        private static string FILES_ELEMENT = "Files";
        // file create element
        private static string FILE_ELEMENT = "File";
        // files retrieval path
        private static string FILE_PATH = "//" + FILE_ELEMENT;
        // file attributes
        private static string FILE_ATTRIBUTE_NAME = "Name";
        private static string FILE_ATTRIBUTE_SIZE = "Size";
        // file number of active users
        private static string FILE_ATTRIBUTE_NUMBER_OF_ACTIVE_USERS = "NumberOfActiveUsers";

        [WebMethod]
        public bool SignIn(string xml)
        {
    
            if (string.IsNullOrEmpty(xml))
                return false;

            if (!validateUser(xml))
                return false;

            activateUser(xml);

            updateFiles(xml);

            //XmlNode address = xmlDoc.SelectSingleNode("//Address");
            //string ip = address.Attributes["IP"].Value;
            //string port = address.Attributes["PORT"].Value;

            return true;
        }

        private void activateUser(string xml)
        {
            string username = getUsername(xml);

            var ds = new DataSetUsersTableAdapters.UsersTableAdapter();
            ds.ActivateUser(username);
        }

        private void updateFiles(string xml)
        {
            string username = getUsername(xml);

            XmlNodeList files = getFilesList(xml);

            var ds = new DataSetUsersTableAdapters.FilesTableAdapter();

            ds.DeleteUserFiles(username);

            foreach (XmlNode file in files)
            {
                string name = file.Attributes[FILE_ATTRIBUTE_NAME].Value;
                int size = int.Parse(file.Attributes[FILE_ATTRIBUTE_SIZE].Value);
                ds.Insert(username, name, size);
            }
        }

        private XmlNodeList getFilesList(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            return xmlDoc.SelectNodes(FILE_PATH);
        }

        [WebMethod]
        public string RequestFiles(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return null;

            if (!validateUser(xml))
                return null;

            if (!fileExists(xml))
                return null;

            return requestedFiles(xml);
        }

        public bool fileExists(string xml)
        {
            var ds = new DataSetUsersTableAdapters.FilesTableAdapter();

            string fileName = getFileName(xml);

            var filesFound = ds.GetFilesByName(fileName);

            return filesFound.Count == 0 ? false : true;
        }

        public string requestedFiles(string xml)
        {
            var ds = new DataSetUsersTableAdapters.FilesTableAdapter();

            string fileName = getFileName(xml);

            var filesFound = ds.GetFilesByName(fileName);

            XmlDocument xmlDoc = new XmlDocument();
            XmlNode filesNode = xmlDoc.CreateElement(FILES_ELEMENT);
            xmlDoc.AppendChild(filesNode);

            XmlAttribute attribute;
            XmlNode fileNode;
            foreach (var file in filesFound)
            {
                fileNode = xmlDoc.CreateElement(FILE_ELEMENT);

                attribute = xmlDoc.CreateAttribute(FILE_ATTRIBUTE_NAME);
                attribute.Value = file.Name;
                fileNode.Attributes.Append(attribute);

                attribute = xmlDoc.CreateAttribute(FILE_ATTRIBUTE_SIZE);
                attribute.Value = file.Size.ToString();
                fileNode.Attributes.Append(attribute);

                attribute = xmlDoc.CreateAttribute(FILE_ATTRIBUTE_NUMBER_OF_ACTIVE_USERS);
                attribute.Value = ds.GetFileNumberOfActiveUsers(file.Name).ToString();
                fileNode.Attributes.Append(attribute);

                filesNode.AppendChild(fileNode);
            }

            return xmlDoc.OuterXml;
        }

        public string getFileName(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode file = xmlDoc.SelectSingleNode(FILE_PATH);
            return file.Attributes[FILE_ATTRIBUTE_NAME].Value;
        }

        [WebMethod]
        public bool SignOut(string xml)
        {
               if (string.IsNullOrEmpty(xml))
                return false;

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

        private string getUsername(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode user = xmlDoc.SelectSingleNode(USER_PATH);
            return user.Attributes[USER_ATTRIBUTE_NAME].Value;
        }

        private string getPassword(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode user = xmlDoc.SelectSingleNode(USER_PATH);
            return user.Attributes[USER_ATTRIBUTE_PASSWORD].Value;
        }
    }
}
