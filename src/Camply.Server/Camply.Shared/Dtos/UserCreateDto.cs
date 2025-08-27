namespace Camply.Shared.Dtos
{
    public record UserRegisterDto(
        string Name,
        string Surname,
        string Username,
        string Email,
        string Password,
        DateTime BirthDate
    );
}