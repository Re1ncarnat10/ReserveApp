using Microsoft.AspNetCore.Identity;
using ReserveApp.Dtos;
using ReserveApp.Interfaces;
using ReserveApp.Models;

namespace ReserveApp.Services;

public class UserService : IUserService
{
  private readonly UserManager<User> _userManager;

  public UserService(UserManager<User> userManager)
  {
    _userManager = userManager;
  }

  public async Task<UserDto> GetUserInfoAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      throw new Exception("User not found");
    }
    return new UserDto
    {
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            Department = user.Department,
    };
  }

  public async Task<IdentityResult> UpdateUserInfoAsync(string userId, UserDto userDto)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      return IdentityResult.Failed(new IdentityError { Description = "User not found" });
    }
    user.Name = userDto.Name;
    user.Surname = userDto.Surname;
    user.Email = userDto.Email;
    user.Department = userDto.Department;
    return await _userManager.UpdateAsync(user);
  }
}