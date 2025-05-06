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

        /// <summary>
        /// Generate quiz from skills
        /// Difficulty: easy: 1, medium: 2, hard: 3, veryhard: 4
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("GenerateQuizFromSkills")]
        public async Task<IActionResult> GenerateQuizFromSkills( [FromBody] GenerateQuizFromSkills input)
        {
            input.UserId = GetUserId();
            var (message, questions) = await _aiQuizService.GenerateQuizFromSkills(input);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Tạo quiz thành công.", data = questions });
        }
      
        [HttpPost("GenerateQuizFromWrongAnswers")]
        public async Task<IActionResult> GenerateQuizFromWrongAnswers([FromBody] GenerateQuizWrongAnswers input)
        {
            input.UserId = GetUserId();
            var (message, questions) = await _aiQuizService.GenerateQuizFromWrongAnswers(input.UserId, input.NumberQuestion,input.Title, input.Description);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Tạo quiz từ câu trả lời sai thành công.", data = questions });
        }

        /// <summary
        /// This api is erroring
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="numberQuestion"></param>
        /// <param name="difficulty"></param>
        /// <returns></returns>
        [HttpPost("GenerateQuizFromWeakSkills")]
        public async Task<IActionResult> GenerateQuizFromWeakSkills([FromForm] GenerateQuizWeaknessSkills input)
        {
            input.UserId = GetUserId();
            var (message, questions) = await _aiQuizService.GenerateQuizFromWeakSkills(input);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Tạo quiz từ kỹ năng yếu thành công.", data = questions });
        }

    }

}
