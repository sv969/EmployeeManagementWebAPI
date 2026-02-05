namespace SampleWebAPI.Repositories
{
    using Dapper;
    using SampleWebAPI.Data;
    using SampleWebAPI.Models;
    public class EmployeeReadRepository: IEmployeeReadRepository
    {
        public readonly DapperDBContext _context;

        public EmployeeReadRepository(DapperDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            var query = "SELECT Id, Name, Department, Salary FROM Employees";
            using (var connection = _context.CreateConnection())
            {
                var employees = await connection.QueryAsync<Employee>(query);
                return employees;
            }
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            var query = "SELECT ID, Name, Department, Salary FROM Employees WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var employee = await connection.QuerySingleOrDefaultAsync<Employee>(query, new { Id = id });
                return employee;
            }
        }

    }
}
