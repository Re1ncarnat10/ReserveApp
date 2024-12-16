using ReserveApp.Dtos;
using ReserveApp.Models;

namespace ReserveApp.Interfaces;

public interface IResourceService
{
    Task<IEnumerable<ResourceDto>> GetAllResourcesAsync();
    Task<ResourceDto> GetResourceByIdAsync(int id);
}