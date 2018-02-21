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

        public Torrent(UserDataBinding user)
        {         
            InitializeComponent();

            AsynchronousClient.SharedFolderPath = user.FolderPath;
            AsynchronousSocketListener.DownloadFolderPath = user.FolderPath;

            this.User = user;
            startPeer();
        }

        private void startPeer()
        {



            peer = new Peer(User);
            peerThread = new Thread(peer.Run) { IsBackground = true };
            peerThread.Start();

            while (peer.Channel == null) ;

            peer.Channel.BroadcastAddSocket(User);
        }

        private void stopPeer()
        {
            peer.Channel.BroadcastRemoveSocket(User);
            peer.Stop();
            peerThread.Join();
        }     

        private void askPeerToConnect(string peerUsername)
        {
            peer.Channel.BroadcastPeerToConnect(User.Username, peerUsername);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // sign out
            stopPeer();

            MediationServer.WebService ws = new MediationServer.WebService();

            UserXML userXML = new UserXML(User.Username, User.Password);
            if (ws.SignOut(userXML.getXMLFormatToString()))
            {
                Configuration mainWindow = new Configuration();
                mainWindow.Show();
                this.Close();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
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
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // download
            if (listView.HasItems) {

                UserXML.File file = (UserXML.File)listView.SelectedItem;

                AsynchronousSocketListener.FileNameToRequest = file.FileName;

                MediationServer.WebService ws = new MediationServer.WebService();

                string targetPeerUsername = ws.GetNameByFilename(file.FileName);

                Thread t = new Thread(x => AsynchronousSocketListener.StartListening(User.PORT));
                t.IsBackground = true;
                t.Start();             
                peer.Channel.BroadcastPeerToConnect(User.Username, targetPeerUsername);
                AsynchronousSocketListener.StartClient(User.IP, User.PORT);

            }
            else
            {
                MessageBox.Show("Please select an item");
            }
        }
    }
}
