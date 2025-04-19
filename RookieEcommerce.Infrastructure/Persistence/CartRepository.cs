using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.Infrastructure.Persistence
{
    public class CartRepository(ApplicationDbContext context) : BaseRepository<Cart>(context.Carts), ICartRepository
    {
    }
}