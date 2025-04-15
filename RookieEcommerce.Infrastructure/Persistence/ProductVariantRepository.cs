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
        //private static readonly string variantName = nameof(ProductVariant.Name).ToLowerInvariant();
        //private static readonly string variantSku = nameof(ProductVariant.Sku).ToLowerInvariant();
        //private static readonly string variantPrice = nameof(ProductVariant.Price).ToLowerInvariant();
        //private static readonly string variantType = nameof(ProductVariant.VariantType).ToLowerInvariant();
        //private static readonly string variantQuantity = nameof(ProductVariant.StockQuantity).ToLowerInvariant();
        //private static readonly string variantCreatedDate = nameof(ProductVariant.CreatedDate).ToLowerInvariant();
        //private static readonly string variantModifiedDate = nameof(ProductVariant.ModifiedDate).ToLowerInvariant();

        public Task<PaginationList<ProductVariant>> GetPaginated(GetProductVariantsQuery query)
        {
            var variants = context.ProductVariants.AsQueryable();

            // Apply include query if includeProperties is not null
            variants = ApplyInclude(query, variants);

            // Apply filtering if ParantCategoryId is not null
            variants = ApplyFilter(query, variants);

            // Apply searching term if SearchTerm is not null
            variants = ApplySearch(query, variants);

            // Apply sorting if SortBy is not null
            variants = ApplySort(query, variants);
            return Task.FromResult(PaginationList<ProductVariant>.Create(variants, query.PageSize, query.PageNumber));
        }

        private static IQueryable<ProductVariant> ApplySort(GetProductVariantsQuery query, IQueryable<ProductVariant> variants)
        {
            //if (query.SortBy != null)
            //{
            //    // Split multiple attribute if have
            //    string[] attributeToSort = query.SortBy.Split(',');

            //    // Create an instance of IOrderQueryable for accessing to ThenBy/ThenByDescending method
            //    IQueryable<ProductVariant>? sortEntity = null;

            //    // Iterate through attribute to sort to get the attribute name and sort order, then handle sorting with switch case
            //    foreach (var attribute in attributeToSort)
            //    {
            //        // Create the whitespace in caseof ex: " name desc"
            //        var trimmedAttribute = attribute.Trim();

            //        // Get the name of the attribute by splitting and choose the index 0
            //        var attributeName = trimmedAttribute.Split(" ")[0];

            //        // Check if there's sort order by checking array if length > 1 (mean array is holding two string)
            //        var sortOrder = trimmedAttribute.Split(" ").Length > 1
            //            ? trimmedAttribute.Split(" ")[1].ToLowerInvariant()
            //            : "asc".ToLowerInvariant();

            //        // Create the sorting flag by comparision sort order with constant sort name: "desc
            //        var isDescending = sortOrder == "desc";

            //        switch (attributeName)
            //        {
            //            case string s when s == variantName:
            //                if (sortEntity != null)
            //                {
            //                    sortEntity = isDescending
            //                        ? sortEntity.OrderByDescending(c => c.Name)
            //                        : sortEntity.OrderBy(c => c.Name);
            //                }
            //                sortEntity = isDescending
            //                    ? variants.OrderByDescending(c => c.Name)
            //                    : variants.OrderBy(c => c.Name);
            //                break;

            //            case string s when s == variantSku:
            //                if (sortEntity != null)
            //                {
            //                    sortEntity = isDescending
            //                        ? sortEntity.OrderByDescending(c => c.Sku)
            //                        : sortEntity.OrderBy(c => c.Sku);
            //                }
            //                sortEntity = isDescending
            //                    ? variants.OrderByDescending(c => c.Sku)
            //                    : variants.OrderBy(c => c.Sku);
            //                break;

            //            case string s when s == variantPrice:
            //                if (sortEntity != null)
            //                {
            //                    sortEntity = isDescending
            //                        ? sortEntity.OrderByDescending(c => c.Price)
            //                        : sortEntity.OrderBy(c => c.Price);
            //                }
            //                sortEntity = isDescending
            //                    ? variants.OrderByDescending(c => c.Price)
            //                    : variants.OrderBy(c => c.Price);
            //                break;

            //            case string s when s == variantType:
            //                if (sortEntity != null)
            //                {
            //                    sortEntity = isDescending
            //                        ? sortEntity.OrderByDescending(c => c.VariantType)
            //                        : sortEntity.OrderBy(c => c.VariantType);
            //                }
            //                sortEntity = isDescending
            //                    ? variants.OrderByDescending(c => c.VariantType)
            //                    : variants.OrderBy(c => c.VariantType);
            //                break;

            //            case string s when s == variantQuantity:
            //                if (sortEntity != null)
            //                {
            //                    sortEntity = isDescending
            //                        ? sortEntity.OrderByDescending(c => c.StockQuantity)
            //                        : sortEntity.OrderBy(c => c.StockQuantity);
            //                }
            //                sortEntity = isDescending
            //                    ? variants.OrderByDescending(c => c.StockQuantity)
            //                    : variants.OrderBy(c => c.StockQuantity);
            //                break;

            //            case string s when s == variantCreatedDate:
            //                if (sortEntity != null)
            //                {
            //                    sortEntity = isDescending
            //                        ? sortEntity.OrderByDescending(c => c.CreatedDate)
            //                        : sortEntity.OrderBy(c => c.CreatedDate);
            //                }
            //                sortEntity = isDescending
            //                    ? variants.OrderByDescending(c => c.CreatedDate)
            //                    : variants.OrderBy(c => c.CreatedDate);
            //                break;

            //            case string s when s == variantModifiedDate:
            //                if (sortEntity != null)
            //                {
            //                    sortEntity = isDescending
            //                        ? sortEntity.OrderByDescending(c => c.ModifiedDate)
            //                        : sortEntity.OrderBy(c => c.ModifiedDate);
            //                }
            //                sortEntity = isDescending
            //                    ? variants.OrderByDescending(c => c.ModifiedDate)
            //                    : variants.OrderBy(c => c.ModifiedDate);
            //                break;
            //        }
            //    }

            //    if (sortEntity != null) { variants = sortEntity; }
            //}
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
                variants = variants.Where(c => c.Id == query.ProductId);
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