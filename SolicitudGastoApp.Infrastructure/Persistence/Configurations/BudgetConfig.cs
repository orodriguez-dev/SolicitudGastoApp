
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolicitudGastoApp.Domain.Entities;

namespace SolicitudGastoApp.Infrastructure.Persistence.Configurations
{
    public class BudgetConfig : IEntityTypeConfiguration<Budget>
    {
        public void Configure(EntityTypeBuilder<Budget> b)
        {
            b.ToTable("Budgets");
            b.HasKey(x => x.Id);

            b.Property(x => x.AreaCode).IsRequired().HasMaxLength(50);
            b.Property(x => x.AllocatedAmount).HasPrecision(18, 2);
            b.Property(x => x.ConsumedAmount).HasPrecision(18, 2);

            b.HasIndex(x => new { x.AreaCode, x.Year, x.Month }).IsUnique();
        }
    }
}
