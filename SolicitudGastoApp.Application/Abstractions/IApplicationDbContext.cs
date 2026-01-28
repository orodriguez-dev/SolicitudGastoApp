using Microsoft.EntityFrameworkCore;
using SolicitudGastoApp.Domain.Entities;

namespace SolicitudGastoApp.Application.Abstractions
{
    public interface IApplicationDbContext
    {
        DbSet<Employee> Employees { get; }
        DbSet<ExpenseRequest> ExpenseRequests { get; }
        DbSet<ExpenseItem> ExpenseItems { get; }
        DbSet<Approval> Approvals { get; }
        DbSet<Budget> Budgets { get; }
        DbSet<Policy> Policies { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
