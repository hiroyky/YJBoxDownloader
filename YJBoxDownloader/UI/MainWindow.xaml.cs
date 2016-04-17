using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;
using YJBoxDownloader.Models;

namespace YJBoxDownloader.UI {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {
        private AccessTokenParam accessToken = null;
        private UserInfo userInfo = null;
        private YJBoxUserInfo yjBoxUserInfo = null;
        private FileTreeLoader loader = null;
        private FileDownloader downloader = null;

        private List<BoxContent> boxContents = new List<BoxContent>();

        public MainWindow() {
            InitializeComponent();            
        }

        protected string statusBarText {
            get { return (string)statusBarItem.Content; }
            set { statusBarItem.Content = value; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            initializeAccessToken();
            userInfo = UserInfoFactory.Load(accessToken, AppId.Get());
            yjBoxUserInfo = YJBoxUserInfoFactory.Load(userInfo.UserId, accessToken);
            usernameLabel.Content = userInfo.Name;
            fileListLoadButton.IsEnabled = true;
        }

        private void initializeAccessToken() {
            accessToken = null;
            if (isTokenSaved()) {
                try {
                    accessToken = YahooIdAuthentication.updateAuthentication(AccessTokenParam.LoadSavedAccessToken(), AppId.Get());
                    accessToken.Save();
                } catch (WebException) { }
            }

            if (accessToken == null) {
                LoginWindow w = new LoginWindow();
                w.Owner = this;
                if (!(bool)w.ShowDialog()) {
                    return;
                }
                accessToken = w.AccessTokenParam;
                accessToken.Save();
            }
        }

        private bool isTokenSaved() {
            if (Properties.Settings.Default.AccessToken == string.Empty) {
                return false;
            }
            if (Properties.Settings.Default.TokenType == string.Empty) {
                return false;
            }
            if (Properties.Settings.Default.ExpiresIn == 0) {
                return false;
            }
            if (Properties.Settings.Default.RefreshToken == string.Empty) {
                return false;
            }            
            return true;
        }

        private void fileListLoadButton_Click(object sender, RoutedEventArgs e) {
            loader = new FileTreeLoader(yjBoxUserInfo, accessToken);
            loader.DirectoryLoaded += delegate (Object s, FileListLoadEventArgs arg) {
                Dispatcher.BeginInvoke(new Action(delegate {
                    numOfAllFilesLabel.Content = string.Format("{0} Files", arg.FileNum);
                }));
            };
            statusBarText = "Loading file list....";       
            var task = Task.Factory.StartNew(delegate {
                boxContents = loader.LoadRecursive();
            });
            task.ContinueWith(delegate {
                Dispatcher.BeginInvoke(new Action(delegate {
                    fileList_Loaded();
                }));                
            });
        }        

        private void fileList_Loaded() {
            statusBarText = "";
            numOfAllFilesLabel.Content = string.Format("{0} Files", boxContents.Count);
            downloadButton.IsEnabled = true;
            filelistView.IsEnabled = true;
            filelistView.ItemsSource = boxContents;
        }

        private void downloadButton_Click(object sender, RoutedEventArgs e) {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.EnsureReadOnly = false;
            dialog.AllowNonFileSystemItems = false;
            dialog.DefaultDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (dialog.ShowDialog() != CommonFileDialogResult.Ok) {
                return;
            }

            abortButton.IsEnabled = true;
            downloadButton.IsEnabled = false;

            initializeDownloader();
            var task = Task.Factory.StartNew(delegate {
                downloader.DownLoadAll(dialog.FileName);                
            });
            task.ContinueWith(delegate {
                MessageBox.Show("Download Completed.");
            });          
        }

        protected void initializeDownloader() {
            downloader = new FileDownloader(accessToken, boxContents);
            downloader.DownloadStandBy += Downloader_DownloadStandBy;
            downloader.Donloaded += Downloader_Donloaded;
            downloader.Aborted += Downloader_Aborted;
        }

        private void Downloader_Aborted(object sender, EventArgs e) {
            Dispatcher.BeginInvoke(new Action(delegate {
                MessageBox.Show("Aborted");
            }));
        }

        private void Downloader_DownloadStandBy(object sender, DownloadEventArgs e) {
            Dispatcher.BeginInvoke(new Action(delegate {
                filelistView.Items.Refresh();
            }));
        }

        private void Downloader_Donloaded(object sender, DownloadEventArgs e) {
            Dispatcher.BeginInvoke(new Action(delegate {
                numOfdonloadedFiles.Content = string.Format("{0} Files OK", e.Count);
                filelistView.Items.Refresh();
            }));
        }

        private void fileListOpenButton_Click(object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "XML File(*.xml)|*.xml|All File(*.*)|*.*";
            dialog.Title = "Select Open File";
            if (!(bool)dialog.ShowDialog()) {
                return;
            }
            boxContents = openFileList(dialog.FileName);
            fileList_Loaded();
        }

        private void fileListSaveButton_Click(object sender, RoutedEventArgs e) {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Filter = "XML File(*.xml) | *.xml | All File(*.*) | *.*";
            dialog.Title = "Select Save File";
            if (!(bool)dialog.ShowDialog()) {
                return;
            }
            saveFileList(dialog.FileName, boxContents);
        }

        private List<BoxContent> openFileList(string path) {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<BoxContent>));
            System.IO.StreamReader sr = new System.IO.StreamReader(path);
            var retval = serializer.Deserialize(sr);
            sr.Close();
            return (List<BoxContent>)retval;
        }

        protected void saveFileList(string path, List<BoxContent> fileList) {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<BoxContent>));
            System.IO.StreamWriter sw = new System.IO.StreamWriter(path);
            serializer.Serialize(sw, fileList);
            sw.Close();
        }

        private void abortButton_Click(object sender, RoutedEventArgs e) {
            if (downloader == null) {
                return;
            }
            downloader.IsContinue = false;
            downloadButton.IsEnabled = true;
        }
    }
}
