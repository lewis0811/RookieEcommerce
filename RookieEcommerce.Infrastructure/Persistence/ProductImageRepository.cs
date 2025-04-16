using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Features.ProductImages.Queries;
using RookieEcommerce.Domain.Entities;
using System.Linq.Dynamic.Core;

namespace RookieEcommerce.Infrastructure.Persistence
{
    public class ProductImageRepository(ApplicationDbContext context) : BaseRepository<ProductImage>(context.ProductImages), IProductImageRepository
    {
        public Task<PaginationList<ProductImage>> GetPaginated(GetProductImagesQuery query)
        {
            var instance = context.ProductImages.AsQueryable();

            // Apply include query if includeProperties is not null
            instance = ApplyInclude(query, instance);

            // Apply filter if it is not null
            instance = ApplyFilter(query, instance);

            // Apply searching term if search term is not null
            instance = ApplySearch(query, instance);

            // Apply sorting if sort by is not null
            instance = ApplySort(query, instance);

            return Task.FromResult(PaginationList<ProductImage>.Create(instance, query.PageSize, query.PageNumber));
        }

        private static IQueryable<ProductImage> ApplySort(GetProductImagesQuery query, IQueryable<ProductImage> instance)
        {
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                instance = instance.OrderBy(query.SortBy);
            }

            return instance;
        }

        private static IQueryable<ProductImage> ApplySearch(GetProductImagesQuery query, IQueryable<ProductImage> instance)
        {
            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                instance = instance.Where(c => c.AltText != null && c.AltText.Contains(query.SearchTerm, StringComparison.CurrentCultureIgnoreCase));
            }

            return instance;
        }

        private static IQueryable<ProductImage> ApplyFilter(GetProductImagesQuery query, IQueryable<ProductImage> instance)
        {
            if (query.ProductId != null)
            {
                instance = instance.Where(c => c.ProductId.Equals(query.ProductId));
            }

            if (query.IsPrimary != null)
            {
                instance = instance.Where(c => c.IsPrimary.Equals(query.IsPrimary));
            }

            return instance;
        }

        private static IQueryable<ProductImage> ApplyInclude(GetProductImagesQuery query, IQueryable<ProductImage> instance)
        {
            if (query.IncludeProperties != null)
            {
                instance = AddIncludesToQuery(query.IncludeProperties, instance);
            }

            return instance;
        }
    }
}