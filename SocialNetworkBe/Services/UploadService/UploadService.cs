using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Interfaces.ServiceInterfaces;
using Google.Apis.Upload;

namespace SocialNetworkBe.Services.UploadService
{
    public class UploadService : IUploadService
    {
        private readonly IConfiguration _configuration;
        public UploadService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<string>?> UploadFile(List<IFormFile> files, string folderName)
        {
            var uploadUrls = new List<string>();
            var cloudinaryApi = _configuration["Cloudinary:Api"];
            var cloudinary = new Cloudinary(cloudinaryApi);
            cloudinary.Api.Secure = true;
            foreach (var file in files)
            {
                UploadResult? uploadResult = null;
                using (var stream = file.OpenReadStream())
                {
                    if (folderName == "messages/images")
                    {
                        var uploadParams = new ImageUploadParams
                        {
                            File = new CloudinaryDotNet.FileDescription(file.FileName, stream),
                            Folder = folderName,
                            UseFilename = true,
                            UniqueFilename = false,
                        };
                        uploadResult = await cloudinary.UploadAsync(uploadParams);
                        if (uploadResult.Error != null)
                        {
                            return null;
                        }
                    } else
                    {
                        var uploadParams = new RawUploadParams
                        {
                            File = new CloudinaryDotNet.FileDescription(file.FileName, stream),
                            Folder = folderName,
                            UseFilename = true,
                            UniqueFilename = false,
                        };
                        uploadResult = await cloudinary.UploadAsync(uploadParams);
                        if (uploadResult.Error != null)
                        {
                            return null;
                        }
                    }
                   
                    if (uploadResult != null) uploadUrls.Add(uploadResult.SecureUrl.ToString());
                }
            }

            return uploadUrls;
        }

    }
}
