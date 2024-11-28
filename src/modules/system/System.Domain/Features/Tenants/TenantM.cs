﻿using System.Domain.Features.Identity;

namespace System.Domain.Features.Tenants;

public class TenantM
{
    public int TenantId { get; set; }
    public int TenantTypeId { get; set; }
    public int ParentTenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public DateTime LicenceExpiryDate { get; set; }
    public virtual TenantTypeM? TenantType { get; set; }
    public virtual ICollection<UserM>? Users { get; }

    public static TenantM Create(
        int tenantTypeId,
        int parentTypeId,
        string tenantName,
        string databaseName,
        string connectionString)
    {
        TenantM tenant = new()
        {
            TenantTypeId = tenantTypeId,
            ParentTenantId = parentTypeId,
            TenantName = tenantName,
            DatabaseName = databaseName,
            ConnectionString = connectionString,
            LicenceExpiryDate = DateTime.Now.AddDays(7),
        };

        return tenant;
    }
}

