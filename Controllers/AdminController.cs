using Microsoft.AspNetCore.Mvc;
using ReserveApp.Interfaces;
using ReserveApp.Dtos;

namespace ReserveApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("resource")]
        public async Task<IActionResult> CreateResource( ResourceDto resourceDto)
        {
            var result = await _adminService.CreateResourceAsync(resourceDto);
            return Ok(result);
        }
        
        [HttpPut("resource/{id}")]
        public async Task<IActionResult> UpdateResource(int id, ResourceDto resourceDto)
        {
            await _adminService.UpdateResourceAsync(id, resourceDto);
            return NoContent();
        }

        [HttpDelete("resource/{id}")]
        public async Task<IActionResult> DeleteResource(int id)
        {
            await _adminService.DeleteResourceAsync(id);
            return NoContent();
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpDelete("user/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _adminService.DeleteUserAsync(id);
            return NoContent();
        }
        
        [HttpPut("resource/status")]
        public async Task<IActionResult> ChangeResourceStatus(
                        [FromBody] UserResourceDto userResourceDto, string newStatus)
        {
            try
            {
                await _adminService.ChangeResourceStatusAsync(userResourceDto, newStatus);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("requests")]
        public async Task<ActionResult<IEnumerable<UserResourceDto>>> GetAllUserResources()
        {
            var userResources = await _adminService.GetAllUserResourcesAsync();
            return Ok(userResources);
        }
        
        [HttpPost("initialize-admin")]
        public async Task<IActionResult> InitializeAdmin()
        {
            await _adminService.InitializeAdminAsync();
            return Ok();
        }
    }
}