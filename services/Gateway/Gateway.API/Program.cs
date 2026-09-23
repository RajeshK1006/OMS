using Gateway.API.Configuration;
using Gateway.API.Forwarding;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Gateway facade: public controllers forward to downstream paths mapped in appsettings.json
var gatewayOptions = new GatewayOptions();
builder.Configuration.GetSection("Gateway").Bind(gatewayOptions);
builder.Services.Configure<GatewayOptions>(builder.Configuration.GetSection("Gateway"));
foreach (var (name, svc) in gatewayOptions.Services)
    builder.Services.AddHttpClient(name.ToLowerInvariant(), c => c.BaseAddress = new Uri(svc.BaseUrl));
builder.Services.AddScoped<GatewayForwarder>();

// YARP reverse proxy: React -> Gateway -> service APIs (routes in appsettings.json)
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger(o =>
    {
        // Keep the gateway's own doc out of YARP's way: Swashbuckle's default
        // "swagger/{documentName}/swagger.json" template would swallow the
        // downstream "/swagger/<service>/swagger.json" proxy routes below.
        o.RouteTemplate = "swagger/gateway/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(o =>
    {
        o.SwaggerEndpoint("/swagger/gateway/v1/swagger.json", "Gateway.API v1");
        o.SwaggerEndpoint("/swagger/identity/swagger.json", "Identity v1");
        o.SwaggerEndpoint("/swagger/catalog/swagger.json", "Catalog v1");
        o.SwaggerEndpoint("/swagger/ordering/swagger.json", "Ordering v1");
        o.SwaggerEndpoint("/swagger/inventory/swagger.json", "Inventory v1");
        o.SwaggerEndpoint("/swagger/customer/swagger.json", "Customer v1");
        o.SwaggerEndpoint("/swagger/payment/swagger.json", "Payment v1");
        o.SwaggerEndpoint("/swagger/fulfillment/swagger.json", "Fulfillment v1");
        o.SwaggerEndpoint("/swagger/returns/swagger.json", "Returns v1");
        o.SwaggerEndpoint("/swagger/procurement/swagger.json", "Procurement v1");
        o.SwaggerEndpoint("/swagger/notification/swagger.json", "Notification v1");
        o.SwaggerEndpoint("/swagger/reporting/swagger.json", "Reporting v1");
    });
}

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
    .WithName("Health");

app.MapControllers(); // public facade (wins over YARP for owned paths)
app.MapReverseProxy(); // swagger aggregation + future service routes

app.Run();
