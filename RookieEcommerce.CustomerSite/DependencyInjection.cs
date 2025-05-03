using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Client;
using Quartz;
using RookieEcommerce.CustomerSite.Services;
using RookieEcommerce.Infrastructure;
using System.Net.Http.Headers;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace RookieEcommerce.CustomerSite
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebServices(
            this IServiceCollection services,
#pragma warning disable IDE0060 // Remove unused parameter
            IConfiguration configuration) // configuration might be needed for some services
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // Register HttpClient
            services.AddHttpClient<ProductApiClient>()
                .ConfigureRookieEcommerceApi(configuration);
            services.AddHttpClient<CategoryApiClient>()
                .ConfigureRookieEcommerceApi(configuration);
            services.AddHttpClient<ProductRatingApiClient>()
                .ConfigureRookieEcommerceApi(configuration);
            services.AddHttpClient<OrderApiClient>()
                .ConfigureRookieEcommerceApi(configuration);
            services.AddHttpClient<CartApiClient>()
                .ConfigureRookieEcommerceApi(configuration);
            services.AddHttpClient<CartItemApiClient>()
                .ConfigureRookieEcommerceApi(configuration);
            services.AddHttpClient<VnPayApiClient>()
                .ConfigureRookieEcommerceApi(configuration);
            services.AddHttpClient<VnPublicApiClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["VnPublicApi:BaseUrl"]!);
            });

            // --- Register DbContext service for Worker usage ---
            // Retrieve Connection Strings
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // Add DbContext, use SqlServer, OpenIddict
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
                options.UseOpenIddict();
            });

            // Add Cookie-based AuthN
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(50);
                options.SlidingExpiration = false;
            });

            // OpenIddict offers native integration with Quartz.NET to perform scheduled tasks
            services.AddQuartz(options =>
            {
                options.UseSimpleTypeLoader();
                options.UseInMemoryStore();
            });

            // Add Quartz.NET service and configure it to block shutdown until jobs are complete.
            services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

            // Add OpenIddict
            services.AddOpenIddict()
            // Register the OpenIddict core components.
            .AddCore(options =>
            {
                // Configure OpenIddict to use the Entity Framework Core stores and models.
                // Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.
                options.UseEntityFrameworkCore()
                       .UseDbContext<ApplicationDbContext>();

                // Enable Quartz.NET integration.
                options.UseQuartz();
            })
            // Register the OpenIddict client components
            .AddClient(options =>
            {
                options.AllowAuthorizationCodeFlow();

                options.AddDevelopmentEncryptionCertificate()
                           .AddDevelopmentSigningCertificate();

                options.UseAspNetCore()
                    .EnableStatusCodePagesIntegration()
                    .EnableRedirectionEndpointPassthrough();

                options.UseSystemNetHttp()
                    .SetProductInformation(typeof(Program).Assembly);

                options.AddRegistration(new OpenIddictClientRegistration
                {
                    Issuer = new Uri(configuration["OpenIddict:Issuer"]!, UriKind.Absolute),
                    ClientId = configuration["OpenIddict:ClientId"],
                    ClientSecret = configuration["OpenIddict:ClientSecret"],
                    Scopes = { Scopes.Email, Scopes.Profile },

                    RedirectUri = new Uri(configuration["OpenIddict:RedirectUri"]!, UriKind.Relative),
                    PostLogoutRedirectUri = new Uri(configuration["OpenIddict:PostLogoutRedirectUri"]!, UriKind.Relative)
                });
            })
            // Register the OpenIddict server components.
            .AddServer(options =>
            {
                options.SetAuthorizationEndpointUris("connect/authorize")
                   .SetEndSessionEndpointUris("connect/logout")
                   .SetTokenEndpointUris("connect/token")
                   .SetUserInfoEndpointUris("connect/userinfo");

                options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);

                options.AllowAuthorizationCodeFlow();

                // Register the signing and encryption credentials.
                options.AddDevelopmentEncryptionCertificate()
                   .AddDevelopmentSigningCertificate();

                // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                options.UseAspNetCore()
                   .EnableAuthorizationEndpointPassthrough()
                   .EnableEndSessionEndpointPassthrough()
                   .EnableTokenEndpointPassthrough()
                   .EnableUserInfoEndpointPassthrough()
                   .EnableStatusCodePagesIntegration();
            })
            // Register the OpenIddict validation components
            .AddValidation(options =>
            {
                // Import the configuration from the local OpenIddict server instance.
                options.UseLocalServer();

                // Register the ASP.NET Core host.
                options.UseAspNetCore();
            });

            // Register the worker responsible for creating the database used to store tokens.
            services.AddHostedService<Worker>();

            return services;
        }

        // Helper method to configure HttpClient
        public static IHttpClientBuilder ConfigureRookieEcommerceApi(this IHttpClientBuilder builder, IConfiguration configuration)
        {
            var baseAddress = configuration["HttpClient:ClientAddress"]
                              ?? throw new InvalidOperationException("Configuration key 'HttpClient:ClientAddress' is missing.");

            builder.ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            return builder;
        }
    }
}