using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Features.ProductVariants.Queries;
using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.Application.Contacts.Persistence
{
    public interface IProductVariantRepository : IBaseRepository<ProductVariant>
    {
        Task<PaginationList<ProductVariant>> GetPaginated(GetProductVariantsQuery query);
    }
}