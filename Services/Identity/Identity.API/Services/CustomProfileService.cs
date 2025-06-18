using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Extensions;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using IdentityModel;

namespace Identity.API.Services;

public class CustomProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;

    public CustomProfileService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory)
    {
        _userManager = userManager;
        _claimsFactory = claimsFactory;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var subjectId = context.Subject.GetSubjectId();
        var user = await _userManager.FindByIdAsync(subjectId);

        if (user == null)
        {
            return;
        }

        // Start with subject claims (already filtered by IdentityServer)
        var claims = new List<Claim>(context.Subject.Claims);

        // Only add claims that are specifically requested but might be missing
        var requestedClaimTypes = context.RequestedClaimTypes.ToList();

        // Ensure we have the essential claims for requested scopes
        if (requestedClaimTypes.Contains(JwtClaimTypes.Name) && !claims.Any(c => c.Type == JwtClaimTypes.Name))
        {
            claims.Add(new Claim(JwtClaimTypes.Name, user.FullName ?? user.UserName ?? ""));
        }

        if (requestedClaimTypes.Contains(JwtClaimTypes.GivenName) && !claims.Any(c => c.Type == JwtClaimTypes.GivenName))
        {
            claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName ?? ""));
        }

        if (requestedClaimTypes.Contains(JwtClaimTypes.FamilyName) && !claims.Any(c => c.Type == JwtClaimTypes.FamilyName))
        {
            claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName ?? ""));
        }

        if (requestedClaimTypes.Contains(JwtClaimTypes.Email) && !claims.Any(c => c.Type == JwtClaimTypes.Email))
        {
            claims.Add(new Claim(JwtClaimTypes.Email, user.Email ?? ""));
        }

        if (requestedClaimTypes.Contains(JwtClaimTypes.EmailVerified) && !claims.Any(c => c.Type == JwtClaimTypes.EmailVerified))
        {
            claims.Add(new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed.ToString().ToLower()));
        }

        // Add role claims if requested
        if (requestedClaimTypes.Contains(JwtClaimTypes.Role))
        {
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                if (!claims.Any(c => c.Type == JwtClaimTypes.Role && c.Value == role))
                {
                    claims.Add(new Claim(JwtClaimTypes.Role, role));
                }
            }
        }

        // Filter claims to only include requested types
        context.IssuedClaims = claims
            .Where(x => requestedClaimTypes.Contains(x.Type))
            .ToList();
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var subjectId = context.Subject.GetSubjectId();
        var user = await _userManager.FindByIdAsync(subjectId);

        context.IsActive = user != null && user.EmailConfirmed;
    }
}
