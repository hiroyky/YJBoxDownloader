using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


namespace YJBoxDownloader {
    public class UserInfoFactory {

        public const string UserInfoEndPointUri = "https://userinfo.yahooapis.jp/yconnect/v1/attribute";

        public static UserInfo Load(AccessTokenParam accessTokenParam, string appId) {
            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.Authorization] = String.Format("{0} {1}", accessTokenParam.TokenType, accessTokenParam.AccessToken);
            client.Headers[HttpRequestHeader.UserAgent] = "";
            NameValueCollection data = new NameValueCollection();
            data.Add("schema", "openid");
            client.Credentials = CredentialCache.DefaultCredentials;
            client.QueryString = data;
            try {
                byte[] resData = client.DownloadData(UserInfoEndPointUri);
                return serializeUserInfo(resData);
            } catch (WebException ex) {
                System.Diagnostics.Debug.Write(ex.ToString());
            }
            return null;
        }

        private static UserInfo serializeUserInfo(byte[] data) {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ResponseClass));
            var stream = new System.IO.MemoryStream(data);
            ResponseClass tokenData = (ResponseClass)serializer.ReadObject(stream);
            UserInfo retval = new UserInfo();
            retval.UserId = tokenData.user_id;
            retval.Name = tokenData.name;
            retval.GivenName = tokenData.given_name;
            retval.FamilyName = tokenData.family_name;
            retval.Gender = tokenData.gender;
            return retval;
        }

        [DataContract]
        private class ResponseClass {
            [DataMember]
            public string user_id { get; set; }
            [DataMember]
            public string name { get; set; }
            [DataMember]
            public string given_name { get; set; }
            [DataMember]
            public string family_name { get; set; }
            [DataMember]
            public string gender { get; set; }
        }
    }
}
