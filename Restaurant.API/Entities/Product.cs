using Restaurant.API.Entities.Abstractions;

namespace Restaurant.API.Entities;

public sealed class Product : Entity
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string ImageUrl { get; set; }
    public decimal? OldPrice { get; set; } = 0.00M;
    public decimal Price { get; set; }
    public required ProductCategory Category { get; set; }
}
