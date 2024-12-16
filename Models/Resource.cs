using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ReserveApp.Data;

namespace ReserveApp.Models;

public class Resource
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ResourceId { get; set; }
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
    [Required]
    public string Type { get; set; }
    [Required]
    public string Location { get; set; }
    [Required]
    public int Quantity { get; set; }
    
    public virtual ICollection<UserResource> UserResources { get; set; }

    public Resource()
    {
        UserResources = new HashSet<UserResource>();
    }
}