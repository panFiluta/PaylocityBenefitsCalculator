using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Microsoft.Extensions.Options;

namespace Api.Services;

public class EmployeeService : IEmployeeService
{
    private readonly BenefitsConfiguration _benefitsConfig;
    private readonly List<GetEmployeeDto> _employees = new List<GetEmployeeDto>();

    public EmployeeService(IOptions<BenefitsConfiguration> benefitsConfig)
    {
        _benefitsConfig = benefitsConfig.Value;
    
        AddEmployee(new GetEmployeeDto
        {
            Id = 1,
            FirstName = "LeBron",
            LastName = "James",
            Salary = 75420.99m,
            DateOfBirth = new DateTime(1984, 12, 30),
            RelationshipStatus = RelationshipType.None
        });

        AddEmployee(new GetEmployeeDto
        {
            Id = 2,
            FirstName = "Ja",
            LastName = "Morant",
            Salary = 92365.22m,
            DateOfBirth = new DateTime(1999, 8, 10),
            RelationshipStatus = RelationshipType.Spouse,
            Spouse = new GetDependentDto
            {
                Id = 1,
                FirstName = "Spouse",
                LastName = "Morant",
                Relationship = Relationship.Spouse,
                DateOfBirth = new DateTime(1998, 3, 3)
            },
            Children = new List<GetDependentDto>
            {
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
                },
            }
        });

        AddEmployee(new GetEmployeeDto
        {
            Id = 3,
            FirstName = "Michael",
            LastName = "Jordan",
            Salary = 143211.12m,
            DateOfBirth = new DateTime(1963, 2, 17),
            RelationshipStatus = RelationshipType.DomesticPartner,
            DomesticPartner = new GetDependentDto 
            {
                Id = 4,
                FirstName = "DP",
                LastName = "Jordan",
                Relationship = Relationship.DomesticPartner,
                DateOfBirth = new DateTime(1974, 1, 2)
            }
        });

        AddEmployee(new GetEmployeeDto
        {
            Id = 4,
            FirstName = "Kobe",
            LastName = "Bryant",
            Salary = 123000m,
            DateOfBirth = new DateTime(1983, 2, 17),
            RelationshipStatus = RelationshipType.None
        });

        AddEmployee(new GetEmployeeDto
        {
            Id = 5,
            FirstName = "Alice",
            LastName = "Smith",
            Salary = 70000,
            DateOfBirth = new DateTime(1970, 1, 1),
            RelationshipStatus = RelationshipType.Spouse,
            Spouse = new GetDependentDto
            {
                Id = 5,
                FirstName = "Bob",
                LastName = "Smith",
                Relationship = Relationship.Spouse,
                DateOfBirth = new DateTime(1965, 1, 1) // Over 50
            },
        });

        AddEmployee(new GetEmployeeDto
        {
            Id = 6,
            FirstName = "Kevin",
            LastName = "Durant",
            Salary = 72365.22m,
            DateOfBirth = new DateTime(1999, 8, 10),
            RelationshipStatus = RelationshipType.Spouse,
            Spouse = new GetDependentDto
            {
                Id = 6,
                FirstName = "Spouse",
                LastName = "Durant",
                Relationship = Relationship.Spouse,
                DateOfBirth = new DateTime(1998, 3, 3)
            },
            Children = new List<GetDependentDto>
            {
                new()
                {
                    Id = 7,
                    FirstName = "Child1",
                    LastName = "Durant",
                    Relationship = Relationship.Child,
                    DateOfBirth = new DateTime(2020, 6, 23)
                },
                new()
                {
                    Id = 8,
                    FirstName = "Child2",
                    LastName = "Durant",
                    Relationship = Relationship.Child,
                    DateOfBirth = new DateTime(2021, 5, 18)
                },
            }
        });
    }
    
    public Task<GetEmployeeDto?> GetEmployeeAsync(int id)
    {
        var employee = _employees.FirstOrDefault(e => e.Id == id);
        return Task.FromResult(employee);
    }

    public Task<List<GetEmployeeDto>> GetAllEmployeesAsync()
    {
        return Task.FromResult(_employees.ToList());
    }

    public Task<GetPaycheckDto?> CalculatePaycheckAsync(int id)
    {
        var employee = _employees.FirstOrDefault(e => e.Id == id);
        if (employee == null)
        {
            return Task.FromResult<GetPaycheckDto?>(null);
        }

        var paycheckDto = CalculatePaycheck(employee);

        return Task.FromResult(paycheckDto);
    }

    private decimal CalculateHighEarnerBenefits(GetEmployeeDto employee)
    {
        decimal benefitsCost = 0m;
        decimal annualSalary = employee.Salary;  

        // Check if the employee is a high earner
        if (employee.Salary > _benefitsConfig.HighEarnerLimit)
        {
            benefitsCost = annualSalary * _benefitsConfig.HighEarnerPercentage;
        }
        return benefitsCost;
    }

    private decimal CalculateOldDependentBenefits(GetDependentDto dependent)
    {
        decimal benefitsCost = 0m;
        
        // Add $200 per month for dependents over 50
        // Can child be over 50? 
        // We assume that the answer is yes :D
        if (dependent.DateOfBirth.AddYears(50) <= DateTime.Now)
        {
            benefitsCost += _benefitsConfig.DependentOver50CostPerMonth * 12;
        }
        return benefitsCost;
    }

    private decimal CalculateChildBenefits(GetEmployeeDto employee)
    {
        decimal benefitsCost = 0m;
        
        if(employee != null && employee.Children != null)
        {
            foreach (var child in employee.Children)
            {
                // Can a child dependent be >50? I assume yes :)
                benefitsCost += CalculateOldDependentBenefits(child);
                benefitsCost += _benefitsConfig.DependentCostPerMonth * 12; // $600 per month for each dependent
            }
        }
        return benefitsCost;
    }

    public decimal CalculatePartnerBenefits(GetEmployeeDto employee)
    {
        decimal benefitsCost = 0m;
        
        if (employee != null)
        {
            GetDependentDto? partner = employee.RelationshipStatus switch
            {
                RelationshipType.DomesticPartner => employee.DomesticPartner,
                RelationshipType.Spouse => employee.Spouse,
                _ => null,
            };
            if (partner == null)
                return benefitsCost;
            
            benefitsCost += _benefitsConfig.DependentCostPerMonth * 12; 
            benefitsCost += CalculateOldDependentBenefits(partner);
        }
        return benefitsCost;
    }

    private GetPaycheckDto? CalculatePaycheck(GetEmployeeDto employee)
    {
        // Calculate annual salary and benefits cost
        // I assume the employee salary is yearly salary here
        // (as confirmed by Jake Langan -Talent Acquisition Partner)
        decimal annualSalary = employee.Salary;  
        decimal anualBenefitsCost = _benefitsConfig.BaseCostPerMonth * 12; // Base cost for benefits per year

        anualBenefitsCost += CalculateHighEarnerBenefits(employee);
        anualBenefitsCost += CalculateChildBenefits(employee);
        anualBenefitsCost += CalculatePartnerBenefits(employee);
     
        // Calculate net salary
        decimal annualNetSalary = annualSalary - anualBenefitsCost;

        // Calculate the amount per paycheck (evenly spread deductions) and round to 2 decimal places
        var paycheckDto = new GetPaycheckDto
        {
            BaseSalary = Math.Round(annualSalary/26, 2),
            Deductions = Math.Round(anualBenefitsCost/26, 2),
            NetSalary = Math.Round(annualNetSalary/26, 2)
        };

        return paycheckDto;
    }

    private void AddEmployee(GetEmployeeDto employeeDto)
    {
        ValidateEmployee(employeeDto);
        _employees.Add(employeeDto);
    }

    /// <summary>
    /// Method validating that an employee doesn't have both a spouse and a domestic partner
    /// </summary>
    /// <param name="employeeDto"></param>
    /// <exception cref="Exception"></exception>
    private void ValidateEmployee(GetEmployeeDto employeeDto)
    {
        var message = "An employee cannot have both a spouse and a domestic partner.";

        if (employeeDto.RelationshipStatus == RelationshipType.Spouse &&
            employeeDto.DomesticPartner != null)
        {
            throw new Exception(message);
        }
        else if (employeeDto.RelationshipStatus == RelationshipType.DomesticPartner &&
                employeeDto.Spouse != null)
        {
            throw new Exception(message);
        }
    }

}
