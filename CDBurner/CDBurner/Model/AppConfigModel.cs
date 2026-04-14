using System;
using System.Collections.Generic;
using System.Text;

namespace CDBurner.Model
{
    public class AppConfigModel
    {
        public string ApiBaseUrl { get; set; }
        public int PageSize { get; set; }
        public long CdLimit { get; set; }
        public long DvdLimit { get; set; }
    }
}
