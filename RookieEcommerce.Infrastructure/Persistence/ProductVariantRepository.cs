using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.Infrastructure.Persistence
{
    public class ProductVariantRepository(ApplicationDbContext context)
        : BaseRepository<ProductVariant>(context.ProductVariants), IProductVariantRepository
    {
    }
}