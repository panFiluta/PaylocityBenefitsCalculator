using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Api.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace ApiTests.Services
{
    public class EmployeeServiceTests
    {
        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[]
            {
                new BenefitsConfiguration
                {
                    DependentCostPerMonth = 600m,
                    BaseCostPerMonth = 1000m,
                    DependentOver50CostPerMonth = 200m,
                    HighEarnerLimit = 80000,
                    HighEarnerPercentage = 0.02m,
                    PaychecksPerYear = 26
                }
            };
        }

        [Theory]
        [MemberData(nameof(TestCases))]
        public async Task CalculatePaycheckAsync_NoDependentsNoHighEarner_CalculatesCorrectly(BenefitsConfiguration configuration)
        {
            // Arrange
            var employee = new GetEmployeeDto
            {
                Id = 1,
                FirstName = "LeBron",
                LastName = "James",
                Salary = 75420.99m,
                DateOfBirth = new DateTime(1984, 12, 30),
                RelationshipStatus = RelationshipType.None
            };

            var service = new EmployeeService(Options.Create(configuration));

            // Act
            var result = await service.CalculatePaycheckAsync(employee.Id);

            var expectedBaseSalary = Math.Round(employee.Salary/configuration.PaychecksPerYear, 2);
            var expectedDeductions = Math.Round(configuration.BaseCostPerMonth*12/configuration.PaychecksPerYear, 2);
            var expectedNetSalary = Math.Round((employee.Salary -(configuration.BaseCostPerMonth*12))/configuration.PaychecksPerYear, 2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedBaseSalary, result!.BaseSalary); // Check base salary calculation
            Assert.Equal(expectedDeductions, result.Deductions); // Check deduction
            Assert.Equal(expectedNetSalary, result.NetSalary); // Check net salary
        }


        [Theory]
        [MemberData(nameof(TestCases))]
        public async Task CalculatePaycheckAsync_HighEarner_CalculatesCorrectly(BenefitsConfiguration configuration)
        {
            // Arrange
            var employee = new GetEmployeeDto
            {
                Id = 4,
                FirstName = "Kobe",
                LastName = "Bryant",
                Salary = 123000m, // >80.000
                DateOfBirth = new DateTime(1983, 2, 17),
                RelationshipStatus = RelationshipType.None
            };

            var service = new EmployeeService(Options.Create(configuration));

            // Act
            var result = await service.CalculatePaycheckAsync(employee.Id);

            var expectedBaseSalary = employee.Salary/configuration.PaychecksPerYear;
            var expectedDeductions = ((employee.Salary * configuration.HighEarnerPercentage)+12*configuration.BaseCostPerMonth)/configuration.PaychecksPerYear;
            var expectedNetSalary = expectedBaseSalary - expectedDeductions;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Math.Round(expectedBaseSalary, 2), result!.BaseSalary); // Check base salary calculation
            Assert.Equal(Math.Round(expectedDeductions,2), result.Deductions); // Check high earner deduction
            Assert.Equal(Math.Round(expectedNetSalary, 2), result.NetSalary); // Check net salary
        }

        [Theory]
        [MemberData(nameof(TestCases))]
        public async Task CalculatePaycheckAsync_NoChildren_Spouse_Over50_CalculatesCorrectly(BenefitsConfiguration configuration)
        {
            // Arrange
            var employee = new GetEmployeeDto
            {
                Id = 5,
                FirstName = "Alice",
                LastName = "Smith",
                Salary = 70000,
                DateOfBirth = new DateTime(1970, 1, 1),
                Spouse = new GetDependentDto
                {
                    Id = 5,
                    FirstName = "Bob",
                    LastName = "Smith",
                    Relationship = Relationship.Spouse,
                    DateOfBirth = new DateTime(1965, 1, 1), // Over 50
                },
            };

            var service = new EmployeeService(Options.Create(configuration));

            // Act
            var result = await service.CalculatePaycheckAsync(employee.Id);

            var expectedBaseSalary = employee.Salary/configuration.PaychecksPerYear;
            var expectedDeductions = (configuration.BaseCostPerMonth+configuration.DependentCostPerMonth+configuration.DependentOver50CostPerMonth)*12/configuration.PaychecksPerYear;
            var expectedNetSalary = expectedBaseSalary - expectedDeductions;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Math.Round(expectedBaseSalary, 2), result!.BaseSalary); // Check base salary calculation
            Assert.Equal(Math.Round(expectedDeductions,2), result.Deductions); // Check deduction
            Assert.Equal(Math.Round(expectedNetSalary, 2), result.NetSalary); // Check net salary
        }

        [Theory]
        [MemberData(nameof(TestCases))]
        public void CalculatePartnerBenefits_Over50_CalculatesCorrectly(BenefitsConfiguration configuration)
        {
            // Arrange
            var employee = new GetEmployeeDto
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
                    DateOfBirth = new DateTime(1965, 1, 1), // Over 50
                },
            };

            var service = new EmployeeService(Options.Create(configuration));

            // Act
            var result = service.CalculatePartnerBenefits(employee);

            var expected = (configuration.DependentCostPerMonth+configuration.DependentOver50CostPerMonth)*12;
            
            // Assert
            Assert.Equal(Math.Round(expected, 2), result);
        }

        [Theory]
        [MemberData(nameof(TestCases))]
        public async Task CalculatePaycheckAsync_Children_Spouse_Under50_CalculatesCorrectly(BenefitsConfiguration configuration)
        {
            // Arrange
            var employee = new GetEmployeeDto
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
            };
            
            var service = new EmployeeService(Options.Create(configuration));

            // Act
            var result = await service.CalculatePaycheckAsync(employee.Id);

            // Assert
            var expectedBaseSalary = employee.Salary/configuration.PaychecksPerYear;
            var expectedDeductions = (configuration.BaseCostPerMonth+3*configuration.DependentCostPerMonth)*12/configuration.PaychecksPerYear;
            var expectedNetSalary = expectedBaseSalary - expectedDeductions;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Math.Round(expectedBaseSalary, 2), result!.BaseSalary); // Check base salary calculation
            Assert.Equal(Math.Round(expectedDeductions,2), result.Deductions); // Check deduction
            Assert.Equal(Math.Round(expectedNetSalary, 2), result.NetSalary); // Check net salary
        }

        [Theory]
        [MemberData(nameof(TestCases))]
        public async Task CalculatePaycheckAsync_HighEarner_Children_Spouse_Under50_CalculatesCorrectly(BenefitsConfiguration configuration)
        {
            // Arrange
            var employee = new GetEmployeeDto
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
            };
            
            var service = new EmployeeService(Options.Create(configuration));

            // Act
            var result = await service.CalculatePaycheckAsync(employee.Id);

            // Assert
            var expectedBaseSalary = employee.Salary/configuration.PaychecksPerYear;
            var expectedDeductions = ((employee.Salary * configuration.HighEarnerPercentage)+(configuration.BaseCostPerMonth+3*configuration.DependentCostPerMonth)*12)/configuration.PaychecksPerYear;
            var expectedNetSalary = expectedBaseSalary - expectedDeductions;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Math.Round(expectedBaseSalary, 2), result!.BaseSalary); // Check base salary calculation
            Assert.Equal(Math.Round(expectedDeductions,2), result.Deductions); // Check deduction
            Assert.Equal(Math.Round(expectedNetSalary, 2), result.NetSalary); // Check net salary
        }
    }
}
