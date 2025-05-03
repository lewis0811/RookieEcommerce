using MediatR;
using Microsoft.AspNetCore.Mvc;
using RookieEcommerce.Application.Features.Orders.Commands;
using RookieEcommerce.Application.Features.Orders.Queries;

namespace RookieEcommerce.Api.Controllers
{
    [Route("api/v{version:apiVersion}/orders")]
    [ApiController]
    public class OrdersController(IMediator mediator) : ControllerBase
    {
        [HttpGet("customer/{customer-id}")]
        public async Task<IActionResult> GetOrders([FromRoute(Name = "customer-id")] Guid customerId, bool isIncludeItems, CancellationToken cancellationToken)
        {
            var query = new GetOrdersQuery
            {
                CustomerId = customerId,
                IsIncludeItems = isIncludeItems
            };
            var result = await mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(AddOrder), result);
        }

        [HttpPut("{order-id}")]
        public async Task<IActionResult> UpdateOrder([FromRoute(Name = "order-id")] Guid orderId, UpdateOrderCommand command, CancellationToken cancellationToken)
        {
            command.OrderId = orderId;
            await mediator.Send(command, cancellationToken);

            return Ok();
        }

        [HttpDelete("{order-id}")]
        public async Task<IActionResult> DeleteOrder([FromRoute(Name = "order-id")] Guid orderId, CancellationToken cancellationToken)
        {
            var command = new DeleteOrderCommand { OrderId = orderId };
            await mediator.Send(command, cancellationToken);

            return NoContent();
        }
    }
}