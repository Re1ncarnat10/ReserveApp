using Microsoft.AspNetCore.Identity;
using ReserveApp.Dtos;

namespace ReserveApp.Interfaces;

public interface IUserService
{
  Task<UserDto> GetUserInfoAsync(string userId);
  Task<IdentityResult> UpdateUserInfoAsync(string userId, UserDto userDto);
  
}