namespace WebApplication1.Models
{
    public class JwtTokenResponse
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public string Role { get; set; }
        public string Username { get; set; }
    }
}