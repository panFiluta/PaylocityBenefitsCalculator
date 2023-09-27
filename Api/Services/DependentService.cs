using Api.Dtos.Dependent;
using Api.Models;

namespace Api.Services;

public class DependentService : IDependentService
{
    private readonly List<GetDependentDto> _dependents = new List<GetDependentDto>
    {
        new GetDependentDto
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
        },
        new()
        {
            Id = 4,
            FirstName = "DP",
            LastName = "Jordan",
            Relationship = Relationship.DomesticPartner,
            DateOfBirth = new DateTime(1974, 1, 2)
        }
    };

    public Task<GetDependentDto?> GetDependentAsync(int id)
    {
        var dependent = _dependents.FirstOrDefault(e => e.Id == id);
        return Task.FromResult(dependent);
    }

    public Task<List<GetDependentDto>> GetAllDependentsAsync()
    {
        return Task.FromResult(_dependents.ToList());
    }
}
