using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SolicitudGastoApp.Domain.Entities;
using SolicitudGastoApp.Infrastructure.Identity;

namespace SolicitudGastoApp.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<SolicitudGasto> Solicitudes { get; set; } = default!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}