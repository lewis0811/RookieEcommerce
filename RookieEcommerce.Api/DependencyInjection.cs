using Asp.Versioning;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;
using RookieEcommerce.Api.Configurations;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.Infrastructure;
using VNPAY.NET;

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
                // Define the Bearer authentication scheme
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer {token}' (without quotes).",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                // Add a global security requirement using the Bearer scheme
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" // Must match the name defined in AddSecurityDefinition
                },
                Scheme = "bearer",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>() // No scopes needed for JWT Bearer
        }
    });
            });


            // --- Add Vnpay Service ---
            services.AddScoped<IVnpay, Vnpay>();

            // --- Add OpenIddict ---
            services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore().UseDbContext<ApplicationDbContext>();
                })
                .AddServer(options =>
                {
                    options.RequireProofKeyForCodeExchange();

                    options.AddEncryptionKey(new SymmetricSecurityKey(
                               Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));

                    // Register the signing credentials.
                    options.AddDevelopmentSigningCertificate();
                })
                .AddValidation(options =>
                {
                    // Note: the validation handler uses OpenID Connect discovery
                    // to retrieve the issuer signing keys used to validate tokens.
                    options.SetIssuer("https://localhost:7004/");

                    // Register the encryption credentials. This sample uses a symmetric
                    // encryption key that is shared between the server and the API project.
                    //
                    // Note: in a real world application, this encryption key should be
                    // stored in a safe place (e.g in Azure KeyVault, stored as a secret).
                    options.AddEncryptionKey(new SymmetricSecurityKey(
                        Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));

                    // Register the System.Net.Http integration.
                    options.UseSystemNetHttp();

                    // Register the ASP.NET Core host.
                    options.UseAspNetCore();
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
            services.AddIdentity<Customer, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();

            return services;
        }
    }
}