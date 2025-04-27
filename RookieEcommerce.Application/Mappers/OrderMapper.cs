using Riok.Mapperly.Abstractions;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.OrderDtos;

namespace RookieEcommerce.Application.Mappers
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None, EnumMappingStrategy = EnumMappingStrategy.ByValue)]
    public static partial class OrderMapper
    {
        public static partial OrderDetailsDto OrderToOrderDetailsDto(Order order);

        public static partial OrderCreateDto OrderToOrderCreateDto(Order order);

        public static partial ICollection<OrderItem> ListOrderItemDtoToListOrderItem(List<CreateOrderItemDto> orderItems);

        public static partial CreateOrderDto CreateOrderDtoToOrder(CreateOrderDto dto);
    }
}