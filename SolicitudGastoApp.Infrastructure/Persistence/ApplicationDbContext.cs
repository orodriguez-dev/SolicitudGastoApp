using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SolicitudGastoApp.Application.Abstractions;
using SolicitudGastoApp.Domain.Entities;
using SolicitudGastoApp.Infrastructure.Identity;

namespace SolicitudGastoApp.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<SolicitudGasto> Solicitudes { get; set; } = default!;
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<ExpenseRequest> ExpenseRequests => Set<ExpenseRequest>();
        public DbSet<ExpenseItem> ExpenseItems => Set<ExpenseItem>();
        public DbSet<Approval> Approvals => Set<Approval>();
        public DbSet<Budget> Budgets => Set<Budget>();
        public DbSet<Policy> Policies => Set<Policy>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}