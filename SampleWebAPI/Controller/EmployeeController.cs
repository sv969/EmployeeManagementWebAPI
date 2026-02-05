using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using SampleWebAPI.Data;
using SampleWebAPI.Models;
using SampleWebAPI.Caching;
using Microsoft.EntityFrameworkCore;
using SampleWebAPI.Repositories;
using SampleWebAPI.DTOs;


namespace SampleWebAPI.Controller
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiversion}/[controller]")]

    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly IEmployeeReadRepository _employeeReadRepository;

        public EmployeeController(AppDbContext appDbContext, IMemoryCache memoryCache, IEmployeeReadRepository employeeReadRepository)
        {
            _context = appDbContext;
            _memoryCache = memoryCache;
            _employeeReadRepository = employeeReadRepository;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult> GetEmployees()
        {
            if (_memoryCache.TryGetValue(CacheKeys.employeeListKey, out List<Employee>? cachedEmployees))
            {
                return Ok(new { success = true, message = "Employees list.", data = cachedEmployees });
            }

            var employeeList = (await _employeeReadRepository.GetAllAsync()).ToList();
            if (employeeList == null || employeeList.Count == 0)
            {
                return BadRequest(new { success = false, message = "Employees not found" });
            }
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2),
            };

            _memoryCache.Set(CacheKeys.employeeListKey, employeeList, cacheEntryOptions);
            var result = employeeList.Select(emp => new EmployeeReadDto
            {
                ID = emp.ID,
                Name = emp.Name,
                Department = emp.Department,
                Salary = emp.Salary
            }).ToList();
            return Ok(new { success = true, message = "Employees successfully loaded.", data = result });
        }

        [HttpGet("GetByID/{id}")]
        public async Task<IActionResult> EmployeeGetByID(int id)
        {
            var cacheKey = CacheKeys.EmployeeKey(id);
            if (_memoryCache.TryGetValue(cacheKey, out Employee? cachedEmployee))
            {
                return Ok(new { success = true, message = "Got the details from cache", data = cachedEmployee });
            }

            var employeeDetails = await _employeeReadRepository.GetByIdAsync(id);

            if (employeeDetails == null)
            {
                return NotFound(new { success = false, message = "Not found" });
            }
            _memoryCache.Set(cacheKey, employeeDetails, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2),
            });

            var result = new EmployeeReadDto
            {
                ID = employeeDetails.ID,
                Name = employeeDetails.Name,
                Department = employeeDetails.Department,
                Salary = employeeDetails.Salary
            };
            return Ok(new { success = true, message = "Got the details", data = result });
        }

        [HttpPost("Add")]
        public async Task<ActionResult> PostEmployee([FromBody] EmployeeCreateDto emp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Employee invalid Data" });
            }
            // Create new Employee entity
            var newEmp = new Employee
            {
                Name = emp.Name,
                Salary = emp.Salary,
                Department = emp.Department
            };
            _context.employees.Add(newEmp);
            await _context.SaveChangesAsync();

            _memoryCache.Remove(CacheKeys.employeeListKey);

            var result = new EmployeeReadDto
            {
               ID = newEmp.ID,
                Name = newEmp.Name,
                Department = newEmp.Department,
                Salary = newEmp.Salary
            };

            return CreatedAtAction(nameof(GetEmployees), new { id = result.ID }, new { success = true, message = "Employee created successsfully.", data = result });

        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateEmployee([FromBody] EmployeeUpdateDto emp)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                   var exstingEmployee = await _context.employees.FindAsync(emp.ID);
                    if (exstingEmployee == null)
                    {
                        BadRequest(new { success = false, message = "Employee invalid Data" });
                    }
                    exstingEmployee.Name = emp.Name;
                    exstingEmployee.Salary = emp.Salary;
                    exstingEmployee.Department = emp.Department;

                    _context.employees.Update(exstingEmployee);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    _memoryCache.Remove(CacheKeys.employeeListKey);
                    _memoryCache.Remove(CacheKeys.EmployeeKey(emp.ID));

                    var result = new EmployeeReadDto
                    {
                        ID = exstingEmployee.ID,
                        Name = exstingEmployee.Name,
                        Department = exstingEmployee.Department,
                        Salary = exstingEmployee.Salary
                    };

                    return Ok(new { success = true, message = "Employee updated successsfully.", data = result });
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
            //var exstingEmployee = await _context.employees.FindAsync(emp.ID);
            //if (exstingEmployee == null)
            //{
            //    return BadRequest(new { success = false, message = "Employee invalid Data" });
            //}

            //exstingEmployee.Name = emp.Name;
            //exstingEmployee.Salary = emp.Salary;
            //exstingEmployee.Department = emp.Department;

            //_context.employees.Update(exstingEmployee);
            //await _context.SaveChangesAsync();

            //_memoryCache.Remove(CacheKeys.employeeListKey);

            //return Ok(new { success = true, message = "Employee created successsfully.", data = exstingEmployee });
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var emp = await _context.employees.FindAsync(id);
            if (emp == null)
            {
                return NotFound();
            }
            _context.employees.Remove(emp);
            await _context.SaveChangesAsync();

            _memoryCache.Remove(CacheKeys.employeeListKey);
            _memoryCache.Remove(CacheKeys.EmployeeKey(emp.ID));

            return Ok(new { message = $"Delete employee {id} Successfully..." });
        }
    }
}
