using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BaitaHora.Application.Abstractions.Data;

public interface IAppDbContext
{
    ChangeTracker ChangeTracker { get; }
}