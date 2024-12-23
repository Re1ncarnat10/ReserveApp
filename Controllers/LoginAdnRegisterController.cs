using Microsoft.AspNetCore.Mvc;
using ReserveApp.Interfaces;
using ReserveApp.Dtos;

namespace ReserveApp.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class LoginAndRegisterController : ControllerBase
  {
    private readonly ILoginAndRegisterService _loginAndRegisterService;

    public LoginAndRegisterController(ILoginAndRegisterService loginAndRegisterService)
    {
      _loginAndRegisterService = loginAndRegisterService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
      var result = await _loginAndRegisterService.RegisterUserAsync(registerDto);
      if (!result.Succeeded)
      {
        return BadRequest(result.Errors);
      }
      return Ok("User registered successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
      try
      {
        var tokenDto = await _loginAndRegisterService.LoginUserAsync(loginDto);
        return Ok(tokenDto);
      }
      catch (Exception ex)
      {
        return Unauthorized(ex.Message);
      }
    }
  }
}