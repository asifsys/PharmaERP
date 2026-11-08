
using Microsoft.AspNetCore.Identity;

namespace PharmaERP.Identity.API.Configurations;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager =
            services.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles =
        [
            "ExecutiveAdmin",
            "ProductAdmin",
            "OrderAdmin",
            "StoreAdmin",
            "FinanceAdmin",
            "PaymentAdmin",
            "SalaryAdmin",
            "ReportsAdmin",
            "ManufacturingAdmin",
            "QAAdmin",
            "QCAdmin",
            "DepartmentManager",
            "DepartmentEmployee",
            "Auditor"
        ];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(
                    new IdentityRole(role));

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Could not create role: {role}");
                }
            }
        }
    }
}