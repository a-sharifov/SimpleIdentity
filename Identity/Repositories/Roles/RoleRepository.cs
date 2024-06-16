using Identity.DbContexts;
using Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.Repositories.Roles;

public sealed class RoleRepository(UserDbContext context) : IRoleRepository
{
    private readonly UserDbContext _context = context;

    public async Task<Role> AddAsync(Role role)
    {
        await _context.Roles.AddAsync(role);
        return role;
    }

    public async Task<Role> GetAsync(int id)
    {
        var role = await _context.Roles.AsNoTracking().FirstAsync(x => x.Id == id);
        return role;
    }

    public async Task<Role> GetByNameAsync(string name)
    {
        var role = await _context.Roles.AsNoTracking().FirstAsync(x => x.Name == name);
        return role;
    }

    public async Task<bool> IsExist(int id)
    {
        var isExist = await _context.Roles.AsNoTracking().AnyAsync(x => x.Id == id);
        return isExist;
    }

    public async Task<bool> IsExist(string name)
    {
        var isExist = await _context.Roles.AsNoTracking().AnyAsync(x => x.Name == name);
        return isExist;
    }

    public void Update(Role role) => _context.Roles.Update(role);

    public async Task DeleteAsync(int id)
    {
        var role = await _context.Roles.FirstAsync(x => x.Id == id);
        _context.Roles.Remove(role);
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        var roles = await _context.Roles.AsNoTracking().ToListAsync();
        return roles;
    }
}
