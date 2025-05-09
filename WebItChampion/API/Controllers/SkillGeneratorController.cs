﻿using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillGeneratorController : ControllerBase
    {
        private readonly ISkillGeneratorService _skillGeneratedService;
        public SkillGeneratorController(ISkillGeneratorService skillGeneratedService)
        {
            _skillGeneratedService = skillGeneratedService;
        }

        [HttpPost("GenerateSkillFromAnswers")]
        public async Task<IActionResult> GenerateSkillFromAnswers(string userId, int numberSkill)
        {
            var (message, skills) = await _skillGeneratedService.GenerateSkillFromAnswers(userId);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Tạo kỹ năng thành công.", data = skills });
        }

        [HttpPost("GenerateSkillFromTopic")]
        public async Task<IActionResult> GenerateSkillFromTopic(int topicId, string studentId, int numberSkill)
        {
            var (message, skills) = await _skillGeneratedService.GenerateSkillFromTopic (topicId, studentId, numberSkill);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Tạo kỹ năng từ kỹ năng yếu thành công.", data = skills });
        }
    }
}
