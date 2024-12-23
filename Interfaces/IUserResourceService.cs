using ReserveApp.Dtos;

namespace ReserveApp.Interfaces
{
  public interface IUserResourceService
  {
    Task<UserResourceDto> RequestResourceAsync(int userId, int resourceId, DateTime rentalStartTime, TimeSpan rentalDuration);
    Task<UserResourceDto> ReturnResourceAsync(int userResourceId);
    Task DeleteResourceAsync(int userResourceId);
  }
}