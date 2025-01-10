using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Options;
using PruebaFireBase.Interfaces;
using PruebaFireBase.Models;

namespace PruebaFireBase.Services
{
    public class PhoneRepository : IPhoneRepository
    {
        private readonly FirebaseClient _firebaseClient;

        public PhoneRepository(IOptions<FirebaseConfig> firebaseConfig)
        {
            var config = firebaseConfig.Value;

            _firebaseClient = new FirebaseClient(
                config.BasePath,
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(config.AuthSecret)
                });
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
    }
}
