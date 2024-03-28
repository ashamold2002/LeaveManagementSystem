using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Models
{
    public class LeaveTypes
    {
        [Key]
        public int Id { get; set; }

        
        public string LeaveType { get; set; }

        //public ICollection<LeaveDetails> LeaveDetails { get; set; } = new List<LeaveDetails>();
    }
}
