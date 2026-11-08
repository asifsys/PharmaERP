using Microsoft.EntityFrameworkCore;
using PharmaERP.Product.API.Entities;

namespace PharmaERP.Product.API.Data;

public sealed class ProductDbContext(DbContextOptions<ProductDbContext> options)
    : DbContext(options)
{
    public DbSet<MProduct> Products => Set<MProduct>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MProduct>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(x => x.ProductId);
            entity.Property(x => x.ProductCode).HasMaxLength(40).IsRequired();
            entity.Property(x => x.ProductName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.GenericName).HasMaxLength(200);
            entity.Property(x => x.BrandName).HasMaxLength(150);
            entity.Property(x => x.DosageForm).HasMaxLength(80);
            entity.Property(x => x.Strength).HasMaxLength(80);
            entity.Property(x => x.UnitOfMeasure).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Manufacturer).HasMaxLength(200);
            entity.Property(x => x.StorageCondition).HasMaxLength(200);
            entity.Property(x => x.HSNCode).HasMaxLength(20);
            entity.Property(x => x.Barcode).HasMaxLength(80);
            entity.Property(x => x.GstPercent).HasPrecision(5, 2);
            entity.Property(x => x.ProductType).HasConversion<string>().HasMaxLength(40);
            entity.Property(x => x.CreatedOn).IsRequired();
            entity.HasIndex(x => x.ProductCode).IsUnique();

            // Fixed: Semicolon added right below
            entity.HasIndex(x => x.Barcode).IsUnique()
                .HasFilter("[Barcode] IS NOT NULL");

            entity.HasIndex(x => new { x.ProductCategoryId, x.IsActive });

            entity.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.ProductCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.ToTable("ProductCategories");
            entity.HasKey(x => x.ProductCategoryId);
            entity.Property(x => x.CategoryCode).HasMaxLength(40).IsRequired();
            entity.Property(x => x.CategoryName).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.HasIndex(x => x.CategoryCode).IsUnique();
            entity.HasIndex(x => x.CategoryName).IsUnique();
        });
    }
}
