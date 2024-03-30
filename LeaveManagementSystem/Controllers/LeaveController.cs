using LeaveManagementSystem.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using LeaveManagementSystem.Models;
using System.Data;
using System.Runtime.Intrinsics.X86;
using System.Net.Mail;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static Mysqlx.Notice.Warning.Types;

namespace LeaveManagementSystem.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IEmailService _emailService;

        public LeaveController(AppDbContext context, IWebHostEnvironment environment, IEmailService emailService)
        {
            _context = context;
            _environment = environment;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromForm] EmployeeDetails employeeDetails)
        {

            // Generate a unique file name
            var uniqueFileName = $"{Guid.NewGuid()}_{employeeDetails.EmployeeImage.FileName}";

            // Save the image to a designated folder (e.g., wwwroot/images)
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "Images");
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await employeeDetails.EmployeeImage.CopyToAsync(stream);
            }

            // Store the file path in the database
            employeeDetails.EmployeeImageName = uniqueFileName;


            _context.EmployeeDetails.Add(employeeDetails);
            await _context.SaveChangesAsync();
            // var imageUrl = $"{Request.Scheme}://{Request.Host}/images/{cart.UniqueFileName}";

            // Return the image URL or any other relevant response
            return Ok();
        }

        [Route("{id}")]
        [HttpGet]
        public ActionResult<EmployeeDetails> GetIndividual(int id)
        {
            var employee = _context.EmployeeDetails.Find(id);
            if (employee == null)
            {
                return NotFound();
            }
            var emp = new EmployeeDetails
            {
                Employee_id = employee.Employee_id,
                EmployeeName = employee.EmployeeName,
                EmployeeImageName = String.Format("{0}://{1}{2}/wwwroot/images/{3}", Request.Scheme, Request.Host, Request.PathBase, employee.EmployeeImageName),
                EmployeeDesignation = employee.EmployeeDesignation,
                EmployeeEmail = employee.EmployeeEmail,
                EmployeeGender = employee.EmployeeGender,


            };
            //employee.EmployeeImageName = String.Format("{0}://{1}{2}/wwwroot/images/{3}", Request.Scheme, Request.Host, Request.PathBase, employee.EmployeeImageName);
            //employee.EmployeeImageName = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/wwwroot/images/{employee.EmployeeImageName}";
            //_context.EmployeeDetails.Add(employee);
            //_context.SaveChanges();


            return Ok(emp);
        }

        [Route("{id}")]
        [HttpGet]
        public ActionResult<LeaveAllocated> GetAllotedLeave(int id)
        {
            var allotment=_context.leaveAllocateds.FirstOrDefault(s=>s.EmployeeDetails.Employee_id== id);

            if (allotment == null)
            {
                return NotFound();
            }
            
            return Ok(allotment);
        }




        [HttpPost]

        public async Task<ActionResult> CheckEmployee(EmployeeLogin employeeLogin)
        {
            if (_context.EmployeeDetails.Any(s => s.EmployeeEmail == employeeLogin.Email))
            {
                var checkEmployee = _context.EmployeeDetails.FirstOrDefault(s => s.EmployeeEmail == employeeLogin.Email);

                if (checkEmployee.EmployeePassword == employeeLogin.Password)
                {
                    return Ok(checkEmployee);

                }
            }
            return NotFound();


        }



        [HttpPost]
        public async Task<IActionResult> ApplyLeave([FromBody] LeaveRequestDTO leaveDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Set default status to "Pending"
            leaveDetails.Status = "Pending";

            // Retrieve employee details using the provided employeeId
            EmployeeDetails employeeDetails = _context.EmployeeDetails.FirstOrDefault(s => s.Employee_id == Convert.ToInt16(leaveDetails.empid));


            string employeeDesignation = employeeDetails.EmployeeDesignation;



            if (employeeDetails == null)
            {
                return BadRequest("Invalid employee ID.");
            }

            AdminDetails adminDetails = null;

            switch (employeeDesignation)
            {
                case "Trainee Software Developer":
                    adminDetails = _context.AdminDetails.Find(3);
                    break;

                case "Trainer":
                    adminDetails = _context.AdminDetails.Find(1);
                    break;

                // Add more cases if needed for other designations

                default:
                    return BadRequest("Unknown employee designation.");
            }

            LeaveTypes leaveTypes = _context.leaveTypes.FirstOrDefault(s => s.Id == Convert.ToInt16(leaveDetails.LeaveTypes_id));
            //AdminDetails adminDetails = _context.AdminDetails.Find(leaveDetails.Admin_id);

            // Add leave details to the database
            LeaveDetails leaveDetails1 = new LeaveDetails()
            {
                LeaveId = 0,
                NoOfDays = leaveDetails.NoOfDays,
                FromDate = leaveDetails.FromDate,
                ToDate = leaveDetails.ToDate,
                Reason = leaveDetails.Reason,
                Status = leaveDetails.Status,
                EmployeeDetails = employeeDetails,
                LeaveTypes = leaveTypes,
                AdminDetails = adminDetails
            };

            // Validate the leave request and update allocated leave
            bool isValid = ValidateAndUpdateLeave(leaveDetails1);

            if (!isValid)
            {
                return BadRequest("Invalid leave request or insufficient allocated leave.");
            }

            _context.leaveDetails.Add(leaveDetails1);
            await _context.SaveChangesAsync();
            await SendEmailToAdminAsync(leaveDetails, adminDetails, employeeDetails, leaveTypes);

            return Ok(leaveDetails1);
        }
        // Validate leave request and update allocated leave
        private bool ValidateAndUpdateLeave(LeaveDetails leaveRequest)
        {
            // Get the employee ID from the leave request
            int employeeId = leaveRequest.EmployeeDetails.Employee_id;


            // Retrieve the allocated leave details for the employee
            var allocatedLeave = _context.leaveAllocateds.FirstOrDefault(l => l.EmployeeDetails.Employee_id == employeeId);

            if (allocatedLeave == null)
            {
                return false; // Employee not found in allocated leave records
            }

            // Check if the requested leave type is valid (e.g., CL, ML, PL, etc.)
            string requestedLeaveType = leaveRequest.LeaveTypes.LeaveType;
            if (!new[] { "CL", "ML", "LOP", "PL", "SickLeave", "Permission", "OnDuty" }.Contains(requestedLeaveType))
            {
                return false; // Invalid leave type
            }

            // Check if the requested leave days exceed the allocated leave
            int requestedLeaveDays = leaveRequest.NoOfDays;
            if (requestedLeaveDays > GetAllocatedLeave(allocatedLeave, requestedLeaveType))
            {
                return false; // Insufficient allocated leave
            }
            else if (requestedLeaveDays < 0)
            {
                return false;
            }
            else
            {
                // Update the allocated leave (decrease by requested days)
                UpdateAllocatedLeave(allocatedLeave, requestedLeaveType, requestedLeaveDays);

                return true; // Leave request validated and allocated leave updated
            }
        }

        // Helper method to get allocated leave for a specific type
        private int GetAllocatedLeave(LeaveAllocated allocatedLeave, string leaveType)
        {
            switch (leaveType)
            {
                case "CL": return allocatedLeave.CL;
                case "LOP": return allocatedLeave.LOP;
                case "ML": return allocatedLeave.ML;
                case "PL": return allocatedLeave.PL;
                case "SickLeave": return allocatedLeave.SickLeave;
                case "Permission": return allocatedLeave.Permission;
                case "OnDuty": return allocatedLeave.OnDuty;
                default: return 0;
            }
        }

        // Helper method to update allocated leave
        private void UpdateAllocatedLeave(LeaveAllocated allocatedLeave, string leaveType, int requestedDays)
        {
            switch (leaveType)
            {
                case "CL": allocatedLeave.CL -= requestedDays; break;
                case "ML": allocatedLeave.ML -= requestedDays; break;
                case "PL": allocatedLeave.PL -= requestedDays; break;
                case "SickLeave": allocatedLeave.SickLeave -= requestedDays; break;
                case "Permission": allocatedLeave.Permission -= requestedDays; break;
            }
        }
        private async Task SendEmailToAdminAsync(LeaveRequestDTO leaveDetails, AdminDetails adminDetails, EmployeeDetails employeeDetails, LeaveTypes leaveTypes)
        {
            try
            {
                var name = employeeDetails.EmployeeName;
                var leave = leaveTypes.LeaveType;
                var email = adminDetails.AdminEmail;
                var days = leaveDetails.NoOfDays;
                var fromdate = leaveDetails.FromDate;
                var todate = leaveDetails.ToDate;
                var adminEmail = email; // Replace with actual admin email address
                var subject = $"Leave Request from Employee {name}";
                var body = $"Leave request details:\n\nEmployee Name: {name}\nLeave Type:{leave}\nStart Date:{fromdate} \nEnd Date:{todate}\nNo of days :{days} ";

                await _emailService.SendEmailAsync(adminEmail, subject, body);
                // Log success or handle any exceptions
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                return;
            }
        }

        //[HttpGet("EmployeeLeaveRequests/{employeeId}")]
        //public async Task<IActionResult> GetEmployeeLeaveRequests(int employeeId)
        //{
        //    var employeeLeaveRequests = _context.leaveDetails
        //        .Where(ld => ld.EmployeeDetails.Employee_id == employeeId)
        //        .ToList();

        //    if (employeeLeaveRequests.Count == 0)
        //    {
        //        return NotFound($"No leave requests found for employee with ID {employeeId}.");
        //    }

        //    return Ok(employeeLeaveRequests);
        //}
        [HttpGet("EmployeeLeaveRequests/{employeeId}")]
        public async Task<IActionResult> GetEmployeeLeaveRequests(int employeeId)
        {
            var employeeLeaveRequests = await _context.leaveDetails
                .Where(ld => ld.EmployeeDetails.Employee_id == employeeId)
                .Include(ld => ld.EmployeeDetails)
                .Include(ld => ld.LeaveTypes) // Include related LeaveTypes
                .Include(ld => ld.AdminDetails) // Include related AdminDetails
                .ToListAsync();



            // Map to a simplified DTO (Data Transfer Object) if needed
            var leaveRequestDtos = employeeLeaveRequests.Select(ld => new
            {
                ld.LeaveId,
                ld.NoOfDays,
                ld.FromDate,
                ld.ToDate,
                ld.Reason,
                ld.Status,
                EmployeeName = ld.EmployeeDetails?.EmployeeName,
                LeaveType = ld.LeaveTypes?.LeaveType,
                AdminName = ld.AdminDetails?.AdminName
            });

            return Ok(leaveRequestDtos);
        }


        //[Route("api/DeleteLeaveDetails/{id}")]
        //[HttpDelete]
        //public async Task<IActionResult> DeleteLeaveRequest(int id, int employeeid,LeaveDetails leaveDetails)
        //{

        //    if (id < 1)
        //        return BadRequest("Invalid leave request ID.");

        //    // Retrieve the leave details (you can adapt this based on your data access method)
        //    leaveDetails = await _context.leaveDetails.FirstOrDefaultAsync(ld => ld.LeaveId == id);
        //    if (leaveDetails == null)
        //        return NotFound("Leave request not found.");

        //    // Get the allocated leave for the employee
        //    var allocatedLeave = await _context.leaveAllocateds.FirstOrDefaultAsync(la => la.EmployeeDetails.Employee_id == employeeid);
        //    if (allocatedLeave == null)
        //        return NotFound("Allocated leave not found for the employee.");

        //    // Get the requested leave type and days
        //    string requestedLeaveType = leaveDetails.LeaveTypes.LeaveType;
        //    int requestedLeaveDays = leaveDetails.NoOfDays;

        //    // Update the allocated leave based on the leave type and days
        //    UpdateAllocatedLeave_Delete(allocatedLeave, requestedLeaveType, requestedLeaveDays);

        //    // Remove the leave request
        //    _context.leaveDetails.Remove(leaveDetails);
        //    await _context.SaveChangesAsync();

        //    return Ok("Leave request deleted successfully.");
        //}

        // Assuming you have a function UpdateAllocatedLeave_Delete that updates the allocated leave
        // based on the leave type and number of days, here's an example of how you can implement it:

        [HttpPost]
        public IActionResult CancelRequest([FromBody] UpdateStatusDTO updateStatus)
        {
            //EmployeeDetails employeeDetails = _context.EmployeeDetails.Find(empid);

            var leaveRequest = _context.leaveDetails.FirstOrDefault(s => s.LeaveId ==updateStatus.leaveid);


            if (leaveRequest == null)
                return NotFound();

            // Set status to "Approved"
            leaveRequest.Status = "Cancelled";
            LeaveDetails leaveDetails1 = new LeaveDetails()
            {

                Status = leaveRequest.Status,


            };
            _context.SaveChanges();




            return Ok("Request cancelled successfully.");
        }






        private void UpdateAllocatedLeave_Delete(LeaveAllocated allocatedLeave, string leaveType, int leaveDays)
        {
            switch (leaveType)
            {
                case "CL": allocatedLeave.CL += leaveDays; break;
                case "ML": allocatedLeave.ML += leaveDays; break;
                case "PL": allocatedLeave.PL += leaveDays; break;
                case "SickLeave": allocatedLeave.SickLeave += leaveDays; break;
                case "Permission": allocatedLeave.Permission += leaveDays; break;
            }

        }
        [HttpGet("{id}")]
        public ActionResult<LeaveDetails> GetIndividualLeave(int id)
        {
            var leave = _context.leaveDetails.Find(id);
            if (leave == null)
            {
                return NotFound();
            }
            var leavebyid = new LeaveDetails
            {
                LeaveId = leave.LeaveId,



            };

            return Ok(leavebyid);

        }

        [HttpGet("{empId}")]
        public IActionResult GetLeaveApplyHistory(int empId)
        {
            // Query the database for pending leave requests associated with adminId
            var pendingRequests = _context.leaveDetails
                .Where(ld => (ld.LeaveTypes.LeaveType=="CL"|| ld.LeaveTypes.LeaveType == "pL"|| ld.LeaveTypes.LeaveType == "LOP"|| ld.LeaveTypes.LeaveType == "SickLeave"|| ld.LeaveTypes.LeaveType == "ML") && ld.EmployeeDetails.Employee_id == empId)
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
                
                LeaveType = ld.LeaveTypes?.LeaveType,

            });
            return Ok(leaveRequestDtos);
        }

        [HttpGet("{empId}")]
        public IActionResult GetPermissionHistory(int empId)
        {
            // Query the database for pending leave requests associated with adminId
            var pendingRequests = _context.leaveDetails
                .Where(ld => ld.LeaveTypes.LeaveType == "Permission" && ld.EmployeeDetails.Employee_id == empId)
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

                LeaveType = ld.LeaveTypes?.LeaveType,

            });
            return Ok(leaveRequestDtos);
        }

        [HttpGet("{empId}")]
        public IActionResult GetOnDutyHistory(int empId)
        {
            // Query the database for pending leave requests associated with adminId
            var pendingRequests = _context.leaveDetails
                .Where(ld => ld.LeaveTypes.LeaveType == "OnDuty" && ld.EmployeeDetails.Employee_id == empId)
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

                LeaveType = ld.LeaveTypes?.LeaveType,

            });
            return Ok(leaveRequestDtos);
        }



    }
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }

    // Implement your email service (e.g., using SmtpClient)
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {

            using (var client = new SmtpClient("smtp-mail.outlook.com"))
            {
                client.Port = 587;
                client.Credentials = new NetworkCredential("ashamold2002@gmail.com", "AshaShneha_._2705");
                client.EnableSsl = true;

                var message = new MailMessage
                {
                    From = new MailAddress("ashamold2002@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };
                message.To.Add(toEmail);

                await client.SendMailAsync(message);
            }
        }
    }
}

