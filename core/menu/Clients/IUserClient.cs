using menu.Models;

namespace menu.Clients
{
    public interface IUserClient
    {
        Task<User?> GetUserAsync(string userId);
    }
}
