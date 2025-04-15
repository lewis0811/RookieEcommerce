namespace RookieEcommerce.Application.Common
{
    public class PaginationList<T>(List<T> items, int count, int pageNumber, int pageSize)
    {
        public List<T> Items { get; set; } = items;
        public int PageNumber { get; set; } = pageNumber;
        public int PageSize { get; set; } = pageSize;
        public int TotalCount { get; set; } = count;
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public static PaginationList<T> Create(IQueryable<T> source, int pageSize, int pageNumber)
        {
            // Count the total number of items in the source
            int count = source.Count();

            // Calculate the total number of pages
            var items = source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginationList<T>(items, count, pageNumber, pageSize);
        }
    }
}