namespace SolicitudGastoApp.Application.DTOs
{
    public class AddExpenseItemDto
    {
        public string Concept { get; set; } = default!;
        public decimal Amount { get; set; }
        public string? EvidenceFileUrl { get; set; }
        public string? EvidenceNumber { get; set; }
    }
}
