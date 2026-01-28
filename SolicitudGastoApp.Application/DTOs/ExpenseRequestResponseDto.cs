using SolicitudGastoApp.Domain.Enums;

namespace SolicitudGastoApp.Application.DTOs
{
    public class ExpenseRequestResponseDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = "";
        public string AreaCode { get; set; } = "";

        public ExpenseType Type { get; set; }
        public ExpenseRequestStatus Status { get; set; }
        public DateOnly RequestDate { get; set; }
        public decimal TotalAmount { get; set; }

        public bool MissingEvidence { get; set; }
        public bool RequiresDoubleApproval { get; set; }
        public bool ExceedsBudget { get; set; }

        public string? Notes { get; set; }

        public List<ExpenseItemResponseDto> Items { get; set; } = new();
        public List<ApprovalResponseDto> Approvals { get; set; } = new();
    }
}
