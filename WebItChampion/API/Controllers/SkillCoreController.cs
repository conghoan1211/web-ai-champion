using API.Services;
using API.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillCoreController : ControllerBase
    {
        private readonly ISkillCoreService _skillService;

        public SkillCoreController(ISkillCoreService skillService)
        {
            _skillService = skillService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var (message, list) = await _skillService.GetAll();
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Lấy danh sách kĩ năng thành công.", data = list });
        }

        [HttpPost("create-update")]
        public async Task<IActionResult> CreateUpdate([FromBody] CreateUpdateSkillVM model)
        {
            var (message, skill) = await _skillService.CreateUpdate(model);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Thao tác thành công.", data = skill });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var message = await _skillService.Delete(id);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Xóa kĩ năng thành công." });
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var (message, skill) = await _skillService.GetById(id);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Lấy kĩ năng thành công.", data = skill });
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string query)
        {
            var (message, list) = await _skillService.Search(query);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Tìm kiếm kĩ năng thành công.", data = list });
        }

    }
}
