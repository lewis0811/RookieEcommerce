using MediatR;
using Microsoft.EntityFrameworkCore;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.CategoryDtos;
using System.Text.Json.Serialization;

namespace RookieEcommerce.Application.Features.Categories.Queries
{
    public class GetCategoryByIdQuery : IRequest<CategoryDetailsDto>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public bool IsIncludeItems { get; set; }
    }

    public class GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository) : IRequestHandler<GetCategoryByIdQuery, CategoryDetailsDto>
    {
        public async Task<CategoryDetailsDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            // Check if category exist
            Category? category;

            if (!request.IsIncludeItems)
            {
                category = await categoryRepository.GetByIdAsync(
                    request.Id,
                    null,
                    cancellationToken)
                    ?? throw new InvalidOperationException($"Category Id {request.Id} not found.");
            }
            else
            {
                category = await categoryRepository.GetByIdAsync(
                    request.Id,
                    filter => filter.Include(c => c.Products),
                    cancellationToken)
                    ?? throw new InvalidOperationException($"Category Id {request.Id} not found.");
            }

            // Mapping to dto and return
            return CategoryMapper.CategoryToCategoryDetailsDtoList(category);
        }
    }
}