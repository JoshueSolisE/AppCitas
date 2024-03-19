using AppCitas.Interfaces;
using CloudinaryDotNet.Actions;

namespace AppCitas.Services;

public class PhotoService : IPhotoService
{
    public Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        throw new NotImplementedException();
    }

    public Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        throw new NotImplementedException();
    }
}
