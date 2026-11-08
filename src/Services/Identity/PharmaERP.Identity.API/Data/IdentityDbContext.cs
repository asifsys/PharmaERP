using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PharmaERP.Identity.API.Entities;

namespace PharmaERP.Identity.API.Data;

public class IdentityDbContext
    : IdentityDbContext<ApplicationUser>
{
    public IdentityDbContext(
        DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(
        ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
            .Property(x => x.FullName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Entity<ApplicationUser>()
            .Property(x => x.EmployeeCode)
            .HasMaxLength(30);

        builder.Entity<ApplicationUser>()
            .HasIndex(x => x.EmployeeCode)
            .IsUnique()
            .HasFilter("[EmployeeCode] IS NOT NULL AND [EmployeeCode] <> ''");
    }
}