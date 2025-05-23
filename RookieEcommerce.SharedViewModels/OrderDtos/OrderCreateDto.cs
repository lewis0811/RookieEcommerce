﻿using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.SharedViewModels.OrderDtos
{
    public class OrderCreateDto : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;

        public Address ShippingAddress { get; set; } = new();
        public List<CreateOrderItemDto> OrderItems { get; set; } = [];
    }
}