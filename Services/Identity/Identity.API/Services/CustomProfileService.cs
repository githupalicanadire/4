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

        // Add standard identity claims based on requested scopes
        var requestedClaimTypes = context.RequestedClaimTypes.ToList();

        // Add subject (always required)
        claims.Add(new Claim(JwtClaimTypes.Subject, user.Id));

        // Add profile claims if profile scope requested
        if (requestedClaimTypes.Contains(JwtClaimTypes.Name))
        {
            claims.Add(new Claim(JwtClaimTypes.Name, user.FullName?.Trim() ?? user.UserName ?? ""));
        }

        if (requestedClaimTypes.Contains(JwtClaimTypes.GivenName))
        {
            claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName ?? ""));
        }

        if (requestedClaimTypes.Contains(JwtClaimTypes.FamilyName))
        {
            claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName ?? ""));
        }

        if (requestedClaimTypes.Contains(JwtClaimTypes.PreferredUserName))
        {
            claims.Add(new Claim(JwtClaimTypes.PreferredUserName, user.UserName ?? ""));
        }

        // Always include username for basket operations (regardless of scope)
        claims.Add(new Claim("username", user.UserName ?? ""));
        claims.Add(new Claim("user_id", user.Id));

        // Add email claims if email scope requested
        if (requestedClaimTypes.Contains(JwtClaimTypes.Email))
        {
            claims.Add(new Claim(JwtClaimTypes.Email, user.Email ?? ""));
            claims.Add(new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed.ToString().ToLower()));
        }

        // Add role claims if roles scope requested
        if (requestedClaimTypes.Contains(JwtClaimTypes.Role))
        {
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role));
            }
        }

        // Filter and return only requested claims
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
