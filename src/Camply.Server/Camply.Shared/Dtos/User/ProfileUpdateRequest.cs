namespace Camply.Shared.Dtos.User
{
    public record ProfileUpdateRequest(string Name, string Surname, DateTime Birthday, string Username);
}