using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizQuestionController : ControllerBase
    {
        private readonly IQuizQuestionService _quizQuestionService;

        public QuizQuestionController(IQuizQuestionService quizQuestionService)
        {
            _quizQuestionService = quizQuestionService;
        }

        [HttpGet("GetListByQuizId/{quizId}")]
        public async Task<IActionResult> GetListByQuizId(string quizId)
        {
            var (message, list) = await _quizQuestionService.GetListByQuizId(quizId);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Lấy danh sách câu hỏi Quiz thành công.", data = list });
        }

        [HttpGet("GetOneQuestionById/{quizQuestionId}")]
        public async Task<IActionResult> GetOneQuestionById(string quizQuestionId)
        {
            var (message, list) = await _quizQuestionService.GetOneQuestionById(quizQuestionId);
            if (message.Length > 0)
            {
               return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Lấy câu hỏi Quiz thành công.", data = list });
        }
    }
}
