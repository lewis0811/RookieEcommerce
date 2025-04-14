using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using System.Text.Json.Serialization;

namespace RookieEcommerce.Application.Features.Categories.Commands
{
    public class DeleteCategoryCommand : IRequest
    {
        [JsonIgnore]
        public Guid Id { get; set; }
    }

    public class DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, ICategoryRepository categoryRepository) : IRequestHandler<DeleteCategoryCommand>
    {
        public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            // Check if the category exist
            var product = await categoryRepository.GetByIdAsync(request.Id, null, cancellationToken)
                ?? throw new InvalidOperationException($"Category Id {request.Id} not found.");

            // Delete product via Repository
            await categoryRepository.DeleteAsync(product, cancellationToken);

            // Save changes via UnitOfWork
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}