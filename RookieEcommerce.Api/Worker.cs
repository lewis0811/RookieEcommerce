﻿using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.Infrastructure;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace RookieEcommerce.Api
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
                        await userManager.AddToRoleAsync(adminUser, "ADMIN");
                    }
                    else
                    {
                        throw new InvalidOperationException($"Error creating admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }

            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
