namespace SolicitudGastoApp.Domain.Enums
{
    public enum ExpenseRequestStatus
    {
        Draft = 0,
        Pending = 1,
        RequiresCorrection = 2,
        UnderFinanceReview = 3,
        Approved = 4,
        Rejected = 5,
        Cancelled = 6
    }
}
