using menu.Models;
using menu.Models.DTO;

namespace menu.Clients
{
    public interface IUserClient
    {
        Task<UserDTO?> GetUserAsync(string userId);
    }
}
