using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Features.Products.Queries;
using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.Application.Contacts.Persistence
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<PaginationList<Product>> GetPaginated(GetProductsQuery query);
    }
}