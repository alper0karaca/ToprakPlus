using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ToprakPlusServer.Application.Services;
using ToprakPlusServer.Domain.Users;
using ToprakPlusServer.Infrastructure.Options;

namespace ToprakPlusServer.Infrastructure.Services;

public sealed class JwtProvider(IOptions<JwtOptions> jwtOptions) : IJwtProvider
{
    public string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim("fullname", user.FullName.Value),
            new Claim("email", user.Email.Value),
        };

        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey));
        SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);
        
        JwtSecurityToken securityToken = new JwtSecurityToken(
            issuer: jwtOptions.Value.Issuer, 
            audience: jwtOptions.Value.Audience,
            claims: claims,
            notBefore:DateTime.Now,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: signingCredentials);
        
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        var token = handler.WriteToken(securityToken);
        return token;
    }
}