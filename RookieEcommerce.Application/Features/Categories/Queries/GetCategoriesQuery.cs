using MediatR;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.SharedViewModels.CategoryDtos;

namespace RookieEcommerce.Application.Features.Categories.Queries
{
    public class GetCategoriesQuery : PaginatedQuery, IRequest<PagedResult<CategoryDetailsDto>>
    {
        public Guid? ParentCategoryId { get; set; }
    }

    public class GetCategoreisQueryHandler(ICategoryRepository categoryRepository) : IRequestHandler<GetCategoriesQuery, PagedResult<CategoryDetailsDto>>
    {
        public async Task<PagedResult<CategoryDetailsDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await categoryRepository.GetPaginated(request);
            //var dtos = categories.Items
            //    .Select(p => new CategoryDetailsDto
            //    {
            //        Id = p.Id,
            //        Name = p.Name,
            //        Description = p.Description,
            //        ParentCategoryId = p.ParentCategoryId,
            //        CreatedDate = p.CreatedDate,
            //        ModifiedDate = p.ModifiedDate,
            //        ParentCategoryName = p.ParentCategory?.Name,
            //        SubCategories = [.. p.SubCategories.Select(sub => new CategorySummaryDto
            //        {
            //            Id = sub.Id,
            //            Name = sub.Name

            //        })],
            //        Products = [..p.Products.Select(prod => new ProductSummaryDto
            //        {
            //            Id = prod.Id,
            //            Name = prod.Name,
            //            Description = prod.Description
            //        })]
            //    })
            //    .ToList();

            var dtos = Mappers.CategoryMapper.CategoryListToDetailsDtoList(categories.Items);

            var pagedResult = new PagedResult<CategoryDetailsDto>(
                dtos,
                categories.TotalCount,
                categories.PageNumber,
                categories.PageSize
                );

            return pagedResult;
        }
    }
}