/* Services/Firebase */

using Firebase.Storage;
using System.IO;

namespace Bina.Services
{
    public class FirebaseCloud
    {
        private readonly string _apiKey;
        private readonly string _bucket;
        private FirebaseStorage _firebaseStorage;

        public FirebaseCloud(string apiKey, string bucket, string authEmail, string authPassword)
        {
            _apiKey = apiKey;
            _bucket = bucket;

            _firebaseStorage = new FirebaseStorage(_bucket, new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(_apiKey),
                ThrowOnCancel = true
            });
        }

        public async Task<string> UploadFileToFirebase(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{DateTime.Now.Ticks}{extension}";
            var stream = file.OpenReadStream();

            var result = await _firebaseStorage
                .Child("images")
                .Child(fileName)
                .PutAsync(stream);

            return result; // Trả về URL của file đã được tải lên
        }

        public async Task<string> UploadAvatarToFirebase(IFormFile file, string avatar)
        {
            if (file == null || file.Length == 0)
                return null;

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{DateTime.Now.Ticks}{extension}";
            var stream = file.OpenReadStream();


            var result = await _firebaseStorage
            .Child(avatar) // Include the folder path in the storage reference
            .Child(fileName)
            .PutAsync(stream);
            return result; // Trả về URL của file đã được tải lên

        }

        public async Task<Stream> DownloadFileFromFirebase(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            var stream = new MemoryStream();
            var reference = _firebaseStorage.Child("images").Child(filePath);
            var task = reference.GetDownloadUrlAsync();

            await Task.Run(async () =>
            {
                var url = await task;
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        await response.Content.CopyToAsync(stream);
                    }
                }
            });

            stream.Position = 0; // Reset stream position after download
            return stream;
        }

    }
}