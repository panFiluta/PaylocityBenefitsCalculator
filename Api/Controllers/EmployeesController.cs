using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController : ControllerBase
{
        // task: use a more realistic production approach
        // solution: In-memory list to simulate data storage (we would use a database in a real application).
        private readonly List<GetEmployeeDto> _employees = new List<GetEmployeeDto>
        {
            new()
            {
                Id = 1,
                FirstName = "LeBron",
                LastName = "James",
                Salary = 75420.99m,
                DateOfBirth = new DateTime(1984, 12, 30)
            },
            new()
            {
                Id = 2,
                FirstName = "Ja",
                LastName = "Morant",
                Salary = 92365.22m,
                DateOfBirth = new DateTime(1999, 8, 10),
                Dependents = new List<GetDependentDto>
                {
                    new()
                    {
                        Id = 1,
                        FirstName = "Spouse",
                        LastName = "Morant",
                        Relationship = Relationship.Spouse,
                        DateOfBirth = new DateTime(1998, 3, 3)
                    },
                    new()
                    {
                        Id = 2,
                        FirstName = "Child1",
                        LastName = "Morant",
                        Relationship = Relationship.Child,
                        DateOfBirth = new DateTime(2020, 6, 23)
                    },
                    new()
                    {
                        Id = 3,
                        FirstName = "Child2",
                        LastName = "Morant",
                        Relationship = Relationship.Child,
                        DateOfBirth = new DateTime(2021, 5, 18)
                    }
                }
            },
            new()
            {
                Id = 3,
                FirstName = "Michael",
                LastName = "Jordan",
                Salary = 143211.12m,
                DateOfBirth = new DateTime(1963, 2, 17),
                Dependents = new List<GetDependentDto>
                {
                    new()
                    {
                        Id = 4,
                        FirstName = "DP",
                        LastName = "Jordan",
                        Relationship = Relationship.DomesticPartner,
                        DateOfBirth = new DateTime(1974, 1, 2)
                    }
                }
            }
        };

    [SwaggerOperation(Summary = "Get employee by id")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> Get(int id)
    {
        var employee = _employees.FirstOrDefault(x => x.Id == id); 

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
        var employees = _employees.ToList();
        
        return Ok(new ApiResponse<List<GetEmployeeDto>> {Data = employees});
    }

    [SwaggerOperation(Summary = "Calculate and get paycheck for an employee")]
    [HttpGet("{id}/paycheck")]
    public async Task<ActionResult<ApiResponse<GetPaycheckDto>>> GetPaycheck(int id)
    {
        var employee = _employees.FirstOrDefault(e => e.Id == id);
        if (employee == null)
        {
            return NotFound(new ApiResponse<GetPaycheckDto> 
            { 
                Success = false, 
                Message = "Employee not found" 
            });
        }

        // Calculate the paycheck based on the provided rules
        decimal baseCost = 1000; // Base cost for benefits per month
        decimal additionalCostPerDependent = 600; // Additional cost per dependent per month
        decimal additionalCostPercentage = 0.02m; // Additional cost for high earners (2% of yearly salary)
        decimal costForDependentsOver50 = 200; // Additional cost for dependents over 50 years old per month

        decimal annualSalary = employee.Salary * 12; // Calculate annual salary

        // Calculate deductions
        decimal deductions = (annualSalary > 80000) ? (annualSalary * additionalCostPercentage) : 0;

        // Calculate dependent costs
        foreach (var dependent in employee.Dependents)
        {
            // Add $200 per month for dependents over 50
            if (dependent.DateOfBirth.AddYears(50) <= DateTime.Now)
            {
                deductions += costForDependentsOver50;
            }
            // Add $600 per month for each dependent
            deductions += additionalCostPerDependent;
        }

        // Calculate net salary
        decimal netSalary = annualSalary / 12 - deductions;

        var paycheckDto = new GetPaycheckDto
        {
            BaseSalary = annualSalary / 12,
            Deductions = deductions,
            NetSalary = netSalary
        };

        return Ok(new ApiResponse<GetPaycheckDto> { Data = paycheckDto });
    }
}
