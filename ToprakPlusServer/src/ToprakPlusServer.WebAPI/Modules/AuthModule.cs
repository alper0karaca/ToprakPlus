using Microsoft.AspNetCore.Http.HttpResults;
using ToprakPlusServer.Application.Auth;
using TS.MediatR;
using TS.Result;

namespace ToprakPlusServer.WebAPI.Modules;

public static class AuthModule
{
    public static void MapAuthEndPoint(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("/auth")
            .RequireRateLimiting("login-fixed");
        
        app.MapPost("/login", 
            async (LoginCommand request, ISender sender, CancellationToken cancellationToken) =>
            {
                var response = await sender.Send(request, cancellationToken);

                if (!response.IsSuccessful)
                {
                    return Results.InternalServerError(response);
                } 
                return Results.Ok(response);
                
            }).Produces<Result<string>>();
    }
}