using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using System.Text.Json.Serialization;

namespace RookieEcommerce.Application.Features.Categories.Commands
{
    public class UpdateCategoryCommand : IRequest
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }

    public class UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, ICategoryRepository categoryRepository) : IRequestHandler<UpdateCategoryCommand>
    {
        public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            // Check if the category exist
            var category = await categoryRepository.GetByIdAsync(request.Id, null, cancellationToken)
                ?? throw new InvalidOperationException($"Product Id {request.Id} not found.");

            // Map request to category
            category.Update(request.Name, request.Description);

            // Update via Repository
            await categoryRepository.UpdateAsync(category, cancellationToken);

            // Save changes via UnitOfWork
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}