using BaitaHora.Application.Common.Results;

namespace BaitaHora.Api.Helpers;

public static class ApiResponseHelper
{
    public static ApiSuccessResponse CreateSuccess(
        object? data = null,
        string? title = null,
        string? code = null,
        IReadOnlyDictionary<string, object?>? meta = null)
        => new(true, title, code, data, meta);

    public static ApiSuccessResponse FromResult(Result result, object? dataOverride = null)
        => new(true, result.Title, result.Code, dataOverride, result.Metadata);

    public static ApiSuccessResponse FromResult<T>(Result<T> result)
        => new(true, result.Title, result.Code, result.Value, result.Metadata);

    public static ApiPagedResponse<T> CreatePagedSuccess<T>(
        IEnumerable<T> items, int page, int pageSize, long? total = null,
        string? title = null, string? code = null,
        IReadOnlyDictionary<string, object?>? meta = null,
        bool? hasNext = null, string? nextCursor = null)
        => new(true, title, code, items,
               new Pagination(page, pageSize, total, hasNext, nextCursor),
               meta);
}

public sealed record ApiSuccessResponse(
    bool Success, string? Title, string? Code, object? Data,
    IReadOnlyDictionary<string, object?>? Meta);

public sealed record ApiPagedResponse<T>(
    bool Success, string? Title, string? Code, IEnumerable<T> Data,
    Pagination Pagination, IReadOnlyDictionary<string, object?>? Meta);

public sealed record Pagination(int Page, int PageSize, long? Total, bool? HasNext, string? NextCursor);
