using PruebaFireBase.Models;

namespace PruebaFireBase.Interfaces
{
    public interface IPhoneStorageRepository
    {
        Task<(string Path, string Url)> UploadImageAsync(string path, Stream imageStream);
        Task<bool> UpdateImageAsync(string phoneId, PhoneModel phone, IFormFile? newImage);
        Task<bool> DeleteImageAsync(string imagePath);
    }
}
