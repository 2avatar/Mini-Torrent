using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;

namespace MediationServer
{
    public class test
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

        private WebService ws = new WebService();
        private XmlDocument xmlDoc = new XmlDocument();

        public void Main(string[] args)
        {
            createUserElementXML("eviatar", "eviatar");
            createFilesElementXML();
            createFileElementXML("f1", "230");
            createFileElementXML("f2", "350");

            Console.WriteLine(ws.SignIn(xmlDoc.OuterXml));
        }

        private void createUserElementXML(string username, string password)
        {         
            XmlNode userNode = xmlDoc.CreateElement(USER_ELEMENT);
            xmlDoc.AppendChild(userNode);

            XmlAttribute attribute;

            attribute = xmlDoc.CreateAttribute(USER_ATTRIBUTE_NAME);
            attribute.Value = username;
            userNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute(USER_ATTRIBUTE_PASSWORD);
            attribute.Value = ComputeHash(password);
            userNode.Attributes.Append(attribute);
        }

        private void createAddressElementXML(string IP, string PORT)
        {
            XmlNode userNode = xmlDoc.SelectSingleNode(USER_PATH);

            XmlNode addressNode = xmlDoc.CreateElement(ADDRESS_ELEMENT);

            XmlAttribute attribute;

            attribute = xmlDoc.CreateAttribute(ADDRESS_ATTRIBUTE_IP);
            attribute.Value = IP;
            addressNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute(ADDRESS_ATTRIBUTE_PORT);
            attribute.Value = PORT;
            addressNode.Attributes.Append(attribute);
            userNode.AppendChild(addressNode);
        }

        private void createFilesElementXML()
        {
            XmlNode filesNode = xmlDoc.CreateElement(FILES_ELEMENT);
            XmlNode userNode = xmlDoc.SelectSingleNode(USER_PATH);
            userNode.AppendChild(filesNode);
        }

        private void createFileElementXML(string fileName, string fileSize)
        {
            XmlNode fileNode;
            XmlNode filesNode = xmlDoc.SelectSingleNode(FILES_PATH);
            XmlAttribute attribute;

            fileNode = xmlDoc.CreateElement(FILE_ELEMENT);
            attribute = xmlDoc.CreateAttribute(FILE_ATTRIBUTE_NAME);
            attribute.Value = fileName;
            fileNode.Attributes.Append(attribute);
            attribute = xmlDoc.CreateAttribute(FILE_ATTRIBUTE_SIZE);
            attribute.Value = fileSize;
            fileNode.Attributes.Append(attribute);
            filesNode.AppendChild(fileNode);
        }

        private string ComputeHash(string hash)
        {
            SHA256 sha256 = SHA256.Create();
            return byteToString(sha256.ComputeHash(stringToByte(hash)));
        }

        private byte[] stringToByte(string buffer)
        {
            return Encoding.UTF8.GetBytes(buffer);
        }

        private string byteToString(byte[] buffer)
        {
            return BitConverter.ToString(buffer).Replace("-", "");
        }
    }
}