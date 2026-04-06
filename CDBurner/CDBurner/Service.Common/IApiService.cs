using CDBurner.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CDBurner.Service.Common
{
    public interface IApiService
    {
        Task<List<StudyModel>> GetStudiesAsync(int currentPage,
                                               int pageSize, 
                                               string keyword, 
                                               DateTime? dateFrom,
                                               DateTime? dateTo);
        Task<int> GetTotalStudiesCountAsync(string keyword,
                                            DateTime? dateFrom,
                                            DateTime? dateTo);
        Task<bool> DownloadStudyAsync(string studyUrl,
                                      string destinationFolder);
    }
}
