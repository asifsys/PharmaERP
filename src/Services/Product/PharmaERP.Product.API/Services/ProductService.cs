using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using PharmaERP.Product.API.DTOs;
using PharmaERP.Product.API.Entities;
using PharmaERP.Product.API.Repositories;

namespace PharmaERP.Product.API.Services;

public sealed class ProductService(
    IProductRepository products,
    IProductCategoryRepository categories) : IProductService
{
    private static readonly HashSet<string> AllowedSorts = new(StringComparer.OrdinalIgnoreCase)
    {
        "ProductCode", "ProductName", "GenericName", "BrandName", "ProductType",
        "CreatedOn", "IsActive"
    };

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request, string? actor, CancellationToken cancellationToken)
    {
        var code = request.ProductCode.Trim().ToUpperInvariant();
        var barcode = Clean(request.Barcode);

        if (await products.ProductCodeExistsAsync(code, null, cancellationToken))
            throw new ConflictException($"Product code '{code}' already exists.");

        if (barcode is not null &&
            await products.BarcodeExistsAsync(barcode, null, cancellationToken))
            throw new ConflictException($"Barcode '{barcode}' is already assigned.");

        var category = await categories.GetByIdAsync(request.ProductCategoryId, cancellationToken);
        if (category is null || !category.IsActive)
            throw new ValidationException("The selected product category does not exist or is inactive.");

        var product = new MProduct
        {
            ProductCode = code,
            ProductName = request.ProductName.Trim(),
            GenericName = Clean(request.GenericName),
            BrandName = Clean(request.BrandName),
            ProductCategoryId = request.ProductCategoryId,
            ProductType = request.ProductType,
            DosageForm = Clean(request.DosageForm),
            Strength = Clean(request.Strength),
            UnitOfMeasure = request.UnitOfMeasure.Trim().ToUpperInvariant(),
            Manufacturer = Clean(request.Manufacturer),
            StorageCondition = Clean(request.StorageCondition),
            HSNCode = Clean(request.HSNCode),
            GstPercent = request.GstPercent,
            Barcode = barcode,
            IsPrescriptionRequired = request.IsPrescriptionRequired,
            IsActive = true,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = actor
        };

        await products.AddAsync(product, cancellationToken);
        await products.SaveChangesAsync(cancellationToken);
        product.Category = category;
        return Map(product);
    }

    public async Task<ProductResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var product = await products.GetByIdAsync(id, cancellationToken);
        return product is null ? null : Map(product);
    }

    public async Task<PagedResult<ProductResponse>> SearchAsync(
        ProductQuery query, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var source = products.Query();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            source = source.Where(x =>
                x.ProductCode.Contains(term) ||
                x.ProductName.Contains(term) ||
                (x.GenericName != null && x.GenericName.Contains(term)) ||
                (x.BrandName != null && x.BrandName.Contains(term)) ||
                (x.Barcode != null && x.Barcode.Contains(term)));
        }

        if (query.ProductCategoryId.HasValue)
            source = source.Where(x => x.ProductCategoryId == query.ProductCategoryId.Value);
        if (query.ProductType.HasValue)
            source = source.Where(x => x.ProductType == query.ProductType.Value);
        if (query.IsActive.HasValue)
            source = source.Where(x => x.IsActive == query.IsActive.Value);

        var total = await source.CountAsync(cancellationToken);
        var sortBy = AllowedSorts.Contains(query.SortBy) ? query.SortBy : "ProductName";
        var descending = string.Equals(query.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        source = sortBy.ToLowerInvariant() switch
        {
            "productcode" => descending ? source.OrderByDescending(x => x.ProductCode) : source.OrderBy(x => x.ProductCode),
            "genericname" => descending ? source.OrderByDescending(x => x.GenericName) : source.OrderBy(x => x.GenericName),
            "brandname" => descending ? source.OrderByDescending(x => x.BrandName) : source.OrderBy(x => x.BrandName),
            "producttype" => descending ? source.OrderByDescending(x => x.ProductType) : source.OrderBy(x => x.ProductType),
            "createdon" => descending ? source.OrderByDescending(x => x.CreatedOn) : source.OrderBy(x => x.CreatedOn),
            "isactive" => descending ? source.OrderByDescending(x => x.IsActive) : source.OrderBy(x => x.IsActive),
            _ => descending ? source.OrderByDescending(x => x.ProductName) : source.OrderBy(x => x.ProductName)
        };

        var rows = await source.Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductResponse>(
            rows.Select(Map).ToList(), page, pageSize, total,
            (int)Math.Ceiling(total / (double)pageSize));
    }

    public async Task<ProductResponse?> UpdateAsync(
        int id, UpdateProductRequest request, string? actor, CancellationToken cancellationToken)
    {
        var product = await products.GetByIdAsync(id, cancellationToken);
        if (product is null) return null;

        var code = request.ProductCode.Trim().ToUpperInvariant();
        var barcode = Clean(request.Barcode);

        if (await products.ProductCodeExistsAsync(code, id, cancellationToken))
            throw new ConflictException($"Product code '{code}' already exists.");
        if (barcode is not null &&
            await products.BarcodeExistsAsync(barcode, id, cancellationToken))
            throw new ConflictException($"Barcode '{barcode}' is already assigned.");

        var category = await categories.GetByIdAsync(request.ProductCategoryId, cancellationToken);
        if (category is null || (!category.IsActive && category.ProductCategoryId != product.ProductCategoryId))
            throw new ValidationException("The selected product category does not exist or is inactive.");

        product.ProductCode = code;
        product.ProductName = request.ProductName.Trim();
        product.GenericName = Clean(request.GenericName);
        product.BrandName = Clean(request.BrandName);
        product.ProductCategoryId = request.ProductCategoryId;
        product.ProductType = request.ProductType;
        product.DosageForm = Clean(request.DosageForm);
        product.Strength = Clean(request.Strength);
        product.UnitOfMeasure = request.UnitOfMeasure.Trim().ToUpperInvariant();
        product.Manufacturer = Clean(request.Manufacturer);
        product.StorageCondition = Clean(request.StorageCondition);
        product.HSNCode = Clean(request.HSNCode);
        product.GstPercent = request.GstPercent;
        product.Barcode = barcode;
        product.IsPrescriptionRequired = request.IsPrescriptionRequired;
        product.IsActive = request.IsActive;
        product.ModifiedOn = DateTime.UtcNow;
        product.ModifiedBy = actor;

        products.Update(product);
        await products.SaveChangesAsync(cancellationToken);
        product.Category = category;
        return Map(product);
    }

    public async Task<bool> DeactivateAsync(int id, string? actor, CancellationToken cancellationToken)
    {
        var product = await products.GetByIdAsync(id, cancellationToken);
        if (product is null) return false;
        product.IsActive = false;
        product.ModifiedOn = DateTime.UtcNow;
        product.ModifiedBy = actor;
        products.Update(product);
        await products.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<ProductDashboardSummary> GetDashboardSummaryAsync(CancellationToken cancellationToken)
    {
        var q = products.Query();
        var total = await q.CountAsync(cancellationToken);
        var active = await q.CountAsync(x => x.IsActive, cancellationToken);
        var types = await q.Where(x => x.IsActive)
            .GroupBy(x => x.ProductType)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);
        int Count(ProductType type) => types.FirstOrDefault(x => x.Type == type)?.Count ?? 0;

        return new ProductDashboardSummary(total, active, total - active,
            Count(ProductType.RawMaterial), Count(ProductType.PackagingMaterial),
            Count(ProductType.Intermediate), Count(ProductType.FinishedProduct),
            Count(ProductType.ActivePharmaceuticalIngredient));
    }

    private static ProductResponse Map(MProduct x) => new(
        x.ProductId, x.ProductCode, x.ProductName, x.GenericName, x.BrandName,
        x.ProductCategoryId, x.Category?.CategoryName ?? string.Empty, x.ProductType,
        x.DosageForm, x.Strength, x.UnitOfMeasure, x.Manufacturer,
        x.StorageCondition, x.HSNCode, x.GstPercent, x.Barcode,
        x.IsPrescriptionRequired, x.IsActive, x.CreatedOn, x.ModifiedOn);

    private static string? Clean(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
