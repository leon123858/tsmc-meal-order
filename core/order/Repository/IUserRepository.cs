using order.Model;

namespace order.Repository;

public interface IUserRepository
{
    User GetUser(Guid userId);
}