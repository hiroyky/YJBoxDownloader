using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;
using System.Net;
using YJBoxDownloader.Models;

namespace YJBoxDownloader {
    public class FileDownloader {

        private AccessTokenParam accessTokenParam;
        private static string DistributeUrl = "https://ybox.yahooapis.jp/v1/download/";

        public List<BoxContent> FileList { get; private set; }

        public event EventHandler<DownloadEventArgs> DownloadStandBy;
        public event EventHandler<DownloadEventArgs> Donloaded;
        public event EventHandler Aborted;

        public bool IsContinue { get; set; }

        public FileDownloader(AccessTokenParam param, List<BoxContent> fileList) {
            this.accessTokenParam = param;
            this.FileList = fileList;
        }

        public void DownLoadAll(String saveDir) {
            IsContinue = true;
            int count = 0;
            foreach (BoxContent file in FileList) {
                if (!IsContinue) {
                    if (Aborted!= null) {
                        Aborted(this, new EventArgs());
                    }
                    break;
                }

                if (file.Type == "dir") {
                    continue;
                }
                if (file.Status == "Completed") {
                    ++count;
                    continue;
                }

                string localPath = standByLocalPath(file, saveDir);
                string url = createDownloadUrl(file);

                file.Status = "Downloading";
                if (DownloadStandBy != null) {
                    DownloadStandBy(this, new DownloadEventArgs(file, count));
                }

                bool isSuccess = download(file, url, localPath);

                if (isSuccess) {
                    ++count;
                    file.Status = "Completed";
                } else {
                    file.Status = "Failed";
                }

                if (Donloaded != null) {
                    Donloaded(this, new DownloadEventArgs(file, count));
                }

                System.Threading.Thread.Sleep(500);
            }
        }

        protected string standByLocalPath(BoxContent file, string saveDir) {
            String localPath = saveDir + file.Path;
            String dir = Path.GetDirectoryName(localPath);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            return localPath;
        }

        protected string createDownloadUrl(BoxContent file) {
            return DistributeUrl + file.Sid + "/" + file.UniqId;
        }

        protected bool download(BoxContent file, string url, string localPath) {
            try {                
                WebClient client = createWebClient();
                client.DownloadFile(new Uri(url), localPath);
                client.Dispose();
                return true;         
            } catch (WebException ex) {
                if(((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized) {
                    accessTokenParam = YahooIdAuthentication.updateAuthentication(accessTokenParam, AppId.Get());
                }
                return false;
            }            
        }

        protected string parseRecievedData(string data) {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return (string)jss.Deserialize<dynamic>(data)["Object"]["Url"];
        }

        protected WebClient createWebClient() {
            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.Authorization] = accessTokenParam.AuthorizationHeader;
            client.Credentials = CredentialCache.DefaultCredentials;
            return client;
        }

    }
}
