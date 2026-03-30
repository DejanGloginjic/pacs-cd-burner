using CDBurner.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CDBurner.Service.Common
{
    public interface IApiService
    {
        Task<List<Study>> GetStudiesAsync();

        Task<bool> DownloadStudyAsync(string studyId, string destinationFolder);
    }
}
