using Microsoft.AspNetCore.Http.HttpResults;
using ToprakPlusServer.Application.Auth;
using ToprakPlusServer.Domain.Users;
using TS.MediatR;
using TS.Result;

namespace ToprakPlusServer.WebAPI.Modules;

public static class AuthModule
{
    public static void MapAuthEndPoint(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("/auth");
        
        app.MapPost("/login", 
            async (LoginCommand request, ISender sender, CancellationToken cancellationToken) =>
            {
                var response = await sender.Send(request, cancellationToken);

                if (!response.IsSuccessful)
                {
                    return Results.InternalServerError(response);
                } 
                return Results.Ok(response);
                
            }
        )
            .Produces<Result<string>>()
            .RequireRateLimiting("login-fixed");;
        
        app.MapPost("/forgot-password/{email}", 
            async (string email, LoginCommand request, ISender sender, CancellationToken cancellationToken) =>
            {
                var response = await sender.Send(new ForgotPasswordCommand(email) , cancellationToken);

                if (!response.IsSuccessful)
                {
                    return Results.InternalServerError(response);
                } 
                return Results.Ok(response);
            }
        )
            .Produces<Result<string>>()
            .RequireRateLimiting("forgot-password-fixed");
        
        app.MapPost("/reset-password", 
                async (ResetPasswordCommand request, ISender sender, CancellationToken cancellationToken) =>
                {
                    var response = await sender.Send(request, cancellationToken);

                    if (!response.IsSuccessful)
                    {
                        return Results.InternalServerError(response);
                    } 
                    return Results.Ok(response);
                }
            )
            .Produces<Result<string>>()
            .RequireRateLimiting("reset-password-fixed");
        
        app.MapGet("/check-forgot-password-code/{forgotPasswordCode}", 
                async (Guid forgotPasswordCode, ISender sender, CancellationToken cancellationToken) =>
                {
                    var response = await sender.Send(new CheckForgotPasswordCodeCommand(forgotPasswordCode), cancellationToken);

                    if (!response.IsSuccessful)
                    {
                        return Results.InternalServerError(response);
                    } 
                    return Results.Ok(response);
                }
            )
            .Produces<Result<string>>()
            .RequireRateLimiting("check-forgot-password-code-fixed");
        
    }
}