using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


namespace YJBoxDownloader {    
    public class YahooIdAuthentication {

        public const string TokenEndPointUri = "https://auth.login.yahoo.co.jp/yconnect/v1/token";

        public static AccessTokenParam newAuthentication(string authCode, string appId) {
            WebClient client = new WebClient();
            client.Credentials = CredentialCache.DefaultCredentials;
            NameValueCollection data = new NameValueCollection();
            data.Add("appid", appId);
            data.Add("grant_type", "authorization_code");
            data.Add("code", authCode);
            data.Add("client_id", appId);
            data.Add("redirect_uri", "oob");
            byte[] responseData = client.UploadValues(TokenEndPointUri, data);
            client.Dispose();
            return serializeResponseData(responseData);
        }

        public static AccessTokenParam updateAuthentication(AccessTokenParam p, string appId) {
            WebClient client = new WebClient();
            client.Credentials = CredentialCache.DefaultCredentials;
            NameValueCollection data = new NameValueCollection();
            data.Add("appid", appId);
            data.Add("grant_type", "refresh_token");
            data.Add("client_id", appId);
            data.Add("refresh_token", p.RefreshToken);
            try {
                byte[] responseData = client.UploadValues(TokenEndPointUri, data);
                client.Dispose();
                return serializeResponseData(responseData);
            } catch (WebException ex) {
                throw ex; 
            }
        }

        private static AccessTokenParam serializeResponseData(byte[] responseData) {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ResponseClass));
            var stream = new System.IO.MemoryStream(responseData);
            ResponseClass tokenData = (ResponseClass)serializer.ReadObject(stream);
            AccessTokenParam retval = new AccessTokenParam();
            retval.AccessToken = tokenData.access_token;
            retval.TokenType = tokenData.token_type;
            retval.ExpiresIn = tokenData.expires_in;
            retval.RefreshToken = tokenData.refresh_token;
            retval.IdToken = tokenData.id_token;
            return retval;
        }

        [DataContract]
        private class ResponseClass {
            [DataMember]
            public string access_token { get; set; }
            [DataMember]
            public string token_type { get; set; }
            [DataMember]
            public int expires_in { get; set; }
            [DataMember]
            public string refresh_token { get; set; }
            [DataMember]
            public string id_token { get; set; }
        }
    }
}
