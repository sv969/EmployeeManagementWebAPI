namespace SampleWebAPI.DTOs
{
    using System.ComponentModel.DataAnnotations;
    public class EmployeeReadDto
    {
        public int ID { get; set; }
        public string Name { get; set; } = "";
        public string Department { get; set; } = "";
        public decimal Salary { get; set; }
    }
}
