using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Features.Customers.Queries;
using RookieEcommerce.Domain.Entities;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace RookieEcommerce.Infrastructure.Persistence
{
    public class CustomerRepository(ApplicationDbContext context) : ICustomerRepository
    {
        public async Task<bool> AnyAsync(Expression<Func<Customer, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await context.Customers.AnyAsync(filter, cancellationToken);
        }

        public virtual async Task<Customer?> GetByIdAsync(Guid id, Func<IQueryable<Customer>, IIncludableQueryable<Customer, object>>? include = null, CancellationToken cancellationToken = default)
        {
            IQueryable<Customer> query = context.Customers;

            //query = AddIncludesToQuery(includeProperties, query);
            if (include != null)
            {
                query = include(query);
            }

            return await query.FirstOrDefaultAsync(c => c.Id == id.ToString(), cancellationToken);
        }

        public Task<PaginationList<Customer>> GetPaginated(GetCustomersQuery request)
        {
            var customers = context.Customers.AsQueryable();

            return Task.FromResult(PaginationList<Customer>.Create(customers, request.PageSize, request.PageNumber));
        }
    }
}