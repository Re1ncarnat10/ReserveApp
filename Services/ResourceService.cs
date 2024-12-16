using Microsoft.EntityFrameworkCore;
using ReserveApp.Data;
using ReserveApp.Dtos;
using ReserveApp.Interfaces;
using ReserveApp.Models;

namespace ReserveApp.Services;

public class ResourceService: IResourceService
{
  private readonly AppDbContext _context;
  public ResourceService(AppDbContext context)
  {
    _context = context;
  }
  
  public async Task<IEnumerable<ResourceDto>> GetAllResourcesAsync()
  {
    return await _context.Resources
            .Include(r => r.UserResources)
            .Select(r => new ResourceDto
    {
      ResourceId = r.ResourceId,
      Name = r.Name,
      Description = r.Description,
      Type = r.Type,
      Quantity = r.Quantity,
      QuantityAvailable = CalculateQuantityAvailable(r)
    })
            .ToListAsync();
            
  }
  
  public async Task<ResourceDto> GetResourceByIdAsync(int id)
  {
    var resource = await _context.Resources
            .Include(r => r.UserResources)
            .FirstOrDefaultAsync(r => r.ResourceId == id);
    if(resource == null)
    {
      return null;
    }
    return new ResourceDto
    {
      ResourceId = resource.ResourceId,
      Name = resource.Name,
      Description = resource.Description,
      Type = resource.Type,
      Quantity = resource.Quantity,
      QuantityAvailable = CalculateQuantityAvailable(resource)
    };
  }
  private int CalculateQuantityAvailable(Resource resource)
  {
    var reservedCount = resource.UserResources.Count(ur => ur.Status == "Reserved");
    return resource.Quantity - reservedCount;
  }
}