using PharmaERP.Product.API.DTOs;

namespace PharmaERP.Product.API.Services;

public interface IProductCategoryService
{
    Task<ProductCategoryResponse> CreateAsync(CreateProductCategoryRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductCategoryResponse>> GetAllAsync(bool activeOnly, CancellationToken cancellationToken);
    Task<ProductCategoryResponse?> UpdateAsync(int id, UpdateProductCategoryRequest request, CancellationToken cancellationToken);
}
