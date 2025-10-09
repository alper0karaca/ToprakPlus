using GenericRepository;
using ToprakPlusServer.Domain.Users;
using ToprakPlusServer.Infrastructure.Context;

namespace ToprakPlusServer.Infrastructure.Repositories;

public class UserRepository : Repository<User, ApplicationDbContext>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }
}