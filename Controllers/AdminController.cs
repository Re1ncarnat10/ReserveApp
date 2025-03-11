using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReserveApp.Interfaces;
using ReserveApp.Dtos;

namespace ReserveApp.Controllers
{
  [Authorize(Roles = "Admin")]
  [ApiController]
  [Route("api/[controller]")]
  public class AdminController : ControllerBase
  {
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
      _adminService = adminService;
    }

    [HttpPost("resource")]
    public async Task<IActionResult> CreateResource(ResourceDto resourceDto)
    {
      var result = await _adminService.CreateResourceAsync(resourceDto);
      return Ok(result);
    }

    [HttpPut("resource/{id}")]
    public async Task<IActionResult> UpdateResource(int id, ResourceDto resourceDto)
    {
      await _adminService.UpdateResourceAsync(id, resourceDto);
      return NoContent();
    }

    [HttpDelete("resource/{id}")]
    public async Task<IActionResult> DeleteResource(int id)
    {
      await _adminService.DeleteResourceAsync(id);
      return NoContent();
    }

    [HttpGet("user/{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
      var user = await _adminService.GetUserByIdAsync(id);
      if (user == null)
      {
        return NotFound();
      }

      return Ok(user);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
      var users = await _adminService.GetAllUsersAsync();
      return Ok(users);
    }

    [HttpDelete("user/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
      await _adminService.DeleteUserAsync(id);
      return NoContent();
    }

    [HttpPut("request/approve/{userId}/{resourceId}")]
    public async Task<IActionResult> ApproveUserRequest(string userId, int resourceId)
    {
      try
      {
        await _adminService.ApproveUserRequestAsync(userId, resourceId);
        return NoContent();
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpPut("request/reject/{userId}/{resourceId}")]
    public async Task<IActionResult> RejectUserRequest(string userId, int resourceId)
    {
      try
      {
        await _adminService.RejectUserRequestAsync(userId, resourceId);
        return NoContent();
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpGet("expired-resources")]
    public async Task<IActionResult> GetExpiredUserResources()
    {
      var expiredResources = await _adminService.GetExpiredUserResourcesAsync();
      return Ok(expiredResources);
    }

    [HttpDelete("return-resource/{userResourceId}")]
    public async Task<IActionResult> ReturnResourceToCirculation(int userResourceId)
    {
      try
      {
        var resource = await _adminService.ReturnResourceToCirculationAsync(userResourceId);
        return Ok(resource);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpGet("requests")]
    public async Task<ActionResult<IEnumerable<UserResourceDto>>> GetAllUserResources()
    {
      var userResources = await _adminService.GetAllUserResourcesAsync();
      return Ok(userResources);
    }

    [HttpPost("initialize-admin")]
    public async Task<IActionResult> InitializeAdmin()
    {
      await _adminService.InitializeAdminAsync();
      return Ok();
    }
  }
}