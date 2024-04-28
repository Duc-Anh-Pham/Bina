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

        // Method to upload a default avatar
        public async Task<string> UploadDefaultAvatar()
        {
            var defaultAvatarPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Avatar.png");
            using (var stream = File.Open(defaultAvatarPath, FileMode.Open))
            {
                var defaultFileName = $"default_avatar_{DateTime.Now.Ticks}.png";
                var result = await _firebaseStorage
                    .Child("avatar")
                    .Child(defaultFileName)
                    .PutAsync(stream);

                return result; // Returns the URL of the uploaded default avatar
            }
        }


        // Modified method to check and upload new avatar or default
        public async Task<string> UploadAvatarToFirebase(IFormFile avatarFile)
        {
            if (avatarFile == null || avatarFile.Length == 0)
            {
                // Upload and return default avatar URL if no file provided
                return await UploadDefaultAvatar();
            }
            else
            {
                var extension = Path.GetExtension(avatarFile.FileName);
                var fileName = $"{Path.GetFileNameWithoutExtension(avatarFile.FileName)}_{DateTime.Now.Ticks}{extension}";
                var stream = avatarFile.OpenReadStream();

                var result = await _firebaseStorage
                    .Child("avatar")
                    .Child(fileName)
                    .PutAsync(stream);

                return result; // Returns the URL of the uploaded file
            }
        }

    }
}