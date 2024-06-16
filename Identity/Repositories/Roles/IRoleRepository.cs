using Identity.Models;

namespace Identity.Repositories.Roles;

public interface IRoleRepository
{
    Task<Role> AddAsync(Role role);
    Task DeleteAsync(int id);
    Task<IEnumerable<Role>> GetAllAsync();
    Task<Role> GetAsync(int id);
    Task<Role> GetByNameAsync(string name);
    Task<bool> IsExist(int id);
    Task<bool> IsExist(string name);
    void Update(Role role);
}