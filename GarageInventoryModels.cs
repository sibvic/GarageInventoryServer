using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class Project
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; }
}

public class Location
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
}

public enum ItemStatus {
    Sold,
    Used,
    InStock
}

public class Item
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? ManufacturerNumber { get; set; }
    // Foreign key for Location
    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;
    public string? SKU { get; set; }
    public decimal? InPrice { get; set; }
    public DateTime? InDate { get; set; }
    public decimal? OutPrice { get; set; }
    public DateTime? OutDate { get; set; }
    public ItemStatus Status { get; set; }
    public string? Description { get; set; }
    // Foreign key for Project
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}

public class GarageInventoryContext : DbContext
{
    public GarageInventoryContext(DbContextOptions<GarageInventoryContext> options) : base(options) {}
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Item> Items => Set<Item>();
}
