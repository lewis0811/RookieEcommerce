using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.CategoryDtos;

namespace RookieEcommerce.Application.Features.Categories.Commands
{
    public class CreateCategoryCommand : IRequest<CategorySummaryDto>
    {
        public CategoryCreateDto Create { get; set; } = new();
    }

    public class CreateCategoryCommandHandler(IUnitOfWork unitOfWork, ICategoryRepository categoryRepository) : IRequestHandler<CreateCategoryCommand, CategorySummaryDto>
    {
        public async Task<CategorySummaryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            // Check if category already exist
            var categoryExist = await categoryRepository.AnyAsync(c => c.Name.ToLowerInvariant().Equals(request.Create.Name.ToLowerInvariant()), cancellationToken);
            if (categoryExist) { throw new ArgumentException($"Category name {request.Create.Name} already exist."); }
            // Create product instance
            var category = Category.Create(request.Create.Name, request.Create.Description, request.Create.ParentCategoryId);

            // Add product via Repository
            await categoryRepository.AddAsync(category, cancellationToken);

            // Save changes
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to dto and return the result
            return new CategorySummaryDto { Id = category.Id, Name = category.Name, ParentCategoryId = category.ParentCategoryId };
        }
    }
}