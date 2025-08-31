namespace Camply.Application.Contracts.Security
{
    public interface IPasswordHasher<TUser>
    {
        string HashPassword(TUser user, string password);
        bool VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword);
    }
}