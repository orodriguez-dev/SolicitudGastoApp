using SolicitudGastoApp.Domain.Enums;

namespace SolicitudGastoApp.Domain.Entities
{
    public class Approval
    {
        public Guid Id { get; set; }

        public Guid ExpenseRequestId { get; set; }
        public ExpenseRequest ExpenseRequest { get; set; } = default!;

        public Guid ApproverEmployeeId { get; set; }
        public Employee ApproverEmployee { get; set; } = default!;

        public ApprovalDecision Decision { get; set; } = ApprovalDecision.Pending;

        public DateTime? DecidedAtUtc { get; set; } = DateTime.UtcNow;
        public string? Comment { get; set; }

        // Para saber si fue 1ra o 2da aprobación
        public int StepNumber { get; set; } = 1;
    }
}
