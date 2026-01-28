using SolicitudGastoApp.Domain.Enums;

namespace SolicitudGastoApp.Domain.Entities
{
    public class Employee
    {
        public Guid Id { get; set; }

        // Link con Identity (AspNetUsers)
        public string IdentityUserId { get; set; } = default!;

        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;

        public string AreaCode { get; set; } = default!; // ej: "FIN", "OPS", "SALES"
        public EmployeeRole Role { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<ExpenseRequest> ExpenseRequests { get; set; } = new List<ExpenseRequest>();
        public ICollection<Approval> ApprovalsMade { get; set; } = new List<Approval>();
    }
}
