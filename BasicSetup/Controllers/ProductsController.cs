using Microsoft.AspNetCore.Mvc;

namespace BasicSetup.Controllers;

[ApiController]
[Route("api/products")]
// [Route("api/[controller]")]
public class ProductsController : ControllerBase
{

    [HttpGet]
    public  string Get()
    {
        return "Product #1, Price: $100";
    }
}