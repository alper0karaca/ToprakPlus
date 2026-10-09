using ToprakPlusServer.Domain.Abstractions;

namespace ToprakPlusServer.Domain.Users;

public sealed record ForgotPasswordCode(Guid Value)
{
}