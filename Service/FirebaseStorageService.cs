using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Service.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class FirebaseStorageService : IFirebaseStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly string _bucket = "safe-city-gsu25se61.firebasestorage.app";

        public FirebaseStorageService(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder = "uploads")
        {
            var filePath = Path.Combine(_env.ContentRootPath, _configuration["Firebase:CredentialPath"]);

            // Load credentials
            GoogleCredential credential = GoogleCredential.FromFile(filePath);
            var storage = StorageClient.Create(credential);

            var safeFileName = Path.GetFileNameWithoutExtension(file.FileName).Replace(" ", "_");
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{folder}/{Guid.NewGuid()}_{safeFileName}{extension}";

            using var stream = file.OpenReadStream();

            await storage.UploadObjectAsync(new Google.Apis.Storage.v1.Data.Object
            {
                Bucket = _bucket,
                Name = fileName
            }, stream, new UploadObjectOptions
            {
                PredefinedAcl = PredefinedObjectAcl.PublicRead
            });

            return $"https://storage.googleapis.com/{_bucket}/{fileName}";
        }
    }
}
