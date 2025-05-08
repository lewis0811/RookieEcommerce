using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Features.Orders.Commands;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.Domain.Enums;
using RookieEcommerce.SharedViewModels.OrderDtos;
using Xunit;

namespace RookieEcommerce.UnitTest.Features.Orders.Commands
{
    public class OrderCommandTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IProductVariantRepository> _productVariantRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IEmailService> _emailServiceMock;

        public OrderCommandTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _productVariantRepositoryMock = new Mock<IProductVariantRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _emailServiceMock = new Mock<IEmailService>();
        }

        [Fact]
        public async Task CreateOrderCommand_ValidData_ShouldCreateOrder()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var variantId = Guid.NewGuid();
            
            var command = new CreateOrderCommand
            {
                Email = "customer@example.com",
                CustomerName = "Nguyễn Văn A",
                ShippingPhoneNumber = "0123456789",
                CustomerId = customerId,
                PaymentMethod = PaymentMethod.COD,
                ShippingAddress = new Address
                {
                    StreetAddress = "123 Đường ABC",
                    Ward = "Phường XYZ",
                    District = "Quận 1",
                    CityProvince = "TP. Hồ Chí Minh"
                },
                OrderItems = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto
                    {
                        ProductId = productId,
                        ProductVariantId = variantId,
                        Quantity = 2
                    }
                }
            };

            // Setup product
            var product = Product.Create(
                "Reuzel Matte Clay", 
                "Sáp tạo kiểu tóc matte clay từ Reuzel", 
                249000, 
                Guid.NewGuid(), 
                "Độ bám tốt, lưu hương lâu, dễ gội sạch"
            );
            product.Update(null, null, null, null, 10, 0);

            // Setup variant
            var variant = ProductVariant.Create(
                productId, 
                "Hộp 113g", 
                0, 
                5, 
                "Kích thước"
            );

            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            _productVariantRepositoryMock.Setup(x => x.GetByIdAsync(variantId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(variant);

            var handler = new CreateOrderCommandHandler(
                _unitOfWorkMock.Object,
                _orderRepositoryMock.Object,
                _productVariantRepositoryMock.Object,
                _productRepositoryMock.Object,
                _emailServiceMock.Object
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customerId, result.CustomerId);
            Assert.Equal(PaymentMethod.COD.ToString(), result.PaymentMethod);
            Assert.Equal(498000, result.TotalAmount); // 249000 * 2
            Assert.Single(result.OrderItems);
            
            _orderRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
            _productVariantRepositoryMock.Verify(x => x.UpdateAsync(
                It.Is<ProductVariant>(v => v.StockQuantity == 3), // 5 - 2
                It.IsAny<CancellationToken>()), 
                Times.Once);
            _productRepositoryMock.Verify(x => x.UpdateAsync(
                It.Is<Product>(p => p.TotalQuantity == 8 && p.TotalSell == 2), // 10 - 2, 0 + 2
                It.IsAny<CancellationToken>()), 
                Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _emailServiceMock.Verify(x => x.SendEmailAsync(
                command.Email,
                It.IsAny<string>(),
                It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateOrderCommand_ProductNotFound_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            
            var command = new CreateOrderCommand
            {
                Email = "customer@example.com",
                CustomerName = "Nguyễn Văn A",
                ShippingPhoneNumber = "0123456789",
                CustomerId = customerId,
                PaymentMethod = PaymentMethod.COD,
                ShippingAddress = new Address
                {
                    StreetAddress = "123 Đường ABC",
                    Ward = "Phường XYZ",
                    District = "Quận 1",
                    CityProvince = "TP. Hồ Chí Minh"
                },
                OrderItems = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto
                    {
                        ProductId = productId,
                        Quantity = 1
                    }
                }
            };

            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var handler = new CreateOrderCommandHandler(
                _unitOfWorkMock.Object,
                _orderRepositoryMock.Object,
                _productVariantRepositoryMock.Object,
                _productRepositoryMock.Object,
                _emailServiceMock.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task CreateOrderCommand_InvalidVariant_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var variantId = Guid.NewGuid();
            
            var command = new CreateOrderCommand
            {
                Email = "customer@example.com",
                CustomerName = "Nguyễn Văn A",
                ShippingPhoneNumber = "0123456789",
                CustomerId = customerId,
                PaymentMethod = PaymentMethod.COD,
                ShippingAddress = new Address
                {
                    StreetAddress = "123 Đường ABC",
                    Ward = "Phường XYZ",
                    District = "Quận 1",
                    CityProvince = "TP. Hồ Chí Minh"
                },
                OrderItems = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto
                    {
                        ProductId = productId,
                        ProductVariantId = variantId,
                        Quantity = 1
                    }
                }
            };

            var product = Product.Create(
                "Reuzel Fiber Pomade", 
                "Pomade fiber từ Reuzel", 
                249000, 
                Guid.NewGuid(), 
                "Độ bám tốt, mùi thơm cola, dễ gội sạch"
            );

            var variant = ProductVariant.Create(
                Guid.NewGuid(), // Different product ID
                "Hộp 113g", 
                0, 
                5, 
                "Kích thước"
            );

            _productRepositoryMock.Setup(x => x.GetByIdAsync(productId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            _productVariantRepositoryMock.Setup(x => x.GetByIdAsync(variantId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(variant);

            var handler = new CreateOrderCommandHandler(
                _unitOfWorkMock.Object,
                _orderRepositoryMock.Object,
                _productVariantRepositoryMock.Object,
                _productRepositoryMock.Object,
                _emailServiceMock.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task CreateOrderCommand_MultipleItems_ShouldCalculateTotalCorrectly()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var product1Id = Guid.NewGuid();
            var product2Id = Guid.NewGuid();
            var variant1Id = Guid.NewGuid();
            var variant2Id = Guid.NewGuid();
            
            var command = new CreateOrderCommand
            {
                Email = "customer@example.com",
                CustomerName = "Nguyễn Văn A",
                ShippingPhoneNumber = "0123456789",
                CustomerId = customerId,
                PaymentMethod = PaymentMethod.COD,
                ShippingAddress = new Address
                {
                    StreetAddress = "123 Đường ABC",
                    Ward = "Phường XYZ",
                    District = "Quận 1",
                    CityProvince = "TP. Hồ Chí Minh"
                },
                OrderItems = new List<CreateOrderItemDto>
                {
                    new CreateOrderItemDto
                    {
                        ProductId = product1Id,
                        ProductVariantId = variant1Id,
                        Quantity = 2
                    },
                    new CreateOrderItemDto
                    {
                        ProductId = product2Id,
                        ProductVariantId = variant2Id,
                        Quantity = 1
                    }
                }
            };

            // Setup products
            var product1 = Product.Create(
                "Reuzel Matte Clay", 
                "Sáp tạo kiểu tóc matte clay từ Reuzel", 
                249000, 
                Guid.NewGuid(), 
                "Độ bám tốt, lưu hương lâu, dễ gội sạch"
            );
            product1.Update(null, null, null, null, 10, 0);

            var product2 = Product.Create(
                "Firsthand Water Based Pomade", 
                "Pomade gốc nước FirstHand", 
                459000, 
                Guid.NewGuid(), 
                "Độ bóng vừa phải, giữ nếp cả ngày"
            );
            product2.Update(null, null, null, null, 5, 0);

            // Setup variants
            var variant1 = ProductVariant.Create(
                product1Id, 
                "Hộp 113g", 
                0, 
                5, 
                "Kích thước"
            );

            var variant2 = ProductVariant.Create(
                product2Id, 
                "Hộp 113g", 
                0, 
                3, 
                "Kích thước"
            );

            _productRepositoryMock.Setup(x => x.GetByIdAsync(product1Id, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product1);
            _productRepositoryMock.Setup(x => x.GetByIdAsync(product2Id, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product2);

            _productVariantRepositoryMock.Setup(x => x.GetByIdAsync(variant1Id, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(variant1);
            _productVariantRepositoryMock.Setup(x => x.GetByIdAsync(variant2Id, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(variant2);

            var handler = new CreateOrderCommandHandler(
                _unitOfWorkMock.Object,
                _orderRepositoryMock.Object,
                _productVariantRepositoryMock.Object,
                _productRepositoryMock.Object,
                _emailServiceMock.Object
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(957000, result.TotalAmount); // (249000 * 2) + 459000
            Assert.Equal(2, result.OrderItems.Count);
            
            _orderRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
            _productVariantRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ProductVariant>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _emailServiceMock.Verify(x => x.SendEmailAsync(
                command.Email,
                It.IsAny<string>(),
                It.IsAny<string>()),
                Times.Once);
        }
    }
} 