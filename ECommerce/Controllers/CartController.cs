using ECommerce.Models;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CartController(EcommerceContext dbContext) : ControllerBase
	{
		private readonly EcommerceContext _dbContext = dbContext;
		[HttpGet("{cartid}")]
		public async Task<IActionResult> Get(Guid cartid)
		{
			if (cartid == Guid.Empty)
			{
				return NoContent();
			}
			var cartItems = await _dbContext.Carts
			.Where(x => x.CartId == cartid)
			.GroupBy(x => new { x.CartId, x.ProductId })
			.Select(x => new CartItem
			{
				ProductId = x.Key.ProductId,
				ProductName = string.Empty,
				Quantity = x.Sum(x => x.Quantity)
			})
			.Join(_dbContext.Products,
				cartItem => cartItem.ProductId,
				product => product.Id,
				(cartItem, product) => new CartItem
				{
					ProductId = cartItem.ProductId,
					ProductName = product.Name,
					Quantity = cartItem.Quantity
				}).ToListAsync();

			return Ok(cartItems.Where(x => x.Quantity > 0));
		}

		[HttpPost]
		public async Task<IActionResult> Add([FromBody] Cart cart)
		{
			if (cart.CartId == Guid.Empty)
			{
				cart.CartId = Guid.NewGuid();
			}

			_ = await _dbContext.Carts.AddAsync(cart);
			await _dbContext.SaveChangesAsync();
			return Ok(cart.CartId);
		}
	}
}