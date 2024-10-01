using ECommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CategoryController(EcommerceContext dbContext) : ControllerBase
	{
		private readonly EcommerceContext _dbContext = dbContext;
		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var categories = await _dbContext.Categories.ToListAsync();
			return Ok(categories);
		}

		[HttpPost]
		public async Task<IActionResult> Post()
		{
			var categories = await _dbContext.Categories.ToListAsync();
			return Ok(categories);
		}
	}
}