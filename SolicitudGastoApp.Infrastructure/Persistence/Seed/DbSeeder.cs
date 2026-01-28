using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SolicitudGastoApp.Domain.Entities;
using SolicitudGastoApp.Domain.Enums;
using SolicitudGastoApp.Infrastructure.Identity;
using System.Linq;

namespace SolicitudGastoApp.Infrastructure.Persistence.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await db.Database.MigrateAsync();

            // 1) ROLES
            var roles = new[] { "Employee", "Manager", "Finance", "Admin" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // 2) USUARIOS (ECUADOR) - Password: Ecuador123$
            var users = new[]
            {
                new { Email="juan.perez@empresa.ec",   Name="Juan Pérez",   Role="Employee", Area="OPS" },
                new { Email="maria.lopez@empresa.ec",  Name="María López",  Role="Employee", Area="OPS" },
                new { Email="carlos.mora@empresa.ec",  Name="Carlos Mora",  Role="Manager",  Area="OPS" },
                new { Email="ana.viteri@empresa.ec",   Name="Ana Viteri",   Role="Finance",  Area="FIN" },
                new { Email="diego.ortiz@empresa.ec",  Name="Diego Ortiz",  Role="Admin",    Area="IT"  }
            };

            foreach (var u in users)
            {
                var identityUser = await userManager.FindByEmailAsync(u.Email);

                if (identityUser == null)
                {
                    identityUser = new ApplicationUser
                    {
                        UserName = u.Email,
                        Email = u.Email,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(identityUser, "Ecuador123$");
                    if (!result.Succeeded)
                        throw new Exception(string.Join(" | ", result.Errors.Select(e => e.Description)));

                    await userManager.AddToRoleAsync(identityUser, u.Role);
                }

                if (!await db.Employees.AnyAsync(e => e.IdentityUserId == identityUser.Id))
                {
                    db.Employees.Add(new Employee
                    {
                        Id = Guid.NewGuid(),
                        IdentityUserId = identityUser.Id,
                        FullName = u.Name,
                        Email = u.Email,
                        AreaCode = u.Area,
                        Role = u.Role switch
                        {
                            "Employee" => EmployeeRole.Employee,
                            "Manager" => EmployeeRole.Manager,
                            "Finance" => EmployeeRole.Finance,
                            "Admin" => EmployeeRole.Admin,
                            _ => EmployeeRole.Employee
                        },
                        IsActive = true
                    });
                }
            }

            await db.SaveChangesAsync();

            // 3) POLICIES
            if (!await db.Policies.AnyAsync())
            {
                db.Policies.AddRange(
                    new Policy
                    {
                        Id = Guid.NewGuid(),
                        ExpenseType = ExpenseType.Travel,
                        Role = EmployeeRole.Employee,
                        RoleLimitAmount = 250m,
                        DoubleApprovalThreshold = 400m,
                        EvidenceRequired = true,
                        IsActive = true
                    },
                    new Policy
                    {
                        Id = Guid.NewGuid(),
                        ExpenseType = ExpenseType.Travel,
                        Role = EmployeeRole.Manager,
                        RoleLimitAmount = 1200m,
                        DoubleApprovalThreshold = 1500m,
                        EvidenceRequired = true,
                        IsActive = true
                    },
                    new Policy
                    {
                        Id = Guid.NewGuid(),
                        ExpenseType = ExpenseType.MinorPurchase,
                        Role = EmployeeRole.Employee,
                        RoleLimitAmount = 100m,
                        DoubleApprovalThreshold = 200m,
                        EvidenceRequired = true,
                        IsActive = true
                    }
                );
            }

            // 4) BUDGETS
            var now = DateTime.UtcNow;
            if (!await db.Budgets.AnyAsync(b => b.AreaCode == "OPS" && b.Year == now.Year && b.Month == now.Month))
            {
                db.Budgets.Add(new Budget
                {
                    Id = Guid.NewGuid(),
                    AreaCode = "OPS",
                    Year = now.Year,
                    Month = now.Month,
                    AllocatedAmount = 800m,
                    ConsumedAmount = 0m,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.SaveChangesAsync();
        }
    }
}