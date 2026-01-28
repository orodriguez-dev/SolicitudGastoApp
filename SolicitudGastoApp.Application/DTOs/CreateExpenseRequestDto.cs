using SolicitudGastoApp.Domain.Enums;

namespace SolicitudGastoApp.Application.DTOs
{
    public class CreateExpenseRequestDto
    {
        public ExpenseType Type { get; set; }
        public DateOnly RequestDate { get; set; }
        public string? Notes { get; set; }

        public List<AddExpenseItemDto> Items { get; set; } = new();
    }
}
