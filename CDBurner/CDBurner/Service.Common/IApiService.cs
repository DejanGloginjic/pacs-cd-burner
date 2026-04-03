using CDBurner.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CDBurner.Service.Common
{
    public interface IApiService
    {
        Task<List<StudyModel>> GetStudiesAsync(int currentPage, int pageSize, string keyword = "");
        Task<int> GetTotalStudiesCountAsync(string keyword = "");
        Task<bool> DownloadStudyAsync(string studyUrl, string destinationFolder);
    }
}
