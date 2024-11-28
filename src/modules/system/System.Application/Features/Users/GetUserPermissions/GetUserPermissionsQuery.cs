using Common.Application.Authorization;

namespace System.Application.Features.Users.GetUserPermissions;

public sealed record GetUserPermissionsQuery(string IdentityId) : IQuery<PermissionsResponse>;
