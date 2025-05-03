using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.Infrastructure
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<IdentityUser>(options)
    {
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductRating> ProductRatings { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // --- Category ---
            builder.Entity<Category>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Entity<Category>()
                .Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(200);

            builder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Product ---
            builder.Entity<Product>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Entity<Product>()
                .Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(200);

            builder.Entity<Product>()
               .Property(p => p.Price)
               .HasColumnType("decimal(18,2)");

            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Or Cascade if desired

            builder.Entity<Product>()
                .Property(c => c.Sku)
                .IsRequired()
                .HasMaxLength(50);

            // --- ProductImage
            builder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Delete images if product is deleted

            // --- ProductRating ---
            builder.Entity<ProductRating>()
                 .HasOne(pr => pr.Product)
                 .WithMany()
                 .HasForeignKey(pr => pr.ProductId)
                 .OnDelete(DeleteBehavior.Cascade); // Delete ratings if product is deleted

            builder.Entity<ProductRating>()
                .HasOne(pr => pr.Customer)
                .WithMany(c => c.Ratings)
                .HasForeignKey(pr => pr.CustomerId)
                .OnDelete(DeleteBehavior.Cascade); // Delete ratings if customer is deleted

            builder.Entity<ProductRating>()
                .Property(p => p.Comment)
                .HasMaxLength(2000);

            // --- ProductVariant ---
            builder.Entity<ProductVariant>()
                .HasOne(pv => pv.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Delete variant if product is deleted
            builder.Entity<ProductVariant>()
                .Property(pv => pv.Price)
                .HasColumnType("decimal(18,2)");

            // --- Cart / CartItem ---
            builder.Entity<Cart>()
                .HasOne(ca => ca.Customer)
                .WithOne(cu => cu.Cart)
                .HasForeignKey<Cart>(ca => ca.CustomerId);

            builder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(ca => ca.Items)
                .HasForeignKey(ci => ci.CartId);

            builder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId);

            // --- Order / OrderItem ---
            // Configure the owned Address type
            builder.Entity<Order>()
                .OwnsOne(o => o.ShippingAddress);

            builder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<string>(); // Store enum as string for readability

            builder.Entity<Order>()
                 .HasOne(o => o.Customer)
                 .WithMany(c => c.Orders)
                 .HasForeignKey(o => o.CustomerId)
                 .OnDelete(DeleteBehavior.Restrict); // Don't delete customer if they have orders

            builder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasColumnType("decimal(18, 2)");
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.ProductVariant)
                .WithMany()
                .HasForeignKey(oi => oi.ProductVariantId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<OrderItem>()
                .HasOne(c => c.Order)
                .WithMany(c => c.OrderItems)
                .HasForeignKey(c => c.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}