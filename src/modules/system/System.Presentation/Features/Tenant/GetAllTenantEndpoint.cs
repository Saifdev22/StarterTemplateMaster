using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Application.Features.Tenant.GetAllTenant;
using System.Domain.Features.Tenant;
using System.Presentation.Common;

namespace System.Presentation.Features.Tenant;

internal sealed class GetAllTenantEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("tenant/all", async (ISender sender) =>
        {
            Result<List<TenantM>> result = await sender.Send(new GetAllTenantQuery());

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Tenant);
    }
}
