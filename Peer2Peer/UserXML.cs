using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Peer2Peer
{
    public class UserXML
    {
        // user create element
        private static string USER_ELEMENT = "User";
        // user retrieval path
        private static string USER_PATH = "//" + USER_ELEMENT;
        // user attributes
        private static string USER_ATTRIBUTE_NAME = "Username";
        private static string USER_ATTRIBUTE_PASSWORD = "Password";
        // address create element
        private static string ADDRESS_ELEMENT = "Address";
        // address retrieval path
        private static string ADDRESS_PATH = "//" + ADDRESS_ELEMENT;
        // address attributes
        private static string ADDRESS_ATTRIBUTE_IP = "IP";
        private static string ADDRESS_ATTRIBUTE_PORT = "PORT";
        // files create element
        private static string FILES_ELEMENT = "Files";
        private static string FILES_PATH = "//" + FILES_ELEMENT;
        // file create element
        private static string FILE_ELEMENT = "File";
        // files retrieval path
        private static string FILE_PATH = "//" + FILE_ELEMENT;
        // file attributes
        private static string FILE_ATTRIBUTE_NAME = "Name";
        private static string FILE_ATTRIBUTE_SIZE = "Size";
        // file number of active users
        private static string FILE_ATTRIBUTE_NUMBER_OF_ACTIVE_USERS = "NumberOfActiveUsers";

        private XmlDocument xmlDoc = new XmlDocument();
        
        public UserXML(string XML)
        {
            xmlDoc.LoadXml(XML);
        }

        public UserXML(List<File> listOfFiles)
        {
            createUserElementXML();
            createFilesElementXML();
            foreach (File f in listOfFiles)
            {
                createFileElementXML();
                addFileNameAttribute(f.FileName);
                addFileSizeAttribute(f.FileSize.ToString());
                addFileNumberOfActiveUsers(f.NumberOfActiveUsers.ToString());
            }
        }

        public UserXML(string username, string password)
        {
            createUserElementXML();
            addUsernameAttribute(username);
            addPasswordAttribute(password);
        }

        public UserXML(string username, string password, List<File> listOfFiles)
        {
            createUserElementXML();
            addUsernameAttribute(username);
            addPasswordAttribute(password);

            createFilesElementXML();
            foreach (File f in listOfFiles)
            {
                createFileElementXML();
                addFileNameAttribute(f.FileName);
                addFileSizeAttribute(f.FileSize.ToString());
            }
        }


        public UserXML(string username, string password, string IP, string PORT, List<File> listOfFiles)
        {
            createUserElementXML();
            addUsernameAttribute(username);
            addPasswordAttribute(password);

            createAddressElementXML();
            addIPAttribute(IP);
            addPORTAttribute(PORT);

            createFilesElementXML();
            foreach (File f in listOfFiles)
            {
                createFileElementXML();
                addFileNameAttribute(f.FileName);
                addFileSizeAttribute(f.FileSize.ToString());
            }
        }


        private void createUserElementXML()
        {
            XmlNode userNode = xmlDoc.CreateElement(USER_ELEMENT);
            xmlDoc.AppendChild(userNode);
        }

        private void addUsernameAttribute(string username)
        {
            XmlNode userNode = xmlDoc.SelectSingleNode(USER_PATH);

            XmlAttribute attribute;
            attribute = xmlDoc.CreateAttribute(USER_ATTRIBUTE_NAME);
            attribute.Value = username;
            userNode.Attributes.Append(attribute);
        }

        private void addPasswordAttribute(string password)
        {
            XmlNode userNode = xmlDoc.SelectSingleNode(USER_PATH);

            XmlAttribute attribute;
            attribute = xmlDoc.CreateAttribute(USER_ATTRIBUTE_PASSWORD);
            attribute.Value = ComputeHash(password);
            userNode.Attributes.Append(attribute);
        }

        private void createAddressElementXML()
        {
            XmlNode userNode = xmlDoc.SelectSingleNode(USER_PATH);
            XmlNode addressNode = xmlDoc.CreateElement(ADDRESS_ELEMENT);
            userNode.AppendChild(addressNode);
        }

        private void addIPAttribute(string IP)
        {
            XmlNode addressNode = xmlDoc.SelectSingleNode(ADDRESS_PATH);

            XmlAttribute attribute;
            attribute = xmlDoc.CreateAttribute(ADDRESS_ATTRIBUTE_IP);
            attribute.Value = IP;
            addressNode.Attributes.Append(attribute);
        }

        private void addPORTAttribute(string PORT)
        {
            XmlNode addressNode = xmlDoc.SelectSingleNode(ADDRESS_PATH);

            XmlAttribute attribute;
            attribute = xmlDoc.CreateAttribute(ADDRESS_ATTRIBUTE_PORT);
            attribute.Value = PORT;
            addressNode.Attributes.Append(attribute);
        }

        private void createFilesElementXML()
        {
            XmlNode filesNode = xmlDoc.CreateElement(FILES_ELEMENT);
            XmlNode userNode = xmlDoc.SelectSingleNode(USER_PATH);
            userNode.AppendChild(filesNode);
        }

        private void createFileElementXML()
        {
            XmlNode fileNode = xmlDoc.CreateElement(FILE_ELEMENT);
            XmlNode filesNode = xmlDoc.SelectSingleNode(FILES_PATH);
            filesNode.AppendChild(fileNode);
        }

        private void addFileNameAttribute(string fileName)
        {
            XmlNode fileNode = xmlDoc.SelectSingleNode(FILES_PATH);

            XmlAttribute attribute;
            attribute = xmlDoc.CreateAttribute(FILE_ATTRIBUTE_NAME);
            attribute.Value = fileName;
            fileNode.LastChild.Attributes.Append(attribute);
        }

        private void addFileSizeAttribute(string fileSize)
        {
            XmlNode fileNode = xmlDoc.SelectSingleNode(FILES_PATH);

            XmlAttribute attribute;
            attribute = xmlDoc.CreateAttribute(FILE_ATTRIBUTE_SIZE);
            attribute.Value = fileSize;
            fileNode.LastChild.Attributes.Append(attribute);
        }

        private void addFileNumberOfActiveUsers(string fileNumberOfActiveUsers)
        {
            XmlNode fileNode = xmlDoc.SelectSingleNode(FILES_PATH);

            XmlAttribute attribute;
            attribute = xmlDoc.CreateAttribute(FILE_ATTRIBUTE_NUMBER_OF_ACTIVE_USERS);
            attribute.Value = fileNumberOfActiveUsers;
            fileNode.LastChild.Attributes.Append(attribute);
        }

        public string getXMLFormatToString()
        {
            return xmlDoc.OuterXml;
        }

        public string getUsername()
        {
            XmlNode user = xmlDoc.SelectSingleNode(USER_PATH);
            return user.Attributes[USER_ATTRIBUTE_NAME].Value;
        }

        public string getPassword()
        {
            XmlNode user = xmlDoc.SelectSingleNode(USER_PATH);
            return user.Attributes[USER_ATTRIBUTE_PASSWORD].Value;
        }

        public string getIP()
        {
            XmlNode address = xmlDoc.SelectSingleNode(ADDRESS_PATH);
            return address.Attributes[ADDRESS_ATTRIBUTE_IP].Value;
        }

        public string getPORT()
        {
            XmlNode address = xmlDoc.SelectSingleNode(ADDRESS_PATH);
            return address.Attributes[ADDRESS_ATTRIBUTE_PORT].Value;
        }

        public List<File> getFilesList()
        {
            XmlNodeList filesNodeList = xmlDoc.SelectNodes(FILE_PATH);
            List<File> filesList = new List<File>();

            foreach (XmlNode file in filesNodeList)
            {
                string fileName = file.Attributes[FILE_ATTRIBUTE_NAME].Value;
                int fileSize = int.Parse(file.Attributes[FILE_ATTRIBUTE_SIZE].Value);

                File f = new File(fileName, fileSize);
                filesList.Add(f);
            }
            return filesList;
        }

        public List<File> getFilesListWithNumberOfActiveUsers()
        {
            XmlNodeList filesNodeList = xmlDoc.SelectNodes(FILE_PATH);
            List<File> filesList = new List<File>();

            foreach (XmlNode file in filesNodeList)
            {
                string fileName = file.Attributes[FILE_ATTRIBUTE_NAME].Value;
                int fileSize = int.Parse(file.Attributes[FILE_ATTRIBUTE_SIZE].Value);
                int numberOfActiveUsers = int.Parse(file.Attributes[FILE_ATTRIBUTE_NUMBER_OF_ACTIVE_USERS].Value);

                File f = new File(fileName, fileSize, numberOfActiveUsers);
                filesList.Add(f);
            }
            return filesList;
        }

        private static string ComputeHash(string hash)
        {
            SHA256 sha256 = SHA256.Create();
            return byteToString(sha256.ComputeHash(stringToByte(hash)));
        }

        private static byte[] stringToByte(string buffer)
        {
            return Encoding.UTF8.GetBytes(buffer);
        }

        private static string byteToString(byte[] buffer)
        {
            return BitConverter.ToString(buffer).Replace("-", "");
        }

        public class File
        {
            public string FileName { get; private set; }
            public long FileSize { get; private set; }
            public int? NumberOfActiveUsers { get; private set; }

            public File(string fileName, long fileSize = -1, int? numberOfActiveUsers = -1)
            {
                FileName = fileName;
                FileSize = fileSize;
                NumberOfActiveUsers = numberOfActiveUsers;
            }
        }
    }
}