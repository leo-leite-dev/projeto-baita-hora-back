using BaitaHora.Application.Abstractions.Auth;
using BaitaHora.Application.Common.Results;
using BaitaHora.Application.Features.Companies.Members.Get.ReadModels;
using BaitaHora.Application.IRepositories.Companies;
using MediatR;

namespace BaitaHora.Application.Features.Companies.Members.Get.Options;

public sealed class ListMembersOptionsHandler
    : IRequestHandler<ListMembersOptionsQuery, Result<IReadOnlyList<MemberOptions>>>
{
    private readonly ICurrentCompany _currentCompany;
    private readonly ICompanyMemberRepository _memberRepository;

    public ListMembersOptionsHandler(
        ICurrentCompany currentCompany,
        ICompanyMemberRepository memberRepository)
    {
        _currentCompany = currentCompany;
        _memberRepository = memberRepository;
    }

    public async Task<Result<IReadOnlyList<MemberOptions>>> Handle(
        ListMembersOptionsQuery request, CancellationToken ct)
    {
        if (!_currentCompany.HasValue)
            return Result<IReadOnlyList<MemberOptions>>.Forbidden("Empresa não selecionada.");

        var items = await _memberRepository.ListMemberOptionsAsync(
            _currentCompany.Id,
            request.Search,
            request.Take,
            ct);

        return Result<IReadOnlyList<MemberOptions>>.Ok(items);
    }
}