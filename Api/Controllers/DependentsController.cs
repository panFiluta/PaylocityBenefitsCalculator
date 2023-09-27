using Api.Dtos.Dependent;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DependentsController : ControllerBase
{
    private readonly IDependentService _dependentService;
 
    public DependentsController(IDependentService dependentService)
    {
        _dependentService = dependentService;
    }

    [SwaggerOperation(Summary = "Get dependent by id")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetDependentDto>>> Get(int id)
    {
        var dependent = await _dependentService.GetDependentAsync(id);
        if (dependent == null)
        {
            return NotFound(new ApiResponse<GetDependentDto> 
            { 
                Success = false, 
                Message = "Dependent not found" 
            });
        }

        return Ok(new ApiResponse<GetDependentDto> { Data = dependent });
    }

    [SwaggerOperation(Summary = "Get all dependents")]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<GetDependentDto>>>> GetAll()
    {
        var dependents = await _dependentService.GetAllDependentsAsync();
        return Ok(new ApiResponse<List<GetDependentDto>> { Data = dependents });
    }
}
