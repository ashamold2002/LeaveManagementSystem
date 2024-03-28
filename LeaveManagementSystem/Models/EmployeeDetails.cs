using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaveManagementSystem.Models
{
    public class EmployeeDetails
    {
        [Key]
        public int Employee_id {  get; set; }

        
        public string? EmployeeName { get; set; }

        [NotMapped]
        public IFormFile? EmployeeImage { get; set; }

        public string? EmployeeImageName { get; set; }

        

        
        public string? EmployeeDesignation {  get; set; }

        [EmailAddress]
        public string? EmployeeEmail {  get; set; }

        [PasswordPropertyText]
        public string? EmployeePassword { get; set;}

        public string EmployeeGender { get; set; }



        //public ICollection<LeaveDetails> LeaveDetails { get; set; } = new List<LeaveDetails>();
    }
}
