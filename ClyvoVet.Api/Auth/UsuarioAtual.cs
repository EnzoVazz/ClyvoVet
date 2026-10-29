using ClyvoVet.Domain.Interfaces;
using System.Security.Claims;

namespace ClyvoVet.Api.Auth;

public class UsuarioAtual : IUsuarioAtual
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioAtual(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? Nome => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name)
        ?? _httpContextAccessor.HttpContext?.User.Identity?.Name;
}
