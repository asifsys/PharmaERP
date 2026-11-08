using System.ComponentModel.DataAnnotations;
using PharmaERP.Product.API.Entities;

namespace PharmaERP.Product.API.DTOs;

public class CreateProductRequest
{
    [Required, StringLength(40, MinimumLength = 2)]
    public string ProductCode { get; init; } = string.Empty;

    [Required, StringLength(200, MinimumLength = 2)]
    public string ProductName { get; init; } = string.Empty;

    [StringLength(200)]
    public string? GenericName { get; init; }

    [StringLength(150)]
    public string? BrandName { get; init; }

    [Range(1, int.MaxValue)]
    public int ProductCategoryId { get; init; }

    [EnumDataType(typeof(ProductType))]
    public ProductType ProductType { get; init; }

    [StringLength(80)]
    public string? DosageForm { get; init; }

    [StringLength(80)]
    public string? Strength { get; init; }

    [Required, StringLength(30)]
    public string UnitOfMeasure { get; init; } = "EA";

    [StringLength(200)]
    public string? Manufacturer { get; init; }

    [StringLength(200)]
    public string? StorageCondition { get; init; }

    [StringLength(20)]
    public string? HSNCode { get; init; }

    [Range(typeof(decimal), "0", "100")]
    public decimal? GstPercent { get; init; }

    [StringLength(80)]
    public string? Barcode { get; init; }

    public bool IsPrescriptionRequired { get; init; }
}

public sealed class UpdateProductRequest : CreateProductRequest
{
    public bool IsActive { get; init; } = true;
}

public sealed class ProductQuery
{
    public string? Search { get; init; }
    public int? ProductCategoryId { get; init; }
    public ProductType? ProductType { get; init; }
    public bool? IsActive { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string SortBy { get; init; } = "ProductName";
    public string SortDirection { get; init; } = "asc";
}

public sealed record ProductResponse(
    int ProductId, string ProductCode, string ProductName, string? GenericName,
    string? BrandName, int ProductCategoryId, string CategoryName,
    ProductType ProductType, string? DosageForm, string? Strength,
    string UnitOfMeasure, string? Manufacturer, string? StorageCondition,
    string? HSNCode, decimal? GstPercent, string? Barcode,
    bool IsPrescriptionRequired, bool IsActive, DateTime CreatedOn, DateTime? ModifiedOn);

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount, int TotalPages);

public sealed record ProductDashboardSummary(
    int TotalProducts, int ActiveProducts, int InactiveProducts,
    int RawMaterials, int PackagingMaterials, int Intermediates,
    int FinishedProducts, int ActivePharmaceuticalIngredients);
