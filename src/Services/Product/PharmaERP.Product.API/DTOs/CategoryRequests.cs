using System.ComponentModel.DataAnnotations;

namespace PharmaERP.Product.API.DTOs;

public class CreateProductCategoryRequest
{
    [Required, StringLength(40, MinimumLength = 2)]
    public string CategoryCode { get; init; } = string.Empty;

    [Required, StringLength(120, MinimumLength = 2)]
    public string CategoryName { get; init; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; init; }
}

public sealed class UpdateProductCategoryRequest : CreateProductCategoryRequest
{
    public bool IsActive { get; init; } = true;
}

public sealed record ProductCategoryResponse(
    int ProductCategoryId, string CategoryCode, string CategoryName,
    string? Description, bool IsActive, int ProductCount);
