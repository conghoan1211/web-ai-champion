using API.Configurations;
using API.Models;
using AutoMapper;
using Newtonsoft.Json;
using System.Text;

namespace API.Services
{
    public interface IAIQuestionService
    {

    }

    public class AIQuestionService
    {
        private readonly Sep490Context _context;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public AIQuestionService(Sep490Context context, HttpClient httpClient, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _httpClient = httpClient;
        }
        #region
        private readonly string AIApiKey = ConfigManager.gI().GeminiKey;
        private readonly string AIUri = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

        private readonly string InitialSystemPrompt = @"Bạn là trợ lý AI của VitalCare, chuyên tư vấn về sức khỏe xương khớp. Chỉ trả lời các nội dung liên quan đến cơ, xương, khớp, các sản phẩm và nội dung hỗ trợ chăm sóc xương khớp. 
                                        Trả lời ngắn gọn, dễ hiểu, ưu tiên giải pháp tự nhiên, bài tập hỗ trợ và khuyên người dùng nên gặp bác sĩ khi cần thiết.
                                   Chú ý: Sau khi trả lời câu hỏi, hãy nói cho người dùng rằng: 'Câu trả lời trên chỉ mang tính chất tham khảo, quyết định cuối cùng vẫn phụ thuộc vào bạn, vui lòng tham khảo ý kiến bác sĩ hoặc chuyên gia trước khi quyết định'.";

        private readonly string SecondarySystemPrompt = @"Website cung cấp sản phẩm (sữa, miếng dán, thực phẩm dinh dưỡng) và nội dung xương khớp.\n
                                        Hướng người dùng vào danh sách sản phẩm nếu hỏi cụ thể, trả lời trọn vẹn, cô đọng, tối đa 600 token, ít dùng đậm/nghiêng.";

        private readonly string StrictScopePrompt = @"Chú ý: Tuyệt đối không trả lời các câu hỏi không liên quan đến cơ xương khớp. Nếu phát hiện nội dung hỏi không đúng phạm vi,
                                        hãy lịch sự từ chối và nhắc rằng bạn chỉ hỗ trợ chuyên sâu về sức khỏe cơ xương khớp. Không cố gắng đưa ra câu trả lời cho các lĩnh vực khác.";
        #endregion

        //public async Task<string> GetAIResponse(string question)
        //{
        //    var request = new HttpRequestMessage(HttpMethod.Post, AIUri);
        //    request.Headers.Add("Authorization", $"Bearer {AIApiKey}");
        //    request.Headers.Add("Accept", "application/json");

        //    var body = new
        //    {
        //        prompt = new
        //        {
        //            context = InitialSystemPrompt + SecondarySystemPrompt + StrictScopePrompt,
        //            query = question
        //        },
        //        temperature = 0.5,
        //        maxOutputTokens = 600,
        //        topK = 40,
        //        topP = 0.95,
        //        stopSequences = new[] { "\n" }
        //    };

        //    request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

        //    var response = await _httpClient.SendAsync(request);
        //    response.EnsureSuccessStatusCode();

        //    var jsonResponse = await response.Content.ReadAsStringAsync();
        //    var aiResponse = JsonConvert.DeserializeObject<AIResponse>(jsonResponse);

        //    return aiResponse?.candidates?[0]?.output ?? "Không có phản hồi từ AI.";
        //}

    }
}
