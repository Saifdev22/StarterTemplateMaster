namespace Common.Domain.TransferObjects.System;

public sealed record ReadTenantTypeDto
(
    int TenantTypeId,
    string TenantTypeCode,
    string TenantTypeName
);

public sealed record WriteTenantTypeDto
(
    int TenantTypeId,
    string TenantTypeCode,
    string TenantTypeName
);