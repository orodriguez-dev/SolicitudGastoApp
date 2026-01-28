using Microsoft.EntityFrameworkCore;
using SolicitudGastoApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SolicitudGastoApp.Infrastructure.Persistence.Configurations
{
    public class EmployeeConfig : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> b)
        {
            b.ToTable("Employees");
            b.HasKey(x => x.Id);

            b.Property(x => x.IdentityUserId).IsRequired().HasMaxLength(450); // típico de AspNetUsers
            b.HasIndex(x => x.IdentityUserId).IsUnique();

            b.Property(x => x.FullName).IsRequired().HasMaxLength(200);
            b.Property(x => x.Email).IsRequired().HasMaxLength(200);
            b.HasIndex(x => x.Email);

            b.Property(x => x.AreaCode).IsRequired().HasMaxLength(50);
            b.HasIndex(x => x.AreaCode);

            b.Property(x => x.Role).IsRequired();

            b.Property(x => x.IsActive).IsRequired();
        }
    }
}
