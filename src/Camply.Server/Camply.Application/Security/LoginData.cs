namespace Camply.Application.Security
{
    public class LoginData
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = null!;
    }
}