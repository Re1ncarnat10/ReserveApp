using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ReserveApp.Models
{
  public class User : IdentityUser
  {
    [Required] public string Name { get; set; }
    [Required] public string Surname { get; set; }
    public string Department { get; set; }


    public User(string Name, string Surname, string Department)
    {
      this.Name = Name;
      this.Surname = Surname;
      this.Department = Department;
    }

    public User()
    {
    }
  }
}