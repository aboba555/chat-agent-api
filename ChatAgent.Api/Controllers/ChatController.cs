using Microsoft.AspNetCore.Mvc;

namespace ChatAgent.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        public class ChatRequest
        {
            public string Message { get; set; } = default!;
            public string WalletJwt { get; set; } = default!;
        }

        public class ChatResponse
        {
            public string Content { get; set; } = default!;
        }

        [HttpPost]
        public IActionResult Post([FromBody] ChatRequest request)
        {
            // todo
            var echo = $"Echo: {request.Message}";
            return Ok(new ChatResponse { Content = echo });
        }
    }
}