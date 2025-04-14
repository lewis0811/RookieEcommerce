namespace RookieEcommerce.Api.Constants
{
    public static class ApiEndPointConstant
    {
        public const string RootEndPoint = "/api";
        public const string ApiVersion = "/v1";
        public const string ApiEndPoint = RootEndPoint + ApiVersion;

        public static class Product
        {
            public const string ProductsEndpoint = ApiEndPoint + "/products";
            public const string ProductEndpoint = ProductsEndpoint + "/{productId}";
        }

        public static class Categories
        {
            public const string CategoriesEndpoint = ApiEndPoint + "/categories";
            public const string CategoryEndpoint = CategoriesEndpoint + "/{categoryId}";
        }
    }
}
