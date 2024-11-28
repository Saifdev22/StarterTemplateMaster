using Microsoft.AspNetCore.Mvc;
using System.Application.Common.Interfaces;
using System.Domain.Features.Tenants;

namespace Starter.API.Contollers;

[Route("api/[controller]")]
[ApiController]
#pragma warning disable CA1515 // Consider making public types internal
public class TenantController(ITenantService tenantService) : ControllerBase
#pragma warning restore CA1515 // Consider making public types internal
{
    [HttpGet]
    public async Task<ActionResult<List<TenantM>>> GetTenants()
    {
        return Ok(await tenantService.GetAllTenants());
    }

    [HttpPost]
    public async Task<ActionResult<TenantM>> CreatePost(CreateTenant request)
    {
        return Ok(await tenantService.CreateTenant(request));
    }

    [HttpDelete]
    public async Task<ActionResult<List<TenantM>>> DeleteTenant(int tenantId)
    {
        return Ok(await tenantService.DeleteTenant(tenantId));
    }

}
