using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaveManagementSystem.Models
{
    public class AdminDetails
    {
        [Key]
        public int AdminId { get; set; }

        [Required]
        public string? AdminName { get; set; }

        [NotMapped]
        public IFormFile? AdminImage { get; set; }

        public string? AdminImageName { get; set; }

        [EmailAddress]
        public string? AdminEmail { get; set; }

        [PasswordPropertyText]
        public string? AdminPassword { get; set;}

       // public ICollection<LeaveDetails> LeaveDetails { get; set; }
    }
}
