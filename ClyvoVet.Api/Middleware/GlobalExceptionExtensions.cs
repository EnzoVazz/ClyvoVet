using ClyvoVet.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;

namespace ClyvoVet.Api.Middleware;

public static class GlobalExceptionExtensions
{
    public static void UseGlobalExceptionHandling(this WebApplication app)
    {
        app.UseExceptionHandler(handler =>
        {
            handler.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                var (status, message) = Map(exception);

                if (status >= StatusCodes.Status500InternalServerError)
                    Log.Error(exception, "Erro não tratado");
                else
                    Log.Warning("Requisição rejeitada com {Status}: {Message}", status, message);

                context.Response.StatusCode = status;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message });
            });
        });
    }

    private static (int Status, string Message) Map(Exception? exception) => exception switch
    {
        ArgumentException ex => (StatusCodes.Status400BadRequest, ex.Message),
        InvalidOperationException ex => (StatusCodes.Status400BadRequest, ex.Message),
        NotFoundException ex => (StatusCodes.Status404NotFound, ex.Message),
        ConflictException ex => (StatusCodes.Status409Conflict, ex.Message),
        UnauthorizedAccessException ex => (StatusCodes.Status401Unauthorized, ex.Message),
        _ => (StatusCodes.Status500InternalServerError, "Erro interno ao processar a requisição.")
    };
}
