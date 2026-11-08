using Microsoft.AspNetCore.Identity;
using PharmaERP.Identity.API.DTOs;
using PharmaERP.Identity.API.Entities;

namespace PharmaERP.Identity.API.Services;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<LoginResponse> ValidateUserAsync(
        LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(
            request.Email);

        if (user == null || !user.IsActive)
        {
            return new LoginResponse
            {
                Succeeded = false,
                Message = "Invalid login credentials."
            };
        }

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            return new LoginResponse
            {
                Succeeded = false,
                Message = "Invalid login credentials."
            };
        }

        var roles = await _userManager.GetRolesAsync(user);

        return new LoginResponse
        {
            Succeeded = true,
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Roles = roles.ToList(),
            Message = "User validated successfully."
        };
    }
}