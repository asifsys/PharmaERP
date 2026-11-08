using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaERP.Product.API.DTOs;
using PharmaERP.Product.API.Services;

namespace PharmaERP.Product.API.Controllers;

[ApiController]
[Route("api/products")]
//[Authorize(Policy = "ProductRead")]
public sealed class ProductsController(IProductService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductResponse>>> Search(
        [FromQuery] ProductQuery query, CancellationToken cancellationToken) =>
        Ok(await service.SearchAsync(query, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await service.GetByIdAsync(id, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpGet("dashboard-summary")]
    public async Task<ActionResult<ProductDashboardSummary>> DashboardSummary(CancellationToken cancellationToken) =>
        Ok(await service.GetDashboardSummaryAsync(cancellationToken));

    [HttpPost]
   // [Authorize(Policy = "ProductWrite")]
    public async Task<ActionResult<ProductResponse>> Create(
        [FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await service.CreateAsync(request, User.Identity?.Name, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.ProductId }, result);
        }
        catch (ValidationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "ProductWrite")]
    public async Task<ActionResult<ProductResponse>> Update(
        int id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await service.UpdateAsync(id, request, User.Identity?.Name, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (ValidationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "ProductWrite")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        var changed = await service.DeactivateAsync(id, User.Identity?.Name, cancellationToken);
        return changed ? NoContent() : NotFound();
    }
}
