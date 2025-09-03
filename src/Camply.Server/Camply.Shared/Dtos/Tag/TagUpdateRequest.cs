namespace Camply.Shared.Dtos.Tag
{
    public record TagUpdateRequest(Guid Id, string Name, Guid UserId);
}