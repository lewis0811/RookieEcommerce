using RookieEcommerce.SharedViewModels.CategoryDtos;
using RookieEcommerce.SharedViewModels.ProductDtos;
using RookieEcommerce.SharedViewModels.ResponseDtos;

namespace RookieEcommerce.CustomerSite.Models
{
    public class HomeViewModel
    {
        public List<CategoryDetailsDto>? Categories { get; set; }
        public PaginationResponseDto<ProductDetailsDto>? Products { get; set; }
    }
}
