using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReserveApp.Data;
using ReserveApp.Dtos;
using ReserveApp.Interfaces;
using ReserveApp.Models;

namespace ReserveApp.Services;

public class AdminService: IAdminService
{
  private readonly UserManager<User> _userManager;
  private readonly AppDbContext _context;
  private readonly ResourceService _resourceService;
  
  public AdminService(UserManager<User> userManager, AppDbContext context)
  {
    _userManager = userManager;
    _context = context;
  }
  public async Task<ResourceDto> CreateResource(ResourceDto resourceDto)
  {
    var resource = new Resource
    {
      Name = resourceDto.Name,
      Description = resourceDto.Description,
      Type = resourceDto.Type,
      Quantity = resourceDto.Quantity,
    };
    _context.Resources.Add(resource);
    await _context.SaveChangesAsync();
    return resourceDto;
  }
  public async Task<IEnumerable<ResourceDto>> GetAllResourcesAsync()
  {
    return await _resourceService.GetAllResourcesAsync();
  }

  public async Task<ResourceDto> GetResourceByIdAsync(int id)
  {
    return await _resourceService.GetResourceByIdAsync(id);
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
    resource.Quantity = resourceDto.Quantity;
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
  
  public async Task<UserDto> GetUserByIdAsync(string id)
  {
    var user = await _userManager.FindByIdAsync(id);
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
      WorkCity = user.WorkCity,
    };
  }
  public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
  {
    var users = await _userManager.Users.ToListAsync();
    var userDtos = new List<UserDto>();
    foreach (var user in users)
    {
      userDtos.Add(new UserDto
      {
              Id = user.Id,
              Name = user.Name,
              Surname = user.Surname,
              Email = user.Email,
              Department = user.Department,
              WorkCity = user.WorkCity,
      });
    }
    return userDtos;
  }
  public async Task DeleteUserAsync(string id)
  {
    var user = await _userManager.FindByIdAsync(id);
    if (user == null)
    {
      throw new Exception("User not found");
    }
    await _userManager.DeleteAsync(user);
  }
  public async Task UpdateUserAsync(string id, UserDto userDto)
  {
    var user = await _userManager.FindByIdAsync(id);
    if (user == null)
    {
      throw new Exception("User not found");
    }
    user.Name = userDto.Name;
    user.Surname = userDto.Surname;
    user.Email = userDto.Email;
    user.Department = userDto.Department;
    user.WorkCity = userDto.WorkCity;
    await _userManager.UpdateAsync(user);
  }

  public async Task InitializeAdminAsync()
  {
    {
            const string adminEmail = "admin@gmail.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (adminUser != null)
            { 
              return; 
            } 
            adminUser = new User
    {
            Email = adminEmail,
            Name = "admin",
            Surname = "admin",
            Department = "admin",
            WorkCity = "admin"
    };
    var createUserResult = await _userManager.CreateAsync(adminUser, "Admin123!");
    if (!createUserResult.Succeeded)
    {
      return;
    }
    await _userManager.AddToRoleAsync(adminUser, "Admin");
    }
  }
}