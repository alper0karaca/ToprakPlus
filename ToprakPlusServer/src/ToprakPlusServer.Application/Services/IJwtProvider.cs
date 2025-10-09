using ToprakPlusServer.Domain.Users;

namespace ToprakPlusServer.Application.Services;

public interface IJwtProvider
{
    string CreateToken(User user);
}