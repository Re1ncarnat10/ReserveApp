using ReserveApp.Interfaces;
using ReserveApp.Data;
using ReserveApp.Models;

namespace ReserveApp.Services;

public class UserResourceService : IUserResourceService
{
  private readonly AppDbContext _context;

  public UserResourceService(AppDbContext context)
  {
    _context = context;
  }
  public async Task<UserResource> RequestResourceAsync(int userId, int resourceId, DateTime rentalStartTime, TimeSpan rentalDuration)
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
    return userResource;
  }

  public async Task<UserResource> ExtendResourceAsync(int userResourceId, TimeSpan rentalDuration)
  {
    var userResource = await _context.UserResources.FindAsync(userResourceId);
    if (userResource == null)
    {
      throw new Exception("Resource not found in user's inventory");
    }

    if (userResource.Status == "Accepted")
    {
      userResource.Status = "Waiting for decision";
    }
    await _context.SaveChangesAsync();
    return userResource;
  }

  public async Task<UserResource> ReturnResourceAsync(int userResourceId)
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
    return userResource;
  }
}