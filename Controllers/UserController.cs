using Microsoft.AspNetCore.Mvc;
using ReserveApp.Interfaces;
using ReserveApp.Dtos;

namespace ReserveApp.Controllers
{
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
  }
}