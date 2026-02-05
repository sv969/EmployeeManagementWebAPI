namespace SampleWebAPI.Repositories
{
    using SampleWebAPI.Models;
    public interface IEmployeeReadRepository
    {
        public Task<IEnumerable<Employee>> GetAllAsync();
        public Task<Employee?> GetByIdAsync(int id);
    }
}
