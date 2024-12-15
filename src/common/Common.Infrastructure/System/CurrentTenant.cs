using Common.Infrastructure.Authentication;
using Common.Infrastructure.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace Common.Infrastructure.System;

public sealed class CurrentTenant(IConfiguration config, IHttpContextAccessor httpContextAccessor)
{
    private const string TenantIdHeaderName = "Tenant";
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IConfiguration _configuration = config;

    public string? TenantId => _httpContextAccessor
            .HttpContext?
            .Request
            .Headers[TenantIdHeaderName];

    public string? TenantClaim => _httpContextAccessor
            .HttpContext?
            .User
            .GetTenantDbName();
    public string GetSystemConnectionString()
    {
        return _configuration.GetValueOrThrow<string>("Database:DefaultConnection");
    }

    public string GetTenantConnectionString()
    {
        if (string.IsNullOrWhiteSpace(TenantClaim))
        {
            return _configuration.GetValueOrThrow<string>("Parent:Database:DefaultConnection");
        }

        string pattern = @"(?<=Database=)([^;]*)";
        string newConnectionString = Regex.Replace(GetSystemConnectionString(), pattern, TenantClaim);

        return newConnectionString;
    }

}


