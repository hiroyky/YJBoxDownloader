using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YJBoxDownloader.Models;
using System.Collections.Generic;

namespace YJBoxDownloaderTest {
    [TestClass]
    public class BoxContentTest {
        [TestMethod]
        public void コンストラクタのテスト() {
            var dict = new Dictionary<String, Object>() {
                {"Type", "file"}, {"ContentType", "plain/text"}, {"Name", "ファイル名"}
            };

            BoxContent content = new BoxContent(dict);
            Assert.AreEqual("file", content.Type);
            Assert.AreEqual("plain/text", content.ContentType);
            Assert.AreEqual("ファイル名", content.Name);
        }
    }
}
