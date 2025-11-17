using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Schedulings.Get.ReadModels;
using BaitaHora.Application.IRepositories.Schedulings;
using MediatR;

namespace BaitaHora.Application.Features.Schedulings.Get.ById;

public sealed class GetScheduleByUserHandler
    : IRequestHandler<GetScheduleByUserQuery, Result<ScheduleDetailsDto>>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly ICurrentUser _currentUser;

    public GetScheduleByUserHandler(
        IScheduleRepository scheduleRepository,
        ICurrentUser currentUser)
    {
        _scheduleRepository = scheduleRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<ScheduleDetailsDto>> Handle(GetScheduleByUserQuery request, CancellationToken ct)
    {
        var dto = await _scheduleRepository.GetDetailsByMemberIdAsync(
            _currentUser.UserId, request.FromUtc, request.ToUtc, ct);

        return dto is null
            ? Result<ScheduleDetailsDto>.NotFound("Agenda não encontrada pro usuário logado.")
            : Result<ScheduleDetailsDto>.Ok(dto);
    }
}