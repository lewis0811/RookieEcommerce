using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.Infrastructure.Persistence
{
    public class OrderRepository(ApplicationDbContext context) : BaseRepository<Order>(context.Orders), IOrderRepository
    {
    }
}