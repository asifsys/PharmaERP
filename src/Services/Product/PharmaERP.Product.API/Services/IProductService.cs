using PharmaERP.Product.API.DTOs;

namespace PharmaERP.Product.API.Services;

public interface IProductService
{
    Task<ProductResponse> CreateAsync(CreateProductRequest request, string? actor, CancellationToken cancellationToken);
    Task<ProductResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<PagedResult<ProductResponse>> SearchAsync(ProductQuery query, CancellationToken cancellationToken);
    Task<ProductResponse?> UpdateAsync(int id, UpdateProductRequest request, string? actor, CancellationToken cancellationToken);
    Task<bool> DeactivateAsync(int id, string? actor, CancellationToken cancellationToken);
    Task<ProductDashboardSummary> GetDashboardSummaryAsync(CancellationToken cancellationToken);
}
