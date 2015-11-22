using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YJBoxDownloader;
using YJBoxDownloader.Models;
using System.IO;

namespace YJBoxDownloaderTest {
    [TestClass]
    public class FileTreeLoaderTest {

        class LoadDirectoryTestClass : FileTreeLoader {
            public int LoopCount { get; set; }
            public LoadDirectoryTestClass(YJBoxUserInfo info, AccessTokenParam param) : base(info, param) { }
            protected override bool shouldNextRequest(dynamic data) {
                return --LoopCount > 0;
            }
            protected override void sleep() {
                // None                
            }
            protected override dynamic load(string uniqId, int start) {
                return new Dictionary<string, object>() {
                    { "Object", new dynamic[] { } }
                };
            }
            protected override List<BoxContent> toBoxContentList(dynamic[] objectList) {
                return new List<BoxContent>() {
                    new BoxContent()
                };
            }
        }

        [TestMethod]
        public void LoadDirectoryのテスト() {
            LoadDirectoryTestClass loader;
            List<BoxContent> actual;

            loader = new LoadDirectoryTestClass(new YJBoxUserInfo(), new AccessTokenParam());
            loader.LoopCount = 1;
            actual = loader.LoadDirectory("");
            Assert.AreEqual(actual.Count, 1);

            loader = new LoadDirectoryTestClass(new YJBoxUserInfo(), new AccessTokenParam());
            loader.LoopCount = 10;
            actual = loader.LoadDirectory("");
            Assert.AreEqual(actual.Count, 10);
        }

        [TestMethod]
        public void toBoxContentListのテスト() {            
            PrivateObject loader;
            loader = new PrivateObject(typeof(FileTreeLoader), new object[] { new YJBoxUserInfo(), new AccessTokenParam() });

            StreamReader sr = new StreamReader("filetree.txt");
            String jsonStr = sr.ReadToEnd();
            var dict = (dynamic)loader.Invoke("parseRecievedData", new object[] { jsonStr });
            dynamic[] data = dict["Object"];

            var actual = (List<BoxContent>)loader.Invoke("toBoxContentList", new object[] { data });
        }

        [TestMethod]
        public void parseRecievedDataのテスト() {
            PrivateObject loader;
            StreamReader sr = new StreamReader("filetree.txt");
            String jsonStr = sr.ReadToEnd();

            loader = new PrivateObject(typeof(FileTreeLoader), new object[] { new YJBoxUserInfo(), new AccessTokenParam() });
            var dict = (dynamic)loader.Invoke("parseRecievedData", new object[] { jsonStr });
            Assert.AreEqual(1, dict["FirstResultPosition"]);
            Assert.AreEqual(10, dict["TotalResultsAvailable"]);
            Assert.AreEqual(10, dict["TotalResultsReturned"]);
            var files = dict["Object"];
            Assert.AreEqual(2, files.Length);

            jsonStr = "{\"ObjectList\":{\"TotalResultsAvailable\":13,\"TotalResultsReturned\":12,\"FirstResultPosition\":2,\"Object\":[{\"Type\":\"file\",\"ContentType\":\"text\\/plain\",\"Name\":\"\\u306f\\u3058\\u3081\\u3066\\u3054\\u5229\\u7528\\u306e\\u65b9\\u3078.txt\",\"Sid\":\"box-l-243hpsdyztigrmy66irur37jfu-1001\",\"UniqId\":\"78f422b2-b8e4-420a-86e2-2840288bab6e\",\"Path\":\"\\/\\u306f\\u3058\\u3081\\u3066\\u3054\\u5229\\u7528\\u306e\\u65b9\\u3078.txt\",\"Url\":\"https:\\/\\/fs.yahoobox.jp\\/res\\/box-l-243hpsdyztigrmy66irur37jfu-1001?account=storage&expires=1449403837&uid=78f422b2-b8e4-420a-86e2-2840288bab6e&signature=OTQ5MWVkNTgxMmVmNDgxYzIwOGQ4ZWNjY2U5MDAyNGYzODFjYzExOA--\",\"PublicUrl\":\"https:\\/\\/box.yahoo.co.jp\\/guest\\/viewer?sid=box-l-243hpsdyztigrmy66irur37jfu-1001&uniqid=78f422b2-b8e4-420a-86e2-2840288bab6e&viewtype=detail\",\"Size\":3208,\"ModifiedTime\":\"2011-12-16T03:24:44+09:00\",\"Md5\":\"ba346ec8a1069e41de086ecdaa69087d\",\"Etag\":\"\\\"456ba99413239734843208\\\"\",\"Public\":false,\"PublicInherited\":false},{\"Type\":\"dir\",\"Name\":\"\\u81ea\\u52d5\\u30a2\\u30c3\\u30d7\\u30ed\\u30fc\\u30c9\\uff08Nexus 7\\uff09\",\"Sid\":\"box-l-243hpsdyztigrmy66irur37jfu-1001\",\"UniqId\":\"cba29fd7-04ed-4eb1-8dc4-fe8c7ddacd5a\",\"Path\":\"\\/\\u81ea\\u52d5\\u30a2\\u30c3\\u30d7\\u30ed\\u30fc\\u30c9\\uff08Nexus 7\\uff09\",\"PublicUrl\":\"https:\\/\\/box.yahoo.co.jp\\/guest\\/viewer?sid=box-l-243hpsdyztigrmy66irur37jfu-1001&uniqid=cba29fd7-04ed-4eb1-8dc4-fe8c7ddacd5a\",\"ModifiedTime\":\"2014-04-15T21:25:25+09:00\",\"Public\":false,\"PublicInherited\":false},{\"Type\":\"dir\",\"Name\":\"\\u81ea\\u52d5\\u30a2\\u30c3\\u30d7\\u30ed\\u30fc\\u30c9\\uff08iPad\\uff09\",\"Sid\":\"box-l-243hpsdyztigrmy66irur37jfu-1001\",\"UniqId\":\"7f6b3fc2-5aee-4bc5-b191-211b71986350\",\"Path\":\"\\/\\u81ea\\u52d5\\u30a2\\u30c3\\u30d7\\u30ed\\u30fc\\u30c9\\uff08iPad\\uff09\",\"PublicUrl\":\"https:\\/\\/box.yahoo.co.jp\\/guest\\/viewer?sid=box-l-243hpsdyztigrmy66irur37jfu-1001&uniqid=7f6b3fc2-5aee-4bc5-b191-211b71986350\",\"ModifiedTime\":\"2014-07-07T09:33:49+09:00\",\"Public\":false,\"PublicInherited\":false},{\"Type\":\"dir\",\"Name\":\"SO-04D\",\"Sid\":\"box-l-243hpsdyztigrmy66irur37jfu-1001\",\"UniqId\":\"16dad6d3-f8cf-4975-ae71-64ef097a6f35\",\"Path\":\"\\/SO-04D\",\"PublicUrl\":\"https:\\/\\/box.yahoo.co.jp\\/guest\\/viewer?sid=box-l-243hpsdyztigrmy66irur37jfu-1001&uniqid=16dad6d3-f8cf-4975-ae71-64ef097a6f35\",\"ModifiedTime\":\"2014-10-13T20:43:51+09:00\",\"Public\":false,\"PublicInherited\":false},{\"Type\":\"dir\",\"Name\":\"\\u81ea\\u52d5\\u30a2\\u30c3\\u30d7\\u30ed\\u30fc\\u30c9\\uff08SO-04D\\uff09 (2)\",\"Sid\":\"box-l-243hpsdyztigrmy66irur37jfu-1001\",\"UniqId\":\"80d0a67a-7919-4ce5-aff0-5c43f021fdde\",\"Path\":\"\\/\\u81ea\\u52d5\\u30a2\\u30c3\\u30d7\\u30ed\\u30fc\\u30c9\\uff08SO-04D\\uff09 (2)\",\"PublicUrl\":\"https:\\/\\/box.yahoo.co.jp\\/guest\\/viewer?sid=box-l-243hpsdyztigrmy66irur37jfu-1001&uniqid=80d0a67a-7919-4ce5-aff0-5c43f021fdde\",\"ModifiedTime\":\"2014-10-13T20:43:56+09:00\",\"Public\":false,\"PublicInherited\":false},{\"Type\":\"dir\",\"Name\":\"\\u5199\\u771f\",\"Sid\":\"box-l-243hpsdyztigrmy66irur37jfu-1001\",\"UniqId\":\"9965db88-4ee7-485b-8792-64331008f362\",\"Path\":\"\\/\\u5199\\u771f\",\"PublicUrl\":\"https:\\/\\/box.yahoo.co.jp\\/guest\\/viewer?sid=box-l-243hpsdyztigrmy66irur37jfu-1001&uniqid=9965db88-4ee7-485b-8792-64331008f362\",\"ModifiedTime\":\"2015-04-06T22:53:37+09:00\",\"Public\":false,\"PublicInherited\":false},{\"Type\":\"dir\",\"Name\":\"\\u81ea\\u52d5\\u30d0\\u30c3\\u30af\\u30a2\\u30c3\\u30d7\\uff08SO-03F\\uff09 (2)\",\"Sid\":\"box-l-243hpsdyztigrmy66irur37jfu-1001\",\"UniqId\":\"c1279a4e-5b2c-49cb-ad5b-a1224097e5c9\",\"Path\":\"\\/\\u81ea\\u52d5\\u30d0\\u30c3\\u30af\\u30a2\\u30c3\\u30d7\\uff08SO-03F\\uff09 (2)\",\"PublicUrl\":\"https:\\/\\/box.yahoo.co.jp\\/guest\\/viewer?sid=box-l-243hpsdyztigrmy66irur37jfu-1001&uniqid=c1279a4e-5b2c-49cb-ad5b-a1224097e5c9\",\"ModifiedTime\":\"2015-05-18T21:08:16+09:00\",\"Public\":false,\"PublicInherited\":false},{\"Type\":\"dir\",\"Name\":\"\\u5171\\u6709\",\"Sid\":\"box-l-243hpsdyztigrmy66irur37jfu-1001\",\"UniqId\":\"621e7073-2857-4905-9001-c2ee20d1bab8\",\"Path\":\"\\/\\u5171\\u6709\",\"PublicUrl\":\"https:\\/\\/box.yahoo.co.jp\\/guest\\/viewer?sid=box-l-243hpsdyztigrmy66irur37jfu-1001&uniqid=621e7073-2857-4905-9001-c2ee20d1bab8\",\"ModifiedTime\":\"2015-06-13T14:06:30+09:00\",\"Public\":false,\"PublicInherited\":false},{\"Type\":\"dir\",\"Name\":\"\\u66f8\\u985e\",\"Sid\":\"box-l-243hpsdyztigrmy66irur37jfu-1001\",\"UniqId\":\"5fa46bd1-cf11-4f98-8480-5109a5655983\",\"Path\":\"\\/\\u66f8\\u985e\",\"PublicUrl\":\"https:\\/\\/box.yahoo.co.jp\\/guest\\/viewer?sid=box-l-243hpsdyztigrmy66irur37jfu-1001&uniqid=5fa46bd1-cf11-4f98-8480-5109a5655983\",\"ModifiedTime\":\"2015-06-14T05:24:50+09:00\",\"Public\":false,\"PublicInherited\":false},{\"Type\":\"dir\",\"Name\":\"\\u81ea\\u52d5\\u30d0\\u30c3\\u30af\\u30a2\\u30c3\\u30d7\\uff08iPhone\\uff09\",\"Sid\":\"box-l-243hpsdyztigrmy66irur37jfu-1001\",\"UniqId\":\"d4f3d1c4-49d8-40e8-827f-36bef4030f1e\",\"Path\":\"\\/\\u81ea\\u52d5\\u30d0\\u30c3\\u30af\\u30a2\\u30c3\\u30d7\\uff08iPhone\\uff09\",\"PublicUrl\":\"https:\\/\\/box.yahoo.co.jp\\/guest\\/viewer?sid=box-l-243hpsdyztigrmy66irur37jfu-1001&uniqid=d4f3d1c4-49d8-40e8-827f-36bef4030f1e\",\"ModifiedTime\":\"2015-09-15T23:56:05+09:00\",\"Public\":false,\"PublicInherited\":false},{\"Type\":\"dir\",\"Name\":\"\\u81ea\\u52d5\\u30d0\\u30c3\\u30af\\u30a2\\u30c3\\u30d7\\uff08SO-03F\\uff09\",\"Sid\":\"box-l-243hpsdyztigrmy66irur37jfu-1001\",\"UniqId\":\"6fb59cd9-ca3f-4bb6-8c43-cb305a47482e\",\"Path\":\"\\/\\u81ea\\u52d5\\u30d0\\u30c3\\u30af\\u30a2\\u30c3\\u30d7\\uff08SO-03F\\uff09\",\"PublicUrl\":\"https:\\/\\/box.yahoo.co.jp\\/guest\\/viewer?sid=box-l-243hpsdyztigrmy66irur37jfu-1001&uniqid=6fb59cd9-ca3f-4bb6-8c43-cb305a47482e\",\"ModifiedTime\":\"2015-11-07T22:02:44+09:00\",\"Public\":false,\"PublicInherited\":false},{\"Type\":\"dir\",\"Name\":\"\\u81ea\\u52d5\\u30d0\\u30c3\\u30af\\u30a2\\u30c3\\u30d7\\uff08iPad\\uff09\",\"Sid\":\"box-l-243hpsdyztigrmy66irur37jfu-1001\",\"UniqId\":\"292eff1a-a5c6-43ca-8a3e-35a3f36b1c65\",\"Path\":\"\\/\\u81ea\\u52d5\\u30d0\\u30c3\\u30af\\u30a2\\u30c3\\u30d7\\uff08iPad\\uff09\",\"PublicUrl\":\"https:\\/\\/box.yahoo.co.jp\\/guest\\/viewer?sid=box-l-243hpsdyztigrmy66irur37jfu-1001&uniqid=292eff1a-a5c6-43ca-8a3e-35a3f36b1c65\",\"ModifiedTime\":\"2015-11-07T22:05:00+09:00\",\"Public\":false,\"PublicInherited\":false}]}}";
            loader = new PrivateObject(typeof(FileTreeLoader), new object[] { new YJBoxUserInfo(), new AccessTokenParam() });
            dict = (dynamic)loader.Invoke("parseRecievedData", new object[] { jsonStr });
        }

        [TestMethod]
        public void shouldNextRequestのテスト() {
            Dictionary<String, Object> dict;
            PrivateObject loader = new PrivateObject(typeof(FileTreeLoader), new object[] { new YJBoxUserInfo(), new AccessTokenParam() });
            bool actual;

            dict = new Dictionary<string, object>() {
                    {"FirstResultPosition", 1 }, {"TotalResultsAvailable", 10 }, {"TotalResultsReturned", 10 }
                };
            actual = (bool)loader.Invoke("shouldNextRequest", new object[] { dict });
            Assert.AreEqual(false, actual);

            dict = new Dictionary<string, object>() {
                    {"FirstResultPosition", 1 }, {"TotalResultsAvailable", 10 }, {"TotalResultsReturned", 5 }
                };
            actual = (bool)loader.Invoke("shouldNextRequest", new object[] { dict });
            Assert.AreEqual(true, actual);
        }
    }
}
