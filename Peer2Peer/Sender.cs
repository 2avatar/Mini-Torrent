using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.IO;

namespace Peer2Peer
{
    public class AsynchronousClient
    {
        private const int c_clientSockets = 100;
        private const int c_bufferSize = 5242880;

        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent allDone =
            new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        // The response from the remote device. 
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
                client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                // Receive the response from the remote device.  
                Receive(client);
                receiveDone.WaitOne();

                // Release the socket.  
                client.Shutdown(SocketShutdown.Both);
                client.Close();


                FileToSend = SharedFolderPath + "\\" + FileName;

                Console.WriteLine(FileToSend);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void StartListening(string port)
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, int.Parse(port));
            // 
            // Use IPv4 as the network protocol,if you want to support IPV6 protocol, you can update here.
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
            }
            catch (SocketException ex)
            {
                //  MessageBox.Show(ex.Message);
                return;
            }

            listener.Listen(c_clientSockets);

            //loop listening the client.

            allDone.Reset();
            listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
            allDone.WaitOne();

        }


        private static void AcceptCallback(IAsyncResult ar)
        {
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            Send(handler);
            sendDone.WaitOne();

            allDone.Set();
        }


        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Receive(Socket client)
        {
            try
            {
                // Create the state object.  
                StateObject2 state = new StateObject2();
                state.workSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject2.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                StateObject2 state = (StateObject2)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, StateObject2.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.  
                    if (state.sb.Length > 1)
                    {
                        FileName = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.  
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        private static void SendFileInfo(Socket client)
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

        private static void Send(Socket client)
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


        //public static int Main(String[] args)
        //{
        //    AsynchronousClient.SharedFolderPath = "C:\\Users\\eviat\\Desktop";
        //    StartClient("127.0.0.1", "8001");
        //    StartListening("8002");
        //    return 0;
        //}
    }
}
