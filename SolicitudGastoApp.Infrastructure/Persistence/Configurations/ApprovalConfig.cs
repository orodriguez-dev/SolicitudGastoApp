
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolicitudGastoApp.Domain.Entities;

namespace SolicitudGastoApp.Infrastructure.Persistence.Configurations
{
    public class ApprovalConfig : IEntityTypeConfiguration<Approval>
    {
        public void Configure(EntityTypeBuilder<Approval> b)
        {
            b.ToTable("Approvals");
            b.HasKey(x => x.Id);

            b.Property(x => x.Decision).IsRequired();
            b.Property(x => x.Comment).HasMaxLength(1000);

            b.HasOne(x => x.ExpenseRequest)
             .WithMany(r => r.Approvals)
             .HasForeignKey(x => x.ExpenseRequestId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.ApproverEmployee)
             .WithMany(e => e.ApprovalsMade)
             .HasForeignKey(x => x.ApproverEmployeeId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.ExpenseRequestId, x.StepNumber }).IsUnique();
            b.HasIndex(x => x.ApproverEmployeeId);
        }
    }
}
