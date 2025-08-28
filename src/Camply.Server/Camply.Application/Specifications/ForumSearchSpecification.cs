using Camply.Domain.Entities;
using Camply.Shared.Dtos;
using Camply.Shared.Dtos.Forum;

namespace Camply.Application.Specifications
{
    public class ForumSearchSpecification : BaseSpecification<Forum>
    {
        public ForumSearchSpecification(ForumSearchRequest request)
            : base(f =>
                (string.IsNullOrEmpty(request.Title) || f.Title.Contains(request.Title)) &&
                (request.TagIds == null || f.Tags.Any(t => request.TagIds.Contains(t.Id)))
            )
        {
            AddInclude(f => f.Tags);
            AddInclude(f => f.Posts);

            ApplyOrderBy(f => f.Title);
        }
    }
}