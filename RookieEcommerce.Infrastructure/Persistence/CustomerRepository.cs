using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Features.Customers.Queries;
using RookieEcommerce.Application.Features.Products.Queries;
using RookieEcommerce.Domain.Entities;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;

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

        public Task<PaginationList<Customer>> GetPaginated(GetCustomersQuery query)
        {
            var customers = context.Customers.AsQueryable();

            customers = ApplySearch(query, customers);

            // Apply sorting if sort by is not null
            customers = ApplySort(query, customers);

            return Task.FromResult(PaginationList<Customer>.Create(customers, query.PageSize, query.PageNumber));
        }

        private static IQueryable<Customer> ApplySort(GetCustomersQuery query, IQueryable<Customer> customers)
        {
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                customers = customers.OrderBy(query.SortBy);
            }
            return customers;
        }

        private static IQueryable<Customer> ApplySearch(GetCustomersQuery query, IQueryable<Customer> customers)
        {
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                customers = customers.Where(c => c.Email!.Contains(query.SearchTerm) ||
                                               c.UserName!.Contains(query.SearchTerm) ||
                                               c.FirstName!.Contains(query.SearchTerm) ||
                                               c.LastName!.Contains(query.SearchTerm)
                                               );
            }

            return customers;
        }
    }
}