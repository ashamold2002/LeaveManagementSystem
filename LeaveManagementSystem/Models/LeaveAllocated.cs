using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Models
{
    public class LeaveAllocated
    {
        [Key]
        public int Id { get; set; }

        [Range(2,int.MaxValue)]
        public int CL { get; set; }

        public int LOP { get; set; }


        [Range(80,int.MaxValue)]
        public int ML { get; set; }


        [Range(8,int.MaxValue)]
        public int PL { get; set; }

        [Range(8, int.MaxValue)]
        public int SickLeave { get; set; }

        [Range(5,int.MaxValue)]
        public int Permission {  get; set; }

        public int OnDuty { get; set; }

        public EmployeeDetails EmployeeDetails { get; set; }
        


    }
}
