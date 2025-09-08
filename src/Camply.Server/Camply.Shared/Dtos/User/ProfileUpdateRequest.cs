namespace Camply.Shared.Dtos.User
{
    public record ProfileUpdateRequest(Guid Id,string Name, string Surname, DateTime Birthday, string Username);
}