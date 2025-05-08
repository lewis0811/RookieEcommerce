using Moq;
using RookieEcommerce.Application.Common;
using RookieEcommerce.Application.Contacts.Persistence;
using RookieEcommerce.Application.Features.Categories.Queries;
using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.UnitTest.Features.Categories.Queries
{
    public class CategoryQueryTest
    {
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;

        public CategoryQueryTest()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
        }

        [Fact]
        public async Task GetCategoriesQuery_ValidData_ShouldReturnCategories()
        {
            // Arrange
            var query = new GetPCategoriesQuery
            {
                ParentCategoryId = null,
                IsIncludeItems = false
            };

            var expectedCategories = new PaginationList<Category>(
                new List<Category>
                {
                    Category.Create("Sáp Vuốt Tóc Nam", "Chuyên cung cấp các sản phẩm sáp vuốt tóc nam chính hãng", null),
                    Category.Create("Pomade", "Các sản phẩm pomade chính hãng", null),
                    Category.Create("Pre-Styling", "Các sản phẩm pre-styling", null),
                    Category.Create("Gôm Xịt Tóc", "Các sản phẩm gôm xịt tóc", null),
                    Category.Create("Dầu Gội & Dầu Xả", "Các sản phẩm dầu gội và dầu xả", null)
                },
                5, 1, 10
            );

            _categoryRepositoryMock.Setup(x => x.GetPaginated(query, null))
                .ReturnsAsync(expectedCategories);

            var handler = new GetPcategoriesQueryHandler(_categoryRepositoryMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCategories.Items.Count, result.Items.Count);
            Assert.Equal(expectedCategories.TotalCount, result.TotalCount);
            Assert.Equal("Sáp Vuốt Tóc Nam", result.Items[0].Name);
            Assert.Equal("Pomade", result.Items[1].Name);
        }

        [Fact]
        public async Task GetCategoryByIdQuery_ValidId_ShouldReturnCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var expectedCategory = Category.Create("Nước Hoa Nam", "Các sản phẩm nước hoa nam chính hãng", null);

            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCategory);

            var handler = new GetCategoryByIdQueryHandler(_categoryRepositoryMock.Object);

            // Act
            var result = await handler.Handle(new GetCategoryByIdQuery { Id = categoryId }, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCategory.Name, result.Name);
            Assert.Equal(expectedCategory.Description, result.Description);
        }

        [Fact]
        public async Task GetCategoryByIdQuery_InvalidId_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            var handler = new GetCategoryByIdQueryHandler(_categoryRepositoryMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(new GetCategoryByIdQuery { Id = categoryId }, CancellationToken.None));
        }

        [Fact]
        public async Task GetCategoriesQuery_WithParentCategory_ShouldReturnChildCategories()
        {
            // Arrange
            var parentCategoryId = Guid.NewGuid();
            var query = new GetPCategoriesQuery
            {
                ParentCategoryId = parentCategoryId,
                IsIncludeItems = true
            };

            var expectedCategories = new PaginationList<Category>(
                new List<Category>
                {
                    Category.Create("Lockhart's Pomade", "Các sản phẩm pomade từ Lockhart's", parentCategoryId),
                    Category.Create("Reuzel Pomade", "Các sản phẩm pomade từ Reuzel", parentCategoryId),
                    Category.Create("Suavecito Pomade", "Các sản phẩm pomade từ Suavecito", parentCategoryId)
                },
                3, 1, 10
            );

            _categoryRepositoryMock.Setup(x => x.GetPaginated(query, null))
                .ReturnsAsync(expectedCategories);

            var handler = new GetPcategoriesQueryHandler(_categoryRepositoryMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCategories.Items.Count, result.Items.Count);
            Assert.Equal(expectedCategories.TotalCount, result.TotalCount);
            Assert.All(result.Items, item => Assert.Equal(parentCategoryId, item.ParentCategoryId));
        }
    }
}