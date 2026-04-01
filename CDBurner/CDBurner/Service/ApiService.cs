using CDBurner.Model;
using CDBurner.Service.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
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
            
            // Ovo osgiruati da ne pada aplikacija
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            var json = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<AppConfigModel>(json);
            _baseUrl = config.ApiBaseUrl;
            _httpClient = new HttpClient { BaseAddress = new Uri(_baseUrl) };
        }

        public async Task<List<StudyModel>> GetStudiesAsync(int currentPage, int pageSize, string keyword)
        {
            try
            {
                // includefield: 00081030
                // za svaki parametar trebace poseban poziv i onda konkatenacija na kraju
                var response = await _httpClient.GetAsync($"?offset={(currentPage - 1) * pageSize}&limit={pageSize}&includefield=00081030");

                if (!response.IsSuccessStatusCode)
                {
                    return new List<StudyModel>();
                }

                var json = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(json))
                    return new List<StudyModel>();

                var raw = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(json);

                IObjectMapper mapper = new ObjectMapper();
                return raw?.Select(mapper.MapStudy).ToList() ?? new List<StudyModel>();
            }
            catch (HttpRequestException)
            {
                return new List<StudyModel>();
            }
            catch (JsonException)
            {
                return new List<StudyModel>();
            }
            catch (Exception)
            {
                return new List<StudyModel>();
            }
        }

        public async Task<int> GetTotalStudiesCountAsync(string keyword)
        {
            try
            {
                var url = _baseUrl + "/count";

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    // za svaki parametar trebace poseban poziv i onda konkatenacija na kraju
                    // ovo sve isto vrijedi i za metodu iznad
                    // izdvoji Uri.EscapeDataString(keyword)
                    // PatientID
                    // StudyDate za njega napraviti DateTime picker od-do, treba da ima poseban format u url-u
                    // StudyDate=20240101-20261231&
                    // StudyTime = 000000 - 235959
                    url += $"?PatientName={Uri.EscapeDataString(keyword)}*"; // ovdje nije samo patient name
                }

                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Accept", "application/json");

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return 0;

                var json = await response.Content.ReadAsStringAsync();
                var obj = JsonSerializer.Deserialize<Dictionary<string, int>>(json);

                return obj != null && obj.ContainsKey("count") ? obj["count"] : 0;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<bool> DownloadStudyAsync(StudyModel study, string destinationFolder)
        {
            try
            {
                var response = await _httpClient.GetAsync(new Uri(study.Url));

                if (!response.IsSuccessStatusCode)
                    return false;

                Directory.CreateDirectory(destinationFolder);

                var filePath = Path.Combine(destinationFolder, "study.zip"); // ovo jos nije definisan nacin na koji ce se cuvati

                using var stream = await response.Content.ReadAsStreamAsync();
                using var file = File.Create(filePath);

                await stream.CopyToAsync(file);

                // ovo jos nije definisan nacin na koji ce se cuvati
                System.IO.Compression.ZipFile.ExtractToDirectory(filePath, destinationFolder, true);

                return true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
            catch (System.IO.InvalidDataException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
