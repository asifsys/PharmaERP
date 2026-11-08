namespace PharmaERP.Identity.API.DTOs;

public sealed class LoginResponse
{
    public bool Succeeded { get; set; }

    public string? UserId { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public List<string> Roles { get; set; } = [];

    public string? Message { get; set; }
}