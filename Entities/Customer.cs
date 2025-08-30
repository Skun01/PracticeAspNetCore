using System;
using System.ComponentModel.DataAnnotations;

namespace LearnWebApi.Entities;

public class Customer
{
    [Key]
    public int Id { set; get; }
    public required string FirstName { set; get; }
    public required string LastName { set; get; }
    public required string Email { set; get; }
    public DateTime? DateOfBirth { set; get; }
    public string Role { set; get; } = "User";
    public string PasswordHash { set; get; } = string.Empty;
}
