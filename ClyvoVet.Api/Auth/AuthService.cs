using Microsoft.Extensions.Options;

namespace ClyvoVet.Api.Auth;

public class AuthService
{
    private readonly AuthUsersOptions _users;
    private readonly TokenService _tokenService;

    public AuthService(IOptions<AuthUsersOptions> users, TokenService tokenService)
    {
        _users = users.Value;
        _tokenService = tokenService;
    }

    public LoginResponse Autenticar(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Usuário e senha são obrigatórios.");

        var user = _users.Users.FirstOrDefault(candidate =>
            string.Equals(candidate.Username, request.Username.Trim(), StringComparison.OrdinalIgnoreCase)
            && candidate.Password == request.Password);

        if (user is null)
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        return _tokenService.Create(user);
    }
}
