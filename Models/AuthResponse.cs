namespace PruebaFireBase.Models
{
    public class AuthResponse
    {
        public string IdToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public string LocalId { get; set; } = null!;
    }
}
