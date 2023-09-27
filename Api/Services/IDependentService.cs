using Api.Dtos.Dependent;

namespace Api.Services;

public interface IDependentService
{
    Task<GetDependentDto?> GetDependentAsync(int id);
    Task<List<GetDependentDto>> GetAllDependentsAsync();
}

