using Api.Dtos.Dependent;
using Api.Models;

namespace Api.Dtos.Employee;

public class GetEmployeeDto
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public decimal Salary { get; set; }
    public DateTime DateOfBirth { get; set; }
    public RelationshipType RelationshipStatus { get; set; }
    public GetDependentDto Spouse { get; set; }
    public GetDependentDto DomesticPartner { get; set; }
    public List<GetDependentDto> Children { get; set; }
}
