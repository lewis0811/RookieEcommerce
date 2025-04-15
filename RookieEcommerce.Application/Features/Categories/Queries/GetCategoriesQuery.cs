using MediatR;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.SharedViewModels.CategoryDtos;

namespace RookieEcommerce.Application.Features.Categories.Queries
{
    public class GetCategoriesQuery : PaginatedQuery, IRequest<PaginationList<CategoryDetailsDto>>
    {
        public Guid? ParentCategoryId { get; set; }
    }

    public class GetCategoreisQueryHandler(ICategoryRepository categoryRepository) : IRequestHandler<GetCategoriesQuery, PaginationList<CategoryDetailsDto>>
    {
        public async Task<PaginationList<CategoryDetailsDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            // Get paginated of categories
            var categories = await categoryRepository.GetPaginated(request);
            
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