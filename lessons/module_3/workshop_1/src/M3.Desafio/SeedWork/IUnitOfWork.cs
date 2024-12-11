namespace M3.Desafio.SeedWork;

public interface IUnitOfWork
{
    Task Commit(CancellationToken cancellationToken);
}
