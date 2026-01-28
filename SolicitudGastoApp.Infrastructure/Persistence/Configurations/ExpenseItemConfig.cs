using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolicitudGastoApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SolicitudGastoApp.Infrastructure.Persistence.Configurations
{
    public class ExpenseItemConfig : IEntityTypeConfiguration<ExpenseItem>
    {
        public void Configure(EntityTypeBuilder<ExpenseItem> b)
        {
            b.ToTable("ExpenseItems");
            b.HasKey(x => x.Id);

            b.Property(x => x.Concept).IsRequired().HasMaxLength(250);
            b.Property(x => x.Amount).HasPrecision(18, 2);

            b.Property(x => x.EvidenceFileUrl).HasMaxLength(1000);
            b.Property(x => x.EvidenceNumber).HasMaxLength(100);

            b.HasOne(x => x.ExpenseRequest)
             .WithMany(r => r.Items)
             .HasForeignKey(x => x.ExpenseRequestId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => x.ExpenseRequestId);
        }
    }
}
