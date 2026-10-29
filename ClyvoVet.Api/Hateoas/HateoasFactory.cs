using ClyvoVet.Domain.Common;

namespace ClyvoVet.Api.Hateoas;

public class HateoasFactory
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HateoasFactory(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
    {
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    public LinkDto Link(string rel, string routeName, string method, object? values = null)
    {
        var http = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HttpContext indisponível para montar HATEOAS.");

        var href = _linkGenerator.GetUriByName(http, routeName, values)
            ?? throw new InvalidOperationException($"Rota '{routeName}' não encontrada.");

        return new LinkDto
        {
            Rel = rel,
            Href = href,
            Method = method
        };
    }

    public ResourceResponse<T> Resource<T>(T data, params LinkDto[] links)
    {
        return new ResourceResponse<T>
        {
            Data = data,
            Links = links
        };
    }

    public PagedResponse<T> Page<T>(PagedResult<T> result, Func<T, IReadOnlyList<LinkDto>> itemLinks)
    {
        var request = _httpContextAccessor.HttpContext?.Request
            ?? throw new InvalidOperationException("HttpContext indisponível para montar HATEOAS.");

        return new PagedResponse<T>
        {
            Items = result.Items.Select(item => new ResourceResponse<T>
            {
                Data = item,
                Links = itemLinks(item)
            }).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages,
            Links = PageLinks(request, result.Page, result.TotalPages)
        };
    }

    private static IReadOnlyList<LinkDto> PageLinks(HttpRequest request, int page, int totalPages)
    {
        var links = new List<LinkDto>
        {
            new() { Rel = "self", Href = Build(request, page), Method = "GET" }
        };

        if (page > 1)
            links.Add(new LinkDto { Rel = "prev", Href = Build(request, page - 1), Method = "GET" });

        if (totalPages > 0 && page < totalPages)
            links.Add(new LinkDto { Rel = "next", Href = Build(request, page + 1), Method = "GET" });

        return links;
    }

    private static string Build(HttpRequest request, int page)
    {
        var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in request.Query)
            values[item.Key] = item.Value.ToString();

        values["page"] = page.ToString();
        var query = QueryString.Create(values);
        return $"{request.Scheme}://{request.Host}{request.Path}{query}";
    }
}
