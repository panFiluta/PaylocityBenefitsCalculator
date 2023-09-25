using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;

namespace Api.Services
{
    public class EmployeeService : IEmployeeService
    {
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
            },
            new()
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
            }
        };
        public Task<GetEmployeeDto?> GetEmployee(int id)
        {
            var employee = _employees.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(employee);
        }

        public Task<List<GetEmployeeDto>> GetAllEmployees()
        {
            return Task.FromResult(_employees.ToList());
        }

        public Task<GetPaycheckDto?> CalculatePaycheck(int id)
        {
            var employee = _employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                return Task.FromResult<GetPaycheckDto?>(null);
            }

            var paycheckDto = CalculatePaycheck(employee);

            return Task.FromResult(paycheckDto);
        }

        private GetPaycheckDto? CalculatePaycheck(GetEmployeeDto employee)
        {
            // Constants for benefit costs
            decimal baseCostPerMonth = 1000;   // Base cost for benefits per month
            decimal dependentCostPerMonth = 600; // Cost for each dependent per month
            decimal highEarnerPercentage = 0.02m; // Additional cost percentage for high earners
            decimal dependentOver50CostPerMonth = 200; // Additional cost for dependents over 50

            // Calculate annual salary and benefits cost
            decimal annualSalary = employee.Salary * 12;
            decimal benefitsCost = baseCostPerMonth * 12; // Base cost for benefits per year

            // Check if the employee is a high earner
            if (annualSalary > 80000)
            {
                benefitsCost += annualSalary * highEarnerPercentage;
            }

            // Calculate dependent costs
            if(employee != null && employee.Children != null)
            {
                foreach (var dependent in employee.Children)
                {
                    // Add $200 per month for dependents over 50
                    if (dependent.DateOfBirth.AddYears(50) <= DateTime.Now)
                    {
                        benefitsCost += dependentOver50CostPerMonth * 12;
                    }

                    benefitsCost += dependentCostPerMonth * 12; // $600 per month for each dependent
                }
            }

            // Calculate net salary
            decimal netSalary = annualSalary - benefitsCost;

            // Calculate the amount per paycheck (evenly spread deductions)
            decimal paycheckAmount = netSalary / 26;

            var paycheckDto = new GetPaycheckDto
            {
                BaseSalary = annualSalary / 12,
                Deductions = benefitsCost / 12,
                NetSalary = paycheckAmount
            };

            return paycheckDto;
        }
    }
}
