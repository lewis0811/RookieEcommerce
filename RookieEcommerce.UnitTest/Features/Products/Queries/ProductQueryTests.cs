using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Features.Products.Queries;
using RookieEcommerce.Domain.Entities;
using Xunit;

namespace RookieEcommerce.UnitTest.Features.Products.Queries
{
    public class ProductQueryTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;

        public ProductQueryTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
        }

        [Fact]
        public async Task GetProductsQuery_ValidData_ShouldReturnProducts()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var query = new GetProductsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                CategoryId = categoryId,
                MaxPrice = 500000,
                MinPrice = 200000,
                IsIncludeItems = true
            };

            var pomadeCategory = new Category { Id = categoryId, Name = "Pomade" };

            var expectedProducts = new PaginationList<Product>(
                new List<Product>
                {
                    Product.Create("Reuzel Matte Clay", "Sáp tạo kiểu tóc matte clay từ Reuzel", 249000, categoryId, "Độ bám tốt, lưu hương lâu, dễ gội sạch"),
                    Product.Create("Reuzel Fiber Pomade", "Pomade fiber từ Reuzel", 249000, categoryId, "Độ bám tốt, mùi thơm cola, dễ gội sạch"),
                    Product.Create("Powermade - By Vilain", "Pomade cao cấp từ By Vilain", 399000, categoryId, "Độ bóng cao, hương thơm dễ chịu, giữ nếp cả ngày"),
                    Product.Create("Original Matte - Tough & Tumble", "Sáp vuốt tóc matte từ Tough & Tumble", 399000, categoryId, "Độ giữ nếp cao, không bóng, dễ tạo kiểu"),
                    Product.Create("Solid & Shine - Tough & Tumble", "Pomade bóng từ Tough & Tumble", 399000, categoryId, "Độ bóng cao, giữ nếp tốt, hương thơm dễ chịu")
                },
                5, 1, 10
            );

            foreach (var product in expectedProducts.Items)
            {
                product.Category = pomadeCategory;
            }

            _productRepositoryMock.Setup(x => x.GetPaginated(
                    It.IsAny<GetProductsQuery>(),
                    It.IsAny<Func<IQueryable<Product>, IIncludableQueryable<Product, object>>>()))
                .ReturnsAsync(expectedProducts);

            var handler = new GetProductsQueryHandler(_productRepositoryMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProducts.Items.Count, result.Items.Count);
            Assert.Equal(expectedProducts.TotalCount, result.TotalCount);
            Assert.Equal(expectedProducts.PageNumber, result.PageNumber);
            Assert.Equal(expectedProducts.PageSize, result.PageSize);
        }

        [Fact]
        public async Task GetProductsQuery_NoCategoryFilter_ShouldReturnAllProducts()
        {
            // Arrange
            var pomadeCategoryId = Guid.NewGuid();
            var waxCategoryId = Guid.NewGuid();
            
            var query = new GetProductsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                CategoryId = null,
                IsIncludeItems = true
            };

            var pomadeCategory = new Category { Id = pomadeCategoryId, Name = "Pomade" };
            var waxCategory = new Category { Id = waxCategoryId, Name = "Sáp Vuốt Tóc Nam" };

            var expectedProducts = new PaginationList<Product>(
                new List<Product>
                {
                    Product.Create("Reuzel Matte Clay", "Sáp tạo kiểu tóc matte clay từ Reuzel", 249000, waxCategoryId, "Độ bám tốt, lưu hương lâu, dễ gội sạch"),
                    Product.Create("Firsthand Water Based Pomade", "Pomade gốc nước FirstHand", 459000, pomadeCategoryId, "Độ bóng vừa phải, giữ nếp cả ngày"),
                    Product.Create("Heavy Hold Pomade - Lockhart's Pomade", "Pomade giữ nếp mạnh từ Lockhart's", 479000, pomadeCategoryId, "Độ bóng cao, giữ nếp mạnh"),
                    Product.Create("Goon Grease Heavy Hold - Lockhart's Pomade", "Pomade giữ nếp mạnh từ Lockhart's", 479000, pomadeCategoryId, "Độ bóng cao, giữ nếp cực mạnh")
                },
                4, 1, 10
            );

            // Assign correct categories
            expectedProducts.Items[0].Category = waxCategory;
            expectedProducts.Items[1].Category = pomadeCategory;
            expectedProducts.Items[2].Category = pomadeCategory;
            expectedProducts.Items[3].Category = pomadeCategory;

            _productRepositoryMock.Setup(x => x.GetPaginated(
                    It.IsAny<GetProductsQuery>(),
                    It.IsAny<Func<IQueryable<Product>, IIncludableQueryable<Product, object>>>()))
                .ReturnsAsync(expectedProducts);

            var handler = new GetProductsQueryHandler(_productRepositoryMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProducts.Items.Count, result.Items.Count);
            Assert.Equal(expectedProducts.TotalCount, result.TotalCount);
        }

        [Fact]
        public async Task GetProductByIdQuery_ValidId_ShouldReturnProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            
            var category = new Category { Id = categoryId, Name = "Pomade" };
            
            var expectedProduct = Product.Create(
                "Matte Clay Professional - Lockhart Pomade", 
                "Sáp vuốt tóc clay chuyên nghiệp từ Lockhart", 
                579000, 
                categoryId, 
                "Độ giữ nếp cực mạnh, finish matte tự nhiên, hương thơm dễ chịu"
            );
            
            expectedProduct.Category = category;

            _productRepositoryMock.Setup(x => x.GetByIdAsync(
                    productId,
                    It.IsAny<Func<IQueryable<Product>, IIncludableQueryable<Product, object>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedProduct);

            var handler = new GetProductByIdQueryHandler(_productRepositoryMock.Object);

            // Act
            var result = await handler.Handle(new GetProductByIdQuery { Id = productId, IsIncludeItems = true }, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProduct.Name, result.Name);
            Assert.Equal(expectedProduct.Description, result.Description);
            Assert.Equal(expectedProduct.Price, result.Price);
            Assert.Equal(expectedProduct.Category.Name, result.Category!.Name);
        }

        [Fact]
        public async Task GetProductByIdQuery_InvalidId_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var productId = Guid.NewGuid();

            _productRepositoryMock.Setup(x => x.GetByIdAsync(
                    productId,
                    It.IsAny<Func<IQueryable<Product>, IIncludableQueryable<Product, object>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var handler = new GetProductByIdQueryHandler(_productRepositoryMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(new GetProductByIdQuery { Id = productId, IsIncludeItems = true }, CancellationToken.None));
        }

        [Fact]
        public async Task GetProductsQuery_WithPriceFilters_ShouldReturnFilteredProducts()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var query = new GetProductsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                CategoryId = categoryId,
                MinPrice = 450000,
                MaxPrice = 500000,
                IsIncludeItems = true
            };

            var pomadeCategory = new Category { Id = categoryId, Name = "Lockhart's Pomade" };

            var expectedProducts = new PaginationList<Product>(
                new List<Product>
                {
                    Product.Create("Heavy Hold Pomade - Lockhart's Pomade", "Pomade giữ nếp mạnh từ Lockhart's", 479000, categoryId, "Độ bóng cao, giữ nếp mạnh"),
                    Product.Create("Goon Grease Heavy Hold - Lockhart's Pomade", "Pomade giữ nếp mạnh từ Lockhart's", 479000, categoryId, "Độ bóng cao, giữ nếp cực mạnh"),
                    Product.Create("Medium Hold - Lockhart's Pomade", "Pomade giữ nếp vừa phải từ Lockhart's", 479000, categoryId, "Độ bóng cao, giữ nếp trung bình"),
                },
                3, 1, 10
            );

            foreach (var product in expectedProducts.Items)
            {
                product.Category = pomadeCategory;
            }

            _productRepositoryMock.Setup(x => x.GetPaginated(
                    It.IsAny<GetProductsQuery>(),
                    It.IsAny<Func<IQueryable<Product>, IIncludableQueryable<Product, object>>>()))
                .ReturnsAsync(expectedProducts);

            var handler = new GetProductsQueryHandler(_productRepositoryMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProducts.Items.Count, result.Items.Count);
            Assert.True(result.Items.All(p => p.Price >= query.MinPrice && p.Price <= query.MaxPrice));
        }
    }
} 