using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LeaveManagementSystem.Models
{
    public class LeaveRequestDTO
    {
        public int LeaveId {  get; set; }
        public int NoOfDays { get; set; }

        [Required]
        public DateOnly FromDate { get; set; }

        [Required]
        public DateOnly ToDate { get; set; }

        [Required]
        public string? Reason { get; set; }

        [DefaultValue("Pending")]
        public string? Status { get; set; }

        

        public string LeaveTypes_id { get; set; }

        public string empid { get; set; }

        

    }
}
