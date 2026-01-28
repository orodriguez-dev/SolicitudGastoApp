using Microsoft.EntityFrameworkCore;
using SolicitudGastoApp.Application.Abstractions;
using SolicitudGastoApp.Application.DTOs;
using SolicitudGastoApp.Domain.Entities;
using SolicitudGastoApp.Domain.Enums;

namespace SolicitudGastoApp.Application.Services
{
    public class ExpenseRequestService : IExpenseRequestService
    {
        private readonly IApplicationDbContext _db;

        public ExpenseRequestService(IApplicationDbContext db)
        {
            _db = db;
        }

        // 1) Crear solicitud + items
        public async Task<ExpenseRequestResponseDto> CreateAsync(string identityUserId, CreateExpenseRequestDto dto)
        {
            var employee = await GetEmployee(identityUserId);

            var request = new ExpenseRequest
            {
                EmployeeId = employee.Id,
                Type = dto.Type,
                RequestDate = dto.RequestDate,
                Notes = dto.Notes,
                Status = ExpenseRequestStatus.Draft,
                CreatedAtUtc = DateTime.UtcNow
            };

            foreach (var it in dto.Items)
            {
                request.Items.Add(new ExpenseItem
                {
                    Concept = it.Concept,
                    Amount = it.Amount,
                    EvidenceFileUrl = it.EvidenceFileUrl,
                    EvidenceNumber = it.EvidenceNumber
                });
            }

            request.TotalAmount = request.Items.Sum(x => x.Amount);

            _db.ExpenseRequests.Add(request);
            await _db.SaveChangesAsync();

            // Cargar empleado para map completo (si no lo tienes dentro de request)
            request.Employee = employee;

            return Map(request);
        }

        // 2) Enviar solicitud => aplica reglas y pasa a Pending/RequiereCorreccion/etc.
        public async Task<ExpenseRequestResponseDto> SubmitAsync(string identityUserId, Guid requestId)
        {
            var employee = await GetEmployee(identityUserId);

            var request = await _db.ExpenseRequests
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == requestId && r.EmployeeId == employee.Id);

            if (request == null)
                throw new InvalidOperationException("Solicitud no encontrada o no pertenece al empleado.");

            if (request.Status != ExpenseRequestStatus.Draft && request.Status != ExpenseRequestStatus.RequiresCorrection)
                throw new InvalidOperationException("Solo puedes enviar solicitudes en Borrador o RequiereCorrección.");

            // recalcular total
            request.TotalAmount = request.Items.Sum(x => x.Amount);

            // aplicar reglas del Core
            await EvaluateRulesAsync(request, employee);

            // si falta evidencia => requiere corrección
            request.Status = request.MissingEvidence ? ExpenseRequestStatus.RequiresCorrection : ExpenseRequestStatus.Pending;
            request.UpdatedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            request.Employee = employee;
            return Map(request);
        }

        // 3) Decidir (Approve/Reject/NeedsCorrection) => crea registro en Approvals
        public async Task<ExpenseRequestResponseDto> DecideAsync(string identityUserId, Guid requestId, DecideExpenseDto dto)
        {
            var approver = await GetEmployee(identityUserId);

            var request = await _db.ExpenseRequests
                .Include(r => r.Employee)
                .Include(r => r.Items)
                .Include(r => r.Approvals)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
                throw new InvalidOperationException("Solicitud no encontrada.");

            if (request.Status != ExpenseRequestStatus.Pending && request.Status != ExpenseRequestStatus.UnderFinanceReview)
                throw new InvalidOperationException("La solicitud no está en un estado decidible.");

            if (approver.Role == EmployeeRole.Employee)
                throw new InvalidOperationException("No tienes permisos para aprobar.");

            if (request.ExceedsBudget && approver.Role != EmployeeRole.Finance && approver.Role != EmployeeRole.Admin)
                throw new InvalidOperationException("Esta solicitud requiere revisión financiera.");

            var nextStep = request.Approvals.Count + 1;

            request.Approvals.Add(new Approval
            {
                ExpenseRequestId = request.Id,
                ApproverEmployeeId = approver.Id,
                Decision = dto.Decision,
                Comment = dto.Comment,
                StepNumber = nextStep,
                DecidedAtUtc = DateTime.UtcNow
            });

            switch (dto.Decision)
            {
                case ApprovalDecision.NeedsCorrection:
                    request.Status = ExpenseRequestStatus.RequiresCorrection;
                    break;

                case ApprovalDecision.Rejected:
                    request.Status = ExpenseRequestStatus.Rejected;
                    break;

                case ApprovalDecision.Approved:
                    if (request.RequiresDoubleApproval)
                    {
                        request.Status = nextStep >= 2
                            ? ExpenseRequestStatus.Approved
                            : ExpenseRequestStatus.Pending;
                    }
                    else
                    {
                        request.Status = ExpenseRequestStatus.Approved;
                    }
                    break;

                default:
                    throw new InvalidOperationException("Decisión inválida.");
            }

            request.UpdatedAtUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            // Para devolver el nombre del aprobador en approvals, lo resolvemos al mapear
            return await MapWithApprovalsAsync(request);
        }

        // 4) Pendientes para aprobador
        public async Task<List<ExpenseRequestResponseDto>> GetPendingForApproverAsync(string identityUserId)
        {
            var approver = await GetEmployee(identityUserId);

            IQueryable<ExpenseRequest> query = _db.ExpenseRequests
                .Include(r => r.Employee)
                .Include(r => r.Items)
                .Include(r => r.Approvals);

            if (approver.Role == EmployeeRole.Finance)
            {
                query = query.Where(r => r.Status == ExpenseRequestStatus.UnderFinanceReview);
            }
            else
            {
                query = query.Where(r => r.Status == ExpenseRequestStatus.Pending);
            }

            var list = await query
                .OrderByDescending(r => r.CreatedAtUtc)
                .ToListAsync();

            // Mapear a DTO (sin ciclos)
            var result = new List<ExpenseRequestResponseDto>();
            foreach (var r in list)
                result.Add(await MapWithApprovalsAsync(r));

            return result;
        }

        // -------------------------
        // Reglas del core
        // -------------------------
        private async Task EvaluateRulesAsync(ExpenseRequest request, Employee employee)
        {
            request.MissingEvidence = request.Items.Any(i => string.IsNullOrWhiteSpace(i.EvidenceFileUrl));

            var policy = await _db.Policies
                .Where(p => p.IsActive && p.ExpenseType == request.Type && p.Role == employee.Role)
                .OrderByDescending(p => p.RoleLimitAmount)
                .FirstOrDefaultAsync();

            request.RequiresDoubleApproval = policy != null
                ? request.TotalAmount > policy.DoubleApprovalThreshold
                : request.TotalAmount > 500m;

            var budget = await _db.Budgets.FirstOrDefaultAsync(b =>
                b.AreaCode == employee.AreaCode &&
                b.Year == request.RequestDate.Year &&
                b.Month == request.RequestDate.Month);

            if (budget != null)
            {
                request.ExceedsBudget = (budget.ConsumedAmount + request.TotalAmount) > budget.AllocatedAmount;

                if (request.ExceedsBudget && !request.MissingEvidence)
                    request.Status = ExpenseRequestStatus.UnderFinanceReview;
            }
            else
            {
                request.ExceedsBudget = false;
            }
        }

        private async Task<Employee> GetEmployee(string identityUserId)
        {
            var employee = await _db.Employees
                .FirstOrDefaultAsync(e => e.IdentityUserId == identityUserId && e.IsActive);

            if (employee == null)
                throw new InvalidOperationException("Empleado no encontrado para este usuario (IdentityUserId).");

            return employee;
        }

        // -------------------------
        // MAPPERS (evitan ciclos JSON)
        // -------------------------
        private ExpenseRequestResponseDto Map(ExpenseRequest r)
        {
            return new ExpenseRequestResponseDto
            {
                Id = r.Id,
                EmployeeId = r.EmployeeId,
                EmployeeName = r.Employee?.FullName ?? "",
                AreaCode = r.Employee?.AreaCode ?? "",
                Type = r.Type,
                Status = r.Status,
                RequestDate = r.RequestDate,
                TotalAmount = r.TotalAmount,
                MissingEvidence = r.MissingEvidence,
                RequiresDoubleApproval = r.RequiresDoubleApproval,
                ExceedsBudget = r.ExceedsBudget,
                Notes = r.Notes,
                Items = r.Items.Select(i => new ExpenseItemResponseDto
                {
                    Id = i.Id,
                    Concept = i.Concept,
                    Amount = i.Amount,
                    EvidenceFileUrl = i.EvidenceFileUrl,
                    EvidenceNumber = i.EvidenceNumber
                }).ToList(),
                Approvals = new List<ApprovalResponseDto>() // se llena en MapWithApprovalsAsync
            };
        }

        private async Task<ExpenseRequestResponseDto> MapWithApprovalsAsync(ExpenseRequest r)
        {
            // cargar empleados aprobadores para mostrar nombres (opcional pero pro)
            var approverIds = r.Approvals.Select(a => a.ApproverEmployeeId).Distinct().ToList();
            var approvers = await _db.Employees
                .Where(e => approverIds.Contains(e.Id))
                .ToDictionaryAsync(e => e.Id, e => e.FullName);

            var dto = Map(r);
            dto.Approvals = r.Approvals
                .OrderBy(a => a.StepNumber)
                .Select(a => new ApprovalResponseDto
                {
                    Id = a.Id,
                    ApproverEmployeeId = a.ApproverEmployeeId,
                    ApproverName = approvers.TryGetValue(a.ApproverEmployeeId, out var name) ? name : "",
                    Decision = a.Decision,
                    Comment = a.Comment,
                    StepNumber = a.StepNumber,
                    DecidedAtUtc = a.DecidedAtUtc ?? DateTime.MinValue
                })
                .ToList();

            return dto;
        }
    }
}
