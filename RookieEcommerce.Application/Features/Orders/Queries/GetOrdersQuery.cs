using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.OrderDtos;
using System.Text.Json.Serialization;

namespace RookieEcommerce.Application.Features.Orders.Queries
{
    public class GetOrdersQuery : IRequest<OrderDetailsDto>
    {
        [JsonIgnore]
        public Guid CustomerId { get; set; }

        public bool IsIncludeItems { get; set; }
    }

    public class GetOrdersQueryHandler(IOrderRepository orderRepository) : IRequestHandler<GetOrdersQuery, OrderDetailsDto>
    {
        public async Task<OrderDetailsDto> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            // Check if include items
            Func<IQueryable<Order>, IIncludableQueryable<Order, object>>? includeExpression = null;
            if ( request.IsIncludeItems)
            {
                includeExpression = query => query
                    .Include(c => c.OrderItems!)
                        .ThenInclude(c => c.ProductVariant!);
            }

            // Get order via repository
            var order = await orderRepository.GetByAttributeAsync(c => c.CustomerId == request.CustomerId,
                includeExpression,
                cancellationToken) 
                ?? throw new InvalidOperationException($"Customer Id {request.CustomerId} doesn't have any order.");

            // Mapping to dto and return
            return OrderMapper.OrderToOrderDetailsDto(order);
        }
    }
}