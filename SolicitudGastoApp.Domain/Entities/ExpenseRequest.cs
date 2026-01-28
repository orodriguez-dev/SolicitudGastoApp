
using SolicitudGastoApp.Domain.Enums;

namespace SolicitudGastoApp.Domain.Entities
{
    public class ExpenseRequest
    {
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; } = default!;

        public ExpenseType Type { get; set; }
        public ExpenseRequestStatus Status { get; set; } = ExpenseRequestStatus.Pending;

        public DateOnly RequestDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public decimal TotalAmount { get; set; }

        // Señales del motor de reglas
        public bool RequiresDoubleApproval { get; set; }
        public bool ExceedsBudget { get; set; }
        public bool MissingEvidence { get; set; }

        public string? Notes { get; set; }

        // Concurrency (evita “pisadas”)
        public byte[]? RowVersion { get; set; } = default!;

        // Navigation
        public ICollection<ExpenseItem> Items { get; set; } = new List<ExpenseItem>();
        public ICollection<Approval> Approvals { get; set; } = new List<Approval>();
    }
}
