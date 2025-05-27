using System.ComponentModel.DataAnnotations;

namespace ReserveApp.Models
{
    public class UserHistory
    {
        [Key]
        public int UserHistoryId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int ResourceId { get; set; }
        [Required]
        public string ResourceName { get; set; }
        [Required]
        public string ResourceDescription { get; set; }
        public string ResourceType { get; set; }
        public string ResourceImage { get; set; }
        [Required]
        public DateTime ApprovedAt { get; set; }
        [Required]
        public DateTime RentalStartTime { get; set; }
        [Required]
        public DateTime RentalEndTime { get; set; }
    }
}