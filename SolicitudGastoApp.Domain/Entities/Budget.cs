
namespace SolicitudGastoApp.Domain.Entities
{
    public class Budget
    {
        public Guid Id { get; set; }

        public string AreaCode { get; set; } = default!;
        public int Year { get; set; }
        public int Month { get; set; } // 1-12

        public decimal AllocatedAmount { get; set; }
        public decimal ConsumedAmount { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public decimal RemainingAmount => AllocatedAmount - ConsumedAmount;
    }
}
