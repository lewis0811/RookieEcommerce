using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Features.Categories.Queries;
using RookieEcommerce.Domain.Entities;
using System.Linq.Dynamic.Core;

namespace RookieEcommerce.Infrastructure.Persistence
{
    public class CategoryRepository(ApplicationDbContext context) : BaseRepository<Category>(context.Categories), ICategoryRepository
    {
        public Task<PaginationList<Category>> GetPaginated(GetCategoriesQuery query)
        {
            var categories = context.Categories.AsQueryable();

            // Apply include query if includeProperties is not null
            categories = ApplyInclude(query, categories);

            // Apply filtering if ParantCategoryId is not null
            categories = ApplyFilter(query, categories);

            // Apply searching term if SearchTerm is not null
            categories = ApplySearch(query, categories);

            // Apply sorting if SortBy is not null
            categories = ApplySort(query, categories);

            return Task.FromResult(PaginationList<Category>.Create(categories, query.PageSize, query.PageNumber));
        }

        private static IQueryable<Category> ApplySort(GetCategoriesQuery query, IQueryable<Category> categories)
        {
            //if (query.SortBy != null)
            //{
            //    // Split multiple attribute if have
            //    string[] attributeToSort = query.SortBy.Split(',');

            //    // Create an instance of IOrderQueryable for accessing to ThenBy/ThenByDescending method
            //    IOrderedQueryable<Category>? sortCategory = null;

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
            //            case string s when s == categoryName:
            //                if (sortCategory != null)
            //                {
            //                    sortCategory = isDescending
            //                        ? sortCategory.ThenByDescending(c => c.Name)
            //                        : sortCategory.ThenBy(c => c.Name);
            //                }
            //                sortCategory = isDescending
            //                    ? categories.OrderByDescending(c => c.Name)
            //                    : categories.OrderBy(c => c.Name);
            //                break;

            //            case string s when s == categoryDescription:
            //                if (sortCategory != null)
            //                {
            //                    sortCategory = isDescending
            //                        ? sortCategory.ThenByDescending(c => c.Description)
            //                        : sortCategory.ThenBy(c => c.Description);
            //                }
            //                sortCategory = isDescending
            //                    ? categories.OrderByDescending(c => c.Description)
            //                    : categories.OrderBy(c => c.Description);
            //                break;
            //        }
            //    }

            //    if (sortCategory != null) { categories = sortCategory; }
            //}


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