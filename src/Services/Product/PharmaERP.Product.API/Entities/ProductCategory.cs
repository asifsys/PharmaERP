namespace PharmaERP.Product.API.Entities;

public sealed class ProductCategory
{
    public int ProductCategoryId { get; set; }
    public string CategoryCode { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedOn { get; set; }
    public ICollection<MProduct> Products { get; set; } = new List<MProduct>();
}
