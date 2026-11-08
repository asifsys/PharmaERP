using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaERP.Product.API.DTOs;
using PharmaERP.Product.API.Services;

namespace PharmaERP.Product.API.Controllers;

[ApiController]
[Route("api/product-categories")]
[Authorize(Policy = "ProductRead")]
public sealed class ProductCategoriesController(IProductCategoryService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductCategoryResponse>>> GetAll(
        [FromQuery] bool activeOnly = true, CancellationToken cancellationToken = default) =>
        Ok(await service.GetAllAsync(activeOnly, cancellationToken));

    [HttpPost]
    [Authorize(Policy = "ProductWrite")]
    public async Task<ActionResult<ProductCategoryResponse>> Create(
        [FromBody] CreateProductCategoryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await service.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetAll), new { activeOnly = false }, result);
        }
        catch (ValidationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "ProductWrite")]
    public async Task<ActionResult<ProductCategoryResponse>> Update(
        int id, [FromBody] UpdateProductCategoryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await service.UpdateAsync(id, request, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (ValidationException ex) { return BadRequest(new { message = ex.Message }); }
    }
}
