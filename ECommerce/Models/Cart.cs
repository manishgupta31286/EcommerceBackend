using System;
using System.Collections.Generic;

namespace ECommerce.Models;

public partial class Cart
{
    public int Id { get; set; }

    public Guid CartId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public Guid? RequestId { get; set; }
}
