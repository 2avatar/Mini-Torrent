using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Peer2Peer
{
    /// <summary>
    /// Interaction logic for Torrent.xaml
    /// </summary>
    public partial class Torrent : Window
    {
        private Peer peer;
        private UserDataBinding User { get; set; }
        private Thread peerThread;
        private Thread listenerThread;

        public Torrent(UserDataBinding user)
        {
            InitializeComponent();
            this.Title = user.Username;
            Client.SharedFolderPath = user.FolderPath;
            Client.UI = this;
            SocketListener.SharedFolderPath = user.FolderPath;
            SocketListener.UI = this;

            listenerThread = new Thread(x => SocketListener.StartListening(User.PORT))
            {
                IsBackground = true
            };
            listenerThread.Start();

            listView.SelectionChanged += onSelectionChanged;

            this.User = user;
            startPeer();
        }

        public void onSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            progressBar.Value = 0;
            labelDownload.Content = "";
            labelUpload.Content = "";
        }

        private void startPeer()
        {
            peer = new Peer(User);
            peerThread = new Thread(x => peer.Run())
            {
                IsBackground = true
            };
            peerThread.Start();

            while (peer.Channel == null) ;

            peer.Channel.BroadcastAddSocket(User);
        }

        private void stopPeer()
        {

            if (peer.Channel != null)
                peer.Channel.BroadcastRemoveSocket(User);

            peer.Stop();
            peerThread.Join();

            SocketListener.Stop();
            listenerThread.Join();

        }

        private void askPeerToConnect(string peerUsername)
        {
            peer.Channel.BroadcastPeerToConnect(User.Username, peerUsername);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // sign out
            stopPeer();

            signOut(false);


        }

        private void signOut(bool isClosed)
        {
            MediationServer.WebService ws = new MediationServer.WebService();

            UserXML userXML = new UserXML(User.Username, User.Password);
            if (ws.SignOut(userXML.getXMLFormatToString()))
            {
                if (!isClosed)
                {
                    Configuration mainWindow = new Configuration();
                    mainWindow.Show();
                    this.Close();
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            requestFiles();
        }

        private void requestFiles()
        {
            // request file

            string fileName = requestFileTxtBox.Text;
            UserXML.File file = new UserXML.File(fileName);
            List<UserXML.File> listOfFiles = new List<UserXML.File>();

            listOfFiles.Add(file);
            UserXML userXML = new UserXML(User.Username, User.Password, listOfFiles);

            MediationServer.WebService ws = new MediationServer.WebService();

            string respond = ws.RequestFiles(userXML.getXMLFormatToString());
            if (respond != null)
            {
                UserXML listOfFilesXML = new UserXML(respond);

                listOfFiles = listOfFilesXML.getFilesListWithNumberOfActiveUsers();

                listView.ItemsSource = listOfFiles;
            }
            else
            {
                MessageBox.Show("File not exists");
                listView.ItemsSource = null;
                listView.Items.Clear();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // download
            if (listView.HasItems)
            {

                UserXML.File file = (UserXML.File)listView.SelectedItem;

                if (file != null)
                {
                    SocketListener.FileNameToRequest = file.FileName;

                    MediationServer.WebService ws = new MediationServer.WebService();

                    string targetPeerUsername = ws.GetNameByFilename(file.FileName);

                    peer.Channel.BroadcastPeerToConnect(User.Username, targetPeerUsername);
                }
            }
            else
            {
                MessageBox.Show("Please select an item");
            }
        }

        private double progress = 4;
        public void SetProgressLength(int len)
        {
            progress = 100 / len;
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;
            //progressBar.Step = 1;
        }

        public void ProgressChanged(int bytesRead, long timeInMilliseconds)
        {
            double timeInSeconds = timeInMilliseconds * 0.001;
            double kiloBits = bytesRead * 0.008;
            double transferRate = Math.Round(kiloBits / timeInSeconds, 2);
            labelDownload.Content = "Download: \n" +
                                    "Speed: " + transferRate + " Kbps\n" +
                            "Time: " + timeInSeconds + "s";
            progressBar.Value += progress;
        }

        public void UploadChanged(int bytesRead, long timeInMilliseconds)
        {
            double timeInSeconds = timeInMilliseconds * 0.001;
            double kiloBits = bytesRead * 0.008;
            double transferRate = Math.Round(kiloBits / timeInSeconds, 2);
            labelUpload.Content = "Upload: \n" +
                "Speed: " + transferRate + " Kbps";

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            stopPeer();
            signOut(true);
        }
    }
}
