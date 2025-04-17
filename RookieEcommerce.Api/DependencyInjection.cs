using Asp.Versioning;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using RookieEcommerce.Api.Configurations;
using RookieEcommerce.Infrastructure;

namespace RookieEcommerce.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(
    this IServiceCollection services,
#pragma warning disable IDE0060 // Remove unused parameter
    IConfiguration configuration) // configuration might be needed for some services
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // --- Add Controllers & API Behavior ---
            services.AddControllers();

            // --- Add ApiVersioning configuration
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = ApiVersion.Default;
                options.ReportApiVersions = true;
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

            services.ConfigureOptions<ConfigureSwaggerOptions>(); // !Important, this one make the version work on swagger

            // --- Add FluentValidation ASP.NET Core Integration ---
            services.AddFluentValidationAutoValidation();

            // --- Add Swagger/OpenAPI ---
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                // Define the OAuth2.0 scheme that's compatible with OpenIddict
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            // URLs must match EXACTLY what you configured in AddOpenIddict().AddServer()
                            AuthorizationUrl = new Uri("/connect/authorize", UriKind.Relative), // Relative URL to your auth endpoint
                            TokenUrl = new Uri("/connect/token", UriKind.Relative),             // Relative URL to your token endpoint
                            Scopes = new Dictionary<string, string>
                            {
                                // Scopes must match EXACTLY what you configured in AddOpenIddict().AddServer()
                                { "openid", "OpenID Connect Scope" },
                                { "profile", "User Profile Scope" },
                                { "email", "User Email Scope" },
                                { "roles", "User Roles Scope" },
                                { "api", "API Access Scope" }
                            }
                        }
                    }
                });

                // Add a global security requirement to ensure endpoints use the defined scheme
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2" // Must match the name given in AddSecurityDefinition
                            },
                            Scheme = "oauth2",
                            Name = "oauth2",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });

            return services;
        }

        public static IServiceCollection AddIdentityServices(
            this IServiceCollection services,
#pragma warning disable IDE0060 // Remove unused parameter
            IConfiguration configuration)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // --- Add ASP.NET Core Identity ---
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>() // This method is defined in Microsoft.AspNetCore.Identity.EntityFrameworkCore
            .AddDefaultTokenProviders();

            // --- Add OpenIddict ---
            services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                           .UseDbContext<ApplicationDbContext>();
                })
                .AddServer(options =>
                {
                    options.SetAuthorizationEndpointUris("/connect/authorize")
                           .SetTokenEndpointUris("/connect/token")
                           .SetUserInfoEndpointUris("/connect/userinfo");

                    options.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();
                    options.AllowClientCredentialsFlow();
                    options.AllowRefreshTokenFlow();

                    options.AddDevelopmentEncryptionCertificate()
                           .AddDevelopmentSigningCertificate();

                    options.UseAspNetCore()
                           .EnableAuthorizationEndpointPassthrough()
                           .EnableTokenEndpointPassthrough()
                           .EnableUserInfoEndpointPassthrough();

                    options.RegisterScopes("openid", "profile", "email", "roles", "api");
                })
                .AddValidation(options =>
                {
                    options.UseLocalServer();
                    options.UseAspNetCore();
                });

            return services;
        }
    }
}