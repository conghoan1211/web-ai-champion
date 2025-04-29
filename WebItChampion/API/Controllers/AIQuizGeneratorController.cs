using API.Services;
using Microsoft.AspNetCore.Mvc;
using API.ViewModels;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIQuizGeneratorController : BaseController
    {
        private readonly IAIQuizGeneratorService _aiQuizService;

        public AIQuizGeneratorController(IAIQuizGeneratorService aiQuizService)
        {
            _aiQuizService = aiQuizService;
        }

        [HttpPost("GenerateQuizFromSkills")]
        public async Task<IActionResult> GenerateQuizFromSkills([FromForm] GenerateQuizRequest input)
        {
            input.UserId = GetUserId();
            var (message, questions) = await _aiQuizService.GenerateQuizFromSkills(input);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Tạo quiz thành công.", data = questions });
        }

        /// <summary>
        /// 
        /// This api is erroring
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="numberQuestion"></param>
        /// <param name="difficulty"></param>
        /// <returns></returns>
        [HttpPost("GenerateQuizFromWeakSkills")]
        public async Task<IActionResult> GenerateQuizFromWeakSkills(string userId, int numberQuestion, string difficulty = "medium")
        {
            var message = await _aiQuizService.GenerateQuizFromWeakSkillsAsync(userId, numberQuestion, difficulty);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Tạo quiz từ kỹ năng yếu thành công." });
        }

    }

}
