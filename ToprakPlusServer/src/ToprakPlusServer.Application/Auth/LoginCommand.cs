using FluentValidation;
using ToprakPlusServer.Application.Services;
using ToprakPlusServer.Domain.Users;
using TS.MediatR;
using TS.Result;

namespace ToprakPlusServer.Application.Auth;

public sealed record LoginCommand(
    string EmailOrUserName, string Password) : IRequest<Result<string>>;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.EmailOrUserName).NotEmpty().WithMessage("Geçerli bir email yada kullanıcı adı girin");
        RuleFor(x => x.EmailOrUserName).NotNull().WithMessage("Geçerli bir email yada kullanıcı adı girin");
        
        RuleFor(x => x.Password).NotEmpty().WithMessage("Geçerli bir şifre girin");
        RuleFor(x => x.Password).NotNull().WithMessage("Geçerli bir şifre girin");
    }
}

public sealed class LoginCommandHandler(IUserRepository userRepository, IJwtProvider jwtProvider) : IRequestHandler<LoginCommand, Result<string>>
{
    public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FirstOrDefaultAsync(
            x => x.UserName.Value == request.EmailOrUserName 
                 || x.Email.Value == request.EmailOrUserName);

        if (user is null)
        {
            return Result<string>.Failure("Kullanıcı adı ya da şifre yanlış");
        }

        var checkPassword = user.VerifyPasswordHash(request.Password);
        if (!checkPassword)
        {
            return Result<string>.Failure("Kullanıcı adı ya da şifre yanlış ");
        }

        var token = jwtProvider.CreateToken(user);
        return token;
    }
} 
    
