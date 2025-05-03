﻿namespace RookieEcommerce.SharedViewModels.OrderDtos
{
    public class CreateOrderItemDto
    {
        public Guid ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}