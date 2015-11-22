using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.Web.Script.Serialization;

namespace YJBoxDownloader {
    public class YJBoxUserInfoFactory {

        public const string YJBoxUserInfoEndPointUri = "https://ybox.yahooapis.jp/v1/user/fullinfo/";

        public static YJBoxUserInfo Load(string uid, AccessTokenParam accessTokenParam) {
            WebClient client = new WebClient();
            //client.Headers[HttpRequestHeader.Authorization] = String.Format("{0} {1}", accessTokenParam.TokenType, accessTokenParam.AccessToken);
            client.Credentials = CredentialCache.DefaultCredentials;
            client.QueryString.Add("output", "json");
            client.QueryString.Add("access_token", accessTokenParam.AccessToken);
            try {
                string resData = client.DownloadString(YJBoxUserInfoEndPointUri + uid);
                return serializeYJBoxUserInfo(resData);
            } catch (WebException ex){
                System.Diagnostics.Debug.Write(ex.Message);
            }
            return null;
        }

        private static YJBoxUserInfo serializeYJBoxUserInfo(string data) {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var obj = jss.Deserialize<dynamic>(data);
            return new YJBoxUserInfo(obj["User"]);
        }

        private YJBoxUserInfoFactory() { }        
    }
}