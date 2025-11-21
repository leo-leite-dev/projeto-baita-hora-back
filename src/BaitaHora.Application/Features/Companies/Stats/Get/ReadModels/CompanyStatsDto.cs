using BaitaHora.Application.Features.Companies.Stats.ReadModels;

namespace BaitaHora.Application.Features.Companies.Stats.Get.ReadModels;

public sealed class CompanyStatsDto
{
    public int DayAppointmentsCount { get; init; }
    public int MonthAppointmentsCount { get; init; }
    public decimal DayRevenue { get; init; }
    public decimal MonthRevenue { get; init; }

    public AppointmentStatusSummaryDto DaySummary { get; init; } = new();
    public MonthSummaryDto MonthSummary { get; init; } = new();

    public IReadOnlyList<MemberAppointmentsDto> MemberAppointments { get; init; }
        = Array.Empty<MemberAppointmentsDto>();
}

public sealed class AppointmentStatusSummaryDto
{
    public int Pending { get; init; }
    public int Attended { get; init; }
    public int NoShow { get; init; }
    public int Unknown { get; init; }
    public int Finished { get; init; }
    public int Cancelled { get; init; }
}

public sealed class MonthSummaryDto
{
    public int Unknown { get; init; }
    public int Cancelled { get; init; }
}