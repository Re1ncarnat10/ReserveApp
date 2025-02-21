using Microsoft.AspNetCore.Mvc;
using ReserveApp.Interfaces;
using ReserveApp.Dtos;

namespace ReserveApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserResourceController : ControllerBase
    {
        private readonly IUserResourceService _userResourceService;

        public UserResourceController(IUserResourceService userResourceService)
        {
            _userResourceService = userResourceService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestResource([FromBody] UserResourceDto requestDto)
        {
            try
            {
                if (string.IsNullOrEmpty(requestDto.UserId))
                {
                    return BadRequest("Invalid UserId");
                }

                var result = await _userResourceService.RequestResourceAsync(requestDto.UserId, requestDto.ResourceId, requestDto.RentalStartTime, requestDto.RentalDuration);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut("return/{userResourceId}")]
        public async Task<IActionResult> ReturnResource(int userResourceId)
        {
            try
            {
                var result = await _userResourceService.ReturnResourceAsync(userResourceId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{userResourceId}")]
        public async Task<IActionResult> DeleteResource(int userResourceId)
        {
            try
            {
                await _userResourceService.DeleteResourceAsync(userResourceId);
                return Ok("Resource deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}