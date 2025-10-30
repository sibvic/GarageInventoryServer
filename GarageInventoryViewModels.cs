using System.ComponentModel.DataAnnotations;
using GarageInventoryServer;

public class ProjectCreateModel
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public decimal? Price { get; set; }
}

public class ItemCreateModel
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? ManufacturerNumber { get; set; }
    public int LocationId { get; set; }
    public decimal? Price { get; set; }
    public string? SKU { get; set; }
    public decimal? InPrice { get; set; }
    public decimal? OutPrice { get; set; }
    public ItemStatus Status { get; set; }
    public string? Description { get; set; }
    public int ProjectId { get; set; }
}
