using OpenIddict.Abstractions;
using RookieEcommerce.Infrastructure;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace RookieEcommerce.OpenIddictServer
{
    public class Worker(IServiceProvider serviceProvider, IConfiguration configuration) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await using var scope = serviceProvider.CreateAsyncScope();

            // Create scope to resolve services
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureCreatedAsync(cancellationToken);

            // --- Seed OpenIddict Application/Scope ---
            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
            // Check if the OpenIddict application already exists
            if (await manager.FindByClientIdAsync(configuration["OpenIddict:ClientId"]!, cancellationToken) == null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = configuration["OpenIddict:ClientId"],
                    ClientSecret = configuration["OpenIddict:ClientSecret"],
                    ConsentType = ConsentTypes.Explicit,
                    DisplayName = "NashLux",
                    RedirectUris =
                {
                    new Uri(configuration["OpenIddict:RedirectUri"]!)
                },
                    PostLogoutRedirectUris =
                {
                    new Uri(configuration["OpenIddict:PostLogoutRedirectUri"]!)
                },
                    Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.EndSession,
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.GrantTypes.ClientCredentials,
                    Permissions.ResponseTypes.Code,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles,
                    Permissions.Prefixes.Scope + "api"
                },
                    Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    }
                }, cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}