using menu.Models.DTO;

namespace menu.Clients
{
    public interface IRecClient
    {
        Task<IEnumerable<RecItemDTO>?> GetRecAsync(string input);
        Task SyncRecMenuAsync(IEnumerable<RecMenuDTO> dtos);
    }
}
