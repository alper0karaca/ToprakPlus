using FluentValidation;
using GenericRepository;
using ToprakPlusServer.Application.Services;
using ToprakPlusServer.Domain.Users;
using TS.MediatR;
using TS.Result;

namespace ToprakPlusServer.Application.Auth;

public sealed record ForgotPasswordCommand(string Email) : IRequest<Result<string>> 
{
    
}

public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(p => p.Email)
            .NotEmpty()
            .WithMessage("Geçerli bir mail adresi girin");
        
        RuleFor(p => p.Email)
            .EmailAddress()
            .WithMessage("Geçerli bir mail adresi girin");
    }
}

public sealed class ForgotPasswordCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IMailService mailService) : IRequestHandler<ForgotPasswordCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository
            .FirstOrDefaultAsync(p => p.Email.Value == request.Email, 
                cancellationToken);

        if (user is null)
        {
            return Result<string>.Failure("Kullanıcı bulunamadı");
        }
        
        // şifre sıfırlama maili gönder
        user.CreateForgotPasswordCode();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        string to = user.Email.Value;
        string subject = "Şifre Sıfırlama";
        string resetPasswordUrl =
            $"http://localhost:5171/auth/reset-password/{user.ForgotPasswordCode!.Value}";
        
        string body = @"
<!DOCTYPE html>
<html lang=""tr"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Şifre Sıfırlama</title>
</head>
<body style=""font-family: Arial, sans-serif; background-color: #f4f6f8; padding: 20px;"">
    <div style=""max-width: 500px; margin: 20px auto; padding: 30px; background: #ffffff; border-radius: 10px;"">
        <h2 style=""color: #2563eb; text-align: center;"">ToprakPlus</h2>

        <h3>Şifrenizi mi unuttunuz?</h3>

        <p>Merhaba, {UserName}</p>

        <p>Şifrenizi sıfırlamak için aşağıdaki butona tıklayın.</p>

        <p style=""text-align: center; margin: 30px 0;"">
            <a href=""{ResetPasswordUrl}"" target=""_blank""
               style=""display: inline-block; padding: 14px 28px; background: #2563eb; color: #ffffff; text-decoration: none; border-radius: 6px;"">
                Şifremi Sıfırla
            </a>
        </p>

        <p>Bu bağlantı güvenliğiniz için belirli bir süre sonra geçerliliğini yitirecektir.</p>
        <p>Bu talebi siz oluşturmadıysanız e-postayı dikkate almayabilirsiniz.</p>
    </div>
</body>
</html>";

        body = body.Replace(
            "{UserName}",
            System.Net.WebUtility.HtmlEncode(
                $"{user.FirstName.Value} {user.LastName.Value}"));

        body = body.Replace(
            "{ResetPasswordUrl}",
            System.Net.WebUtility.HtmlEncode(resetPasswordUrl));
        
        await mailService.SendAsync(to, subject, body, cancellationToken);
        return "Şifre sıfırlama maili gönderildi";
    }
}