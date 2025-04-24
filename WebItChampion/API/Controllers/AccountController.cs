using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AccountController : BaseController
    {
        private readonly IAccountService _iService;
        public AccountController(IAccountService iService)
        {
            _iService = iService;
        }

        [HttpGet("get-list")]
        public async Task<IActionResult> GetAccountList()
        {
            var userToken = GetUserId();
            var (message, list) = await _iService.GetList(userToken);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Lấy danh sách tài khoản thành công.", data = list });
        }

        [HttpPost("ToggleActive")]
        public async Task<IActionResult> DoToggleActive([FromBody] string userId)
        {
            var userToken = GetUserId();
            string message = await _iService.DoToggleActive(userToken, userId);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Thao tác thành công." });
        }

        [HttpGet("DoSearch")]
        public async Task<IActionResult> DoSearch(string query)
        {
            var (msg, list) = await _iService.DoSearch(query);
            if (msg.Length > 0) return BadRequest(msg);
            return Ok(list);
        }

        [HttpPut("change-role-active")]
        public async Task<IActionResult> ChangeRoleActive([FromBody]ChangeRoleRequest input)
        {
            var message = await _iService.EditRoleAndActive(input.UserId, input.RoleId, input.IsActive);
            if (message.Length > 0) return BadRequest(new { success = false, message });
            return Ok(new { success = true, message = "Thay đổi quyền thành công." });
        }
        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] string userId)
        {
            var message = await _iService.Delete(userId);
            if (message.Length > 0) return BadRequest(new { success = false, message });
            return Ok(new { success = true, message = "Xóa tài khoản thành công." });
        }
        [HttpGet("get-edit")]
        public async Task<IActionResult> GetEdit(string userId)
        {
            var (msg, user) = await _iService.GetEdit(userId);
            if (msg.Length > 0) return BadRequest(new { success = false, message = msg });
            return Ok(new { success = true, message = "Lấy thông tin tài khoản thành công.", data = user });
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetAccount(string userId)
        {
            var (msg, user) = await _iService.GetById(userId);
            if (msg.Length > 0) return BadRequest(new { success = false, message = msg });
            return Ok(new { success = true, message = "Lấy thông tin tài khoản thành công.", data = user });
        }
    }
    public class ChangeRoleRequest
    {
        public string UserId { get; set; }
        public int RoleId{ get; set; }
        public bool IsActive {  get; set; }
    }
}
