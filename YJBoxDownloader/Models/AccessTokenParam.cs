using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YJBoxDownloader {
    public class AccessTokenParam {
        public string AccessToken;
        public string TokenType;
        public int ExpiresIn;
        public string RefreshToken;
        public string IdToken;

        public static AccessTokenParam LoadSavedAccessToken() {
            AccessTokenParam p = new AccessTokenParam();
            p.AccessToken = Properties.Settings.Default.AccessToken;
            p.TokenType = Properties.Settings.Default.TokenType;
            p.ExpiresIn = Properties.Settings.Default.ExpiresIn;
            p.RefreshToken = Properties.Settings.Default.RefreshToken;
            p.IdToken = Properties.Settings.Default.IdToken;
            return p;
        }

        public void Save() {
            Properties.Settings.Default.AccessToken = AccessToken;
            Properties.Settings.Default.TokenType = TokenType;
            Properties.Settings.Default.ExpiresIn = ExpiresIn;
            Properties.Settings.Default.RefreshToken = RefreshToken;
            Properties.Settings.Default.IdToken = IdToken;
            Properties.Settings.Default.Save();
        }

        public String AuthorizationHeader {
            get { return String.Format("{0} {1}", TokenType, AccessToken); }
        }
    }
}
