using Common.Domain.Jwt;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Application.Features.Token;

namespace System.Presentation.Features.Token;

internal sealed class AccessTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("token/access", async (AccessTokenRequest request, ISender sender) =>
        {
            Result<TokenResponse> result = await sender.Send(new AccessTokenCommand(request));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags("Tokens");
    }

}