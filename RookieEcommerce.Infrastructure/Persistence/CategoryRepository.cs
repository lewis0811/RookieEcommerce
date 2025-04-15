using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Features.Categories.Queries;
using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.Infrastructure.Persistence
{
    public class CategoryRepository(ApplicationDbContext context) : BaseRepository<Category>(context.Categories), ICategoryRepository
    {
        private static readonly string categoryName = nameof(Category.Name).ToLowerInvariant();
        private static readonly string categoryDescription = nameof(Category.Description).ToLowerInvariant();

        public Task<PagedResult<Category>> GetPaginated(GetCategoriesQuery query)
        {
            var categories = context.Categories.AsQueryable();

            // Apply include query if includeProperties is not null
            if (!string.IsNullOrEmpty(query.IncludeProperties))
            {
                AddIncludesToQuery(query.IncludeProperties, categories);
            }

            // Apply filtering if ParantCategoryId is not null
            if (query.ParentCategoryId != null)
            {
                categories = categories.Where(c => c.Id == query.ParentCategoryId);
            }

            // Apply searching term if SearchTerm is not null
            if (query.SearchTerm != null)
            {
                categories = categories.Where(c => c.Name.Contains(query.SearchTerm) ||
                                                            c.Description.Contains(query.SearchTerm));
            }

            // Apply sorting if SortBy is not null
            if (query.SortBy != null)
            {
                // Split multiple attribute if have
                string[] attributeToSort = query.SortBy.Split(',');

                // Create an instance of IOrderQueryable for accessing to ThenBy/ThenByDescending method
                IQueryable<Category>? sortCategory = null;

                // Iterate through attribute to sort to get the attribute name and sort order, then handle sorting with switch case
                foreach (var attribute in attributeToSort)
                {
                    // Create the whitespace in caseof ex: " name desc"
                    var trimmedAttribute = attribute.Trim();

                    // Get the name of the attribute by splitting and choose the index 0
                    var attributeName = trimmedAttribute.Split(" ")[0];

                    // Check if there's sort order by checking array if length > 1 (mean array is holding two string)
                    var sortOrder = trimmedAttribute.Split(" ").Length > 1
                        ? trimmedAttribute.Split(" ")[1].ToLowerInvariant()
                        : "asc".ToLowerInvariant();

                    // Create the sorting flag by comparision sort order with constant sort name: "desc
                    var isDescending = sortOrder == "desc";

                    switch (attributeName)
                    {
                        case string s when s == categoryName:
                            if (sortCategory != null)
                            {
                                sortCategory = isDescending
                                    ? sortCategory.OrderByDescending(c => c.Name)
                                    : sortCategory.OrderBy(c => c.Name);
                            }
                            sortCategory = isDescending
                                ? categories.OrderByDescending(c => c.Name)
                                : categories.OrderBy(c => c.Name);
                            break;

                        case string s when s == categoryDescription:
                            if (sortCategory != null)
                            {
                                sortCategory = isDescending
                                    ? sortCategory.OrderByDescending(c => c.Description)
                                    : sortCategory.OrderBy(c => c.Description);
                            }
                            sortCategory = isDescending
                                ? categories.OrderByDescending(c => c.Description)
                                : categories.OrderBy(c => c.Description);
                            break;
                    }
                }

                if (sortCategory != null) { categories = sortCategory; }
            }

            return Task.FromResult(PagedResult<Category>.Create(categories, query.PageSize, query.PageNumber));
        }
    }
}