/* Services/Firebase */

using Firebase.Storage;

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
    }
}
