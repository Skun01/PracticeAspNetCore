using System;
using System.ComponentModel.DataAnnotations;

namespace LearnWebApi.Entities;

public class Product
{
    [Key]
    public int Id { set; get; }
    [Required]
    public string? Name { set; get; }
    public decimal Price { set; get; }
    public List<OrderProduct>? OrderProducts { set; get; }
}
