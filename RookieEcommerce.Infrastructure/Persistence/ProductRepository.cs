using Microsoft.EntityFrameworkCore.Query;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Features.Products.Queries;
using RookieEcommerce.Domain.Entities;
using System.Linq.Dynamic.Core;

namespace RookieEcommerce.Infrastructure.Persistence
{
    public class ProductRepository(ApplicationDbContext context) : BaseRepository<Product>(context.Products), IProductRepository
    {
        public Task<PaginationList<Product>> GetPaginated(GetProductsQuery query, Func<IQueryable<Product>, IIncludableQueryable<Product, object>>? include)
        {
            var products = context.Products.AsQueryable();

            // Apply include query if includeProperties is not null
            products = ApplyInclude(query, products);
            products = ApplyInclude2(include, products);

            // Apply filter if it is not null
            products = ApplyFilter(query, products);

            // Apply searching term if search term is not null
            products = ApplySearch(query, products);

            // Apply sorting if sort by is not null
            products = ApplySort(query, products);

            return Task.FromResult(PaginationList<Product>.Create(products, query.PageSize, query.PageNumber));
        }

        private static IQueryable<Product> ApplyInclude2(Func<IQueryable<Product>, IIncludableQueryable<Product, object>>? include, IQueryable<Product> products)
        {
            if (include != null)
            {
                products = include(products);
            }

            return products;
        }

        private static IQueryable<Product> ApplySort(GetProductsQuery query, IQueryable<Product> products)
        {
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                products = products.OrderBy(query.SortBy);
            }
            return products;
        }

        private static IQueryable<Product> ApplySearch(GetProductsQuery query, IQueryable<Product> products)
        {
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                products = products.Where(c => c.Name.Contains(query.SearchTerm, StringComparison.InvariantCultureIgnoreCase) ||
                                               c.Description.Contains(query.SearchTerm, StringComparison.InvariantCultureIgnoreCase) ||
                                               (c.Category != null && c.Category.Name.Contains(query.SearchTerm, StringComparison.InvariantCultureIgnoreCase)));
            }

            return products;
        }

        private static IQueryable<Product> ApplyFilter(GetProductsQuery query, IQueryable<Product> products)
        {
            if (query.CategoryId != null)
            {
                products = products.Where(c => c.CategoryId == query.CategoryId);
            }
            if (query.MinPrice != null)
            {
                products = products.Where(c => c.Price >= query.MinPrice);
            }
            if (query.MaxPrice != null)
            {
                products = products.Where(c => c.Price <= query.MaxPrice);
            }

            return products;
        }

        private static IQueryable<Product> ApplyInclude(GetProductsQuery query, IQueryable<Product> products)
        {
            if (!string.IsNullOrEmpty(query.IncludeProperties))
            {
                products = AddIncludesToQuery(query.IncludeProperties, products);
            }

            return products;
        }
    }
}