using Microsoft.AspNetCore.Identity;

namespace PharmaERP.Identity.API.Entities
{
    public class ApplicationUser
    {
    }
}

/// <summary>
/// Extends ASP.NET Core Identity user with
/// ERP-specific employee and department information.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string EmployeeCode { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public int? DepartmentId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public DateTime? ModifiedOn { get; set; }
}