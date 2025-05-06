using Microsoft.EntityFrameworkCore.Query;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Features.Customers.Queries;
using RookieEcommerce.Domain.Entities;
using System.Linq.Expressions;

namespace RookieEcommerce.Application.Contacts.Persistence
{
    public interface ICustomerRepository
    {
        public Task<bool> AnyAsync(Expression<Func<Customer, bool>> filter, CancellationToken cancellationToken = default);

        public Task<Customer?> GetByIdAsync(Guid id, Func<IQueryable<Customer>, IIncludableQueryable<Customer, object>>? include = null, CancellationToken cancellationToken = default);
        Task<PaginationList<Customer>> GetPaginated(GetCustomersQuery request);
    }
}