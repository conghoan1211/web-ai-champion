using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : BaseController
    {
        private readonly IQuizService _quizService;
        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpGet("GetListByTopicId")]
        public async Task<IActionResult> GetListByTopicId([FromQuery] List<int> topicId, [FromQuery] int? difficulty)
        {
            var (message, quizzes) = await _quizService.GetListByTopicId(topicId, difficulty);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Lấy danh sách quiz thành công.", data = quizzes });
        }

        [HttpGet("GetOneQuizById")]
        public async Task<IActionResult> GetOneQuizById([FromQuery] string quizId)
        {
            var (message, quiz) = await _quizService.GetByQuizId(quizId);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Lấy quiz thành công.", data = quiz });
        }

        [HttpGet("GetAllQuizzes")]
        public async Task<IActionResult> GetAllQuizzes()
        {
            var (message, quizzes) = await _quizService.GetAll();
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Lấy danh sách quiz thành công.", data = quizzes });
        }

        [HttpGet("GetQuizzesByUserId")]
        public async Task<IActionResult> GetQuizzesByUserId()
        {
            string userId = GetUserId();
            var (message, quizzes) = await _quizService.GetByUserId(userId);
            if (message.Length > 0)
            {
                return BadRequest(new { success = false, message });
            }
            return Ok(new { success = true, message = "Lấy danh sách quiz thành công.", data = quizzes });
        }
    }
}
