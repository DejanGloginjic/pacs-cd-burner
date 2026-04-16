using System;
using System.Collections.Generic;
using System.Text;

namespace CDBurner.Model
{
    public class AppConfigModel
    {
        public string LocalDicomUrl { get; set; }
        public string ProductionDicomUrl { get; set; }
        public int PageSize { get; set; }
        public long CdLimit { get; set; }
        public long DvdLimit { get; set; }
        public string WeasisFolderName { get; set; }
        public string LauncherFolderName { get; set; }
        public string ClientName { get; set; }
    }
}
