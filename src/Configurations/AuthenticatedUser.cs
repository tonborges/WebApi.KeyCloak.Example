using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace WebApi.KeyCloak.Example.Configurations;

public class AuthenticatedUser : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string Id = "a07175fe-ce5b-4fd1-b6ad-31bc2e1f91e4";
    private const string Name = "John Doe";
    private const string Role = "admins";

    public AuthenticatedUser(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, Id),
            new Claim(ClaimTypes.Name, Name),
            new Claim(ClaimTypes.Role, Role),
            new Claim(ClaimTypes.Email, "john.doe@email.com"),
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return await Task.FromResult(AuthenticateResult.Success(ticket));
    }
}