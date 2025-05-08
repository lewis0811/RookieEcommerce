using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RookieEcommerce.SharedViewModels.ProductDtos
{
    public class ProductVariantUpdateDto
    {
        public Guid Id { get; set; }
        public int? StockQuantity { get; set; }
    }
}
