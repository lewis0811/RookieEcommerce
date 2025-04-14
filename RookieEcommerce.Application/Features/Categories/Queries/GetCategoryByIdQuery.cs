using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.SharedViewModels.CategoryDtos;
using RookieEcommerce.SharedViewModels.ProductDtos;
using System.Text.Json.Serialization;

namespace RookieEcommerce.Application.Features.Categories.Queries
{
    public class GetCategoryByIdQuery : IRequest<CategoryDetailsDto>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string? IncludeProperties { get; set; }
    }

    public class GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository) : IRequestHandler<GetCategoryByIdQuery, CategoryDetailsDto>
    {
        public async Task<CategoryDetailsDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            // Check if category exist
            var category = await categoryRepository.GetByIdAsync(request.Id, request.IncludeProperties, cancellationToken)
                ?? throw new InvalidOperationException($"Category Id {request.Id} not found.");

            // Mapping to dto and return
            return new CategoryDetailsDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategoryName = category.ParentCategory?.Name,
                CreatedDate = category.CreatedDate,
                ModifiedDate = category.ModifiedDate,
                SubCategories = [..category.SubCategories.Select(sub => new CategorySummaryDto
                {
                    Id = sub.Id,
                    Name = sub.Name,
                    ParentCategoryId = sub.ParentCategoryId,
                    ParentCategoryName = sub.ParentCategory?.Name
                })],
                Products = [..category.Products.Select(p => new ProductSummaryDto {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                })]
            };
        }
    }
}