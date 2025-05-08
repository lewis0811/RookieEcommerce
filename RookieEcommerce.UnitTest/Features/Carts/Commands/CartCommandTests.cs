using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Features.Carts.Commands;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.CartDtos;
using Xunit;

namespace RookieEcommerce.UnitTest.Features.Carts.Commands
{
    public class CartCommandTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICartRepository> _cartRepositoryMock;
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;

        public CartCommandTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _cartRepositoryMock = new Mock<ICartRepository>();
            _customerRepositoryMock = new Mock<ICustomerRepository>();
        }

        [Fact]
        public async Task CreateCartCommand_ValidData_ShouldCreateCart()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            var command = new CreateCartCommand
            {
                CustomerId = customerId
            };

            var customer = Customer.Create(
                "Dân",
                "Lương",
                "customer@example.com",
                "0123456789"
            );

            _customerRepositoryMock.Setup(x => x.GetByIdAsync(customerId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);


            var handler = new CreateCartCommandHandler(
                _unitOfWorkMock.Object,
                _cartRepositoryMock.Object,
                _customerRepositoryMock.Object
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customerId, result.CustomerId);

            _cartRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateCartCommand_CustomerNotFound_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var command = new CreateCartCommand
            {
                CustomerId = customerId
            };

            _customerRepositoryMock.Setup(x => x.GetByIdAsync(customerId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Customer?)null);

            var handler = new CreateCartCommandHandler(
                _unitOfWorkMock.Object,
                _cartRepositoryMock.Object,
                _customerRepositoryMock.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task CreateCartCommand_ProductNotFound_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            var command = new CreateCartCommand
            {
                CustomerId = customerId
            };

            var customer = Customer.Create(
                "Dân",
                "Lương",
                "customer@example.com",
                "0123456789"
            );

            _customerRepositoryMock.Setup(x => x.GetByIdAsync(customerId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            var handler = new CreateCartCommandHandler(
                _unitOfWorkMock.Object,
                _cartRepositoryMock.Object,
                _customerRepositoryMock.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
} 