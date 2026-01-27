using System;
using System.Collections.Generic;
using System.Text;

namespace SolicitudGastoApp.Domain.Entities
{
    public class SolicitudGasto
    {
        public int Id { get; set; }
        public string Concepto { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }

        // Relaciones, si aplican
        public string ApplicationUserId { get; set; } = string.Empty;
    }
}
