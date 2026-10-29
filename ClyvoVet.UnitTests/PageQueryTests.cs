using ClyvoVet.Domain.Common;

namespace ClyvoVet.UnitTests;

public class PageQueryTests
{
    [Fact]
    public void Create_ValoresForaDoLimite_DeveNormalizarPaginaTamanhoEDirecao()
    {
        var query = PageQuery.Create(0, 500, " Nome ", "DESC", "  rex  ");

        Assert.Equal(1, query.Page);
        Assert.Equal(50, query.PageSize);
        Assert.Equal("Nome", query.SortBy);
        Assert.Equal("desc", query.SortDirection);
        Assert.Equal("rex", query.Search);
    }

    [Fact]
    public void Create_DirecaoDesconhecida_DeveUsarAscendente()
    {
        var query = PageQuery.Create(2, 0, null, "lado", " ");

        Assert.Equal(2, query.Page);
        Assert.Equal(10, query.PageSize);
        Assert.Equal("asc", query.SortDirection);
        Assert.Null(query.Search);
    }
}
