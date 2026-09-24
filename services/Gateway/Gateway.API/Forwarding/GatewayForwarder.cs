using System.Net.Http.Headers;
using Gateway.API.Configuration;
using Microsoft.Extensions.Options;

namespace Gateway.API.Forwarding;

public class GatewayForwarder(IHttpClientFactory factory, IOptions<GatewayOptions> options)
{
    public async Task ForwardAsync(
        HttpContext context,
        string service,
        string pathKey,
        Dictionary<string, string>? routeValues = null,
        CancellationToken ct = default)
    {
        if (!options.Value.Services.TryGetValue(service, out var svc))
        {
            context.Response.StatusCode = StatusCodes.Status502BadGateway;
            await context.Response.WriteAsJsonAsync(new { error = $"Unknown service '{service}'." }, ct);
            return;
        }

        if (!svc.Paths.TryGetValue(pathKey, out var template))
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = $"No path mapping '{pathKey}' for service '{service}'." }, ct);
            return;
        }

        foreach (var (k, v) in routeValues ?? new Dictionary<string, string>())
            template = template.Replace("{" + k + "}", Uri.EscapeDataString(v));

        using var request = new HttpRequestMessage(
            new HttpMethod(context.Request.Method),
            template + context.Request.QueryString.Value);

        if (context.Request.ContentLength.GetValueOrDefault() > 0 || context.Request.Headers.ContainsKey("Content-Type"))
        {
            request.Content = new StreamContent(context.Request.Body);
            if (context.Request.ContentType is not null)
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(context.Request.ContentType);
        }

        if (context.Request.Headers.TryGetValue("Authorization", out var auth))
            request.Headers.TryAddWithoutValidation("Authorization", (string?)auth);

        using var response = await factory.CreateClient(service.ToLowerInvariant())
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);

        context.Response.StatusCode = (int)response.StatusCode;
        if (response.Content.Headers.ContentType is not null)
            context.Response.ContentType = response.Content.Headers.ContentType.ToString();
        await response.Content.CopyToAsync(context.Response.Body, ct);
    }
}

