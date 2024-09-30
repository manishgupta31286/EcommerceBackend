
namespace ECommerce.ViewModels;

public record CartItem
{
	public int ProductId { get; set; }
	public required string ProductName { get; set; }
	public int Quantity { get; set; }
}

public interface IBank
{
	void Check();
}

public class Bank : IBank
{
	void IBank.Check()
	{
		throw new NotImplementedException();
	}
}