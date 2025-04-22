using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Features.Categories.Queries;
using RookieEcommerce.Domain.Entities;
using System.Linq.Dynamic.Core;

namespace RookieEcommerce.Infrastructure.Persistence
{
    public class CategoryRepository(ApplicationDbContext context) : BaseRepository<Category>(context.Categories), ICategoryRepository
    {
        public Task<PaginationList<Category>> GetPaginated(GetCategoriesQuery query, Func<IQueryable<Category>, IIncludableQueryable<Category, object>>? include = null)
        {
            var categories = context.Categories.AsQueryable();

            // Apply include query if includeProperties is not null
            categories = ApplyInclude2(include, categories);
            categories = ApplyInclude(query, categories);

            // Apply filtering if ParantCategoryId is not null
            categories = ApplyFilter(query, categories);

            // Apply searching term if SearchTerm is not null
            categories = ApplySearch(query, categories);

            // Apply sorting if SortBy is not null
            categories = ApplySort(query, categories);

            return Task.FromResult(PaginationList<Category>.Create(categories, query.PageSize, query.PageNumber));
        }

        private static IQueryable<Category> ApplyInclude2(Func<IQueryable<Category>, IIncludableQueryable<Category, object>>? include, IQueryable<Category> categories)
        {
            if (include != null)
            {
                categories = include(categories);
            }

            return categories;
        }

        private static IQueryable<Category> ApplySort(GetCategoriesQuery query, IQueryable<Category> categories)
        {
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                categories = categories.OrderBy(query.SortBy);
            }

            return categories;
        }

        private static IQueryable<Category> ApplySearch(GetCategoriesQuery query, IQueryable<Category> categories)
        {
            if (query.SearchTerm != null)
            {
                categories = categories.Where(c => c.Name.Contains(query.SearchTerm) ||
                                                            c.Description.Contains(query.SearchTerm));
            }

            return categories;
        }

        private static IQueryable<Category> ApplyFilter(GetCategoriesQuery query, IQueryable<Category> categories)
        {
            if (query.ParentCategoryId != null)
            {
                categories = categories.Where(c => c.Id == query.ParentCategoryId);
            }

            return categories;
        }

        private static IQueryable<Category> ApplyInclude(GetCategoriesQuery query, IQueryable<Category> categories)
        {
            if (!string.IsNullOrEmpty(query.IncludeProperties))
            {
                categories = AddIncludesToQuery(query.IncludeProperties, categories);
            }

            return categories;
        }
    }
}