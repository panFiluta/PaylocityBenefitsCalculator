namespace Api.Models;

public class Employee
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public decimal Salary { get; set; }
    public DateTime DateOfBirth { get; set; }
    public RelationshipType RelationshipStatus { get; set; } // Added because an employee may only have 1 spouse or domestic partner (not both)
    public Dependent? Spouse { get; set; } // Added because an employee may only have 1 spouse or domestic partner (not both)
    public Dependent? DomesticPartner { get; set; } // Added because an employee may only have 1 spouse or domestic partner (not both)
    public List<Dependent>? Children { get; set; }
    public ICollection<Dependent> Dependents { get; set; } = new List<Dependent>();
}
