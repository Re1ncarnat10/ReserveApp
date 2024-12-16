using ReserveApp.Dtos;

namespace ReserveApp.Interfaces
{
  public interface IAdminService
  {
    Task<ResourceDto> CreateResource(ResourceDto resourceDto);
    Task<IEnumerable<ResourceDto>> GetAllResourcesAsync();
    Task<ResourceDto> GetResourceByIdAsync(int id);
    Task UpdateResourceAsync(int id, ResourceDto resourceDto);
    Task DeleteResourceAsync(int id);
    Task<UserDto> GetUserByIdAsync(string id);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task DeleteUserAsync(string id);
    Task UpdateUserAsync(string id, UserDto userDto);
    Task InitializeAdminAsync();
  }
}