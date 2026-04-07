using System;
using System.Collections.Generic;
using System.Text;

namespace CDBurner.Service.Common
{
    public interface IBurnerService
    {
        Task<bool> BurnFolderAsync(string folderPath, IProgress<double> progress = null);
    }
}
