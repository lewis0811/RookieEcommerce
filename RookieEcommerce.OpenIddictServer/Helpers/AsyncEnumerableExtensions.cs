namespace RookieEcommerce.OpenIddictServer.Helpers
{
    internal static class AsyncEnumerableExtensions
    {
        public static async Task<List<TSource>> ToListAsync<TSource>(
            this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        {
            var list = new List<TSource>();
            await foreach (var element in source.WithCancellation(cancellationToken))
            {
                list.Add(element);
            }
            return list;
        }
    }
}
