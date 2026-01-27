
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SolicitudGastoApp.Domain.Entities
{
    public class SolicitudGasto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Concepto { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        public DateTimeOffset Fecha { get; set; } = DateTimeOffset.UtcNow;

        // 🔗 FK a Identity
        [Required]
        public string UsuarioId { get; set; } = string.Empty;

        // Estado del flujo
        public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente;
    }

    public enum EstadoSolicitud
    {
        Pendiente = 0,
        Aprobada = 1,
        Rechazada = 2
    }
}
