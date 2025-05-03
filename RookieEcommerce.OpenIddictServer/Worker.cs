using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using RookieEcommerce.Api.Constants;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.Infrastructure;
using StackExchange.Redis;
using System.Data;
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
                    DisplayName = "MVC client application",
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
                    Permissions.ResponseTypes.Code,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles
                },
                    Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    }
                }, cancellationToken);
            }

            // --- Seed role and admin user for database ---
            // Create the role manager
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Add roles if they don't exist
            foreach (var roleName in new[] { ApplicationRole.Admin, ApplicationRole.User })
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new IdentityRole { Name = roleName };
                    await roleManager.CreateAsync(role);
                }
            }

            // --- Seed admin user ---
            // Create the user manager
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Customer>>();

            // Check if the admin user already exists
            var adminUser = await userManager.FindByEmailAsync(configuration["AdminSettings:Email"]!);

            if (adminUser == null)
            {
                adminUser = new Customer
                {
                    UserName = configuration["AdminSettings:Username"]!,
                    Email = configuration["AdminSettings:Email"]!,
                    FirstName = configuration["AdminSettings:FirstName"]!,
                    LastName = configuration["AdminSettings:LastName"]!,
                    EmailConfirmed = true,
                };

                // Create the admin user with the specified password
                var result = await userManager.CreateAsync(adminUser, configuration["AdminSettings:Password"]!);

                if (result.Succeeded)
                {
                    // Assign the admin role to the user
                    await userManager.AddToRoleAsync(adminUser, ApplicationRole.Admin);
                }
                else
                {
                    throw new InvalidOperationException($"Error creating admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}