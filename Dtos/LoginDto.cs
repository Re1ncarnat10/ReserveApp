using System.ComponentModel.DataAnnotations;

namespace ReserveApp.Dtos;

public class LoginDto
{
  [Required]
  [EmailAddress]
  public string Email { get; set; }
  [Required]
  [MinLength(6), MaxLength(30)]
  public string Password { get; set; }
}