using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolicitudGastoApp.Domain.Entities;

namespace SolicitudGastoApp.Infrastructure.Persistence.Configurations
{
    public class ExpenseRequestConfig : IEntityTypeConfiguration<ExpenseRequest>
    {
        public void Configure(EntityTypeBuilder<ExpenseRequest> b)
        {
            b.ToTable("ExpenseRequests");
            b.HasKey(x => x.Id);

            b.Property(x => x.TotalAmount).HasPrecision(18, 2);

            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.Type).IsRequired();

            b.Property(x => x.Notes).HasMaxLength(1000);

            b.Property(x => x.RowVersion).IsRequired(false); // concurrency token

            b.HasOne(x => x.Employee)
             .WithMany(e => e.ExpenseRequests)
             .HasForeignKey(x => x.EmployeeId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.EmployeeId, x.RequestDate });
            b.HasIndex(x => x.Status);
            b.HasIndex(x => new { x.Type, x.Status });
        }
    }
}
