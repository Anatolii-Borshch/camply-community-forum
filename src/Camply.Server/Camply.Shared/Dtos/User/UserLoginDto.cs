namespace Camply.Shared.Dtos.User
{
    public record UserLoginDto(string Password, string? Username, string? Email);
}