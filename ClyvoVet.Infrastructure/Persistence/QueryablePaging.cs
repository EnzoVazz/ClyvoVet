using System.Linq.Expressions;
using System.Reflection;
using ClyvoVet.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace ClyvoVet.Infrastructure.Persistence;

public static class QueryablePaging
{
    public static async Task<PagedResult<T>> ToPagedAsync<T>(
        this IQueryable<T> source,
        PageQuery query,
        string defaultSort,
        params string[] allowed)
    {
        var ordered = ApplySort(source, query, defaultSort, allowed);
        var total = await ordered.CountAsync();
        var items = await ordered
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PagedResult<T>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total
        };
    }

    private static IQueryable<T> ApplySort<T>(IQueryable<T> source, PageQuery query, string defaultSort, string[] allowed)
    {
        var requested = string.IsNullOrWhiteSpace(query.SortBy) ? defaultSort : query.SortBy;
        var propertyName = allowed.FirstOrDefault(property => property.Equals(requested, StringComparison.OrdinalIgnoreCase));
        if (propertyName is null)
            throw new ArgumentException($"Ordenação '{requested}' não é permitida. Campos aceitos: {string.Join(", ", allowed)}.");

        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
            ?? throw new ArgumentException($"Ordenação '{requested}' não é permitida.");

        var parameter = Expression.Parameter(typeof(T), "entity");
        var body = Expression.Property(parameter, property);
        var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), property.PropertyType);
        var lambda = Expression.Lambda(delegateType, body, parameter);

        var descending = string.Equals(query.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        var methodName = descending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);
        var method = typeof(Queryable).GetMethods()
            .Single(candidate => candidate.Name == methodName
                && candidate.IsGenericMethodDefinition
                && candidate.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.PropertyType);

        return (IQueryable<T>)method.Invoke(null, [source, lambda])!;
    }
}
