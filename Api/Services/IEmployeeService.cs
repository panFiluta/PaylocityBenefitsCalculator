using Api.Dtos.Employee;

namespace Api.Services;

public interface IEmployeeService
{
    Task<GetEmployeeDto?> GetEmployeeAsync(int id);
    Task<List<GetEmployeeDto>> GetAllEmployeesAsync();
    Task<GetPaycheckDto?> CalculatePaycheckAsync(int id);
}

