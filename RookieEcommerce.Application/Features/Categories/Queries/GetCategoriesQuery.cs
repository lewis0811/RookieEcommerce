using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.CategoryDtos;

namespace RookieEcommerce.Application.Features.Categories.Queries
{
    public class GetCategoriesQuery : PaginatedQuery, IRequest<List<CategoryDetailsDto>>
    {
        public Guid? ParentCategoryId { get; set; }
        public bool IsIncludeItems { get; set; }
    }

    public class GetCategoriesQueryHandler(ICategoryRepository categoryRepository) : IRequestHandler<GetCategoriesQuery, List<CategoryDetailsDto>>
    {
        public async Task<List<CategoryDetailsDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            Func<IQueryable<Category>, IIncludableQueryable<Category, object>>? query = null;
            // Check if IsIncludeItems is true, then include products and images
            if (request.IsIncludeItems)
            {
                query = filter => filter.Include(c => c.Products)
                    .ThenInclude(c => c.Images);
            }
            // Get paginated of categories
            var categories = await categoryRepository.ListAllAsync(null, query, cancellationToken);
            // Map to dto
            var dtos = Mappers.CategoryMapper.CategoryListToCategoryDetailsDtoList(categories);
            return dtos;
        }
    }
}