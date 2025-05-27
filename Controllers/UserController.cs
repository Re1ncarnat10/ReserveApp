using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReserveApp.Interfaces;
using ReserveApp.Dtos;
using System.Security.Claims;

namespace ReserveApp.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]")]
  public class UserController : ControllerBase
  {
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
      _userService = userService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserInfo(string userId)
    {
      try
      {
        var userInfo = await _userService.GetUserInfoAsync(userId);
        return Ok(userInfo);
      }
      catch (Exception ex)
      {
        return NotFound(ex.Message);
      }
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUserInfo(string userId, [FromBody] UserDto userDto)
    {
      var result = await _userService.UpdateUserInfoAsync(userId, userDto);
      if (!result.Succeeded)
      {
        return BadRequest(result.Errors);
      }

      return Ok("User information updated successfully");
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUserInfo()
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (string.IsNullOrEmpty(userId))
      {
        return Unauthorized("User ID claim not found");
      }

      try
      {
        var userInfo = await _userService.GetUserInfoAsync(userId);
        return Ok(userInfo);
      }
      catch (Exception ex)
      {
        return NotFound(ex.Message);
      }
    }
    [HttpGet("me/history")]
    public async Task<IActionResult> GetMyHistory()
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (string.IsNullOrEmpty(userId))
        return Unauthorized("User ID claim not found");

      try
      {
        var history = await _userService.GetUserHistoryAsync(userId);
        return Ok(history);
      }
      catch (Exception ex)
      {
        return NotFound(ex.Message);
      }
    }
  }
}