using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YJBoxDownloader.Models {
    public class DownloadEventArgs : EventArgs {
        public BoxContent BoxContent { get; set; }
        public int Count { get; set; }

        public DownloadEventArgs() : base() { }
        public DownloadEventArgs(BoxContent file, int count) : base() {
            this.BoxContent = file;
            this.Count = count;
        }
    }
}
