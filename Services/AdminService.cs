using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReserveApp.Data;
using ReserveApp.Dtos;
using ReserveApp.Interfaces;
using ReserveApp.Models;

namespace ReserveApp.Services
{
  public class AdminService : IAdminService
  {
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _context;

    public AdminService(UserManager<User> userManager, AppDbContext context)
    {
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ResourceDto> CreateResourceAsync(ResourceDto resourceDto)
    {
      var resource = new Resource
      {
              Name = resourceDto.Name,
              Description = resourceDto.Description,
              Type = resourceDto.Type,
              Image = resourceDto.Image,
              Availability = resourceDto.Availability
      };
      _context.Resources.Add(resource);
      await _context.SaveChangesAsync();

      resourceDto.ResourceId = resource.ResourceId;
      return resourceDto;
    }

    public async Task UpdateResourceAsync(int id, ResourceDto resourceDto)
    {
      var resource = await _context.Resources.FindAsync(id);
      if (resource == null)
      {
        throw new Exception("Resource not found");
      }

      resource.Name = resourceDto.Name;
      resource.Description = resourceDto.Description;
      resource.Type = resourceDto.Type;
      resource.Image = resourceDto.Image;
      resource.Availability = resourceDto.Availability;
      await _context.SaveChangesAsync();
    }

    public async Task DeleteResourceAsync(int id)
    {
      var resource = await _context.Resources.FindAsync(id);
      if (resource == null)
      {
        throw new Exception("Resource not found");
      }

      _context.Resources.Remove(resource);
      await _context.SaveChangesAsync();
    }

    public async Task ApproveUserRequestAsync(string userId, int resourceId)
    {
      using var transaction = await _context.Database.BeginTransactionAsync();

      try
      {
        var request = await _context.UserResources
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.ResourceId == resourceId);

        if (request == null)
          throw new Exception("Request not found");

        request.Status = "Approved";

        var resource = await _context.Resources.FindAsync(resourceId);
        if (resource == null)
          throw new Exception("Resource not found");

        resource.Availability = false;

        var otherRequests = await _context.UserResources
                .Where(ur =>
                        ur.ResourceId == resourceId && ur.Status == "Pending" &&
                        ur.UserId != userId)
                .ToListAsync();

        _context.UserResources.RemoveRange(otherRequests);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
      }
      catch (Exception)
      {
        await transaction.RollbackAsync();
        throw;
      }
    }

    public async Task RejectUserRequestAsync(string userId, int resourceId)
    {
      var userResource = await _context.UserResources.FindAsync(userId, resourceId);
      if (userResource == null)
      {
        throw new Exception("Resource request not found");
      }

      if (userResource.Status == "Pending")
      {
        _context.UserResources.Remove(userResource);
        await _context.SaveChangesAsync();
      }
      else
      {
        throw new Exception("Resource request is not in pending status");
      }
    }

    public async Task<IEnumerable<UserResourceDto>> GetExpiredUserResourcesAsync()
    {
      var expiredResources = await _context.UserResources
              .Where(ur => ur.RentalEndTime < DateTime.UtcNow && ur.Status != "Returned")
              .ToListAsync();

      return expiredResources.Select(ur => new UserResourceDto
      {
              UserResourceId = ur.UserResourceId,
              UserId = ur.UserId.ToString(),
              ResourceId = ur.ResourceId,
              Status = ur.Status,
              RentalStartTime = ur.RentalStartTime,
              RentalEndTime = ur.RentalEndTime
      }).ToList();
    }

    public async Task<ResourceDto> ReturnResourceToCirculationAsync(int userResourceId)
    {
      var userResource = await _context.UserResources
              .FirstOrDefaultAsync(ur => ur.UserResourceId == userResourceId);

      if (userResource == null)
      {
        throw new Exception("User resource not found");
      }

      var resource = await _context.Resources.FindAsync(userResource.ResourceId);
      if (resource == null)
      {
        throw new Exception("Resource not found");
      }

      resource.Availability = true;
      _context.UserResources.Remove(userResource);
      await _context.SaveChangesAsync();

      return new ResourceDto
      {
              ResourceId = resource.ResourceId,
              Name = resource.Name,
              Description = resource.Description,
              Type = resource.Type,
              Image = resource.Image,
              Availability = resource.Availability
      };
    }

    public async Task<IEnumerable<UserResourceDto>> GetAllUserResourcesAsync()
    {
      var userResources = await _context.UserResources.ToListAsync();
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

    public async Task<UserDto> GetUserByIdAsync(string userId)
    {
      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
      {
        throw new Exception("User not found");
      }

      return new UserDto
      {
              Id = user.Id,
              Name = user.Name,
              Surname = user.Surname,
              Email = user.Email,
              Department = user.Department,
      };
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
      var users = await _userManager.Users.ToListAsync();
      return users.Select(user => new UserDto
      {
              Id = user.Id,
              Name = user.Name,
              Surname = user.Surname,
              Email = user.Email,
              Department = user.Department,
      }).ToList();
    }

    public async Task DeleteUserAsync(string userId)
    {
      var user = await _userManager.FindByIdAsync(userId);
      if (user == null)
      {
        throw new Exception("User not found");
      }

      await _userManager.DeleteAsync(user);
    }

    public async Task InitializeAdminAsync()
    {
      const string adminEmail = "admin@gmail.com";
      var adminUser = await _userManager.FindByEmailAsync(adminEmail);

      if (adminUser != null)
      {
        return;
      }

      adminUser = new User
      {
              UserName = adminEmail,
              Email = adminEmail,
              Name = "Admin",
              Surname = "Adminowski",
              Department = "Administrator",
      };
      var createUserResult = await _userManager.CreateAsync(adminUser, "Admin123!");
      if (!createUserResult.Succeeded)
      {
        return;
      }

      await _userManager.AddToRoleAsync(adminUser, "Admin");
      await _userManager.AddToRoleAsync(adminUser, "User");
    }
}