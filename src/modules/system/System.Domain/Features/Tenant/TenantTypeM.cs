namespace System.Domain.Features.Tenant;

public class TenantTypeM : AggregateRoot
{
    public int TenantTypeId { get; set; }
    public string TenantTypeCode { get; set; } = string.Empty;
    public string TenantTypeName { get; set; } = string.Empty;

    public virtual ICollection<TenantM>? Tenants { get; }

    public static TenantTypeM Create(int tenantTypeId, string tenantTypeCode, string tenantTypeDesc)
    {
        TenantTypeM tenantType = new()
        {
            TenantTypeId = tenantTypeId,
            TenantTypeCode = tenantTypeCode,
            TenantTypeName = tenantTypeDesc,
        };

        return tenantType;
    }
}