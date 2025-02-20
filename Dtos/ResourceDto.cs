using System.ComponentModel.DataAnnotations;

namespace ReserveApp.Dtos;

public class ResourceDto
{
  public int ResourceId { get; set; }
  [Required] public string Name { get; set; }
  public string Description { get; set; }
  [Required] public string Type { get; set; }
  [Required] public string Image { get; set; }
  [Required] public bool Availability { get; set; }
}