using Microsoft.AspNetCore.Mvc;
using SampleWebAPI.Data;
using SampleWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace SampleWebAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController: ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            var employeesList = await _context.employees.ToListAsync();
            if(employeesList == null || employeesList.Count == 0 )
            {
                return BadRequest(new {success = false, message = "Employees not found" });
            }

            return Ok(new { success = true, message = "Employees successfully loaded.", data = employeesList });
        }

        [HttpGet("GetByID/{id}")]
        public async Task<IActionResult> EmployeeGetByID(int id)
        {
            var employeeDetails = await _context.employees.FindAsync(id);

            if(employeeDetails == null)
            {
                return NotFound(new { success = false, message = "Not found" });
            }
            return Ok(new { success = true, message = "Got the details", data = employeeDetails });
        }

        [HttpPost("Add")]
        public async Task<ActionResult<Employee>> PostEmployee([FromBody] Employee emp)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Employee invalid Data" });
            }
            _context.employees.Add(emp);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEmployees), new { id = emp.ID }, new { success = true, message = "Employee created successsfully.", data = emp });

        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateEmployee([FromBody] Employee emp)
        {
            var exstingEmployee = await _context.employees.FindAsync(emp.ID);
            if (exstingEmployee == null)
            {
                return BadRequest(new { success = false, message = "Employee invalid Data" });
            }

            exstingEmployee.Name = emp.Name;
            exstingEmployee.Salary = emp.Salary;
            exstingEmployee.Department = emp.Department;

            _context.employees.Update(exstingEmployee);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Employee created successsfully.", data = exstingEmployee });
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var emp =await  _context.employees.FindAsync(id);
            if(emp == null)
            {
                return NotFound();
            }
            _context.employees.Remove(emp);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Delete employee {id} Successfully..." });
        }
    }
}
