using Api.Dtos.Employee;

namespace Api.Services
{
    public interface IEmployeeService
    {
        Task<GetEmployeeDto?> GetEmployee(int id);
        Task<List<GetEmployeeDto>> GetAllEmployees();
        Task<GetPaycheckDto?> CalculatePaycheck(int id);
    }
}
