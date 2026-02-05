namespace SampleWebAPI.Caching
{
    public static class CacheKeys
    {
        public const string employeeListKey = "employee_list";
        public static string EmployeeKey(int id) => $"employee_{id}";
    }
}
