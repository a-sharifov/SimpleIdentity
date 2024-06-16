using Identity.DbContexts;
using Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.Repositories.Users;

public sealed class UserRepository(UserDbContext context) : IUserRepository
{
    private readonly UserDbContext _context = context;

    public async Task<User> AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        return user;
    }

    public async Task<User> GetAsync(int id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(x => x.Role)
            .FirstAsync(x => x.Id == id);

        return user;
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        var user = await _context.Users.AsNoTracking().Include(x => x.Role).FirstAsync(x => x.Username == username);
        return user;
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        var user = await _context.Users.AsNoTracking().Include(x => x.Role).FirstAsync(x => x.Email == email);
        return user;
    }


    public async Task<bool> UsernameIsExist(string username)
    {
        var isExist = await _context.Users.AsNoTracking().AnyAsync(x => x.Username == username);
        return isExist;
    }

    public async Task<bool> EmailIsExist(string email)
    {
        var isExist = await _context.Users.AsNoTracking().AnyAsync(x => x.Email == email);
        return isExist;
    }

    public async Task<bool> IsExist(int id)
    {
        var isExist = await _context.Users.AsNoTracking().AnyAsync(x => x.Id == id);
        return isExist;
    }

    public async Task<bool> EmailIsConfirmed(string email)
    {
        var isConfirmed = await _context.Users.AsNoTracking().AnyAsync(x => x.Email == email && x.EmailConfirmationToken == null);
        return isConfirmed;
    }



    public void Update(User user) => _context.Users.Update(user);
}
