using Microsoft.EntityFrameworkCore;
using ReserveApp.Interfaces;
using ReserveApp.Data;
using ReserveApp.Models;
using ReserveApp.Dtos;

namespace ReserveApp.Services;

public class UserResourceService : IUserResourceService
{
  private readonly AppDbContext _context;

  public UserResourceService(AppDbContext context)
  {
    _context = context;
  }

  public async Task<UserResourceDto> RequestResourceAsync(string userId, int resourceId,
          DateTime rentalStartTime, TimeSpan rentalDuration)
  {
    var resource = await _context.Resources.FindAsync(resourceId);
    if (resource == null || !resource.Availability)
    {
      throw new Exception("Resource is not available");
    }

    var userResource = new UserResource
    {
            UserId = userId,
            ResourceId = resourceId,
            RentalStartTime = rentalStartTime,
            RentalDuration = rentalDuration
    };
    _context.UserResources.Add(userResource);
    await _context.SaveChangesAsync();

    return new UserResourceDto
    {
            UserResourceId = userResource.UserResourceId,
            UserId = userResource.UserId.ToString(),
            ResourceId = userResource.ResourceId,
            RentalStartTime = userResource.RentalStartTime,
            RentalDuration = userResource.RentalDuration,
            TimeRemaining = userResource.TimeRemaining
    };
  }

  public async Task<IEnumerable<UserResourceDto>> GetUserResourcesByUserIdAsync(string userId)
  {
    var userResources = await _context.UserResources
            .Where(ur => ur.UserId == userId)
            .ToListAsync();

    return userResources.Select(ur => new UserResourceDto
    {
            UserResourceId = ur.UserResourceId,
            UserId = ur.UserId.ToString(),
            ResourceId = ur.ResourceId,
            Status = ur.Status,
            RentalStartTime = ur.RentalStartTime,
            RentalDuration = ur.RentalDuration,
            TimeRemaining = ur.TimeRemaining
    });
  }

  public async Task<UserResourceDto> ReturnResourceAsync(int userResourceId)
  {
    var userResource = await _context.UserResources.FindAsync(userResourceId);
    if (userResource == null)
    {
      throw new Exception("Resource not found in user's inventory");
    }

    if (userResource.Status == "Accepted")
    {
      userResource.Status = "Waiting for confirmation";
    }
    else if (userResource.Status == "Pending")
    {
      userResource.Status = "Cancelled";
    }

    await _context.SaveChangesAsync();

    return new UserResourceDto
    {
            UserResourceId = userResource.UserResourceId,
            UserId = userResource.UserId.ToString(),
            ResourceId = userResource.ResourceId,
            RentalStartTime = userResource.RentalStartTime,
            RentalDuration = userResource.RentalDuration,
            TimeRemaining = userResource.TimeRemaining
    };
  }

  public async Task DeleteResourceAsync(int userResourceId)
  {
    var userResource = await _context.UserResources.FindAsync(userResourceId);
    if (userResource == null)
    {
      throw new Exception("Resource not found in user's inventory");
    }

    if (userResource.Status == "Declined" || userResource.Status == "Cancelled")
    {
      _context.UserResources.Remove(userResource);
      await _context.SaveChangesAsync();
    }
    else
    {
      throw new Exception("Resource status is not Declined or Cancelled");
    }
  }
}