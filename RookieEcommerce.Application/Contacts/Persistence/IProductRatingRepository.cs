using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Features.ProductRatings.Queries;
using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.Application.Contacts.Persistence
{
    public interface IProductRatingRepository : IBaseRepository<ProductRating>
    {
        Task<PaginationList<ProductRating>> GetPaginated(GetProductRatingQuery query, Func<IQueryable<ProductRating>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ProductRating, object>>? filter);
    }
}