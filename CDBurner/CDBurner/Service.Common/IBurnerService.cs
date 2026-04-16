using System;
using System.Collections.Generic;
using System.Text;

namespace CDBurner.Service.Common
{
    public interface IBurnerService
    {
        Task<bool> BurnFolderAsync(string dicomPath, long dicomFolderSize, IProgress<double> progress = null);
    }
}
