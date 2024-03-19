using AppCitas.Interfaces;
using CloudinaryDotNet.Actions;

namespace AppCitas.Services;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;
    public PhotoService(Ioptions<CloudinarySettings> config)
    {
        var acc = new Account
        (
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(acc);
    }

    public Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();
        if (file.lenght > 0)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation()
                                    .Height(500).Width(500)
                                    .Crop("fill").Gravity("face"),
                Folder = "AppCitas-n8"
            };
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }
        return uploadResult;
    }

    public Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        
        return await _cloudinary.DestroyAsync(deleteParams);
    }
}
