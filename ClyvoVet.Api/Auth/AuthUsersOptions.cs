namespace ClyvoVet.Api.Auth;

public class AppUser
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class AuthUsersOptions
{
    public List<AppUser> Users { get; set; } = [];
}
