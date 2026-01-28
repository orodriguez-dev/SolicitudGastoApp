using SolicitudGastoApp.Application.DTOs;

namespace SolicitudGastoApp.Application.Services
{
    public interface IExpenseRequestService
    {
        Task<ExpenseRequestResponseDto> CreateAsync(
            string identityUserId,
            CreateExpenseRequestDto dto
        );

        Task<ExpenseRequestResponseDto> SubmitAsync(
            string identityUserId,
            Guid requestId
        );

        Task<ExpenseRequestResponseDto> DecideAsync(
            string identityUserId,
            Guid requestId,
            DecideExpenseDto dto
        );

        Task<List<ExpenseRequestResponseDto>> GetPendingForApproverAsync(
            string identityUserId
        );
    }
}
