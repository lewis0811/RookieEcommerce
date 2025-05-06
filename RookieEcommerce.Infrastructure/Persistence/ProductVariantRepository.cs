using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Features.ProductVariants.Queries;
using RookieEcommerce.Domain.Entities;
using System.Linq.Dynamic.Core;

namespace RookieEcommerce.Infrastructure.Persistence
{
    public class ProductVariantRepository(ApplicationDbContext context)
        : BaseRepository<ProductVariant>(context.ProductVariants), IProductVariantRepository
    {
        public Task<PaginationList<ProductVariant>> GetPaginated(GetProductVariantsQuery query)
        {
            var variants = context.ProductVariants.AsQueryable();

            // Apply include query if includeProperties is not null
            variants = ApplyInclude(query, variants);

            //// Apply filtering if ParantCategoryId is not null
            variants = ApplyFilter(query, variants);

            // Apply searching term if SearchTerm is not null
            variants = ApplySearch(query, variants);

            // Apply sorting if SortBy is not null
            variants = ApplySort(query, variants);

            return Task.FromResult(PaginationList<ProductVariant>.Create(variants, query.PageSize, query.PageNumber));
        }

        private static IQueryable<ProductVariant> ApplySort(GetProductVariantsQuery query, IQueryable<ProductVariant> variants)
        {
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                variants = variants.OrderBy(query.SortBy);
            }

            return variants;
        }

        private static IQueryable<ProductVariant> ApplySearch(GetProductVariantsQuery query, IQueryable<ProductVariant> variants)
        {
            if (query.SearchTerm != null)
            {
                variants = variants.Where(c => c.Name.Contains(query.SearchTerm) ||
                                                            c.Sku.Contains(query.SearchTerm));
            }

            return variants;
        }

        private static IQueryable<ProductVariant> ApplyFilter(GetProductVariantsQuery query, IQueryable<ProductVariant> variants)
        {
            if (query.ProductId != null)
            {
                variants = variants.Where(c => c.ProductId == query.ProductId);
            }
            if (query.MaxPrice != null)
            {
                variants = variants.Where(c => c.Price <= query.MaxPrice);
            }
            if (query.MinPrice != null)
            {
                variants = variants.Where(c => c.Price >= query.MinPrice);
            }
            if (query.MinPrice != null && query.MaxPrice != null)
            {
                variants = variants.Where(c => c.Price >= query.MinPrice && c.Price <= query.MaxPrice);
            }

            return variants;
        }

        private static IQueryable<ProductVariant> ApplyInclude(GetProductVariantsQuery query, IQueryable<ProductVariant> variants)
        {
            if (!string.IsNullOrEmpty(query.IncludeProperties))
            {
                variants = AddIncludesToQuery(query.IncludeProperties, variants);
            }

            return variants;
        }
    }
}