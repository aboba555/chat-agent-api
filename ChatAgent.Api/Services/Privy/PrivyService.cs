using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ChatAgent.Api.Services.Privy;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace ChatAgent.Api.Services.Privy;
public class PrivyService : IPrivyService
{
    private readonly IConfigurationManager<OpenIdConnectConfiguration> _configManager;
    private readonly TokenValidationParameters _validationParams;

    public PrivyService(IConfiguration config)
    {
        var jwksUrl  = config["Privy:JwksUrl"]!;
        var issuer   = config["Privy:Issuer"]!;
        var audience = config["Privy:Audience"]!;

        _configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            jwksUrl,
            new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever { RequireHttps = true }
        );

        _validationParams = new TokenValidationParameters
        {
            ValidIssuer             = issuer,
            ValidAudience           = audience,
            ValidateIssuer          = true,
            ValidateAudience        = true,
            ValidateLifetime        = true,
            ClockSkew               = TimeSpan.FromMinutes(2),
            IssuerSigningKeyResolver = (token, _, kid, _) =>
                _configManager
                    .GetConfigurationAsync(CancellationToken.None)
                    .Result
                    .SigningKeys
        };
    }

    public ClaimsPrincipal ValidateJwt(string jwt)
    {
        if (string.IsNullOrWhiteSpace(jwt))
            throw new SecurityTokenException("JWT is missing");

        var handler = new JwtSecurityTokenHandler();
        return handler.ValidateToken(jwt, _validationParams, out _);
    }
}