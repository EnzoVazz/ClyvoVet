namespace ClyvoVet.Api.Auth;

public record LoginRequest(string Username, string Password);

public record LoginResponse(string Token, DateTime ExpiresAt, string Username, string Role);
