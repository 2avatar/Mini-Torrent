using System.Collections.Generic;
using System.Web.Services;

namespace MediationServer
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://mediationserver:8004/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
 
    public class WebService : System.Web.Services.WebService
    {

        [WebMethod]
        public bool SignIn(string xml)
        {
    
            if (string.IsNullOrEmpty(xml))
                return false;

            User user = new User(xml);

            if (!validateUser(user))
                return false;

            activateUser(user);

            deleteUserFiles(user);

            updateFiles(user);

            //XmlNode address = xmlDoc.SelectSingleNode("//Address");
            //string ip = address.Attributes["IP"].Value;
            //string port = address.Attributes["PORT"].Value;

            return true;
        }

        private void activateUser(User user)
        {
            string username = user.getUsername();

            var ds = new DataSetUsersTableAdapters.UsersTableAdapter();
            ds.ActivateUser(username);
        }

        public void updateFiles(User user)
        {
            string username = user.getUsername();
            List<User.File> files = user.getFilesList();

            var ds = new DataSetUsersTableAdapters.FilesTableAdapter();

            foreach (User.File file in files)
            {
               ds.Insert(username, file.getFileName(), file.getFileSize().ToString());
            }
        }

        [WebMethod]
        public string RequestFiles(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return null;

            User user = new User(xml);

            if (!validateUser(user))
                return null;

            if (!fileExists(user))
                return null;

            return requestedFiles(user);
        }

        private bool fileExists(User user)
        {
            var ds = new DataSetUsersTableAdapters.FilesTableAdapter();

            string fileName = user.getFilesList()[0].getFileName();

            var filesFound = ds.GetFilesByName(fileName, user.getUsername());

            return filesFound.Count == 0 ? false : true;
        }

        private string requestedFiles(User user)
        {
            var ds = new DataSetUsersTableAdapters.FilesTableAdapter();

            string fileName = user.getFilesList()[0].getFileName();

            var filesFound = ds.GetFilesByName(fileName, user.getUsername());

            List<User.File> requestedFiles = new List<User.File>();
            foreach (var file in filesFound)
            {
                User.File f = new User.File(file.Name, long.Parse(file.Size), ds.GetFileNumberOfActiveUsers(file.Name, user.getUsername()).Value);
                requestedFiles.Add(f);
            }

            User userWithRequestedFiles = new User(requestedFiles);

            return userWithRequestedFiles.getXMLFormatToString();
        }

        [WebMethod]
        public string GetNameByFilename(string filename)
        {
            var ds = new DataSetUsersTableAdapters.FilesTableAdapter();

            var rows = ds.GetNameByFile(filename);

            return rows[0].Username;
        }

        [WebMethod]
        public bool SignOut(string xml)
        {
               if (string.IsNullOrEmpty(xml))
                return false;

            User user = new User(xml);

            if (!validateUser(user))
                return false;

            deleteUserFiles(user);

            disableUser(user);

            return true;
        }

        private void deleteUserFiles(User user)
        {
            var ds = new DataSetUsersTableAdapters.FilesTableAdapter();
            ds.DeleteUserFiles(user.getUsername());
        }

        private void disableUser(User user)
        {
            string username = user.getUsername();

            var ds = new DataSetUsersTableAdapters.UsersTableAdapter();
            ds.DisableUser(username);
        }

        private bool validateUser(User user)
        {
            string username = user.getUsername();
            string password = user.getPassword();

            var ds = new DataSetUsersTableAdapters.UsersTableAdapter();

            return ds.ValidateUser(username, password) == 1 ? true : false;
        }
    }

}
