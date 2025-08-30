using System;

namespace LearnWebApi.Entities;

public class ApiKey
{
    public int Id { get; set; }
    public string Key { get; set; } // Stored as a hashed value
    public string Owner { get; set; } // Application or User identifier
    public DateTime Created { get; set; }
    public bool IsActive { get; set; }
}
