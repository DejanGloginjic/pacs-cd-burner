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

            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            var json = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            var baseUrl = config["ApiBaseUrl"]; // ovo rjesiti
            _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        }

        public async Task<List<Study>> GetStudiesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("studies");

                if (!response.IsSuccessStatusCode)
                {
                    return new List<Study>();
                }

                var json = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(json))
                    return new List<Study>();

                var studies = JsonSerializer.Deserialize<List<Study>>(json);

                return studies ?? new List<Study>();
            }
            catch (HttpRequestException)
            {
                return new List<Study>();
            }
            catch (JsonException)
            {
                return new List<Study>();
            }
            catch (Exception)
            {
                return new List<Study>();
            }
        }

        public async Task<bool> DownloadStudyAsync(string studyId, string destinationFolder)
        {
            try
            {
                var response = await _httpClient.GetAsync($"studies/{studyId}/download");

                if (!response.IsSuccessStatusCode)
                    return false;

                Directory.CreateDirectory(destinationFolder);

                var filePath = Path.Combine(destinationFolder, "study.zip");

                using var stream = await response.Content.ReadAsStreamAsync();
                using var file = File.Create(filePath);

                await stream.CopyToAsync(file);

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
