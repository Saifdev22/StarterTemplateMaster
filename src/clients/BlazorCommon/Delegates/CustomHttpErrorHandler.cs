using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace BlazorCommon.Delegates;

public sealed class CustomHttpErrorHandler(ILogger<CustomHttpErrorHandler> logger, NavigationManager Nav) : DelegatingHandler
{
    private readonly ILogger<CustomHttpErrorHandler> _logger = logger;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = null!;
        try
        {
            response = await base.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                HandleServerError();
            }

            string errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred during the HTTP request.");
            Nav.NavigateTo("/error");
        }

        return response;
    }

    private void HandleServerError()
    {
        _logger.LogError("Navigating to the error page due to an Internal Server Error (500).");
        Nav.NavigateTo("/error");
    }
}
