using order.Model;

namespace order.Repository;

public class MemoryUserRepository : IUserRepository
{
    public User GetUser(Guid userId)
    {
        return new User
        {
            Id = userId,
        };
    }
}