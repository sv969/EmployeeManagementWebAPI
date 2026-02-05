namespace SampleWebAPI.Data
{
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using System.Data;
    public class DapperDBContext
    {
        private readonly IConfiguration _configuration;
        public DapperDBContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
       
    }
}
