namespace SolicitudGastoApp.Domain.Entities
{
    public class ExpenseItem
    {
        public Guid Id { get; set; }

        public Guid ExpenseRequestId { get; set; }
        public ExpenseRequest ExpenseRequest { get; set; } = default!;

        public string Concept { get; set; } = default!;
        public decimal Amount { get; set; }

        // Evidencia (factura/comprobante)
        public string? EvidenceFileUrl { get; set; } // o EvidencePath
        public string? EvidenceNumber { get; set; }  // opcional: No. factura

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
