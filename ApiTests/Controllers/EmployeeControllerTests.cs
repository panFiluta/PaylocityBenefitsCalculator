using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Controllers;
using Api.Dtos.Employee;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ApiTests.Controllers;

public class EmployeesControllerTests
{
    private readonly EmployeesController _controller;
    private readonly Mock<IEmployeeService> _employeeServiceMock;

    public EmployeesControllerTests()
    {
        // Arrange: Create a mock of IEmployeeService
        _employeeServiceMock = new Mock<IEmployeeService>();

        // Arrange: Create an instance of EmployeesController with the mock
        _controller = new EmployeesController(_employeeServiceMock.Object);
    }

    [Fact]
    public async Task Get_ExistingEmployee_ReturnsOk()
    {
        // Arrange: Mock the employee service to return an employee
        var employeeId = 1;
        var employeeDto = new GetEmployeeDto { Id = employeeId };
        _employeeServiceMock.Setup(service => service.GetEmployeeAsync(It.IsAny<int>()))
            .ReturnsAsync(employeeDto);

        // Act: Call the Get method
        var result = await _controller.Get(employeeId);

        // Assert: Check that the result is an OK response with the expected employee
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var apiResponse = Assert.IsType<ApiResponse<GetEmployeeDto>>(okResult.Value);
        
        Assert.NotNull(apiResponse);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(employeeId, apiResponse.Data!.Id);
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        // Arrange: Mock the employee service to return a list of employees
        var employees = new List<GetEmployeeDto>
        {
            new GetEmployeeDto { Id = 1 },
            new GetEmployeeDto { Id = 2 }
        };
        _employeeServiceMock.Setup(service => service.GetAllEmployeesAsync())
            .ReturnsAsync(employees);

        // Act: Call the GetAll method
        var result = await _controller.GetAll();

        // Assert: Check that the result is an OK response with the expected list of employees
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var apiResponse = Assert.IsType<ApiResponse<List<GetEmployeeDto>>>(okResult.Value);

        Assert.NotNull(apiResponse);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(employees.Count, apiResponse.Data!.Count);
    }

    [Fact]
    public async Task GetPaycheck_ExistingEmployee_ReturnsOk()
    {
        // Arrange: Mock the employee service to return an employee
        var employeeId = 1;
        var netSalary = 5000;
        var baseSalary = 60000;
        var employeeDto = new GetEmployeeDto { Id = employeeId, Salary = baseSalary };
        _employeeServiceMock.Setup(service => service.GetEmployeeAsync(employeeId))
            .ReturnsAsync(employeeDto);

        // Mock the CalculatePaycheck method to return a specific paycheck result
        var expectedPaycheck = new GetPaycheckDto
        {
            BaseSalary = baseSalary, 
            Deductions = 1000, 
            NetSalary = netSalary  
        };
        _employeeServiceMock.Setup(service => service.CalculatePaycheckAsync(employeeDto.Id))
            .ReturnsAsync(expectedPaycheck);

        // Act: Call the GetPaycheck method
        var result = await _controller.GetPaycheck(employeeId);

        // Assert: Check that the result is an OK response with the expected paycheck data
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var apiResponse = Assert.IsType<ApiResponse<GetPaycheckDto>>(okResult.Value);
        
        // TODO: Add assertions based on paycheck calculation logic
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(netSalary, apiResponse.Data!.NetSalary);
    }
}

