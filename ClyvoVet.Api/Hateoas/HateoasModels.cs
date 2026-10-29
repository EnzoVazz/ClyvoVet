using System.Text.Json.Serialization;

namespace ClyvoVet.Api.Hateoas;

public class LinkDto
{
    public string Rel { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
}

public class ResourceResponse<T>
{
    public T Data { get; set; } = default!;

    [JsonPropertyName("_links")]
    public IReadOnlyList<LinkDto> Links { get; set; } = [];
}

public class PagedResponse<T>
{
    public IReadOnlyList<ResourceResponse<T>> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    [JsonPropertyName("_links")]
    public IReadOnlyList<LinkDto> Links { get; set; } = [];
}
