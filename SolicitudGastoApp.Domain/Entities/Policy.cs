using SolicitudGastoApp.Domain.Enums;

namespace SolicitudGastoApp.Domain.Entities
{
    public class Policy
    {
        public Guid Id { get; set; }

        public ExpenseType ExpenseType { get; set; }

        // límites por rol (ejemplo simple)
        public EmployeeRole Role { get; set; }
        public decimal RoleLimitAmount { get; set; }

        public bool EvidenceRequired { get; set; } = true;

        // si pasa de este monto => doble aprobación
        public decimal DoubleApprovalThreshold { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
