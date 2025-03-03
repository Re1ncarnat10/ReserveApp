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
          DateTime rentalStartTime, DateTime rentalEndTime)
  {
    var user = await _context.Users.FindAsync(userId);
    var resource = await _context.Resources.FindAsync(resourceId);

    if (user == null || resource == null)
    {
      throw new Exception("User or resource not found");
    }

    if (!resource.Availability)
    {
      throw new Exception("Resource is not available");
    }

    var userResource =
            await _context.UserResources.FirstOrDefaultAsync(ur =>
                    ur.UserId == userId && ur.ResourceId == resourceId);

    if (userResource != null)
    {
      throw new Exception("Resource already requested");
    }

    userResource = new UserResource(userId, resourceId, rentalStartTime, rentalEndTime);

    _context.UserResources.Add(userResource);
    await _context.SaveChangesAsync();

    return new UserResourceDto
    {
            UserResourceId = userResource.UserResourceId,
            UserId = userResource.UserId,
            ResourceId = userResource.ResourceId,
            RentalStartTime = userResource.RentalStartTime,
            RentalEndTime = userResource.RentalEndTime,
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
            RentalEndTime = ur.RentalEndTime
    }).ToList();
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
            UserId = userResource.UserId,
            ResourceId = userResource.ResourceId,
            RentalStartTime = userResource.RentalStartTime,
            RentalEndTime = userResource.RentalEndTime
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