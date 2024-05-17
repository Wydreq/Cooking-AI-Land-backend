using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CookingAILand.Core.Helpers;
using CookingAILand.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CookingAILand.Core.Services;

public interface IPhotoUploadService
{
    Task<PhotoUploadResultDto> upload(IFormFile file);
}

public class PhotoUploadService : IPhotoUploadService
{
    private readonly Cloudinary _cloudinary;

    public PhotoUploadService(IOptions<CloudinarySettings> config)
    {
        var account = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);

        _cloudinary = new Cloudinary(account);
    }

    public async Task<PhotoUploadResultDto> upload(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();
        if (file.Length > 0)
        {
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.Name, stream)
                };
                uploadResult = _cloudinary.Upload(uploadParams);
            }

            return new PhotoUploadResultDto()
            {
                Url = uploadResult.Url.ToString(),
                PublicId = uploadResult.PublicId
            };
        }

        return null;
    }
}