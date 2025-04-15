using Microsoft.EntityFrameworkCore;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Features.Products.Queries;
using RookieEcommerce.Domain.Entities;
using System.Linq;

namespace RookieEcommerce.Infrastructure.Persistence
{
    public class ProductRepository(ApplicationDbContext context) : BaseRepository<Product>(context.Products), IProductRepository
    {
        private static readonly string productName = nameof(Product.Name).ToLowerInvariant();
        private static readonly string productDescription = nameof(Product.Description).ToLowerInvariant();

        public Task<PagedResult<Product>> GetPaginated(GetProductsQuery query)
        {
            var products = context.Products.AsQueryable();
            
            // Apply include query if includeProperties is not null
            if (!string.IsNullOrEmpty(query.IncludeProperties))
            {
                products = AddIncludesToQuery(query.IncludeProperties, products);
            }

            // Apply filtering if it is not null
            if (query.MinPrice != null)
            {
                products = products.Where(c => c.Price >= query.MinPrice);
            }
            else if (query.MaxPrice != null)
            {
                products = products.Where(c => c.Price <= query.MaxPrice);
            }

            // Apply searching term if search term is not null
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                products = products.Include(p => p.Category);
                products = products.Where(c => c.Name.Contains(query.SearchTerm) ||
                                               c.Description.Contains(query.SearchTerm) ||
                                               c.Category!.Name.Contains(query.SearchTerm));
            }

            // Apply sorting if sort by is not null
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                // Split multiple attribute if have
                string[] attributeToSort = query.SortBy.Trim().Split(',');

                // Create an instance of IOrderedQueryable for accessing to ThenBy/ThenByDescending method
                IOrderedQueryable<Product>? sortProduct = null;

                // Iterate through attribute to sort to get the attribute name and sort order, then handle sorting with switch case
                foreach (string attribute in attributeToSort)
                {
                    // Create the whitespace in caseof ex: " name desc"
                    var trimmedAttribute = attribute.Trim();
                    // Get the name of attribute by spliting and choose the index 0
                    var attributeName = trimmedAttribute.Split(' ')[0];
                    // Check if there's sort order by checking array if length > 1 (mean array is holding two strings)
                    var sortOrder = trimmedAttribute.Split(' ').Length > 1
                        ? trimmedAttribute.Split(' ')[1].ToLowerInvariant()
                        : "asc".ToLowerInvariant();
                    // Create the sorting flag by comparision sort order with constant sort name: "desc"
                    var isDescending = sortOrder == "desc";

                    switch (attributeName)
                    {
                        case string s when s == productName:
                            if (sortProduct != null)
                            {
                                sortProduct = isDescending
                                    ? sortProduct.ThenByDescending(c => c.Name)
                                    : sortProduct.ThenBy(c => c.Name);
                                break;
                            }
                            sortProduct = isDescending
                                ? products.OrderByDescending(c => c.Name)
                                : products.OrderBy(c => c.Name);

                            break;

                        case string s when s == productDescription:
                            if (sortProduct != null)
                            {
                                sortProduct = isDescending
                                    ? sortProduct.ThenByDescending(c => c.Description)
                                    : sortProduct.ThenBy(c => c.Description);
                                break;
                            }
                            sortProduct = isDescending
                                ? products.OrderByDescending(c => c.Description)
                                : products.OrderBy(c => c.Description);
                            break;
                    }
                }

                if (sortProduct != null) products = sortProduct;
            }

            return Task.FromResult(PagedResult<Product>.Create(products, query.PageSize, query.PageNumber));
        }
    }
}