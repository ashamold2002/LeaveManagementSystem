using Microsoft.EntityFrameworkCore;

using LeaveManagementSystem.Models;

namespace LeaveManagementSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<EmployeeDetails> EmployeeDetails { get; set; }
        public DbSet<AdminDetails> AdminDetails { get; set; }
        public DbSet<LeaveAllocated> leaveAllocateds { get; set; }
        public  DbSet<LeaveDetails> leaveDetails { get; set; }
        public DbSet<LeaveTypes> leaveTypes { get; set; }

        public DbSet<RoleBasedAdmin> roleBasedAdmins { get; set; }
        
       
    }
}
