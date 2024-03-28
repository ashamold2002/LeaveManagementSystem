
using LeaveManagementSystem.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LeaveManagementSystem.Data;
using LeaveManagementSystem.Models;
using System.Data;
using System.Runtime.Intrinsics.X86;
using System.Net.Mail;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IEmailService _emailService;

        public AdminController(AppDbContext context, IWebHostEnvironment environment, IEmailService emailService)
        {
            _context = context;
            _environment = environment;
            _emailService = emailService;
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateAdmin([FromForm] AdminDetails adminDetails)
        {

            // Generate a unique file name
            var uniqueFileName = $"{Guid.NewGuid()}_{adminDetails.AdminImage.FileName}";

            // Save the image to a designated folder (e.g., wwwroot/images)
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "Images");
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await adminDetails.AdminImage.CopyToAsync(stream);
            }

            // Store the file path in the database
            adminDetails.AdminImageName = uniqueFileName;


            _context.AdminDetails.Add(adminDetails);
            await _context.SaveChangesAsync();
            // var imageUrl = $"{Request.Scheme}://{Request.Host}/images/{cart.UniqueFileName}";

            // Return the image URL or any other relevant response
            return Ok();
        }

        [HttpPost]

        public async Task<ActionResult> CheckAdmin(AdminLogin adminLogin)
        {
            if (_context.AdminDetails.Any(s => s.AdminEmail == adminLogin.Email))
            {
                var checkAdmin = _context.AdminDetails.Where(s => s.AdminEmail == adminLogin.Email).FirstOrDefault();

                if (checkAdmin.AdminPassword == adminLogin.Password)
                {
                    return Ok(checkAdmin);
                }
            }
            return NotFound();

        }
        [Route("api/GetAdmin/{id}")]
        [HttpGet]
        public ActionResult<AdminDetails> GetIndividual(int id)
        {
            var admin = _context.AdminDetails.Find(id);
            if (admin == null)
            {
                return NotFound();
            }
            var admindetail = new AdminDetails
            {
                AdminId=admin.AdminId,
                AdminName =admin.AdminName,
                AdminImageName = String.Format("{0}://{1}{2}/wwwroot/images/{3}", Request.Scheme, Request.Host, Request.PathBase, admin.AdminImageName),
                
                AdminEmail = admin.AdminEmail,
               


            };
            //employee.EmployeeImageName = String.Format("{0}://{1}{2}/wwwroot/images/{3}", Request.Scheme, Request.Host, Request.PathBase, employee.EmployeeImageName);
            //employee.EmployeeImageName = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/wwwroot/images/{employee.EmployeeImageName}";
            //_context.EmployeeDetails.Add(employee);
            //_context.SaveChanges();


            return Ok(admindetail);
        }
        [HttpGet("{adminId}")]
        public IActionResult GetPendingRequests(int adminId)
        {
            // Query the database for pending leave requests associated with adminId
            var pendingRequests = _context.leaveDetails
                .Where(ld => ld.Status == "Pending" && ld.AdminDetails.AdminId == adminId)
                .Include(ld => ld.EmployeeDetails)
                .Include(ld => ld.LeaveTypes)
                .ToList();
            var leaveRequestDtos =pendingRequests.Select(ld => new
            {
                ld.LeaveId,
                ld.NoOfDays,
                ld.FromDate,
                ld.ToDate,
                ld.Reason,
                ld.Status,
                Empid=ld.EmployeeDetails?.Employee_id,
                EmployeeName = ld.EmployeeDetails?.EmployeeName,
                LeaveType = ld.LeaveTypes?.LeaveType,
                
            });
            return Ok(leaveRequestDtos);
        }
        [HttpGet("{adminId}")]
        public IActionResult GetApproveRequests(int adminId)
        {
            // Query the database for pending leave requests associated with adminId
            var pendingRequests = _context.leaveDetails
                .Where(ld => ld.Status == "Approved" && ld.AdminDetails.AdminId == adminId)
                .Include(ld => ld.EmployeeDetails)
                .Include(ld => ld.LeaveTypes)
                .ToList();
            var leaveRequestDtos = pendingRequests.Select(ld => new
            {

                ld.NoOfDays,
                ld.FromDate,
                ld.ToDate,
                ld.Reason,
                ld.Status,
                EmployeeName = ld.EmployeeDetails?.EmployeeName,
                LeaveType = ld.LeaveTypes?.LeaveType,

            });
            return Ok(leaveRequestDtos);
        }
        [HttpGet("{adminId}")]
        public IActionResult GetRejectRequests(int adminId)
        {
            // Query the database for pending leave requests associated with adminId
            var pendingRequests = _context.leaveDetails
                .Where(ld => ld.Status == "Rejected" && ld.AdminDetails.AdminId == adminId)
                .Include(ld => ld.EmployeeDetails)
                .Include(ld => ld.LeaveTypes)
                .ToList();
            var leaveRequestDtos = pendingRequests.Select(ld => new
            {

                ld.NoOfDays,
                ld.FromDate,
                ld.ToDate,
                ld.Reason,
                ld.Status,
                EmployeeName = ld.EmployeeDetails?.EmployeeName,
                LeaveType = ld.LeaveTypes?.LeaveType,

            });
            return Ok(leaveRequestDtos);
        }

        [HttpGet("{adminId}")]
        public IActionResult GetAllRequests(int adminId)
        {
            // Query the database for pending leave requests associated with adminId
            var pendingRequests = _context.leaveDetails
                .Where(ld => ld.AdminDetails.AdminId == adminId)
                .Include(ld => ld.EmployeeDetails)
                .Include(ld => ld.LeaveTypes)
                .ToList();
            var leaveRequestDtos = pendingRequests.Select(ld => new
            {

                ld.NoOfDays,
                ld.FromDate,
                ld.ToDate,
                ld.Reason,
                ld.Status,
                EmployeeName = ld.EmployeeDetails?.EmployeeName,
                LeaveType = ld.LeaveTypes?.LeaveType,

            });
            return Ok(leaveRequestDtos);
        }
        [HttpPost]
        public IActionResult ApproveRequest([FromBody]AdminStatusupdateDTO adminStatusupdate)
        {
            EmployeeDetails employeeDetails = _context.EmployeeDetails.FirstOrDefault(s=>s.Employee_id==adminStatusupdate.empid);

            var leaveRequest = _context.leaveDetails.FirstOrDefault(s=>s.LeaveId==adminStatusupdate.leaveid);
           

            if (leaveRequest == null)
                return NotFound();

            // Set status to "Approved"
            leaveRequest.Status = "Approved";
            LeaveDetails leaveDetails1 = new LeaveDetails()
            {
                
                Status = leaveRequest.Status,
               
               

            };
            _context.SaveChanges();

            // Send email notification to employee
            SendApprovedMail(leaveRequest,employeeDetails);

            return Ok("Request approved successfully.");
        }
        [HttpPost]
        public IActionResult RejectRequest([FromBody]AdminStatusupdateDTO adminStatusupdate)
        {
            EmployeeDetails employeeDetails = _context.EmployeeDetails.FirstOrDefault(s=>s.Employee_id==adminStatusupdate.empid);

            var leaveRequest = _context.leaveDetails.FirstOrDefault(s=>s.LeaveId==adminStatusupdate.leaveid);


            if (leaveRequest == null)
                return NotFound();

            // Set status to "Approved"
            leaveRequest.Status = "Rejected";
            LeaveDetails leaveDetails1 = new LeaveDetails()
            {

                Status = leaveRequest.Status,



            };
            _context.SaveChanges();

            // Send email notification to employee
            SendRejectedMail(leaveRequest, employeeDetails);

            return Ok("Request Rejected.");
        }
        [HttpGet("api/leave/history")]
        public IActionResult GetAllHistory(int adminId)
        {
            // Query the database for pending leave requests associated with adminId
            var pendingRequests = _context.leaveDetails
                .Where(ld => ld.AdminDetails.AdminId == adminId)
                .Include(ld => ld.EmployeeDetails)
                .Include(ld => ld.LeaveTypes)
                .ToList();
            var leaveRequestDtos = pendingRequests.Select(ld => new
            {

                ld.NoOfDays,
                ld.FromDate,
                ld.ToDate,
                ld.Reason,
                ld.Status,
                EmployeeName = ld.EmployeeDetails?.EmployeeName,
                LeaveType = ld.LeaveTypes?.LeaveType,

            });
            return Ok(pendingRequests);
        }




        private async Task SendApprovedMail(LeaveDetails leaveDetails,EmployeeDetails employeeDetails)
        {
            try
            {
                var name = employeeDetails.EmployeeName;

                var email = employeeDetails.EmployeeEmail;
                var days = leaveDetails.NoOfDays;
                var fromdate = leaveDetails.FromDate;
                var todate = leaveDetails.ToDate;
                var adminEmail = email; // Replace with actual admin email address
                var subject = $"Leave Approved for {name}";
                var body = $"Leave request details:\n\nEmployee Name: {name}\n\nStart Date:{fromdate} \nEnd Date:{todate}\nNo of days :{days}\n\n Your Leave request is Approved ";

                await _emailService.SendEmailAsync(adminEmail, subject, body);
                // Log success or handle any exceptions
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return;
            }
        }
        private async Task SendRejectedMail(LeaveDetails leaveDetails, EmployeeDetails employeeDetails)
        {
            try
            {
                var name = employeeDetails.EmployeeName;

                var email = employeeDetails.EmployeeEmail;
                var days = leaveDetails.NoOfDays;
                var fromdate = leaveDetails.FromDate;
                var todate = leaveDetails.ToDate;
                var adminEmail = email; // Replace with actual admin email address
                var subject = $"Leave Rejected for {name}";
                var body = $"Leave request details:\n\nEmployee Name: {name}\n\nStart Date:{fromdate} \nEnd Date:{todate}\nNo of days :{days}\n\n Your Leave request is Rejected ";

                await _emailService.SendEmailAsync(adminEmail, subject, body);
                // Log success or handle any exceptions
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return;
            }
        }





    }

}

