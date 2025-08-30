using System;
using System.ComponentModel.DataAnnotations;

namespace LearnWebApi.Entities;

public class Order
{
    [Key]
    public int Id { set; get; }
    public DateTime OrderDate { set; get; } = DateTime.UtcNow;
    public List<OrderProduct>? OrderProducts { set; get; }
    
}
