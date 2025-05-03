namespace RookieEcommerce.Application.Common
{
    public class PaginatedQuery
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; } = "id desc";
        public string? IncludeProperties { get; set; }
    }
}