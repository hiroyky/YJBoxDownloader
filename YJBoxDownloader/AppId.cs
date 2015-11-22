using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YJBoxDownloader {
    /// <summary>
    /// YJDNのアプリケーションIDを管理するクラスです．
    /// </summary>
    public class AppId {
        private static AppId instance = null;
        private const string appIdFile = @".\appid.txt";

        private AppId() {            
            using (StreamReader reader = new StreamReader(appIdFile)) {
                id = reader.ReadLine();
            }                
        }

        private string id;

        public static string Get() {
            if (instance == null) {
                instance = new AppId();
            }
            return instance.id;
        }

        public static byte[] GetBytes() {
            string appid = Get();
            return System.Text.Encoding.ASCII.GetBytes(appid);
        }
    }
}
