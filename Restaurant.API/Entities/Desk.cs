using Restaurant.API.Entities.Abstractions;

namespace Restaurant.API.Entities;

public sealed class Desk : Entity
{
    public required string Name { get; set; }
}
