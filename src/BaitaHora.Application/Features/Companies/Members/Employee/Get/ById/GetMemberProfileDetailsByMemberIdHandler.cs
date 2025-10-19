using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Members.Get.ReadModels;
using BaitaHora.Application.IRepositories.Companies;
using MediatR;

public sealed class GetMemberProfileDetailsByMemberIdHandler
    : IRequestHandler<GetMemberProfileDetailsByMemberIdQuery, Result<MemberProfileDetails>>
{
    private readonly ICompanyMemberRepository _memberRepo;
    private readonly ICurrentCompany _currentCompany;

    public GetMemberProfileDetailsByMemberIdHandler(
        ICompanyMemberRepository memberRepo,
        ICurrentCompany currentCompany)
    {
        _memberRepo = memberRepo;
        _currentCompany = currentCompany;
    }

    public async Task<Result<MemberProfileDetails>> Handle(
        GetMemberProfileDetailsByMemberIdQuery request, CancellationToken ct)
    {
        var dto = await _memberRepo.GetMemberFullDetailsAsync(
            _currentCompany.Id, request.MemberId, ct);

        return dto is null
            ? Result<MemberProfileDetails>.NotFound("Membro não encontrado.")
            : Result<MemberProfileDetails>.Ok(dto);
    }
}
