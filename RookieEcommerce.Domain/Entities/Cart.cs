using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RookieEcommerce.Domain.Entities
{
    public class Cart : BaseEntity
    {
        // Foreign key
        public Guid CustomerId { get; set; }

        // Navigation Properties
        public virtual Customer Customer { get; set; } = new();
        public virtual ICollection<CartItem> Items { get; set; } = [];
    }
}
