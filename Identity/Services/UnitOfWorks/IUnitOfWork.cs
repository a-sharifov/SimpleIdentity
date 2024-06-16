namespace Identity.Services.UnitOfWorks;

public interface IUnitOfWork
{
    Task Commit(CancellationToken cancellationToken = default);
}