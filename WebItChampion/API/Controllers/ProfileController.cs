using API.Services;
using API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : BaseController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProfileService _iProfileService;
        public ProfileController(IProfileService iProfileService, IHttpContextAccessor httpContextAccessor)
        {
            _iProfileService = iProfileService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Lấy thông tin người dùng bằng ID 
        /// </summary>
        /// 
        /// <returns></returns>
        [HttpGet("GetUserByID")]
        public async Task<IActionResult> GetUserByID()
        {
            string userId = GetUserId();
            var (msg, user) = await _iProfileService.GetProfile(userId);
            if (msg.Length > 0) return BadRequest(msg);
            return Ok(user);
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetProfile()
        {
            string userId = GetUserId();
            var (message, user) = await _iProfileService.GetProfileUpdate(userId);
            if (message.Length > 0)
            {
                return BadRequest( new {
                    success = false,  message
                });
            }
            return Ok( new {
                success = true,  message = "Đã lấy được dữ liệu.", data = user
            });
        }


        [HttpPost("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileModels updatedProfile)
        {
            string userId = GetUserId();
            if (updatedProfile == null)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errorCode = "INVALID_REQUEST" });
            }
            string message = await _iProfileService.UpdateProfile(userId, updatedProfile, HttpContext);
            if (message.Length > 0)
            {
                return BadRequest(new {
                    success = false,  message,
                });
            }
            return Ok(new
            {
                success = true,  message = "Update Profile Successfully!",
            });
        }

        [HttpPost("ChangeAvatar")]
        public async Task<IActionResult> DoChangeAvatar(UpdateAvatarVM input)
        {
            string userID = GetUserId();
            if (input.Image == null || input.Image.Length == 0)
            {
                return BadRequest(new  {
                    success = false,  message = "Không có tệp avatar."
                });
            }
             var (message, url) = await _iProfileService.DoChangeAvatar(userID, input, HttpContext);
            if (message.Length > 0)
            {
                return BadRequest( new {
                    success = false,  message
                });
            }
            return Ok( new {
                success = true,  message = "Đổi avatar thành công.", data = url
            });
        }
    }
}
