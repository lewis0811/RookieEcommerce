using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.SharedViewModels.CategoryDtos;
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
            return CategoryMapper.CategoryToCategoryDetailsDtoList(category);
        }
    }
}