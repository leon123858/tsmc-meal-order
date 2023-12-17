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

    public Task<Dictionary<string, User>> GetUsers(IEnumerable<string> userIds)
    {
        var users = new Dictionary<string, User>();
        foreach (var userId in userIds)
            users.Add(userId, new User
            {
                Id = userId
            });

        return Task.FromResult(users);
    }
}