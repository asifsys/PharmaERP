using PharmaERP.Product.API.Entities;

namespace PharmaERP.Product.API.Repositories;

public interface IProductRepository
{
    IQueryable<MProduct> Query();
    Task<MProduct?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> ProductCodeExistsAsync(string code, int? excludingId, CancellationToken cancellationToken);
    Task<bool> BarcodeExistsAsync(string barcode, int? excludingId, CancellationToken cancellationToken);
    Task AddAsync(MProduct product, CancellationToken cancellationToken);
    void Update(MProduct product);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
