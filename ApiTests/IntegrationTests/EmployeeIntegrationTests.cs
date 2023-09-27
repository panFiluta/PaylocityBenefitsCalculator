using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Xunit;

namespace ApiTests.IntegrationTests;

public class EmployeeIntegrationTests : IntegrationTest
{
    public EmployeeIntegrationTests(TestingWebAppFactory<Program> factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task WhenAskedForAllEmployees_ShouldReturnAllEmployees()
    {
        var response = await _client.GetAsync("/api/v1/employees");
        var employees = new List<GetEmployeeDto>
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
            },
            new()
            {
                Id = 4,
                FirstName = "Kobe",
                LastName = "Bryant",
                Salary = 123000m,
                DateOfBirth = new DateTime(1983, 2, 17),
                RelationshipStatus = RelationshipType.None
            },
            new()
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
            },
            new ()
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
            }
        };
        await response.ShouldReturn(HttpStatusCode.OK, employees);
    }

    [Fact]
    //task: make test pass
    public async Task WhenAskedForAnEmployee_ShouldReturnCorrectEmployee()
    {
        var response = await _client.GetAsync("/api/v1/employees/1");
        var employee = new GetEmployeeDto
        {
            Id = 1,
            FirstName = "LeBron",
            LastName = "James",
            Salary = 75420.99m,
            DateOfBirth = new DateTime(1984, 12, 30)
        };
        await response.ShouldReturn(HttpStatusCode.OK, employee);
    }
    
    [Fact]
    //task: make test pass
    public async Task WhenAskedForANonexistentEmployee_ShouldReturn404()
    {
        var response = await _client.GetAsync($"/api/v1/employees/{int.MinValue}");
        await response.ShouldReturn(HttpStatusCode.NotFound);
    }
}

