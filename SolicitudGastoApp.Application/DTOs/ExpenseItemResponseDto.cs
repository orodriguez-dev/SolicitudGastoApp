namespace SolicitudGastoApp.Application.DTOs
{
    public class ExpenseItemResponseDto
    {
        public Guid Id { get; set; }
        public string Concept { get; set; } = "";
        public decimal Amount { get; set; }
        public string? EvidenceFileUrl { get; set; }
        public string? EvidenceNumber { get; set; }
    }
}
