namespace SampleWebAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    public class Employee
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Department is required.")]
        public string Department { get; set; } = "";

        [Range(1000, double.MaxValue, ErrorMessage = "Salary must be greater than 1000." )]
        public decimal Salary { get; set; }
    }
}
