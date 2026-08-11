namespace AuditCRM.Application.Features.Dashboard.DTOs;

public sealed record DashboardSummaryDto(
    int TotalProcesses,
    int OpenProcesses,
    int Installed,
    int InNegotiation,
    int NotifiedNoResponse,
    int NotAuthorized,
    int LeftShopping,
    int ScheduledAppointments,
    int OverdueActions,
    int DueToday,
    int WithoutNextAction);