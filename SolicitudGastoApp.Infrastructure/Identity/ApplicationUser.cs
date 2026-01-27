using Microsoft.AspNetCore.Identity;
using SolicitudGastoApp.Domain.Entities;

namespace SolicitudGastoApp.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? NombreCompleto { get; set; }
        public ICollection<SolicitudGasto> Solicitudes { get; set; } = new List<SolicitudGasto>();
    }
}
