using Microsoft.AspNetCore.Identity;
using ReserveApp.Dtos;

namespace ReserveApp.Interfaces;

public interface ILoginAndRegisterService
{
  Task CreateRoles();
  Task<IdentityResult> RegisterUserAsync(RegisterDto registerDto);
  Task<TokenDto> LoginUserAsync(LoginDto loginDto);
}