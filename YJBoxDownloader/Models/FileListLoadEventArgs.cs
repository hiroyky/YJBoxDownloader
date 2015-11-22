using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YJBoxDownloader.Models {
    public class FileListLoadEventArgs : EventArgs {
        public int FileNum { get; set; }

        public FileListLoadEventArgs(int fileNum) : base() {
            this.FileNum = fileNum;
        }
    }
}
