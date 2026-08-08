using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Starbender.FileClerk.Demo.Security;
using Volo.Abp.Security.Claims;

namespace Starbender.FileClerk.Demo;

public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AuthenticationScheme = "AdministrativeAccessTest";
    public const string UserHeader = "X-Test-User";
    public const string PermissionHeader = "X-Test-Permission";

    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(UserHeader, out var userId))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new List<Claim>
        {
            new(AbpClaimTypes.UserId, userId.ToString()),
            new(AbpClaimTypes.UserName, "access-test-admin"),
            new(AbpClaimTypes.Email, "access-test-admin@example.test")
        };

        if (Request.Headers.TryGetValue(PermissionHeader, out var permissions))
        {
            claims.AddRange(
                permissions
                    .SelectMany(permission => permission!.Split(
                        ',',
                        StringSplitOptions.RemoveEmptyEntries |
                        StringSplitOptions.TrimEntries))
                    .Select(permission =>
                        new Claim(TestAuthorizationClaimTypes.Permission, permission)));
        }

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, AuthenticationScheme));
        return Task.FromResult(
            AuthenticateResult.Success(new AuthenticationTicket(principal, AuthenticationScheme)));
    }
}
