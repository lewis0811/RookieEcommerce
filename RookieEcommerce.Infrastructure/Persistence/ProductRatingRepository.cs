using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Features.ProductRatings.Queries;
using RookieEcommerce.Domain.Entities;
using System.Linq.Dynamic.Core;

namespace RookieEcommerce.Infrastructure.Persistence
{
    public class ProductRatingRepository(ApplicationDbContext context) : BaseRepository<ProductRating>(context.ProductRatings), IProductRatingRepository
    {
        public Task<PaginationList<ProductRating>> GetPaginated(GetProductRatingQuery query)
        {
            var instance = context.ProductRatings.AsQueryable();

            // Apply include query if includeProperties is not null
            instance = ApplyInclude(query, instance);

            // Apply filter if it is not null
            instance = ApplyFilter(query, instance);

            // Apply searching term if search term is not null
            instance = ApplySearch(query, instance);

            // Apply sorting if sort by is not null
            instance = ApplySort(query, instance);

            return Task.FromResult(PaginationList<ProductRating>.Create(instance, query.PageSize, query.PageNumber));
        }

        private static IQueryable<ProductRating> ApplySort(GetProductRatingQuery query, IQueryable<ProductRating> instance)
        {
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                instance = instance.OrderBy(query.SortBy);
            }

            return instance;
        }

        private static IQueryable<ProductRating> ApplySearch(GetProductRatingQuery query, IQueryable<ProductRating> instance)
        {
            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                instance = instance.Where(c => c.Comment != null && c.Comment.Contains(query.SearchTerm, StringComparison.CurrentCultureIgnoreCase));
            }

            return instance;
        }

        private static IQueryable<ProductRating> ApplyFilter(GetProductRatingQuery query, IQueryable<ProductRating> instance)
        {
            if (query.ProductId != null)
            {
                instance = instance.Where(c => c.ProductId.Equals(query.ProductId));
            }

            if (query.CustomerId != null)
            {
                instance = instance.Where(c => c.CustomerId.Equals(query.CustomerId));
            }

            if (query.MinRatingValue != null)
            {
                instance = instance.Where(c => c.RatingValue >= query.MinRatingValue);
            }

            if (query.MaxRatingValue != null)
            {
                instance = instance.Where(c => c.RatingValue <= query.MaxRatingValue);
            }

            return instance;
        }

        private static IQueryable<ProductRating> ApplyInclude(GetProductRatingQuery query, IQueryable<ProductRating> instance)
        {
            if (query.IncludeProperties != null)
            {
                instance = AddIncludesToQuery(query.IncludeProperties, instance);
            }

            return instance;
        }
    }
}