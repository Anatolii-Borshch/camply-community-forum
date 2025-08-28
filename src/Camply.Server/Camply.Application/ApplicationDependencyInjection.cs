using Camply.Application.Contracts.Services;
using Camply.Application.Implementations;
using Camply.Application.Validators;
using Camply.Shared.Dtos.Forum;
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
            
            return services;
        }
    }
}