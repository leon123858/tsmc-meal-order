using menu.Models.DTO;

namespace menu.Clients
{
    public interface IRecClient
    {
        Task<List<RecItemDTO>?> GetRecAsync(string input);
    }
}
