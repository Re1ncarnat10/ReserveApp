namespace ReserveApp.Dtos;

public class UserDto
{
  public string Id { get; set; }
  public string Name { get; set; }
  public string Surname { get; set; }
  public string Email { get; set; }
  public string Department { get; set; }
  
  public List<string> Roles { get; set; } = new List<string>();
}