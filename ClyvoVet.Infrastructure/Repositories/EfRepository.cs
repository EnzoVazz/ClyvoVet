using ClyvoVet.Domain.Exceptions;
using ClyvoVet.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace ClyvoVet.Infrastructure.Repositories;

public abstract class EfRepository<T> where T : class
{
    protected readonly ClyvoVetContext Context;
    protected readonly DbSet<T> Set;

    protected EfRepository(ClyvoVetContext context)
    {
        Context = context;
        Set = context.Set<T>();
    }

    public async Task<T> AdicionarAsync(T entity)
    {
        Set.Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<T?> ObterPorIdAsync(int id)
    {
        return await Set.FindAsync(id);
    }

    public async Task<T> AtualizarAsync(T entity, int id)
    {
        var tracked = await Set.FindAsync(id)
            ?? throw new NotFoundException($"{typeof(T).Name} não encontrado para atualização.");

        Context.Entry(tracked).CurrentValues.SetValues(entity);
        await Context.SaveChangesAsync();
        return tracked;
    }

    public async Task RemoverAsync(T entity)
    {
        Set.Remove(entity);
        try
        {
            await Context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (EhViolacaoDeIntegridade(ex))
        {
            throw new ConflictException("Não é possível excluir o registro porque existem vínculos no histórico.");
        }
    }

    private static bool EhViolacaoDeIntegridade(DbUpdateException exception)
    {
        var mensagem = exception.InnerException?.Message ?? exception.Message;
        return mensagem.Contains("ORA-02292", StringComparison.OrdinalIgnoreCase)
            || mensagem.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase)
            || mensagem.Contains("REFERENCE", StringComparison.OrdinalIgnoreCase);
    }
}
