using MediatR;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Mappers;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.Domain.Enums;
using RookieEcommerce.SharedViewModels.OrderDtos;
using System.Globalization;
using System.Text;

namespace RookieEcommerce.Application.Features.Orders.Commands
{
    public class CreateOrderCommand : IRequest<OrderCreateDto>
    {
        public string Email { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string ShippingPhoneNumber { get; set; } = "";
        public Guid CustomerId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public Address ShippingAddress { get; set; } = new Address();
        public List<CreateOrderItemDto> OrderItems { get; set; } = [];
    }

    public class CreateOrderCommandHandler(IUnitOfWork unitOfWork,
        IOrderRepository orderRepository,
        IProductVariantRepository productVariantRepository,
        IProductRepository productRepository,
        IEmailService emailService)
        : IRequestHandler<CreateOrderCommand, OrderCreateDto>
    {
        public async Task<OrderCreateDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            decimal calculatedTotalAmount = 0;
            var orderItemsEntities = new List<OrderItem>();
            var orderItemDetailsForEmail = new List<(string ProductName, string? VariantInfo, int Quantity, decimal UnitPrice, decimal LineTotal)>();

            // ---  Calculate Total and Prepare OrderItem Entities (with Variant Logic) ---
            foreach (var itemDto in request.OrderItems)
            {
                decimal unitPrice;
                Guid productId = itemDto.ProductId;
                Guid? productVariantId = itemDto.ProductVariantId;
                int quantity = itemDto.Quantity;
                string productName = "";
                string? variantInfo = null;

                // Use Base Product
                var product = await productRepository.GetByIdAsync(productId, null, cancellationToken)
                    ?? throw new InvalidOperationException($"Product with ID {productId} not found.");
                productName = product.Name; // Get name from product

                // Determine Price based on Variant ID
                if (productVariantId.HasValue && productVariantId.Value != Guid.Empty)
                {
                    // Use Product Variant
                    var variant = await productVariantRepository.GetByIdAsync(productVariantId.Value, null, cancellationToken);

                    // Check if variant exists AND belongs to the base product
                    if (variant == null || variant.ProductId != productId)
                    {
                        throw new InvalidOperationException($"Invalid Product Variant ID {productVariantId.Value} for Product ID {productId}.");
                    }
                    unitPrice = variant.Price; // Get price from variant
                    variantInfo = string.Join(", ", $"{variant.VariantType}: {variant.Name}");
                }
                else
                {
                    unitPrice = product.Price; // Get price from product
                }

                // Calculate line total
                decimal lineTotal = unitPrice * quantity;
                calculatedTotalAmount += lineTotal;

                // Store details for email generation
                orderItemDetailsForEmail.Add((productName, variantInfo, quantity, unitPrice, lineTotal));

                // Create the domain entity for the order item
                orderItemsEntities.Add(new OrderItem
                {
                    ProductId = productId,
                    ProductVariantId = productVariantId,
                    Quantity = quantity,
                    Price = unitPrice
                });
            }

            // Create the Order
            var order = Order.Create(request.CustomerId.ToString(), calculatedTotalAmount, request.PaymentMethod, request.ShippingAddress, orderItemsEntities);

            // Add to repository
            await orderRepository.AddAsync(order, cancellationToken);

            // Save changes
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Send Order Confirmation Email
            string subject = $"Order Confirmation - #{order.Id}";
            string htmlMessage = GenerateOrderConfirmationHtml(order, orderItemDetailsForEmail, request.Email, request.CustomerName, request.ShippingPhoneNumber);
            await emailService.SendEmailAsync(request.Email, subject, htmlMessage);

            // Map to DTO and return
            var orderDto = OrderMapper.OrderToOrderCreateDto(order);
            return orderDto;
        }

        private static string GenerateOrderConfirmationHtml(Order order, List<(string ProductName, string? VariantInfo, int Quantity, decimal UnitPrice, decimal LineTotal)> itemDetails, string customerEmail, string customerName, string shippingPhoneNumber)
        {
            var culture = new CultureInfo("vi-VN");
            var html = new StringBuilder();

            html.Append("<!DOCTYPE html>");
            html.Append("<html lang=\"vi\">");

            html.Append("<head><meta charset=\"UTF-8\"><title>Xác nhận đơn hàng</title>");

            html.Append("<style>");
            html.Append("body { font-family: sans-serif; line-height: 1.6; color: #333; }");
            html.Append(".container { max-width: 600px; margin: 20px auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px; }");
            html.Append("h1, h2 { color: #0056b3; }");
            html.Append("table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }");
            html.Append("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
            html.Append("th { background-color: #f2f2f2; }");
            html.Append(".total { font-weight: bold; text-align: right; }");
            html.Append(".footer { margin-top: 20px; font-size: 0.9em; color: #777; text-align: center; }");
            html.Append("</style>");

            html.Append("</head>");

            html.Append("<body>");
            html.Append("<div class=\"container\">");
            html.Append($"<h1>Cảm ơn bạn đã đặt hàng, {customerEmail}!</h1>"); // Vietnamese greeting
            html.Append($"<p>Đơn hàng <strong>#{order.Id}</strong> của bạn đặt vào ngày {order.CreatedDate.ToString("dd/MM/yyyy HH:mm", culture)} đã được xác nhận.</p>"); // Vietnamese confirmation

            html.Append("<h2>Địa chỉ giao hàng</h2>");

            html.Append("<p>");
            if (!string.IsNullOrEmpty(customerName)) html.Append($"<strong>{customerName}</strong><br>");
            if (!string.IsNullOrEmpty(shippingPhoneNumber)) html.Append($"<strong>{shippingPhoneNumber}</strong><br>");
            if (!string.IsNullOrEmpty(order.ShippingAddress.StreetAddress)) html.Append($"{order.ShippingAddress.StreetAddress}<br>");
            if (!string.IsNullOrEmpty(order.ShippingAddress.Ward)) html.Append($"{order.ShippingAddress.Ward}<br>");
            if (!string.IsNullOrEmpty(order.ShippingAddress.District)) html.Append($"{order.ShippingAddress.District}<br>");
            if (!string.IsNullOrEmpty(order.ShippingAddress.CityProvince)) html.Append($"{order.ShippingAddress.CityProvince}<br>");
            html.Append("</p>");

            html.Append("<h2>Chi tiết đơn hàng</h2>");
            html.Append("<table>");
            html.Append("<thead><tr><th>Sản phẩm</th><th>Số lượng</th><th>Đơn giá</th><th>Thành tiền</th></tr></thead>");
            html.Append("<tbody>");

            foreach (var item in itemDetails)
            {
                html.Append("<tr>");
                html.Append($"<td>{item.ProductName}");
                if (!string.IsNullOrEmpty(item.VariantInfo))
                {
                    html.Append($"<br><small>({item.VariantInfo})</small>");
                }
                html.Append("</td>");
                html.Append($"<td>{item.Quantity}</td>");
                html.Append($"<td>{item.UnitPrice.ToString("C", culture)}</td>");
                html.Append($"<td>{item.LineTotal.ToString("C", culture)}</td>");
                html.Append("</tr>");
            }

            html.Append("</tbody>");
            html.Append("<tfoot>");
            html.Append($"<tr><td colspan=\"3\" class=\"total\">Tổng cộng:</td><td class=\"total\">{order.TotalAmount.ToString("C", culture)}</td></tr>");
            html.Append("</tfoot>");
            html.Append("</table>");

            html.Append($"<p>Phương thức thanh toán: {order.PaymentMethod}</p>");

            html.Append("<div class=\"footer\">");
            html.Append("<p>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với đội ngũ hỗ trợ của chúng tôi.</p>");
            html.Append("<p>&copy; " + DateTime.Now.Year + " NashLux</p>");
            html.Append("</div>");

            html.Append("</div>");
            html.Append("</body>");
            html.Append("</html>");

            return html.ToString();
        }
    }
}