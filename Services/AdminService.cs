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
        private readonly IResourceService _resourceService;

        public AdminService(UserManager<User> userManager, AppDbContext context, IResourceService resourceService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _resourceService = resourceService ?? throw new ArgumentNullException(nameof(resourceService));
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

        public async Task ChangeResourceStatusAsync(UserResourceDto userResourceDto, string newStatus)
        {
            var userResource = await _context.UserResources.FindAsync(userResourceDto.UserResourceId);
            if (userResource == null)
            {
                throw new Exception("Resource request not found");
            }

            userResource.Status = newStatus;

            var resource = await _context.Resources.FindAsync(userResource.ResourceId);
            if (resource == null)
            {
                throw new Exception("Resource not found");
            }

            if (newStatus == "Returned")
            {
                resource.Availability = true;
            }
            else if (newStatus == "Accepted")
            {
                resource.Availability = false;
            }

            _context.UserResources.Update(userResource);
            _context.Resources.Update(resource);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserResourceRentalDurationAsync(UserResourceDto userResourceDto, TimeSpan additionalRentalDuration)
        {
            var userResource = await _context.UserResources.FindAsync(userResourceDto.UserResourceId);
            if (userResource == null)
            {
                throw new Exception("Resource not found in user's inventory");
            }

            var newRentalDuration = userResource.RentalDuration + additionalRentalDuration;
            userResource.RentalDuration = newRentalDuration;

            await _context.SaveChangesAsync();
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
                RentalDuration = ur.RentalDuration,
                TimeRemaining = ur.TimeRemaining,
            }).ToList();
        }

        public async Task<IEnumerable<UserResourceDto>> GetUserResourcesByUserIdAsync(string userId)
        {
            var userResources = await _context.UserResources
                            .Where(ur => ur.UserId == userId)
                            .ToListAsync();

            if (userResources == null || !userResources.Any())
            {
                throw new Exception("No resources found for the specified user");
            }

            return userResources.Select(ur => new UserResourceDto
            {
                            UserResourceId = ur.UserResourceId,
                            UserId = ur.UserId.ToString(),
                            ResourceId = ur.ResourceId,
                            Status = ur.Status,
                            RentalStartTime = ur.RentalStartTime,
                            RentalDuration = ur.RentalDuration,
                            TimeRemaining = ur.TimeRemaining
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

        public async Task UpdateUserAsync(UserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            user.Name = userDto.Name;
            user.Surname = userDto.Surname;
            user.Email = userDto.Email;
            user.Department = userDto.Department;
            await _userManager.UpdateAsync(user);
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
                Email = adminEmail,
                Name = "admin",
                Surname = "admin",
                Department = "admin",
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