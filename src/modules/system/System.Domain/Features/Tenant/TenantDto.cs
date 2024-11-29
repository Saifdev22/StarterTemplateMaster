namespace System.Domain.Features.Tenant;

public class CreateTenant
{
    public int TenantTypeId { get; set; }
    public int ParentTenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;

}


