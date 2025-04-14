using Microsoft.EntityFrameworkCore;
using RookieEcommerce.Domain.Entities;

namespace RookieEcommerce.Infrastructure
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Category ---
            modelBuilder.Entity<Category>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Category>()
                .Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Product ---
            modelBuilder.Entity<Product>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Product>()
                .Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<Product>()
               .Property(p => p.Price)
               .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Or Cascade if desired

            // --- ProductImage
            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Delete images if product is deleted

            // --- ProductRating ---
            modelBuilder.Entity<ProductRating>()
                 .HasOne(pr => pr.Product)
                 .WithMany() // Product might not have direct navigation back to ratings
                 .HasForeignKey(pr => pr.ProductId)
                 .OnDelete(DeleteBehavior.Cascade); // Delete ratings if product is deleted

            modelBuilder.Entity<ProductRating>()
                .HasOne(pr => pr.Customer)
                .WithMany(c => c.Ratings)
                .HasForeignKey(pr => pr.CustomerId)
                .OnDelete(DeleteBehavior.Cascade); // Delete ratings if customer is deleted

            // --- ProductVariant ---
            modelBuilder.Entity<ProductVariant>()
                .HasOne(pv => pv.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Delete variant if product is deleted
            modelBuilder.Entity<ProductVariant>()
                .Property(pv => pv.Price)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ProductVariant>()
                .Property(pv => pv.VariantType)
                .HasConversion<string>();

            // --- Cart / CartItem ---
            modelBuilder.Entity<Cart>()
                .HasOne(ca => ca.Customer)
                .WithOne(cu => cu.Cart) // One-to-one relationship
                .HasForeignKey<Cart>(ca => ca.CustomerId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(ca => ca.Items)
                .HasForeignKey(ci => ci.CartId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId);

            // --- Order / OrderItem ---
            // Configure the owned Address type
            modelBuilder.Entity<Order>().OwnsOne(o => o.ShippingAddress);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<string>(); // Store enum as string for readability

            modelBuilder.Entity<Order>()
                 .HasOne(o => o.Customer)
                 .WithMany(c => c.Orders)
                 .HasForeignKey(o => o.CustomerId)
                 .OnDelete(DeleteBehavior.Restrict); // Don't delete customer if they have orders

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasColumnType("decimal(18, 2)"); // Price at time of order

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.ProductVariant)
                .WithMany() // Product doesn't need navigation back to OrderItems
                .HasForeignKey(oi => oi.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting product if it's in an order
        }
    }
}