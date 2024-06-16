using Identity.DbContexts;

namespace Identity.Services.UnitOfWorks;

public class UnitOfWork(UserDbContext context) : IUnitOfWork
{
    private readonly UserDbContext _context = context;

    public async Task Commit(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
