using BusinessObject.DTOs.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service
{
    public class ScanningCardService : IScanningCardService
    {
        private readonly string? ApiKey;
        public ScanningCardService(IConfiguration configuration)
        {
            ApiKey = configuration["FPTAISettings:ApiKey"];
        }
        public async Task<string> ScanVietnameseIDCardAsync(IFormFile file)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("api-key", ApiKey);

            using var form = new MultipartFormDataContent();

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var fileContent = new StreamContent(memoryStream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            form.Add(fileContent, "image", file.FileName);

            var response = await httpClient.PostAsync("https://api.fpt.ai/vision/idr/vnm/", form);
            var responseContent = await response.Content.ReadAsStringAsync();

            return responseContent;
        }

        public async Task<ParsedIdCardResult?> ParseVietnameseIdCardAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            var resultJson = await ScanVietnameseIDCardAsync(file);
            var result = JsonSerializer.Deserialize<FptIdCardResponse>(resultJson);

            if (result?.ErrorCode != 0 || result.Data == null || result.Data.Count == 0)
                return null;

            var data = result.Data.First();

            var dob = ParseDate(data.DateOfBirth);
            var issueDate = ParseDate(data.IssueDate);
            var expiryDate = ParseDate(data.ExpiryDate);

            var validTypes = new HashSet<string> { "cc_back", "cc_front", "chip_front", "chip_back" };
            if (validTypes.Contains(data.CardSideType))
            {
                return new ParsedIdCardResult
                {
                    FullName = data.FullName,
                    Gender = data.Gender?.ToLower() switch
                    {
                        "nam" => true,
                        "nữ" => false,
                        _ => null
                    },
                    DateOfBirth = dob,
                    IssueDate = issueDate,
                    ExpiryDate = expiryDate,
                    IdNumber = data.IdNumber,
                    Address = data.Address ?? data.Home,
                    PlaceOfIssue = data.PlaceOfIssue,
                    CardSideType = data.CardSideType,
                    PlaceOfBirth = data.PlaceOfBirth
                };
            }
            throw new InvalidOperationException($"Invalid card type: {data.CardSideType}");
        }

        private DateTime? ParseDate(string? rawDate)
        {
            if (string.IsNullOrWhiteSpace(rawDate) || rawDate == "N/A")
                return null;

            string[] formats = { "dd/MM/yyyy", "yyyy-MM-dd", "dd-MM-yyyy" };
            if (DateTime.TryParseExact(rawDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return dt;

            return null;
        }
    }
}
