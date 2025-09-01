using System.Linq.Expressions;
using Camply.Domain.Entities;
using Camply.Shared.Dtos.Post;

namespace Camply.Application.Specifications
{
    public class PostSearchSpecification : BaseSpecification<Post>
    {
        public PostSearchSpecification(ForumPostsSearchRequest request)
            : base(BuildCriteria(request))
        {
            AddInclude(p => p.User);
            AddInclude(p => p.Comments);
            AddInclude(p => p.LikedPosts);
            AddInclude(p => p.SavedPosts);

            switch (request.OrderBy.ToLower())
            {
                case "likes":
                    if (request.Descending)
                        ApplyOrderByDescending(p => p.LikedPosts.Count);
                    else
                        ApplyOrderBy(p => p.LikedPosts.Count);
                    break;

                case "comments":
                    if (request.Descending)
                        ApplyOrderByDescending(p => p.Comments.Count);
                    else
                        ApplyOrderBy(p => p.Comments.Count);
                    break;

                case "title":
                    if (request.Descending)
                        ApplyOrderByDescending(p => p.Title);
                    else
                        ApplyOrderBy(p => p.Title);
                    break;

                default:
                    if (request.Descending)
                        ApplyOrderByDescending(p => p.CreatedDate);
                    else
                        ApplyOrderBy(p => p.CreatedDate);
                    break;
            }

            ApplyPaging(request.Skip, request.Take);
        }

        private static Expression<Func<Post, bool>>? BuildCriteria(ForumPostsSearchRequest request)
        {
            return p =>
                (request.ForumId == null || p.ForumId == request.ForumId) &&
                (request.AuthorId == null || p.UserId == request.AuthorId) &&
                (string.IsNullOrEmpty(request.Title) || p.Title.Contains(request.Title)) &&
                (request.IsPinned == null || p.IsPinned == request.IsPinned) &&
                (request.CreatedAfter == null || p.CreatedDate >= request.CreatedAfter) &&
                (request.CreatedBefore == null || p.CreatedDate <= request.CreatedBefore);
        }
    }
}