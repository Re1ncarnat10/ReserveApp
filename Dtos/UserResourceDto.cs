namespace ReserveApp.Dtos;

public class UserResourceDto
{
  public int UserResourceId { get; set; }
  public string UserId { get; set; }
  public int ResourceId { get; set; }
  public string Status { get; set; }
  public DateTime RentalStartTime { get; set; }
  public DateTime RentalEndTime { get; set; }
}