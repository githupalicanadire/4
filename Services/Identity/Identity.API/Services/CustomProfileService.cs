using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

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

        var principal = await _claimsFactory.CreateAsync(user);
        var claims = principal.Claims.ToList();

        // Add custom claims
        claims.Add(new Claim("sub", user.Id));
        claims.Add(new Claim("name", user.FullName));
        claims.Add(new Claim("given_name", user.FirstName ?? ""));
        claims.Add(new Claim("family_name", user.LastName ?? ""));
        claims.Add(new Claim("email", user.Email ?? ""));
        claims.Add(new Claim("email_verified", user.EmailConfirmed.ToString().ToLower()));
        claims.Add(new Claim("preferred_username", user.UserName ?? ""));

        // Add role claims
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim("role", role));
        }

        // Filter claims based on requested scopes
        context.IssuedClaims = claims
            .Where(x => context.RequestedClaimTypes.Contains(x.Type))
            .ToList();
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var subjectId = context.Subject.GetSubjectId();
        var user = await _userManager.FindByIdAsync(subjectId);

        context.IsActive = user != null && user.EmailConfirmed;
    }
}
