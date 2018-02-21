using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Peer2Peer
{
    public class UserDataBinding
    {
        private string usernameValue;

        public string Username
        {
            get { return usernameValue; }
            set { usernameValue = value; }
        }

        private string passwordValue;

        public string Password
        {
            get { return passwordValue; }
            set { passwordValue = value;}
        }

        private string IPValue;

        public string IP
        {
            get { return IPValue; }
            set { IPValue = value; }
        }

        private string PORTValue;

        public string PORT
        {
            get { return PORTValue; }
            set { PORTValue = value; }
        }

        private string FolderPathValue;

        public string FolderPath
        {
            get { return FolderPathValue; }
            set { FolderPathValue = value; }
        }

        public bool checkUserValidation()
        {

            if (string.IsNullOrEmpty(Username) &&
                string.IsNullOrEmpty(Password) &&
                string.IsNullOrEmpty(IP) &&
                string.IsNullOrEmpty(PORT))
            {
                return false;
            }

            try
            {
                IPAddress.Parse(IP);
                Convert.ToInt32(PORT);
                int port = int.Parse(PORT);

                if (port < 0 || port > 65535)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
