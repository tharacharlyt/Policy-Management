using PolicyManagement.Models;

namespace PolicyManagement.Services;

public interface IUserService
{
    Task<User?> AuthenticateAsync(string username, string password);
    List<User> GetAllUsers();
    void AddUser(User user);
}