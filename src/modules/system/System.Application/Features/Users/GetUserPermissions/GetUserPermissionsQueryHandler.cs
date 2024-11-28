using Common.Application.Authorization;
using Common.Application.Database;
using Common.Domain.Errors;
using Dapper;
using System.Data;

namespace System.Application.Features.Users.GetUserPermissions;

internal sealed class GetUserPermissionsQueryHandler(IDbConnectionFactory _connection)
        : IQueryHandler<GetUserPermissionsQuery, PermissionsResponse>
{
    public async Task<Result<PermissionsResponse>> Handle(
            GetUserPermissionsQuery request,
            CancellationToken cancellationToken)
    {
        const string sql =
                $"""
						SELECT DISTINCT 
								ur.UserId AS {nameof(UserPermission.UserId)}, 
								p.Code AS {nameof(UserPermission.Permission)}
						FROM ID.UserRoles ur
						LEFT JOIN ID.RolePermissions rp ON rp.RoleId = ur.RoleId 
						LEFT JOIN ID.Permissions p ON p.PermissionId = rp.PermissionId
						WHERE ur.UserId = @IdentityId
				""";

        List<UserPermission> permissions = (await _connection.QueryAsync<UserPermission>(sql, request, true)).AsList();

        return permissions.Count == 0
            ? Result.Failure<PermissionsResponse>(CustomError.NotFound("404", request.IdentityId.ToString()))
            : new PermissionsResponse(permissions[0].UserId, permissions.Select(p => p.Permission).ToHashSet());
    }

    internal sealed class UserPermission
    {
        internal int UserId { get; init; }

        internal string Permission { get; init; } = string.Empty;
    }
}
