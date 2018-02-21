using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Peer2Peer
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    public partial class Configuration : Window
    {
        private static string EMPTY_FIELDS = "Please fill the empty fields";
        private static string WRONG_SIGNIN = "Your information is invalid, please try again";


        List<UserXML.File> filesList = new List<UserXML.File>();
        UserDataBinding user = new UserDataBinding();
        FolderBrowserDialog folderDlg = new FolderBrowserDialog();

        public Configuration()
        {
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Count() < 1)
            {    
                    Process.Start("Peer2Peer.exe");  //Change your exe
            }

            user.Username = "eviatar";

            user.IP = "127.0.0.1";
            user.PORT = "8001";

            InitializeComponent();
            DataContext = user;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {


            folderDlg.ShowNewFolderButton = true;

            DialogResult result = folderDlg.ShowDialog();

            LabelPath.Content = "Path: " + folderDlg.SelectedPath;
            user.FolderPath = folderDlg.SelectedPath;

            Environment.SpecialFolder root = folderDlg.RootFolder;

            DirectoryInfo d = new DirectoryInfo(folderDlg.SelectedPath);
            try
            {
                FileInfo[] Files = d.GetFiles();
                foreach (var f in Files)
                {
                    filesList.Add(new UserXML.File(f.Name, f.Length));
                }
            }
            catch { }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            user.Password = txtPassword.Password;

            if (user.checkUserValidation() && !string.IsNullOrEmpty(folderDlg.SelectedPath))
            {
                UserXML userXML = new UserXML(user.Username, user.Password, user.IP, user.PORT, filesList);

                MediationServer.WebService ws = new MediationServer.WebService();

                if (ws.SignIn(userXML.getXMLFormatToString()))
                {
                    Torrent torrentWindow = new Torrent(user);
                    torrentWindow.Show();
                    this.Close();
                }
                else
                {
                    System.Windows.MessageBox.Show(WRONG_SIGNIN);
                }
            }
            else
            {
                System.Windows.MessageBox.Show(EMPTY_FIELDS);
            }
        }
    }
}
