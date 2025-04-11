using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace RookieEcommerce.Application
{
    public static class DependencyInjection
    {
        public interface IAssemblyMarker
        { }

        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            // --- Add FluentValidation ---
            // Register validators from this assembly
            services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();

            return services;
        }
    }
}