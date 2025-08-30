namespace Camply.Shared.Dtos.User
{
    public record ResetPasswordRequest(Guid UserId, string Password);
}