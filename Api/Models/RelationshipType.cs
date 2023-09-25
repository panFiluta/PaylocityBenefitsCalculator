namespace Api.Models;

// Added because an employee may only have 1 spouse or domestic partner (not both)
public enum RelationshipType
{
    None,
    Spouse,
    DomesticPartner
}