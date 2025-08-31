using Camply.Domain.Entities;
using Camply.Shared.Dtos.Forum;
using Camply.Shared.Dtos.Tag;
using Camply.Shared.Dtos.User;

namespace Camply.Application.Mappers
{
    public static class UserMapper
    {
        public static UserProfileDto ToUserProfileDto(this User user)
        {
            return new UserProfileDto
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Birthday = user.BirthDate,
                CreatedPostAmount = user.Posts.Count,
                CreatedForums = user.Forums
                    .Select(f => new ForumPrevievDto(
                        f.Id,
                        f.Title,
                        f.Description,
                        f.Tags.Select(t => new TagDto(t.Id, t.Name)).ToList()
                    ))
                    .ToList()
            };
        }
    }
}