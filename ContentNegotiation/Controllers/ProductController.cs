using System;
using ContentNegotiation.Data;
using ContentNegotiation.Models;
using ContentNegotiation.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ContentNegotiation.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController(ProductRepository productRepository) : ControllerBase
{
    [HttpGet("{productId:guid}")]
    [Produces("application/json", "application/xml")]
    public ActionResult<ProductResponse> GetById(Guid productId)
    {
        var product = productRepository.GetProductById(productId);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(ProductResponse.FromModel(product));
    }
    [HttpGet]
    public IActionResult GetPaged(int page = 1, int pageSize = 10)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        int totalCount = productRepository.GetProductsCount();

        var products = productRepository.GetProductsPage(page, pageSize);

        var pagedResult = PagedResult<ProductResponse>.Create(
            ProductResponse.FromModels(products),
            totalCount,
            pageSize,
            page
        );

        return Ok(pagedResult);
    }

    [HttpGet("products-as-table")]
    [Produces("text/primitives-table")]
    public IActionResult GetProductsAsTextTable()
    {
        var products = productRepository.GetProductsPage(1, 100);
        return Ok(products);
    }
}
