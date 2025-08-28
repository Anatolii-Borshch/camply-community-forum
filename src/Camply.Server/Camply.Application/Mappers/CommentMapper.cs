using Camply.Application.Helpers;
using Camply.Domain.Entities;
using Camply.Shared.Dtos.Comment;

namespace Camply.Application.Mappers
{
    public static class CommentMapper
    {
        public static CommentDto MapToCommentDto(this Comment comment)
        {
            var result = new CommentDto(comment.UserId, comment.Content
                , comment.UserId, comment.User.Username, comment.ParentCommentId);

            result.TimeExisted = TimeHelper.GetTimeExisted(comment.CreatedDate);
            result.IsEdited = comment.CreatedDate == comment.ModifiedDate;
            
            return result;
        }
    }
}