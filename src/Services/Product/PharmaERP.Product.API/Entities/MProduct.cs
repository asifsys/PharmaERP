namespace PharmaERP.Product.API.Entities;

public sealed class MProduct
{
    public int ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? GenericName { get; set; }
    public string? BrandName { get; set; }
    public int ProductCategoryId { get; set; }
    public ProductCategory? Category { get; set; }
    public ProductType ProductType { get; set; }
    public string? DosageForm { get; set; }
    public string? Strength { get; set; }
    public string UnitOfMeasure { get; set; } = "EA";
    public string? Manufacturer { get; set; }
    public string? StorageCondition { get; set; }
    public string? HSNCode { get; set; }
    public decimal? GstPercent { get; set; }
    public string? Barcode { get; set; }
    public bool IsPrescriptionRequired { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedOn { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
}
