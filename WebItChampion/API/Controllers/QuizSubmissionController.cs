using API.Services;
using API.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizSubmissionController : BaseController
    {
        private readonly IQuizSubmissionService _quizSubmissionService;
        public QuizSubmissionController(IQuizSubmissionService quizSubmissionService)
        {
            _quizSubmissionService = quizSubmissionService;
        }

        [HttpPost("SubmitQuiz")]
        public async Task<IActionResult> SubmitQuiz([FromBody] QuizSubmissionRequest request)
        {
            request.UserId = GetUserId();
            var result = await _quizSubmissionService.ProcessQuizSubmission(request);
            if (result == null)
            {
                return BadRequest(new { success = false, message = "Failed to submit quiz." });
            }
            return Ok(new { success = true, message = "Quiz submitted successfully.", data = result });
        }
    }
}
