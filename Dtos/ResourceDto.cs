using System.ComponentModel.DataAnnotations;

namespace ReserveApp.Dtos;

public class ResourceDto
{
 public int ResourceId { get; set; }
 [Required]
 public string Name { get; set; }
 public string Description { get; set; }
 [Required]
 public string Type { get; set; }
 [Required]
 public int Quantity { get; set; }
 public int QuantityAvailable { get; set; }
}