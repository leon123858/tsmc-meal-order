using order.Model;

namespace order.Repository.TestImplement;

public class MemoryUserRepository : IUserRepository
{
    public Task<User> GetUser(string userId)
    {
        return Task.FromResult(new User
        {
            Id = userId
        });
    }
}