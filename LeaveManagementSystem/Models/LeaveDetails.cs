using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Models
{
    public class LeaveDetails
    {
        [Key]
        public int LeaveId { get; set; }

       
        [Required]
        public int NoOfDays { get; set; }

        [Required]
        public DateOnly FromDate { get; set; }

        [Required]
        public DateOnly ToDate { get; set;}

        [Required]
        public string? Reason { get; set; }

        [DefaultValue("Pending")]
        public string? Status { get; set; }

        public EmployeeDetails EmployeeDetails { get; set; }
        

        public LeaveTypes LeaveTypes { get; set; }

        public AdminDetails AdminDetails { get; set; }
        
    }
}
