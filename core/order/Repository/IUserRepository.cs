using order.Model;

namespace order.Repository;

public interface IUserRepository
{
    Task<User> GetUser(string userId);
}