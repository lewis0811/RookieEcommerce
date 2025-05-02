using Microsoft.EntityFrameworkCore.Query;
using RookieEcommerce.Domain.Entities;
using System.Linq.Expressions;

namespace RookieEcommerce.Application.Contacts.Persistence
{
    public interface ICustomerRepository
    {
        public Task<bool> AnyAsync(Expression<Func<Customer, bool>> filter, CancellationToken cancellationToken = default);
        public Task<Customer?> GetByIdAsync(Guid id, Func<IQueryable<Customer>, IIncludableQueryable<Customer, object>>? include = null, CancellationToken cancellationToken = default);
    }
}