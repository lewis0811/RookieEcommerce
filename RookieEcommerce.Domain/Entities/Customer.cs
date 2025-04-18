﻿namespace RookieEcommerce.Domain.Entities
{
    public class Customer : BaseEntity
    {
        // Link to IdentityUser
        public string IdentityUserId { get; set; } = string.Empty;

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // Navigation Properties
        public virtual ICollection<Order> Orders { get; set; } = [];
        public virtual ICollection<ProductRating> Ratings { get; set; } = [];
        public virtual Cart? Cart { get; set; } // A customer can only have one active cart
    }
}