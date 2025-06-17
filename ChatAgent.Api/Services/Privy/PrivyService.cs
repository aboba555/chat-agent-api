using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace ChatAgent.Api.Services.Privy
{
    public class PrivyService : IPrivyService
    {
        private readonly TokenValidationParameters? _validationParams;
        private readonly bool _isDevFallback;

        public PrivyService(IConfiguration config)
        {
            var jwksUrl  = config["Privy:JwksUrl"]!;
            var issuer   = config["Privy:Issuer"]!;
            var audience = config["Privy:Audience"]!;

            try
            {
                var jwksJson = new HttpClient().GetStringAsync(jwksUrl).Result;
                var jwks     = new JsonWebKeySet(jwksJson);

                _validationParams = new TokenValidationParameters
                {
                    IssuerSigningKeys     = jwks.Keys,
                    ValidIssuer           = issuer,
                    ValidAudience         = audience,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer        = true,
                    ValidateAudience      = true
                };
                _isDevFallback = false;
            }
            catch
            {
                // dev fallback
                _isDevFallback = true;
            }
        }

        public ClaimsPrincipal ValidateJwt(string jwt)
        {
            if (_isDevFallback)
            {
                // test principal ( fallback )
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim("walletAddress", "dev-wallet-ADDRESS")
                }, "DevFallback");
                return new ClaimsPrincipal(identity);
            }

            var handler   = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(jwt, _validationParams!, out _);
            return principal;
        }
    }
}