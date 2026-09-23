using BuildingBlocks.Web.Extensions;
using Microsoft.EntityFrameworkCore;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();
builder.Services.AddBuildingBlocksWeb(); // dispatcher + pipeline behaviors + event bus
builder.Services.AddOrderingInfrastructure(builder.Configuration);
// To use RabbitMQ instead of in-memory: builder.Services.AddRabbitMqBus(o => o.HostName = "localhost");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    // Migration-based startup (see backend/migrations.md): applies pending
    // EF Core migrations to OMS_Ordering on launch in Development.
    // UAT/Prod use idempotent SQL scripts instead — no auto-migrate there.
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
    db.Database.Migrate();
}

app.UseBuildingBlocksWeb(); // correlation-id + exception handling
app.UseAuthorization();
app.MapControllers();
app.Run();
