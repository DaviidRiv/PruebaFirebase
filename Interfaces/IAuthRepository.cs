namespace PruebaFireBase.Interfaces
{
    public interface IAuthRepository
    {
        Task<string> LoginAsync(string email, string password);
        Task<string> RegisterAsync(string email, string password);
        Task SendPasswordResetEmailAsync(string email);
    }
}
