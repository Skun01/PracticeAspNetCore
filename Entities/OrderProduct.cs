using System;

namespace LearnWebApi.Entities;

public class OrderProduct
{
    public int ProductId { set; get; }
    public required Product Product { set; get; }
    public int OrderId { set; get; }
    public required Order Order { set; get; }
    public int Quantity { get; set; }
}
