namespace ClyvoVet.Domain.Common;

public class PageQuery
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SortBy { get; init; }
    public string SortDirection { get; init; } = "asc";
    public string? Search { get; init; }

    public static PageQuery Create(int page, int pageSize, string? sortBy, string? sortDirection, string? search)
    {
        if (page < 1)
            page = 1;

        if (pageSize < 1)
            pageSize = 10;

        if (pageSize > 50)
            pageSize = 50;

        var direction = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";
        var termo = string.IsNullOrWhiteSpace(search) ? null : search.Trim();

        return new PageQuery
        {
            Page = page,
            PageSize = pageSize,
            SortBy = string.IsNullOrWhiteSpace(sortBy) ? null : sortBy.Trim(),
            SortDirection = direction,
            Search = termo
        };
    }
}
