using Common.Domain.SharedClient;
using System.Domain.Features.Tenant;

namespace System.Application.Features.Tenant.GetAllTenant;

public sealed record GetAllTenantQuery : IQuery<List<GetAllTenants>>;