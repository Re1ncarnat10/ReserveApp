using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReserveApp.Dtos;
using ReserveApp.Interfaces;
using ReserveApp.Models;
using ReserveApp.Data;

namespace ReserveApp.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _context;

    public UserService(UserManager<User> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<UserDto> GetUserInfoAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        return new UserDto
        {
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            Department = user.Department,
        };
    }

    public async Task<IdentityResult> UpdateUserInfoAsync(string userId, UserDto userDto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found" });
        }

        user.Name = userDto.Name;
        user.Surname = userDto.Surname;
        user.Email = userDto.Email;
        user.Department = userDto.Department;
        return await _userManager.UpdateAsync(user);
    }

    public async Task<IEnumerable<UserHistoryDto>> GetUserHistoryAsync(string userId)
    {
        var histories = await _context.UserHistories
            .Where(h => h.UserId == userId)
            .ToListAsync();

        return histories.Select(h => new UserHistoryDto
        {
            UserHistoryId = h.UserHistoryId,
            UserId = h.UserId,
            ResourceId = h.ResourceId,
            ResourceName = h.ResourceName,
            ResourceDescription = h.ResourceDescription,
            ResourceType = h.ResourceType,
            ResourceImage = h.ResourceImage,
            ApprovedAt = h.ApprovedAt,
            RentalStartTime = h.RentalStartTime,
            RentalEndTime = h.RentalEndTime
        });
    }
}