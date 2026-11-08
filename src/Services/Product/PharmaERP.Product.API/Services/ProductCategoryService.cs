using Microsoft.EntityFrameworkCore;
using PharmaERP.Product.API.DTOs;
using PharmaERP.Product.API.Entities;
using PharmaERP.Product.API.Repositories;
using System.ComponentModel.DataAnnotations;

namespace PharmaERP.Product.API.Services;

public sealed class ProductCategoryService(IProductCategoryRepository categories)
    : IProductCategoryService
{
    public async Task<ProductCategoryResponse> CreateAsync(
        CreateProductCategoryRequest request, CancellationToken cancellationToken)
    {
        var code = request.CategoryCode.Trim().ToUpperInvariant();
        var name = request.CategoryName.Trim();

        if (await categories.CodeExistsAsync(code, null, cancellationToken))
            throw new ConflictException($"Category code '{code}' already exists.");
        if (await categories.NameExistsAsync(name, null, cancellationToken))
            throw new ConflictException($"Category name '{name}' already exists.");

        var category = new ProductCategory
        {
            CategoryCode = code,
            CategoryName = name,
            Description = request.Description?.Trim(),
            CreatedOn = DateTime.UtcNow,
            IsActive = true
        };
        await categories.AddAsync(category, cancellationToken);
        await categories.SaveChangesAsync(cancellationToken);
        return Map(category);
    }

    public async Task<IReadOnlyList<ProductCategoryResponse>> GetAllAsync(
        bool activeOnly, CancellationToken cancellationToken)
    {
        var q = categories.Query();
        if (activeOnly) q = q.Where(x => x.IsActive);
        var rows = await q.OrderBy(x => x.CategoryName).ToListAsync(cancellationToken);
        return rows.Select(Map).ToList();
    }

    public async Task<ProductCategoryResponse?> UpdateAsync(
        int id, UpdateProductCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await categories.GetByIdAsync(id, cancellationToken);
        if (category is null) return null;

        var code = request.CategoryCode.Trim().ToUpperInvariant();
        var name = request.CategoryName.Trim();
        if (await categories.CodeExistsAsync(code, id, cancellationToken))
            throw new ConflictException($"Category code '{code}' already exists.");
        if (await categories.NameExistsAsync(name, id, cancellationToken))
            throw new ConflictException($"Category name '{name}' already exists.");

        if (!request.IsActive && category.Products.Any(x => x.IsActive))
            throw new ValidationException("A category with active products cannot be deactivated.");

        category.CategoryCode = code;
        category.CategoryName = name;
        category.Description = request.Description?.Trim();
        category.IsActive = request.IsActive;
        category.ModifiedOn = DateTime.UtcNow;
        categories.Update(category);
        await categories.SaveChangesAsync(cancellationToken);
        return Map(category);
    }

    private static ProductCategoryResponse Map(ProductCategory x) =>
        new(x.ProductCategoryId, x.CategoryCode, x.CategoryName,
            x.Description, x.IsActive, x.Products.Count(p => p.IsActive));
}
