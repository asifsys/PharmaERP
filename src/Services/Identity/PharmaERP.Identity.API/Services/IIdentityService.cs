using PharmaERP.Identity.API.DTOs;

namespace PharmaERP.Identity.API.Services;

public interface IIdentityService
{
    Task<LoginResponse> ValidateUserAsync(LoginRequest request);
}