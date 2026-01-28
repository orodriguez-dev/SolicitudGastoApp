using SolicitudGastoApp.Domain.Enums;

namespace SolicitudGastoApp.Application.DTOs
{
    public class DecideExpenseDto
    {
        public ApprovalDecision Decision { get; set; }
        public string? Comment { get; set; }
    }
}
