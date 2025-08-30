using System;
using System.Data;
using LearnWebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearnWebApi.Data;

public class ProjectContext : DbContext
{
    public DbSet<Product> Products { set; get; }
    public DbSet<Customer> Customers { set; get; }
    public DbSet<RefreshToken> RefreshTokens { set; get; }
    public DbSet<ApiKey> ApiKeys { set; get; }

    public ProjectContext(DbContextOptions<ProjectContext> options) : base(options) { }
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity
                .HasIndex(a => a.Key)
                .IsUnique();
        });
        modelBuilder.Entity<Product>(entity =>
        {
            entity
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            entity.HasData(
                new Product { Id = 1, Name = "Laptop Dell XPS 15", Price = 45000000m },
                new Product { Id = 2, Name = "Bàn phím cơ Logitech G Pro", Price = 3500000m },
                new Product { Id = 3, Name = "Chuột gaming Razer DeathAdder V2", Price = 1800000m },
                new Product { Id = 4, Name = "Màn hình cong Samsung Odyssey G9", Price = 28000000m },
                new Product { Id = 5, Name = "Tai nghe chống ồn Sony WH-1000XM5", Price = 8000000m },
                new Product { Id = 6, Name = "Ổ cứng SSD Samsung 980 Pro 1TB", Price = 3200000m },
                new Product { Id = 7, Name = "Máy ảnh Fujifilm X-T5", Price = 38000000m },
                new Product { Id = 8, Name = "Ống kính Canon RF 50mm f/1.8", Price = 6000000m },
                new Product { Id = 9, Name = "Bếp điện từ Kangaroo KG499I", Price = 1500000m },
                new Product { Id = 10, Name = "Nồi chiên không dầu Philips HD9252/90", Price = 2800000m },
                new Product { Id = 11, Name = "Robot hút bụi Xiaomi Robot Vacuum E10", Price = 5500000m },
                new Product { Id = 12, Name = "Tivi Sony Bravia 55 inch", Price = 17500000m },
                new Product { Id = 13, Name = "Loa Bluetooth JBL Flip 6", Price = 2000000m },
                new Product { Id = 14, Name = "Máy lọc không khí Coway AP-1009CH", Price = 4200000m },
                new Product { Id = 15, Name = "Đồng hồ thông minh Apple Watch Series 8", Price = 12000000m },
                new Product { Id = 16, Name = "Giày thể thao Adidas Ultraboost 22", Price = 3000000m },
                new Product { Id = 17, Name = "Áo khoác gió Uniqlo", Price = 800000m },
                new Product { Id = 18, Name = "Sách 'Đắc nhân tâm'", Price = 120000m },
                new Product { Id = 19, Name = "Dầu gội đầu Rejoice", Price = 85000m },
                new Product { Id = 20, Name = "Bộ LEGO Technic Bugatti Chiron", Price = 10500000m }
            );

        });

        modelBuilder.Entity<OrderProduct>(entity =>
        {
            entity.HasKey(op => new { op.OrderId, op.ProductId });

            entity
                .HasOne(op => op.Product)
                .WithMany(p => p.OrderProducts)
                .HasForeignKey(op => op.ProductId);

            entity
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(op => op.OrderId);
        });

        modelBuilder.Entity<Customer>(customer =>
        {
            customer
                .Property(c => c.Role)
                .HasDefaultValue("User");
        });
        
    }

}
