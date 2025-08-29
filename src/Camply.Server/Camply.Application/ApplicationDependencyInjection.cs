using Camply.Application.Contracts.Services;
using Camply.Application.Implementations;
using Camply.Application.Validators;
using Camply.Shared.Dtos.Comment;
using Camply.Shared.Dtos.Forum;
using Camply.Shared.Dtos.Post;
using Camply.Shared.Dtos.Vote;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Camply.Application
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IForumService, ForumService>();
            services.AddScoped<IValidator<ForumCreateRequest>, ForumCreateRequestValidator>();
            services.AddScoped<IValidator<ForumUpdateRequest>, ForumUpdateRequestValidator>();

            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IValidator<CommentCreateRequest>, CommentCreateRequestValidator>();
            services.AddScoped<IValidator<CommentUpdateRequest>, CommentUpdateRequestValidator>();

            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IValidator<PostCreateRequest>, PostCreateRequestValidator>();
            services.AddScoped<IValidator<PostUpdateRequest>, PostUpdateRequestValidator>();

            services.AddScoped<IVoteService, VoteService>();
            services.AddScoped<IValidator<VoteCreateRequest>, VoteCreateRequestValidator>();
            services.AddScoped<IValidator<VoteUpdateRequest>, VoteUpdateRequestValidator>();
            services.AddScoped< IValidator<VoteOptionUpdateRequest>, VoteOptionUpdateRequestValidator>();
                
            return services;
        }
    }
}