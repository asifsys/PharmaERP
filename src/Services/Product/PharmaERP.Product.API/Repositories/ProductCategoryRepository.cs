using Microsoft.EntityFrameworkCore;
using PharmaERP.Product.API.Data;
using PharmaERP.Product.API.Entities;

namespace PharmaERP.Product.API.Repositories;

public sealed class ProductCategoryRepository(ProductDbContext db) : IProductCategoryRepository
{
    public IQueryable<ProductCategory> Query() =>
        db.ProductCategories.AsNoTracking().Include(x => x.Products);

    public Task<ProductCategory?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        db.ProductCategories.Include(x => x.Products)
            .FirstOrDefaultAsync(x => x.ProductCategoryId == id, cancellationToken);

    public Task<bool> CodeExistsAsync(string code, int? excludingId, CancellationToken cancellationToken) =>
        db.ProductCategories.AnyAsync(x => x.CategoryCode == code &&
            (!excludingId.HasValue || x.ProductCategoryId != excludingId.Value), cancellationToken);

    public Task<bool> NameExistsAsync(string name, int? excludingId, CancellationToken cancellationToken) =>
        db.ProductCategories.AnyAsync(x => x.CategoryName == name &&
            (!excludingId.HasValue || x.ProductCategoryId != excludingId.Value), cancellationToken);

    public async Task AddAsync(ProductCategory category, CancellationToken cancellationToken) =>
        await db.ProductCategories.AddAsync(category, cancellationToken);

    public void Update(ProductCategory category) => db.ProductCategories.Update(category);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        db.SaveChangesAsync(cancellationToken);
}
