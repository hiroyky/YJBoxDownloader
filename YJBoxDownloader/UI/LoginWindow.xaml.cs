using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;
using YJBoxDownloader;

namespace YJBoxDownloader.UI {
    /// <summary>
    /// Login.xaml の相互作用ロジック
    /// </summary>
    public partial class LoginWindow : Window {

        const string AuthenticationUri = "https://auth.login.yahoo.co.jp/yconnect/v1/authorization";

        public AccessTokenParam AccessTokenParam { get; private set; }

        Random random = new Random();

        public LoginWindow() {
            InitializeComponent();
        }

        private void loginWindow_Loaded(object sender, RoutedEventArgs e) {
            showAuthPage(AppId.Get());
        }

        private void authCodeTextBox_GotFocus(object sender, RoutedEventArgs e) {
            authCodeTextBox.SelectAll();
        }

        private void showAuthPage(string appId) {
            string uri = AuthenticationUri + "?response_type=code+id_token&redirect_uri=oob&display=touch&scope=openid%20profile&client_id=" + appId + "&nonce=" + getRandomString();
            //authWebBrowser.Source = new Uri(uri);
            System.Diagnostics.Process.Start(uri);
        }        

        private void okButton_Click(object sender, RoutedEventArgs e) {
            if (authCodeTextBox.Text == string.Empty) {
                MessageBox.Show("Get Pin code and enter.");
                return;
            }

            AccessTokenParam = YahooIdAuthentication.newAuthentication(authCodeTextBox.Text, AppId.Get());
            DialogResult = true;            
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }

        private string getRandomString() {
            long time = (long)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
            long randomedVal = random.Next(0, 9) * time;
            return randomedVal.ToString();
        }    
    }
}
