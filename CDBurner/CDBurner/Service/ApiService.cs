using CDBurner.Model;
using CDBurner.Service.Common;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json;

namespace CDBurner.Service
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiService()
        {
            _httpClient = new HttpClient();
            
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            var json = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<AppConfigModel>(json);
            _baseUrl = config.ProductionDicomUrl;
            _httpClient = new HttpClient { BaseAddress = new Uri(_baseUrl) };
        }

        public async Task<List<StudyModel>> GetStudiesAsync(int currentPage,
                                                            int pageSize,
                                                            string keyword,
                                                            DateTime? dateFrom,
                                                            DateTime? dateTo)
        {
            try
            {
                string urlParams = BuildQueryParams(keyword, dateFrom, dateTo);

                if (string.IsNullOrWhiteSpace(urlParams))
                {
                    urlParams += $"?offset={(currentPage - 1) * pageSize}&limit={pageSize}&includefield=00081030";
                }
                else
                {
                    urlParams += $"&offset={(currentPage - 1) * pageSize}&limit={pageSize}&includefield=00081030";
                }

                var response = await _httpClient.GetAsync(urlParams);
                if (!response.IsSuccessStatusCode) return new List<StudyModel>();

                var json = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(json)) return new List<StudyModel>();

                var raw = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(json);

                IObjectMapper mapper = new ObjectMapper();
                return raw?.Select(mapper.MapStudy).ToList() ?? new List<StudyModel>();
            }
            catch
            {
                return new List<StudyModel>();
            }
        }

        public async Task<int> GetTotalStudiesCountAsync(string keyword,
                                                         DateTime? dateFrom,
                                                         DateTime? dateTo)
        {
            try
            {
                var url = _baseUrl + "/count" + BuildQueryParams(keyword, dateFrom, dateTo);

                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Accept", "application/json");

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) return 0;

                var json = await response.Content.ReadAsStringAsync();
                var obj = JsonSerializer.Deserialize<Dictionary<string, int>>(json);

                return obj != null && obj.TryGetValue("count", out int count) ? count : 0;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<bool> DownloadStudyAsync(string studyUrl, string destinationFolder, CancellationToken token)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    studyUrl,
                    HttpCompletionOption.ResponseHeadersRead,
                    token
                );

                response.EnsureSuccessStatusCode();

                var contentType = response.Content.Headers.ContentType;
                if (!contentType.MediaType.Equals("multipart/related", StringComparison.OrdinalIgnoreCase))
                    throw new Exception("Unexpected content type: " + contentType.MediaType);

                var boundary = contentType.Parameters.First(p => p.Name == "boundary").Value.Trim('"');

                if (Directory.Exists(destinationFolder))
                {
                    Directory.Delete(destinationFolder, true);
                }

                Directory.CreateDirectory(destinationFolder);

                using var stream = await response.Content.ReadAsStreamAsync();
                var reader = new Microsoft.AspNetCore.WebUtilities.MultipartReader(boundary, stream);

                MultipartSection section;
                int index = 0;

                while ((section = await reader.ReadNextSectionAsync()) != null)
                {
                    token.ThrowIfCancellationRequested();
                    string filename = null;

                    if (section.Headers.TryGetValue("Content-Disposition", out var values))
                    {
                        var contentDisposition = values.FirstOrDefault();
                        if (!string.IsNullOrEmpty(contentDisposition))
                        {
                            var match = System.Text.RegularExpressions.Regex.Match(
                                contentDisposition,
                                @"filename\*?=(?:UTF-8'')?""?(?<fname>[^\"";]+)""?");
                            if (match.Success)
                                filename = match.Groups["fname"].Value;
                        }
                    }

                    if (string.IsNullOrEmpty(filename))
                        filename = $"img_{index}.dcm";

                    var filePath = Path.Combine(destinationFolder, filename);

                    using (var fileStream = File.Create(filePath))
                    {
                        await section.Body.CopyToAsync(fileStream, token);
                        await fileStream.FlushAsync();
                    }

                    index++;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        private string BuildQueryParams(string keyword, DateTime? dateFrom, DateTime? dateTo)
        {
            var query = new List<string>();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string input = keyword.Trim();
                bool isId = input.All(char.IsDigit);
                query.Add((isId ? "PatientID" : "PatientName") + "=" + Uri.EscapeDataString(keyword) + "*");
            }

            if (dateFrom.HasValue || dateTo.HasValue)
            {
                string fromDate = dateFrom?.ToString("yyyyMMdd") ?? "";
                string toDate = dateTo?.ToString("yyyyMMdd") ?? "";
                string fromTime = dateFrom?.ToString("HHmmss") ?? "";
                string toTime = dateTo?.ToString("HHmmss") ?? "";

                query.Add($"StudyDate={fromDate}-{toDate}");
                query.Add($"StudyTime={fromTime}-{toTime}");
            }

            if (query.Count == 0)
                return "";

            return "?" + string.Join("&", query);
        }
    }
}