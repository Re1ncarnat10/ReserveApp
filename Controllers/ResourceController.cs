using Microsoft.AspNetCore.Mvc;
using ReserveApp.Interfaces;
using ReserveApp.Dtos;

namespace ReserveApp.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ResourceController : ControllerBase
  {
    private readonly IResourceService _resourceService;

    public ResourceController(IResourceService resourceService)
    {
      _resourceService = resourceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllResources()
    {
      var resources = await _resourceService.GetAllResourcesAsync();
      return Ok(resources);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetResourceById(int id)
    {
      var resource = await _resourceService.GetResourceByIdAsync(id);
      if (resource == null)
      {
        return NotFound();
      }
      return Ok(resource);
    }
  }
}