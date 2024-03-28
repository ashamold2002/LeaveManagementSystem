using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Models
{
    public class RoleBasedAdmin
    {
        [Key]
        public int Id { get; set; }

        public string DesignationName { get; set; }

        public AdminDetails Details { get; set; }
    }
}
