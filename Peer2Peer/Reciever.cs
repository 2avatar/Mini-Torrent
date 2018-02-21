using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Peer2Peer
{

    public static class AsynchronousSocketListener
    {
        // Thread signal.  
        private static ManualResetEvent allDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
        private static AutoResetEvent connectDone = new AutoResetEvent(false);

        public static string DownloadFolderPath { get; set; }
        public static string FileNameToRequest { get; set; }

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

                // Set the event to nonsignaled state.  
                allDone.Reset();

                // Start an asynchronous socket to listen for connections.  

                listener.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    listener);

                // Wait until a connection is made before continuing.  
                allDone.WaitOne();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void StartClient(string ip, string port)
        {


            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ip), int.Parse(port));
            // Use IPv4 as the network protocol,if you want to support IPV6 protocol, you can update here.
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Begin to connect the server.
            clientSocket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), clientSocket);
            connectDone.WaitOne();


            // Begin to receive the file after connecting to server successfully.
            Receive(clientSocket);
            receiveDone.WaitOne();

            // Notify the user whether receive the file completely.
            //    Client.BeginInvoke(new FileReceiveDoneHandler(Client.FileReceiveDone));

            // Close the socket.
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket clientSocket = (Socket)ar.AsyncState;

                clientSocket.EndConnect(ar);
            }
            catch
            {
                //        MessageBox.Show(Properties.Resources.InvalidConnectionMsg);
                //       Client.BeginInvoke(new EnableConnectButtonHandler(Client.EnableConnectButton));
                connectDone.Set();
                return;
            }

            //     Client.BeginInvoke(new ConnectDoneHandler(Client.ConnectDone));

            connectDone.Set();
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject1 state = new StateObject1();
            state.WorkSocket = handler;

            requestFile(handler, FileNameToRequest);
            sendDone.WaitOne();

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        private static void requestFile(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);


                sendDone.Set();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
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

            fileSavePath = DownloadFolderPath + "/" + fileName;

            // Get the file length from the server.
            byte[] fileLenByte = new byte[8];
            clientSocket.Receive(fileLenByte);
            fileLen = BitConverter.ToInt64(fileLenByte, 0);
        }

        /// <summary>
        /// Receive the file send by the server.
        /// </summary>
        /// <param name="clientSocket"></param>
        private static void Receive(Socket clientSocket)
        {
            StateObject1 state = new StateObject1();
            state.WorkSocket = clientSocket;

            ReceiveFileInfo(clientSocket);

            int progressLen = checked((int)(fileLen / StateObject1.BufferSize + 1));
            object[] length = new object[1];
            length[0] = progressLen;
            //     Client.BeginInvoke(new SetProgressLengthHandler(Client.SetProgressLength), length);

            // Begin to receive the file from the server.
            try
            {
                clientSocket.BeginReceive(state.Buffer, 0, StateObject1.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch
            {
                if (!clientSocket.Connected)
                {
                    HandleDisconnectException();
                }
            }
        }

        /// <summary>
        /// Callback when receive a file chunk from the server successfully.
        /// </summary>
        /// <param name="ar"></param>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            StateObject1 state = (StateObject1)ar.AsyncState;
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
                //   Client.BeginInvoke(new ProgressChangeHandler(Client.ProgressChanged));

                // Recursively receive the rest file.
                try
                {
                    clientSocket.BeginReceive(state.Buffer, 0, StateObject1.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
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

        //public static int Main(String[] args)
        //{
        //    AsynchronousSocketListener.DownloadFolderPath = "";
        //    AsynchronousSocketListener.FileNameToRequest = "";
        //    StartListening("8001");
        //    StartClient("127.0.0.1", "8002");
        //    return 0;
        //}
    }
}