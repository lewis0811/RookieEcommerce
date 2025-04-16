using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Features.ProductImages.Queries;
using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.Application.Contacts.Persistence
{
    public interface IProductImageRepository : IBaseRepository<ProductImage>
    {
        Task<PaginationList<ProductImage>> GetPaginated(GetProductImagesQuery query);
    }
}