using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Features.Products.Queries;
using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.Application.Contacts.Persistence
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<PagedResult<Product>> GetPaginated(GetProductsQuery query);
    }
}