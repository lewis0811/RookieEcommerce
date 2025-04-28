using RookieEcommerce.CustomerSite.Services;
using System.Net.Http.Headers;

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
