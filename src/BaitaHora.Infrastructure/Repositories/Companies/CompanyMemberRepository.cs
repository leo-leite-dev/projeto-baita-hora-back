using BaitaHora.Application.Features.Addresses.Dtos;
using BaitaHora.Application.Features.Companies.Members.Get.ReadModels;
using BaitaHora.Application.Features.Users.Dtos;
using BaitaHora.Application.IRepositories.Companies;
using BaitaHora.Domain.Features.Companies.Entities;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories.Companies;

public sealed class CompanyMemberRepository : GenericRepository<CompanyMember>, ICompanyMemberRepository
{
    public CompanyMemberRepository(AppDbContext context) : base(context) { }

    public async Task<MemberAdminEditView?> GetByMemberIdAsync(
        Guid companyId, Guid memberId, CancellationToken ct)
    {
        return await _context.Members
            .AsNoTracking()
            .Where(m => m.CompanyId == companyId && m.Id == memberId)
            .Select(m => new MemberAdminEditView
            {
                MemberId = m.Id,
                Name = m.User.Profile.Name,
                Email = m.User.Email.Value,
                Cpf = m.User.Profile.Cpf.Value,
                Rg = m.User.Profile.Rg.HasValue
                            ? m.User.Profile.Rg.Value.Value
                            : null
            })
            .SingleOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<CompanyMember>> GetByUserIdAsync(
    Guid userId, CancellationToken ct = default)
    {
        return await _set
            .Where(m => m.UserId == userId)
            .ToListAsync(ct);
    }

    public async Task<MemberProfileDetails?> GetMemberFullDetailsAsync(
        Guid companyId, Guid memberId, CancellationToken ct)
    {
        return await _context.Members
            .AsNoTracking()
            .Where(m => m.CompanyId == companyId && m.Id == memberId)
            .Select(m => new MemberProfileDetails(
                m.Id,
                m.Role.ToString(),
                m.PrimaryPosition != null ? m.PrimaryPosition.Name : null,
                m.IsActive,
                m.JoinedAt,
                new UserDto(
                    m.User.Username.Value,
                    m.User.Email.Value,
                    new UserProfileDto(
                        m.User.Profile.Name,
                        m.User.Profile.Cpf.Value,
                        m.User.Profile.Rg.HasValue
                            ? m.User.Profile.Rg.Value.Value
                            : null,
                        m.User.Profile.BirthDate.HasValue
                            ? new DateTime(
                                m.User.Profile.BirthDate.Value.Value.Year,
                                m.User.Profile.BirthDate.Value.Value.Month,
                                m.User.Profile.BirthDate.Value.Value.Day)
                            : null,
                        m.User.Profile.Phone.Value,
                        new AddressDto(
                            m.User.Profile.Address.Street,
                            m.User.Profile.Address.Number,
                            m.User.Profile.Address.Complement,
                            m.User.Profile.Address.Neighborhood,
                            m.User.Profile.Address.City,
                            m.User.Profile.Address.State,
                            m.User.Profile.Address.ZipCode
                        ),
                        m.User.Profile.ProfileImageUrl
                    )
                )
            ))
            .SingleOrDefaultAsync(ct);
    }

    public async Task<CompanyMember?> GetByCompanyAndUserWithPositionAsync(
        Guid companyId, Guid userId, CancellationToken ct = default)
    {
        return await _set
            .AsNoTracking()
            .Include(m => m.PrimaryPosition)
            .SingleOrDefaultAsync(
                m => m.CompanyId == companyId && m.UserId == userId, ct);
    }

    public async Task<IReadOnlyList<MemberDetails>> ListAllMembersByCompanyAsync(
        Guid companyId, CancellationToken ct)
    {
        return await _context.Members
            .AsNoTracking()
            .Where(m => m.CompanyId == companyId)
            .OrderBy(m => m.User.Profile.Name)
            .Select(m => new MemberDetails(
                m.Id,
                m.User.Profile.Name,
                m.User.Profile.Phone,
                m.User.Email.ToString(),
                m.Role.ToString(),
                m.PrimaryPosition != null ? m.PrimaryPosition.Name : string.Empty,
                m.IsActive,
                m.CreatedAtUtc,
                m.UpdatedAtUtc,
                m.JoinedAt
            ))
            .ToListAsync(ct);
    }

    public async Task<CompanyMember?> GetByIdWithPositionAsync(
        Guid companyId, Guid memberId, CancellationToken ct = default)
    {
        return await _set
            .AsNoTracking()
            .Include(m => m.User)
                .ThenInclude(u => u.Profile)
            .Include(m => m.PrimaryPosition)
            .SingleOrDefaultAsync(
                m => m.CompanyId == companyId && m.Id == memberId, ct);
    }

    public async Task<IReadOnlyList<MemberOptions>> ListMemberOptionsAsync(
        Guid companyId, string? search, int take, CancellationToken ct = default)
    {
        var query = _context.Members
            .AsNoTracking()
            .Where(m => m.CompanyId == companyId && m.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(m =>
                m.User.Profile.Name.Contains(term));
        }

        return await query
            .OrderBy(m => m.User.Profile.Name)
            .Take(take)
            .Select(m => new MemberOptions(
                m.Id,
                m.User.Profile.Name
            ))
            .ToListAsync(ct);
    }
}