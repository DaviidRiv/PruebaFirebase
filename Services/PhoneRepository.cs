using Dropbox.Api;
using Dropbox.Api.Files;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Options;
using PruebaFireBase.Interfaces;
using PruebaFireBase.Models;
using PruebaFireBase.Models.Configs;

namespace PruebaFireBase.Services
{
    public class PhoneRepository : IPhoneRepository, IPhoneStorageRepository
    {
        private readonly FirebaseClient _firebaseClient;
        private readonly DropboxClient _dropboxClient;

        public PhoneRepository(IOptions<FirebaseConfig> firebaseConfig, IConfiguration configuration)
        {
            var config = firebaseConfig.Value;

            _firebaseClient = new FirebaseClient(
                config.BasePath,
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(config.AuthSecret)
                });

            var accessToken = configuration["Dropbox:AccessToken"];
            _dropboxClient = new DropboxClient(accessToken);
        }

        public async Task<List<PhoneModel>> GetAllPhonesAsync()
        {
            var phones = await _firebaseClient
                .Child("Phones")
                .OnceAsync<PhoneModel>();

            return phones.Select(p => new PhoneModel
            {
                PhoneId = p.Key,
                Name = p.Object.Name,
                Brand = p.Object.Brand,
                Price = p.Object.Price,
                Color = p.Object.Color,
                Description = p.Object.Description,
                UrlImage = p.Object.UrlImage
            }).ToList();
        }

        public async Task<PhoneModel?> GetPhoneByIdAsync(string phoneId)
        {
            var phone = await _firebaseClient
                .Child("Phones")
                .Child(phoneId)
                .OnceSingleAsync<PhoneModel>();

            return phone;
        }

        public async Task<string> AddPhoneAsync(PhoneModel phone)
        {
            var result = await _firebaseClient
                .Child("Phones")
                .PostAsync(phone);

            return result.Key; // Retorna el ID generado por Firebase
        }

        public async Task<bool> UpdatePhoneAsync(string phoneId, PhoneModel phone)
        {
            try
            {
                await _firebaseClient
                    .Child("Phones")
                    .Child(phoneId)
                    .PutAsync(phone);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeletePhoneAsync(string phoneId)
        {
            try
            {
                await _firebaseClient
                    .Child("Phones")
                    .Child(phoneId)
                    .DeleteAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<(string Path, string Url)> UploadImageAsync(string path, Stream imageStream)
        {
            try
            {
                var uploadResponse = await _dropboxClient.Files.UploadAsync(
                    path,
                    WriteMode.Overwrite.Instance,
                    body: imageStream
                );

                var sharedLinkResponse = await _dropboxClient.Sharing.CreateSharedLinkWithSettingsAsync(uploadResponse.PathDisplay);
                return (uploadResponse.PathDisplay, sharedLinkResponse.Url);
            }
            catch (Exception ex)
            {
                return (null!, null!);
            }
        }

        public async Task<bool> UpdateImageAsync(string phoneId, PhoneModel phone, IFormFile? newImage)
        {
            try
            {
                // Eliminar la imagen anterior si hay una nueva
                if (phone.Image != null && !string.IsNullOrEmpty(phone.UrlImage))
                {
                    // Extraer la ruta de Dropbox desde el URL almacenado
                    var previousPath = new Uri(phone.UrlImage).AbsolutePath;
                    await DeleteImageAsync(previousPath);
                }

                // Subir la nueva imagen si se proporciona
                if (newImage != null)
                {
                    var newPath = $"/phones/{phone.Brand}/{newImage.FileName}";
                    using var stream = newImage.OpenReadStream();
                    var newUrl = await UploadImageAsync(newPath, stream);

                    // Actualizar el URL en el modelo
                    phone.UrlImage = newUrl.Url;
                    phone.ImagePath = newUrl.Path;
                }

                // Actualizar los datos en Firebase
                phone.Image = null;
                phone.PhoneId = null;
                await UpdatePhoneAsync(phoneId, phone);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteImageAsync(string path)
        {
            try
            {
                if (path is not null)
                {
                    if (!path.StartsWith("/"))
                    {
                        throw new ArgumentException("El path debe ser relativo a la raíz de Dropbox y comenzar con '/'.");
                    }

                    // Eliminar el archivo en Dropbox
                    await _dropboxClient.Files.DeleteV2Async(path);
                    return true;
                }
                return true;
            }
            catch (ApiException<Dropbox.Api.Files.DeleteError> ex)
            {
                // Manejo específico para errores de Dropbox
                Console.WriteLine($"Error específico de Dropbox: {ex.ErrorResponse}");
                return false;
            }
            catch (Exception ex)
            {
                // Manejo general de errores
                Console.WriteLine($"Error al eliminar imagen: {ex.Message}");
                return false;
            }
        }
    }
}
