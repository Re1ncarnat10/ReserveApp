using ReserveApp.Dtos;

namespace ReserveApp.Interfaces
{
  public interface IAdminService
  {
    Task<ResourceDto> CreateResourceAsync(ResourceDto resourceDto);
    Task UpdateResourceAsync(int id, ResourceDto resourceDto);
    Task DeleteResourceAsync(int id);
    Task ChangeResourceStatusAsync(UserResourceDto userResourceDto, string newStatus);
    Task<IEnumerable<UserResourceDto>> GetAllUserResourcesAsync();
    Task<UserDto> GetUserByIdAsync(string userId);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task DeleteUserAsync(string userId);
    Task InitializeAdminAsync();
  }
}