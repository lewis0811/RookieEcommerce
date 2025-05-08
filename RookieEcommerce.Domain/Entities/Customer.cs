using Microsoft.AspNetCore.Identity;

namespace RookieEcommerce.Domain.Entities
{
    public class Customer : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // Navigation Properties
        public virtual ICollection<Order> Orders { get; set; } = [];

        public virtual ICollection<ProductRating> Ratings { get; set; } = [];
        public virtual Cart? Cart { get; set; } // A customer can only have one active cart

        public static Customer Create(string firstName, string lastName, string email, string phoneNumber)
        {
            return new Customer
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber
            };
        }
    }
}