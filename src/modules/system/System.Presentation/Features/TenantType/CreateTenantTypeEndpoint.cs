﻿using Common.Domain.TransferObjects.System;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Application.Features.TenantType.CreateTenantType;
using System.Domain.Features.Tenant;
using System.Presentation.Common;

namespace System.Presentation.Features.TenantType;

internal sealed class CreateTenantTypeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("tenanttype/create", async (WriteTenantTypeDto request, ISender sender) =>
        {
            Result<TenantTypeM> result = await sender.Send(new CreateTenantTypeCommand(request));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.TenantType);
    }
}