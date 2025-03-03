using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReserveApp.Interfaces;
using ReserveApp.Dtos;
using ReserveApp.Models;


namespace ReserveApp.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class UserResourceController : ControllerBase
  {
    private readonly IUserResourceService _userResourceService;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<UserResourceController> _logger;

    public UserResourceController(IUserResourceService userResourceService,
            UserManager<User> userManager,
            ILogger<UserResourceController> logger)
    {
      _userResourceService = userResourceService;
      _userManager = userManager;
      _logger = logger;
    }

    [HttpPost("request")]
    public async Task<IActionResult> RequestResource([FromBody] UserResourceDto userResourceDto)
    {
      var userId = _userManager.GetUserId(User);
      if (userId == null)
      {
        _logger.LogWarning("Unauthorized access attempt.");
        return Unauthorized("User is not logged in");
      }

      _logger.LogInformation("User {UserId} is requesting resource {ResourceId}", userId,
              userResourceDto.ResourceId);

      var requestedResource = await _userResourceService.RequestResourceAsync(
              userId,
              userResourceDto.ResourceId,
              userResourceDto.RentalStartTime,
              userResourceDto.RentalEndTime
      );

      return Ok(requestedResource);
    }


    [HttpGet("inventory")]
    public async Task<IActionResult> GetUserResourcesByUserId()
    {
      try
      {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
          return Unauthorized("User is not logged in");
        }

        var userResources = await _userResourceService.GetUserResourcesByUserIdAsync(userId);
        return Ok(userResources);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpPut("return/{userResourceId}")]
    public async Task<IActionResult> ReturnResource(int userResourceId)
    {
      try
      {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
          return Unauthorized("User is not logged in");
        }

        var result = await _userResourceService.ReturnResourceAsync(userResourceId);
        return Ok(result);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpDelete("{userResourceId}")]
    public async Task<IActionResult> DeleteResource(int userResourceId)
    {
      try
      {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
          return Unauthorized("User is not logged in");
        }

        await _userResourceService.DeleteResourceAsync(userResourceId);
        return Ok("Resource deleted successfully");
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }
  }
}