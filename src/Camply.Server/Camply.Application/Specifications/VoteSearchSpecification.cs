using Camply.Domain.Entities;
using Camply.Shared.Dtos.Vote;

namespace Camply.Application.Specifications
{
    public class VoteSearchSpecification : BaseSpecification<Vote>
    {
        public VoteSearchSpecification(ForumVotesSearchRequest request)
            : base(x =>
                (request.ForumId == null || x.ForumId == request.ForumId) &&
                (request.AuthorId == null || x.UserId == request.AuthorId) &&
                (request.VoteId == null || x.Id == request.VoteId) &&
                (string.IsNullOrEmpty(request.Title) || x.Title.Contains(request.Title))
            )
        {
            AddInclude(x => x.User);
            AddInclude(x => x.Forum);
            
            if (!string.IsNullOrEmpty(request.OrderBy))
            {
                if (request.OrderBy.Equals("title", StringComparison.OrdinalIgnoreCase))
                {
                    if (request.OrderDescending) ApplyOrderByDescending(x => x.Title);
                    else ApplyOrderBy(x => x.Title);
                }
                else if (request.OrderBy.Equals("createdAt", StringComparison.OrdinalIgnoreCase))
                {
                    if (request.OrderDescending) ApplyOrderByDescending(x => x.CreatedDate);
                    else ApplyOrderBy(x => x.CreatedDate);
                }
            }
            else
            {
                ApplyOrderBy(x => x.Title);
            }

            if (request.Take > 0)
                ApplyPaging(request.Skip, request.Take);
        }
    }
}