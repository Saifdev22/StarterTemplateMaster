﻿namespace Common.Domain.SharedClient;

public class CreateTenantDto
{
    public int TenantTypeId { get; set; }
    public int ParentTenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}

public sealed record CreateTenantTypeDto
(
    string TenantTypeCode,
    string TenantTypeName
);

public sealed record GetAllTenants
(
    string TenantTypeCode,
    string TenantTypeName
);