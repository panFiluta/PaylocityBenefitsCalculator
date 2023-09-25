using Api.Dtos.Employee;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController : ControllerBase
{
    // task: use a more realistic production approach
    // solution: In-memory list to simulate data storage moved to Api/Services/EmployeeService 
    // (we would use a database in a real application).
    
    private readonly IEmployeeService _employeeService;
 
    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [SwaggerOperation(Summary = "Get employee by id")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> Get(int id)
    {
        var employee = await _employeeService.GetEmployee(id);

        if(employee == null)
        {
            return NotFound(new ApiResponse<GetEmployeeDto> 
            {
                Success = false,
                Message = "Employee not found"            
            });
        }
        return Ok(new ApiResponse<GetEmployeeDto> {Data = employee});
    }

    [SwaggerOperation(Summary = "Get all employees")]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<GetEmployeeDto>>>> GetAll()
    {
        var employees = await _employeeService.GetAllEmployees();
        
        return Ok(new ApiResponse<List<GetEmployeeDto>> {Data = employees});
    }

    [SwaggerOperation(Summary = "Calculate and get paycheck for an employee")]
    [HttpGet("{id}/paycheck")]
    public async Task<ActionResult<ApiResponse<GetPaycheckDto>>> GetPaycheck(int id)
    {
        var paycheck = await _employeeService.CalculatePaycheck(id);

        if (paycheck == null)
        {
            return NotFound(new ApiResponse<GetPaycheckDto>
            {
                Success = false,
                Message = "Employee not found"
            });
        }

        return Ok(new ApiResponse<GetPaycheckDto> { Data = paycheck });
    }
}
