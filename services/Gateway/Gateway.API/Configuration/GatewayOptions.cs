namespace Gateway.API.Configuration;

public sealed class GatewayOptions
{
    public Dictionary<string, ServiceRoute> Services { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public sealed class ServiceRoute
    {
        public string BaseUrl { get; set; } = default!;

        /// <summary>
        /// Maps a public endpoint key to the actual downstream service path,
        /// e.g. "OrderById" -> "/api/orders/{id}". Route values like {id}
        /// are substituted from the incoming request.
        /// </summary>
        public Dictionary<string, string> Paths { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }
}
