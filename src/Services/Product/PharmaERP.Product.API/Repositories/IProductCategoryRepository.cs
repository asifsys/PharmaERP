using PharmaERP.Product.API.Entities;

namespace PharmaERP.Product.API.Repositories;

public interface IProductCategoryRepository
{
    IQueryable<ProductCategory> Query();
    Task<ProductCategory?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> CodeExistsAsync(string code, int? excludingId, CancellationToken cancellationToken);
    Task<bool> NameExistsAsync(string name, int? excludingId, CancellationToken cancellationToken);
    Task AddAsync(ProductCategory category, CancellationToken cancellationToken);
    void Update(ProductCategory category);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
