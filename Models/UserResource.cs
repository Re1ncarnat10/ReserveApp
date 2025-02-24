using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReserveApp.Models
{
  public class UserResource
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserResourceId { get; set; }

    [Required] public string UserId { get; set; }
    [ForeignKey("UserId")] public virtual User? User { get; set; }
    [Required] public int ResourceId { get; set; }
    [ForeignKey("ResourceId")] public virtual Resource? Resource { get; set; }
    [Required] public string Status { get; set; } = "Pending";
    [Required] public DateTime RentalStartTime { get; set; }
    [Required] public TimeSpan RentalDuration { get; set; }

    [NotMapped]
    public TimeSpan TimeRemaining
    {
      get
      {
        var endTime = RentalStartTime.Add(RentalDuration);
        return endTime - DateTime.Now;
      }
    }

    public UserResource()
    {
    }
  }
}