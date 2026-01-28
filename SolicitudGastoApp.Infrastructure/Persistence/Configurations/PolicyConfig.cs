
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolicitudGastoApp.Domain.Entities;

namespace SolicitudGastoApp.Infrastructure.Persistence.Configurations
{
    public class PolicyConfig : IEntityTypeConfiguration<Policy>
    {
        public void Configure(EntityTypeBuilder<Policy> b)
        {
            b.ToTable("Policies");
            b.HasKey(x => x.Id);

            b.Property(x => x.RoleLimitAmount).HasPrecision(18, 2);
            b.Property(x => x.DoubleApprovalThreshold).HasPrecision(18, 2);

            b.HasIndex(x => new { x.ExpenseType, x.Role, x.IsActive });
        }
    }
}
