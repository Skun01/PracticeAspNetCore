using System.ComponentModel.DataAnnotations;

namespace LearnWebApi.Entities;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public bool IsRevoked { get; set; } // Đã bị thu hồi hay chưa
}
