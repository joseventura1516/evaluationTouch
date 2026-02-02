using InventarioSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventarioSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(50).IsRequired();
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100).IsRequired();
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash").IsRequired();
                entity.Property(e => e.Role).HasColumnName("role").HasMaxLength(20).IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);

                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
                entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(1000);
                entity.Property(e => e.Price).HasColumnName("price").HasPrecision(18, 2).IsRequired();
                entity.Property(e => e.Quantity).HasColumnName("quantity").IsRequired();
                entity.Property(e => e.Category).HasColumnName("category").HasMaxLength(100).IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);

                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.Name);
            });

            // Notification configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("notifications");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
                entity.Property(e => e.Message).HasColumnName("message").HasMaxLength(500).IsRequired();
                entity.Property(e => e.Type).HasColumnName("type").HasMaxLength(20).IsRequired();
                entity.Property(e => e.IsRead).HasColumnName("is_read").HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.IsRead);
            });

            // Seed data - Sample products (Admin user is created dynamically in Program.cs)
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Laptop HP Pavilion",
                    Description = "Laptop HP Pavilion 15.6\" Intel Core i5 8GB RAM 256GB SSD",
                    Price = 12999.00m,
                    Quantity = 15,
                    Category = "Electrónica",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true
                },
                new Product
                {
                    Id = 2,
                    Name = "Mouse Logitech MX Master",
                    Description = "Mouse inalámbrico ergonómico Logitech MX Master 3",
                    Price = 1899.00m,
                    Quantity = 3,
                    Category = "Accesorios",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true
                },
                new Product
                {
                    Id = 3,
                    Name = "Teclado Mecánico Redragon",
                    Description = "Teclado mecánico RGB switches blue Redragon K552",
                    Price = 899.00m,
                    Quantity = 8,
                    Category = "Accesorios",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true
                },
                new Product
                {
                    Id = 4,
                    Name = "Monitor Samsung 27\"",
                    Description = "Monitor Samsung 27\" Full HD IPS 75Hz",
                    Price = 4599.00m,
                    Quantity = 2,
                    Category = "Electrónica",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true
                },
                new Product
                {
                    Id = 5,
                    Name = "Audífonos Sony WH-1000XM4",
                    Description = "Audífonos inalámbricos con cancelación de ruido Sony",
                    Price = 6499.00m,
                    Quantity = 4,
                    Category = "Audio",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true
                }
            );
        }
    }
}
