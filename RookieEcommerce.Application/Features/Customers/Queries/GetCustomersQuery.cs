using MediatR;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.SharedViewModels.CustomerDtos;


namespace RookieEcommerce.Application.Features.Customers.Queries
{
    public class GetCustomersQuery : PaginatedQuery, IRequest<PaginationList<CustomerDetailsDto>>
    {
    }

    public class GetCustomersQueryHandler(ICustomerRepository customerRepository) : IRequestHandler<GetCustomersQuery, PaginationList<CustomerDetailsDto>>
    {
        public async Task<PaginationList<CustomerDetailsDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        {
            // Get paginated of customers
            var customers = await customerRepository.GetPaginated(request);

            // Map to dto
            var dtos = CustomerMapper.CustomerListToCustomerDetailsDto(customers.Items);

            // Map dto to page result and return
            var pagedResult = new PaginationList<CustomerDetailsDto>(
                dtos,
                customers.TotalCount,
                customers.PageNumber,
                customers.PageSize
                );

            return pagedResult;
        }
    }
}
