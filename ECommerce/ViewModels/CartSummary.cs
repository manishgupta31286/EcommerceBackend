
namespace ECommerce.ViewModels;

public record CartItem
{
	public int ProductId { get; set; }
	public required string ProductName { get; set; }
	public int Quantity { get; set; }
}