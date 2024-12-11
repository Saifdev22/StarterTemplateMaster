using Common.Domain.SharedClient;

namespace System.Application.Features.Tenant.GetAllTenant;

public sealed record GetAllTenantQuery : IQuery<List<GetAllTenants>>;