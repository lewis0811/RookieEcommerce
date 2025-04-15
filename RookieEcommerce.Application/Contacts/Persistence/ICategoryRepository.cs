using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Features.Categories.Queries;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels;

namespace RookieEcommerce.Application.Contacts.Persistence
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<PaginationList<Category>> GetPaginated(GetCategoriesQuery query);
    }
}