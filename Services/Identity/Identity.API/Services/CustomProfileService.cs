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

        var claims = new List<Claim>();

        // Add standard claims
        claims.Add(new Claim(JwtClaimTypes.Subject, user.Id));
        claims.Add(new Claim(JwtClaimTypes.Name, user.FullName ?? ""));
        claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName ?? ""));
        claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName ?? ""));
        claims.Add(new Claim(JwtClaimTypes.Email, user.Email ?? ""));
        claims.Add(new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed.ToString().ToLower()));
        claims.Add(new Claim(JwtClaimTypes.PreferredUserName, user.UserName ?? ""));

        // Add role claims
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(JwtClaimTypes.Role, role));
        }

        // Add custom user claims from ASP.NET Identity
        var userClaims = await _userManager.GetClaimsAsync(user);
        claims.AddRange(userClaims);

        // Remove duplicates and filter by requested claim types
        context.IssuedClaims = claims
            .GroupBy(x => x.Type)
            .Select(group => group.First()) // Take first of each type to avoid duplicates
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
