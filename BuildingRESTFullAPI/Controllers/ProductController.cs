using System;
using System.Text;
using BuildingRESTFullAPI.Data;
using BuildingRESTFullAPI.Models;
using BuildingRESTFullAPI.Requests;
using BuildingRESTFullAPI.Responses;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace BuildingRESTFullAPI.Controllers;


[ApiController]
[Route("api/products")]
public class ProductController(ProductRepository productRepository) : ControllerBase
{
    [HttpOptions]
    public IActionResult GetOptions()
    {
        Response.Headers.Append("Allow", "GET, POST, PUT, DELETE, OPTIONS");
        return NoContent();
    }

    [HttpHead("{productId:guid}")]
    public IActionResult HeadById(Guid productId)
    {
        // In a real application, you would check if the product exists in the database.
        bool productExists = productRepository.ExistsById(productId); // Placeholder for actual existence check.

        if (productExists)
        {
            return Ok();
        }
        else
        {
            return NotFound();
        }
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

    [HttpGet("{productId:guid}", Name = "GetById")]
    public ActionResult<ProductResponse> GetById(Guid productId, bool includeReviews = false)
    {
        var product = productRepository.GetProductById(productId);

        if (product is null)
        {
            return NotFound();
        }
        List<ProductReview>? reviews = null;
        if (includeReviews == true)
        {
            reviews = productRepository.GetProductReviews(productId);

        }

        return ProductResponse.FromModel(product, reviews);
    }

    [HttpPost]
    public IActionResult Create(CreateProductRequest request)
    {

        if (productRepository.ExistsByName(request.Name))
            return Conflict($"A product with the name '{request.Name}' already exists.");

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price
        };

        productRepository.AddProduct(product);

        return CreatedAtRoute(routeName: nameof(GetById),
            routeValues: new { productId = product.Id },
            value: ProductResponse.FromModel(product));
    }

    [HttpPut("{productId:guid}")]
    public IActionResult Update(Guid productId, UpdateProductRequest request)
    {
        if (request is null)
            return BadRequest("Request body is null.");

        var product = productRepository.GetProductById(productId);
        if (product is null)
        {
            return NotFound($"Product with ID '{productId}' not found.");
        }

        product.Name = request.Name;
        product.Price = request.Price;

        var successed = productRepository.UpdateProduct(product);

        if (!successed)
            return StatusCode(500, "An error occurred while updating the product.");

        return NoContent();
    }

    [HttpPatch("{productId:guid}")]
    public IActionResult Patch(Guid productId, JsonPatchDocument<UpdateProductRequest> request)
    {
        if (request is null)
            return BadRequest("Request body is null.");

        var product = productRepository.GetProductById(productId);
        if (product is null)
        {
            return NotFound($"Product with ID '{productId}' not found.");
        }

        var updateModel = new UpdateProductRequest
        {
            Name = product.Name,
            Price = product.Price
        };

        request.ApplyTo(updateModel);

        product.Name = updateModel.Name;
        product.Price = updateModel.Price;

        var successed = productRepository.UpdateProduct(product);

        if (!successed)
            return StatusCode(500, "An error occurred while patch the product.");

        return NoContent();
    }

    [HttpDelete("{productId:guid}")]
    public IActionResult Delete(Guid productId)
    {
        var product = productRepository.GetProductById(productId);
        if (product is null)
        {
            return NotFound($"Product with ID '{productId}' not found.");
        }

        var successed = productRepository.DeleteProduct(productId);

        if (!successed)
            return StatusCode(500, "An error occurred while deleting the product.");

        return NoContent();
    }

    [HttpGet("status/{jobId:guid}", Name = "GetJobStatus")]
    public IActionResult GetJobStatus(Guid jobId)
    {
        // Simulate checking job status
        // In a real application, you would check the status of the job in your background processing system
        var isCompleted = false; // Placeholder for actual job status check

        return Ok(new { jobId = jobId, status = isCompleted ? "Processing" : "Completed" });
    }

    [HttpPost("process")]
    public IActionResult ProcessAsync()
    {
        var jobId = Guid.NewGuid();
        // Simulate starting a background job
        // In a real application, you would enqueue a job in a background processing system
        return Accepted($"api/products/status/{jobId}",

        new { jobId = jobId, status = "Progressing" });
    }

    [HttpGet("csv")]
    public IActionResult GetProductsAsCsv()
    {
        var products = productRepository.GetProductsPage(1, 100);

        var csvBuilder = new StringBuilder();

        csvBuilder.AppendLine("Id,Name,Price");

        foreach (var product in products)
        {
            csvBuilder.AppendLine($"{product.Id},{product.Name},{product.Price}");
        }

        var csvBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

        return File(csvBytes, "text/csv", "products_From-1-To-100.csv");
    }

    [HttpGet("physical-csv-file")]
    public IActionResult GetPhysicalCsvFile()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Products.csv"); // Ensure this path is correct and the file exists

        return PhysicalFile(filePath, "text/csv", "products_physical_file.csv");
    }

    [HttpGet("products-legacy")]
    public IActionResult GetRedirect()
    {
        return Redirect("/api/products/temp-products");
    }

    [HttpGet("temp-products")]
    public IActionResult TempProducts()
    {
        var tempProducts = new List<Product>
        {
            new Product { Id = Guid.NewGuid(), Name = "Temp Product 1", Price = 9.99M },
            new Product { Id = Guid.NewGuid(), Name = "Temp Product 2", Price = 19.99M },
            new Product { Id = Guid.NewGuid(), Name = "Temp Product 3", Price = 29.99M }
        };

        return Ok(ProductResponse.FromModels(tempProducts));
    }

    [HttpGet("legacy-products")]
    public IActionResult GetRedirectPermanent()
    {
        return RedirectPermanent("/api/products/products-category");
    }

    [HttpGet("products-category")]
    public IActionResult GetProductsByCategory()
    {
    
        return Ok("This is a placeholder response for products in category: " );
    }
}
