using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YJBoxDownloader {
    public class YJBoxUserInfo {
        private string Guid { get; set; }
        public string Sid { get; set; }
        public bool ServiceStatus { get; set; }
        public bool ServiceSuspend { get; set; }
        public string RootUniqId { get; set; }
        public Quota Quota { get; set; }

        public YJBoxUserInfo() { }
        public YJBoxUserInfo(dynamic dict) {
            Guid = (String)dict["Guid"];
            Sid = (String)dict["Sid"];
            ServiceStatus = dict["ServiceStatus"];
            ServiceSuspend = dict["ServiceSuspend"];
            RootUniqId = (String)dict["RootUniqId"];
            Quota = new Quota();
            Quota.Max = dict["Quota"]["Max"];
            Quota.TotalUsed = dict["Quota"]["TotalUsed"];
        }
    }

    public class Quota {
        public long Max { get; set; }
        public long TotalUsed { get; set; }
    }
}
