using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.IRepositories.Companies;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Members.Get.ByUserId;

public sealed class GetMemberAdminEditByUserIdHandler
    : IRequestHandler<GetMemberAdminEditByUserIdQuery, Result<MemberAdminEditView>>
{
    private readonly ICompanyMemberRepository _memberRepository;
    private readonly ICurrentCompany _currentCompany;

    public GetMemberAdminEditByUserIdHandler(
        ICompanyMemberRepository memberRepository,
        ICurrentCompany currentCompany)
    {
        _memberRepository = memberRepository;
        _currentCompany = currentCompany;
    }

    public async Task<Result<MemberAdminEditView>> Handle(GetMemberAdminEditByUserIdQuery request, CancellationToken ct)
    {
        var dto = await _memberRepository
            .GetByMemberIdAsync(_currentCompany.Id, request.UserId, ct);

        return dto is null
            ? Result<MemberAdminEditView>.NotFound("Membro não encontrado.")
            : Result<MemberAdminEditView>.Ok(dto);
    }
}