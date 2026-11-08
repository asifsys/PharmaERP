using Microsoft.EntityFrameworkCore;
using PharmaERP.Product.API.Data;
using PharmaERP.Product.API.Entities;

namespace PharmaERP.Product.API.Repositories;

public sealed class ProductRepository(ProductDbContext db) : IProductRepository
{
    public IQueryable<MProduct> Query() => db.Products.AsNoTracking().Include(x => x.Category);

    public Task<MProduct?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        db.Products.Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.ProductId == id, cancellationToken);

    public Task<bool> ProductCodeExistsAsync(string code, int? excludingId, CancellationToken cancellationToken) =>
        db.Products.AnyAsync(x => x.ProductCode == code &&
            (!excludingId.HasValue || x.ProductId != excludingId.Value), cancellationToken);

    public Task<bool> BarcodeExistsAsync(string barcode, int? excludingId, CancellationToken cancellationToken) =>
        db.Products.AnyAsync(x => x.Barcode == barcode &&
            (!excludingId.HasValue || x.ProductId != excludingId.Value), cancellationToken);

    public async Task AddAsync(MProduct product, CancellationToken cancellationToken) =>
        await db.Products.AddAsync(product, cancellationToken);

    public void Update(MProduct product) => db.Products.Update(product);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        db.SaveChangesAsync(cancellationToken);
}
