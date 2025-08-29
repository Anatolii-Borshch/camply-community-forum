using Camply.Application.Contracts.Repositories;
using Camply.Domain.Entities;
using Camply.Persistence.DbContext;
using Camply.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Camply.Persistence
{
    public static class PersistenceDependencyInjection
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IForumRepository, ForumRepository>();
            services.AddScoped<ILikedPostRepository, LikedPostRepository>();
            services.AddScoped<ISavedRepository, SavedRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IUserVoteRepository, UserVoteRepository>();
            services.AddScoped<IVoteOptionRepository, VoteOptionRepository>();
            services.AddScoped<IVoteRepository, VoteRepository>();

            services.AddScoped<ISpecifiedRepository<Forum>, ForumRepository>();
            services.AddScoped<ISpecifiedRepository<Post>, PostRepository>();
            services.AddScoped<ISpecifiedRepository<Vote>, VoteRepository>();
            
            services.AddDbContext<CamplyDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DbConnectionString");
                options.UseSqlServer(connectionString).UseLazyLoadingProxies();
            });
            
            return services;
        }
    }
}