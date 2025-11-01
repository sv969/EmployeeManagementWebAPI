namespace SampleWebAPI.Data
{
    using Microsoft.EntityFrameworkCore;
    using SampleWebAPI.Models;
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){ }
        public DbSet<Employee> employees { get; set; }
    }
}
