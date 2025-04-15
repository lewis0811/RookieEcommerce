using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.CategoryDtos;

namespace RookieEcommerce.Application.Features.Categories.Commands
{
    public class CreateCategoryCommand : IRequest<CategoryCreateDto>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; set; }
    }

    public class CreateCategoryCommandHandler(IUnitOfWork unitOfWork, ICategoryRepository categoryRepository) : IRequestHandler<CreateCategoryCommand, CategoryCreateDto>
    {
        public async Task<CategoryCreateDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            // Check if category already exist
            var categoryExist = await categoryRepository.AnyAsync(c => c.Name.ToLower().Equals(request.Name.ToLower()), cancellationToken);
            if (categoryExist) { throw new ArgumentException($"Category name {request.Name} already exist."); }

            // Create product instance
            var category = Category.Create(request.Name, request.Description, request.ParentCategoryId);

            // Add product via Repository
            await categoryRepository.AddAsync(category, cancellationToken);

            // Save changes
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to dto and return the result
            return CategoryMapper.CategoryToCategoryCreateDto(category);
        }
    }
}