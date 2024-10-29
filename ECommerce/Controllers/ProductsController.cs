using ECommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Controllers
{
	[ApiController]
	[Authorize]
	[Route("api/[controller]")]
	public class ProductsController(EcommerceContext dbContext, ILogger<ProductsController> logger) : ControllerBase
	{
		private readonly EcommerceContext _dbContext = dbContext;
		private readonly ILogger<ProductsController> _logger = logger;
		[HttpGet]
		[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
		public async Task<IActionResult> Get()
		{
			_logger.LogInformation("This is an information log.");
			_logger.LogWarning("This is a warning log.");
			_logger.LogError("This is an error log.");
			var products = await _dbContext.Products.ToListAsync();
			return Ok(products);
		}

		[HttpGet("GetByCategoryId/{id}")]
		public async Task<IActionResult> GetByCategoryId([FromRoute] int id)
		{
			var products = await _dbContext.Products.Where(x => x.CategoryId == id).ToListAsync();
			return Ok(products);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetByProductId([FromRoute] int id)
		{
			var product = await _dbContext.Products.FindAsync(id);
			return Ok(product);
		}
	}
}