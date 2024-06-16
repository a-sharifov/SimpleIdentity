using Identity.Models;

namespace Identity.Repositories.Users;

public interface IUserRepository
{
    Task<User> AddAsync(User user);
    Task<bool> EmailIsConfirmed(string email);
    Task<bool> EmailIsExist(string email);
    Task<User> GetAsync(int id);
    Task<User> GetByEmailAsync(string email);
    Task<User> GetByUsernameAsync(string username);
    Task<bool> IsExist(int id);
    void Update(User user);
    Task<bool> UsernameIsExist(string username);
}