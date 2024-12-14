namespace Common.Domain.TransferObjects.System;
public sealed record ReadUserDto
(
    int UserId,
    string Email,
    int TenantId,
    bool IsActive
);

public sealed record WriteUserDto
(
    int UserId,
    int TenantId,
    string Email,
    string Password
);
