using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Peer2Peer
{
    public class SocketListener
    {
        class StateObject
        {
            public Socket WorkSocket = null;

            public const int BufferSize = 5242880;

            public byte[] Buffer = new byte[BufferSize];

            public bool Connected = false;
        }

        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        private delegate void SetProgressLengthHandler(int len);
        private delegate void ProgressChangeHandler();

        public static Torrent UI { get; set; }

        public static string FileNameToRequest { get; set; }
        public static string SharedFolderPath { get; set; }

        private static string fileName;
        private static string fileSavePath = "C:/";
        private static long fileLen;

        public static void StartListening(string port)
        {
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, int.Parse(port));

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);


                while (true)
                {
                    // Set the event to nonsignaled state.  
                    // Start an asynchronous socket to listen for connections.  
                    Socket handler = listener.Accept();

                    requestFile(handler, FileNameToRequest);

                    Receive(handler);
                    receiveDone.WaitOne();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void requestFile(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data + "<EOF>");

            // Begin sending the data to the remote device.  
            handler.Send(byteData);
        }

        private static void Receive(Socket clientSocket)
        {
            StateObject state = new StateObject();
            state.WorkSocket = clientSocket;

            ReceiveFileInfo(clientSocket);

            int progressLen = checked((int)(fileLen / StateObject.BufferSize + 1));
            object[] length = new object[1];
            length[0] = progressLen;
            UI.Dispatcher.BeginInvoke(new SetProgressLengthHandler(UI.SetProgressLength), length);

            //     Client.BeginInvoke(new SetProgressLengthHandler(Client.SetProgressLength), length);

            // Begin to receive the file from the server.
            try
            {
                clientSocket.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch
            {
                if (!clientSocket.Connected)
                {
                    HandleDisconnectException();
                }
            }
        }

        private static void ReceiveFileInfo(Socket clientSocket)
        {
            // Get the filename length from the server.
            byte[] fileNameLenByte = new byte[4];
            try
            {
                clientSocket.Receive(fileNameLenByte);
            }
            catch
            {
                if (!clientSocket.Connected)
                {
                    HandleDisconnectException();
                }
            }
            int fileNameLen = BitConverter.ToInt32(fileNameLenByte, 0);

            // Get the filename from the server.
            byte[] fileNameByte = new byte[fileNameLen];

            try
            {
                clientSocket.Receive(fileNameByte);
            }
            catch
            {
                if (!clientSocket.Connected)
                {
                    HandleDisconnectException();
                }
            }

            fileName = Encoding.ASCII.GetString(fileNameByte, 0, fileNameLen);

            fileSavePath = SharedFolderPath + "/" + fileName;

            // Get the file length from the server.
            byte[] fileLenByte = new byte[8];
            clientSocket.Receive(fileLenByte);
            fileLen = BitConverter.ToInt64(fileLenByte, 0);
        }



        private static void ReceiveCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket clientSocket = state.WorkSocket;
            BinaryWriter writer;

            int bytesRead = clientSocket.EndReceive(ar);
            if (bytesRead > 0)
            {
                //If the file doesn't exist, create a file with the filename got from server. If the file exists, append to the file.
                if (!File.Exists(fileSavePath))
                {
                    writer = new BinaryWriter(File.Open(fileSavePath, FileMode.Create));
                }
                else
                {
                    writer = new BinaryWriter(File.Open(fileSavePath, FileMode.Append));
                }

                writer.Write(state.Buffer, 0, bytesRead);
                writer.Flush();
                writer.Close();

                // Notify the progressBar to change the position.
                     UI.Dispatcher.BeginInvoke(new ProgressChangeHandler(UI.ProgressChanged));

                // Recursively receive the rest file.
                try
                {
                    clientSocket.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                catch
                {
                    if (!clientSocket.Connected)
                    {
                        // MessageBox.Show(Properties.Resources.DisconnectMsg);
                    }
                }
            }
            else
            {
                // Signal if all the file received.
                receiveDone.Set();
            }
        }


        private static void HandleDisconnectException()
        {
            //MessageBox.Show(Properties.Resources.DisconnectMsg);
            // Thread.CurrentThread.Abort();
        }

    }
}