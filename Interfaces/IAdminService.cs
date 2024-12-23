using ReserveApp.Dtos;

namespace ReserveApp.Interfaces
{
  public interface IAdminService
  {
    Task<ResourceDto> CreateResourceAsync(ResourceDto resourceDto);
    Task<IEnumerable<ResourceDto>> GetAllResourcesAsync();
    Task<ResourceDto> GetResourceByIdAsync(int id);
    Task UpdateResourceAsync(int id, ResourceDto resourceDto);
    Task DeleteResourceAsync(int id);
    Task ChangeResourceStatusAsync(UserResourceDto userResourceDto, string newStatus);
    Task UpdateUserResourceRentalDurationAsync(UserResourceDto userResourceDto, TimeSpan additionalRentalDuration);
    Task<IEnumerable<UserResourceDto>> GetAllUserResourcesAsync();
    Task<IEnumerable<UserResourceDto>> GetUserResourcesByUserIdAsync(int userId);
    Task<UserDto> GetUserByIdAsync(string userId);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task DeleteUserAsync(string userId);
    Task UpdateUserAsync(UserDto userDto);
    Task InitializeAdminAsync();
  }
}