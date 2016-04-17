using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YJBoxDownloader.Models {
    public class BoxContent {
        public string Type { get; set; }
        public string ContentType { get; set; }
        public string Name { get; set; }
        public string Sid { get; set; }
        public string UniqId { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }
        public string PublicUrl { get; set; }
        public long Size { get; set; }
        //public DateTime ModifiedTime { get; set; }
        public string Md5 { get; set; }
        public string Status { get; set; }

        public BoxContent() { }

        public BoxContent(dynamic src) {
            try {
                foreach (var property in GetType().GetProperties()) {
                    if (!src.ContainsKey(property.Name)) {
                        continue;
                    }
                    property.SetValue(this, src[property.Name], null);
                }
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}
