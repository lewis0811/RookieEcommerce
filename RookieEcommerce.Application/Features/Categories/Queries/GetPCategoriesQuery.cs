using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.CategoryDtos;

namespace RookieEcommerce.Application.Features.Categories.Queries
{
    public class GetPCategoriesQuery : PaginatedQuery, IRequest<PaginationList<CategoryDetailsDto>>
    {
        public Guid? ParentCategoryId { get; set; }
        public bool IsIncludeItems { get; set; }
    }

    public class GetPcategoriesQueryHandler(ICategoryRepository categoryRepository) : IRequestHandler<GetPCategoriesQuery, PaginationList<CategoryDetailsDto>>
    {
        public async Task<PaginationList<CategoryDetailsDto>> Handle(GetPCategoriesQuery request, CancellationToken cancellationToken)
        {
            Func<IQueryable<Category>, IIncludableQueryable<Category, object>>? query = null;

            // Check if IsIncludeItems is true, then include products and images
            if (request.IsIncludeItems)
            {
                query = filter => filter.Include(c => c.Products)
                    .ThenInclude(c => c.Images);
            }

            // Get paginated of categories
            var categories = await categoryRepository.GetPaginated(request,
                query);

            // Map to dto
            var dtos = Mappers.CategoryMapper.CategoryListToCategoryDetailsDtoList(categories.Items);

            // Map dto to page result and return
            var pagedResult = new PaginationList<CategoryDetailsDto>(
                dtos,
                categories.TotalCount,
                categories.PageNumber,
                categories.PageSize
                );

            return pagedResult;
        }
    }
}