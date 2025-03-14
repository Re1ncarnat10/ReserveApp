﻿using ReserveApp.Dtos;

namespace ReserveApp.Interfaces
{
  public interface IUserResourceService
  {
    Task<UserResourceDto> RequestResourceAsync(string userId, int resourceId,
            DateTime rentalStartTime, DateTime rentalEndTime);

    Task<IEnumerable<UserResourceDto>> GetUserResourcesByUserIdAsync(string userId);
    Task<UserResourceDto> ReturnResourceAsync(int userResourceId);
  }
}