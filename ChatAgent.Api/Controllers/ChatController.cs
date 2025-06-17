using System.Security.Claims;
using ChatAgent.Api.Models;
using ChatAgent.Api.Services.OpenAI;
using ChatAgent.Api.Services.Privy;
using ChatAgent.Api.Services.Solana;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ChatAgent.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IPrivyService       _privy;
        private readonly IOpenAiService      _openAi;
        private readonly ISolanaAgentService _solana;

        public ChatController(
            IPrivyService privyService,
            IOpenAiService openAiService,
            ISolanaAgentService solanaAgentService)
        {
            _privy   = privyService;
            _openAi  = openAiService;
            _solana  = solanaAgentService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChatRequest request)
        {
            ClaimsPrincipal user;
            try
            {
                user = _privy.ValidateJwt(request.WalletJwt);
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(new { error = "Invalid JWT", details = ex.Message });
            }
            var walletAddress = user.FindFirst("walletAddress")?.Value;
            if (string.IsNullOrEmpty(walletAddress))
                return BadRequest(new { error = "Wallet address not found in JWT" });

            string result;
            var msg = request.Message.Trim();

            if (msg.Contains("swap", StringComparison.OrdinalIgnoreCase) ||
                msg.Contains("переведи", StringComparison.OrdinalIgnoreCase) ||
                msg.Contains("transfer", StringComparison.OrdinalIgnoreCase))
            {

                result = await _solana.ExecuteAsync(walletAddress, msg);
            }
            else
            {
                result = await _openAi.GenerateAsync(msg);
            }

            return Ok(new ChatResponse { Content = result });
        }
    }
}