using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using YJBoxDownloader.Models;

namespace YJBoxDownloader {
    public class FileTreeLoader {
        public readonly string FileListEndPointUri = "https://ybox.yahooapis.jp/v1/filelist/{0}/";

        private string sid;
        private string rootUniqId;
        private int acquisitionUnit;
        private AccessTokenParam accessTokenParam;
        private int delayMilliSec;

        public event EventHandler<FileListLoadEventArgs> DirectoryLoaded;

        public int AcquisitionUnit {
            get { return acquisitionUnit; }
            set {
                if (value <= 0) {
                    throw new ArgumentException("AcquisitionUnit must be a positive number without 0.");
                }
                acquisitionUnit = value;
            }
        }

        public int DelayMilliSec {
            get { return delayMilliSec; }
            set {
                if (value < 0) {
                    throw new ArgumentException("DelayMilliSec must be a positive number without 0.");
                }
                delayMilliSec = value;
            }
        }

        public FileTreeLoader(YJBoxUserInfo boxInfo, AccessTokenParam param) {
            sid = boxInfo.Sid;
            rootUniqId = boxInfo.RootUniqId;
            FileListEndPointUri = String.Format(FileListEndPointUri, sid);
            accessTokenParam = param;
            AcquisitionUnit = 100;
            DelayMilliSec = 10;
        }

        public List<BoxContent> LoadRecursive() {
            List<BoxContent> root = LoadDirectory(rootUniqId);
            List<BoxContent> fileList = root.Where(o => o.Type == "file").ToList();
            List<BoxContent> directoryList = root.Where(o => o.Type == "dir").ToList();

            directoryList.ForEach(delegate (BoxContent dir) {
                List<BoxContent> childList;
                try {
                    childList = LoadDirectory(dir.UniqId);                    
                    fileList.AddRange(childList.Where(o => o.Type == "file").ToList());
                    directoryList.AddRange(childList.Where(o => o.Type == "dir").ToList());
                    if (DirectoryLoaded != null) {
                        DirectoryLoaded(this, new FileListLoadEventArgs(fileList.Count));
                    }
                } catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            });
            return fileList;
        }

        public List<BoxContent> LoadDirectory(string uniqId) {
            List<BoxContent> contents = new List<BoxContent>();
            int iterator = 1;
            dynamic results;
            do {
                sleep();
                results = load(uniqId, iterator);
                if (results == null) {
                    break;
                }
                contents.AddRange(toBoxContentList(results["Object"]));
                ++iterator;
            } while (shouldNextRequest(results));
            return contents;
        }

        protected virtual void sleep() {
            Thread.Sleep(DelayMilliSec);
        }

        protected virtual List<BoxContent> toBoxContentList(dynamic[] objectList) {
            return objectList.Select(o => new BoxContent(o)).ToList();
        }

        protected virtual dynamic load(string uniqId, int start) {
            try {
                WebClient client = createWebClient(uniqId, start);
                string uri = FileListEndPointUri + uniqId;
                string res = client.DownloadString(uri);
                client.Dispose();
                return parseRecievedData(res);
            } catch (WebException ex) {
                // 空のディレクトリを参照した場合, 403エラーになるようであるため，403エラーは無視する．
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                if (res.StatusCode != HttpStatusCode.Forbidden) {
                    throw ex;
                }
            }
            return null;
        }

        protected virtual dynamic parseRecievedData(string res) {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Deserialize<dynamic>(res)["ObjectList"];
        }

        protected virtual bool shouldNextRequest(dynamic data) {
            int total = data["TotalResultsAvailable"];
            int unit = data["TotalResultsReturned"];
            int currentIndex = data["FirstResultPosition"];
            return currentIndex * unit < total;
        }

        WebClient createWebClient(string uniqId, int start) {
            WebClient client = new WebClient();
            client.Headers[HttpRequestHeader.Authorization] = accessTokenParam.AuthorizationHeader;
            client.Credentials = CredentialCache.DefaultCredentials;
            client.QueryString.Add("output", "json");
            client.QueryString.Add("results", AcquisitionUnit.ToString());
            client.QueryString.Add("start", start.ToString());
            return client;
        }
    }
}
