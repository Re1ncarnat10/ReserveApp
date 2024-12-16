using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ReserveApp.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Email { get; set; }
        public string Department { get; set; }
        public string WorkCity { get; set; }
        
        public User (string Name, string Surname, string Email, string Department, string WorkCity)
        {
            this.Name = Name;
            this.Surname = Surname;
            this.Email = Email;
            this.Department = Department;
            this.WorkCity = WorkCity;
        }
        public User()
        {
        }
    }
}