using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.IO;

namespace Peer2Peer
{
    public class Client
    {

        private const int c_clientSockets = 100;
        private const int c_bufferSize = 5242880;

        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);

        public static string SharedFolderPath { get; set; }
        private static string FileName { get; set; }
        private static string FileToSend { get; set; }

        public static void StartClient(string ip, string port)
        {
            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // The name of the   
                // remote device is "host.contoso.com".  
                IPHostEntry ipHostInfo = Dns.GetHostEntry(ip);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, int.Parse(port));

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                client.Connect(remoteEP);

                // Receive the response from the remote device.  
                Receive(client);

                FileToSend = SharedFolderPath + "\\" + FileName;

                Send(client);
                sendDone.WaitOne();

                // Release the socket.  
                // client.Shutdown(SocketShutdown.Both);
                // client.Disconnect(true);
                // client.Close();



            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Receive(Socket handler)
        {
            try
            {
                while (true)
                {
                    string data = null;
                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (data.IndexOf("<EOF>") > -1)
                    {
                        FileName = data.Substring(0, data.Length - "<EOF>".Length);
                        break;
                    }
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
        }

        public static void Send(Socket client)
        {
            int readBytes = 0;
            byte[] buffer = new byte[c_bufferSize];

            // Send file information to the clients.
            SendFileInfo(client);

            // Blocking read file and send to the clients asynchronously.
            using (FileStream stream = new FileStream(FileToSend, FileMode.Open))
            {
                do
                {
                    sendDone.Reset();
                    stream.Flush();
                    readBytes = stream.Read(buffer, 0, c_bufferSize);

                    client.BeginSend(buffer, 0, readBytes, SocketFlags.None, new AsyncCallback(SendCallback), client);

                } while (readBytes > 0);
                sendDone.WaitOne();

            }
            while (readBytes > 0) ;
        }

        private static void SendCallback(IAsyncResult ar)
        {

            Socket handler = null;
            try
            {
                handler = (Socket)ar.AsyncState;
                int bytesSent = handler.EndSend(ar);

                // Close the socket when all the data has sent to the client.
                if (bytesSent == 0)
                {
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (ArgumentException argEx)
            {
                //   MessageBox.Show(argEx.Message);
            }
            catch (SocketException)
            {
                // Close the socket if the client disconnected.
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            finally
            {
                sendDone.Set();
            }
        }

        public static void SendFileInfo(Socket client)
        {


            string fileName = FileToSend.Replace("\\", "/");

            while (fileName.IndexOf("/") > -1)
            {
                fileName = fileName.Substring(fileName.IndexOf("/") + 1);
            }

            FileInfo fileInfo = new FileInfo(FileToSend);
            long fileLen = fileInfo.Length;

            byte[] fileLenByte = BitConverter.GetBytes(fileLen);

            byte[] fileNameByte = Encoding.ASCII.GetBytes(fileName);

            byte[] clientData = new byte[4 + fileNameByte.Length + 8];

            byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);

            fileNameLen.CopyTo(clientData, 0);
            fileNameByte.CopyTo(clientData, 4);
            fileLenByte.CopyTo(clientData, 4 + fileNameByte.Length);



            client.Send(clientData);

        }
    }
}
