using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.SharedViewModels.CustomerDtos;

namespace RookieEcommerce.Application.Features.Customers.Queries
{
    public class GetCustomerByIdQuery : IRequest<CustomerDetailsDto>
    {
        public Guid Id { get; set; }
    }

    public class GetCustomerByIdQueryHandler(ICustomerRepository customerRepository) : IRequestHandler<GetCustomerByIdQuery, CustomerDetailsDto>
    {
        public async Task<CustomerDetailsDto> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var customer = await customerRepository
               .GetByIdAsync(request.Id,
               null,
               cancellationToken)
               ?? throw new InvalidOperationException($"Customer Id {request.Id} not found.");

            // Map to dto and return
            return CustomerMapper.CustomerToCustomerDetailsDto(customer);
        }
    }
}
