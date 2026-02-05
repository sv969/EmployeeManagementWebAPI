namespace SampleWebAPI.DTOs
{
    using System.ComponentModel.DataAnnotations;
    public class EmployeeCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "";

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Department ID must be a positive integer.")]
        public decimal Salary { get; set; }

        [Required]
        [MaxLength(50)]
        public string Department { get; set; } = "";
    }
}
