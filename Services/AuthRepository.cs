using Microsoft.Extensions.Options;
using PruebaFireBase.Interfaces;
using PruebaFireBase.Models;
using PruebaFireBase.Models.Configs;

namespace PruebaFireBase.Services
{
    public class AuthRepository : IAuthRepository
    {
        private readonly HttpClient _httpClient;
        private readonly FirebaseConfig _config;

        public AuthRepository(IOptions<FirebaseConfig> config)
        {
            _httpClient = new HttpClient();
            _config = config.Value;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var requestPayload = new
            {
                email,
                password,
                returnSecureToken = true
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_config.API}",
                requestPayload
            );

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Login failed. Please check your credentials.");
            }

            var responseContent = await response.Content.ReadFromJsonAsync<AuthResponse>();
            return responseContent?.IdToken ?? throw new Exception("Failed to retrieve ID token.");
        }

        public async Task<string> RegisterAsync(string email, string password)
        {
            var requestPayload = new
            {
                email,
                password,
                returnSecureToken = true
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={_config.API}",
                requestPayload
            );

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Registration failed.");
            }

            var responseContent = await response.Content.ReadFromJsonAsync<AuthResponse>();
            return responseContent?.IdToken ?? throw new Exception("Failed to retrieve ID token.");
        }

        public async Task SendPasswordResetEmailAsync(string email)
        {
            var requestPayload = new
            {
                email,
                requestType = "PASSWORD_RESET"
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={_config.API}",
                requestPayload
            );

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al enviar el correo: {errorContent}");
            }
        }
    }
}
