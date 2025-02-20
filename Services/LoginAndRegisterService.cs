using Microsoft.AspNetCore.Identity;
using ReserveApp.Dtos;
using ReserveApp.Models;
using ReserveApp.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ReserveApp.Services;

public class LoginAndRegisterService:ILoginAndRegisterService
{
  private readonly RoleManager<IdentityRole> _roleManager;
  private readonly UserManager<User> _userManager;
  private readonly IConfiguration _configuration;
  
  public LoginAndRegisterService(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IConfiguration configuration)
  {
    _roleManager = roleManager;
    _userManager = userManager;
    _configuration = configuration;
  }

  public async Task CreateRoles()
  {
    string[] roleNames = { "Admin", "User" };
    foreach(var roleName in roleNames)
    {
      var roleExist = await _roleManager.RoleExistsAsync(roleName);
      if(!roleExist)
      {
        await _roleManager.CreateAsync(new IdentityRole(roleName));
      }
    }
  }
  public async Task<IdentityResult>RegisterUserAsync(RegisterDto registerDto)
  {
    var user = new User
    {
      Name = registerDto.Name,
      Surname = registerDto.Surname,
      Email = registerDto.Email,
      Department = registerDto.Department,
    };
    var result = await _userManager.CreateAsync(user, registerDto.Password);
    if(!result.Succeeded)
    {
      return result;
    }
    var roleResult = await _userManager.AddToRoleAsync(user, "User");
    return !roleResult.Succeeded
    ? IdentityResult.Failed(new IdentityError { Description = "User could not be assigned a role"})
            : IdentityResult.Success;
  }
  public async Task<TokenDto> LoginUserAsync(LoginDto loginDto)
  {
    var user = await _userManager.FindByEmailAsync(loginDto.Email);
    if(user == null)
    {
      throw new Exception("User not found");
    }
    var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
    if(!result)
    {
      throw new Exception("Password is incorrect");
    }
    var userRoles = await _userManager.GetRolesAsync(user);
    var claims = new List<Claim>
    {
      new Claim(JwtRegisteredClaimNames.Sub, user.Id),
      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
      new Claim(ClaimTypes.NameIdentifier, user.Id),
    };
    claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));
    
    var jwtKey = _configuration["Jwt:Key"];
    var key = Encoding.UTF8.GetBytes(jwtKey);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      Expires = DateTime.UtcNow.AddHours(1),
      Issuer = _configuration["Jwt:Issuer"],
      Audience = _configuration["Jwt:Audience"],
      SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    var tokenHandler = new JwtSecurityTokenHandler();
    var jwtToken = tokenHandler.CreateEncodedJwt(tokenDescriptor);
    return new TokenDto { Token = jwtToken };
  }
}