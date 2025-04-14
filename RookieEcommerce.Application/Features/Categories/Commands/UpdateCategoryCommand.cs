using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.SharedViewModels.CategoryDtos;

namespace RookieEcommerce.Application.Features.Categories.Commands
{
    public class UpdateCategoryCommand : IRequest
    {
        public CategoryUpdateDto Update { get; set; } = new();
    }

    public class UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, ICategoryRepository categoryRepository) : IRequestHandler<UpdateCategoryCommand>
    {
        public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            // Check if the category exist
            var category = await categoryRepository.GetByIdAsync(request.Update.Id, null, cancellationToken)
                ?? throw new InvalidOperationException($"Product Id {request.Update.Id} not found.");

            // Map request to product
            category.Update(request.Update.Name, request.Update.Description);

            // Update via Repository
            await categoryRepository.UpdateAsync(category, cancellationToken);

            // Save changes via UnitOfWork
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}