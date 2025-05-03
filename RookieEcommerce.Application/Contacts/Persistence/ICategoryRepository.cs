using Microsoft.EntityFrameworkCore.Query;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Features.Categories.Queries;
using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.Application.Contacts.Persistence
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<PaginationList<Category>> GetPaginated(GetPCategoriesQuery query, Func<IQueryable<Category>, IIncludableQueryable<Category, object>>? include = null);
    }
}