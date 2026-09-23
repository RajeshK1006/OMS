# Migrations Runbook (per service, migration-based)

Each service owns its database (`OMS_<Service>`) and its own EF Core
migrations. Run every command from `backend/` (where `OMS.slnx` lives).

Conventions
- DbContext per service: `{Service}DbContext` in `{Service}.Infrastructure`
  (e.g. `OrderingDbContext`). Only Ordering has one today; the commands
  below activate for a service as soon as its DbContext exists.
- Migration files live in `services/<S>/<S>.Infrastructure/Migrations/`
  and are auto-named `{yyyyMMddHHmmss}_{MigrationName}.cs`
  (e.g. `20260923155704_InitialCreate.cs`) + `.Designer.cs` + model snapshot.
  Never rename them — `__EFMigrationsHistory.MigrationId` must match.
- Tooling: `dotnet-ef` 10.x global tool + `EFCore.Design` on every `*.API`
  (both already installed). Builds on this machine need
  `/p:DisableImplicitNuGetFallbackFolder=true`; `dotnet ef ... --no-build`
  is used below so the CLI reuses those binaries.

One-time setup per NEW service (Ordering already done)
1. `DbContext` + `IEntityTypeConfiguration`s + `DbSet<OutboxMessage>` +
   `b.ConfigureOutbox()` (BuildingBlocks) in `{S}.Infrastructure`.
2. `AddDbContext` + repository + hosted outbox processor in
   `{S}.Infrastructure/DependencyInjection.cs`.
3. `db.Database.Migrate()` in `{S}.API/Program.cs` (Development only).

Generate a migration (after changing Domain / configurations)
```powershell
# Ordering (live example)
dotnet build services/Ordering/Ordering.API/Ordering.API.csproj /p:DisableImplicitNuGetFallbackFolder=true
dotnet ef migrations add <MigrationName> --project services/Ordering/Ordering.Infrastructure/Ordering.Infrastructure.csproj --startup-project services/Ordering/Ordering.API/Ordering.API.csproj --no-build

# Template for any service (replace <S> and <MigrationName>)
dotnet build services/<S>/<S>.API/<S>.API.csproj /p:DisableImplicitNuGetFallbackFolder=true
dotnet ef migrations add <MigrationName> --project services/<S>/<S>.Infrastructure/<S>.Infrastructure.csproj --startup-project services/<S>/<S>.API/<S>.API.csproj --no-build
```

Apply to local dev DB
```powershell
# Ordering
dotnet ef database update --project services/Ordering/Ordering.Infrastructure/Ordering.Infrastructure.csproj --startup-project services/Ordering/Ordering.API/Ordering.API.csproj --no-build

# Any service
dotnet ef database update --project services/<S>/<S>.Infrastructure/<S>.Infrastructure.csproj --startup-project services/<S>/<S>.API/<S>.API.csproj --no-build
```
(Dev APIs also auto-apply pending migrations at startup via `Migrate()`.)

Remove the LAST migration (only if NOT yet applied anywhere)
```powershell
dotnet ef migrations remove --project services/<S>/<S>.Infrastructure/<S>.Infrastructure.csproj --startup-project services/<S>/<S>.API/<S>.API.csproj --no-build
```
If it was already applied, add a new corrective migration instead — never
delete a migration row by hand.

List / inspect status
```powershell
dotnet ef migrations list --project services/<S>/<S>.Infrastructure/<S>.Infrastructure.csproj --startup-project services/<S>/<S>.API/<S>.API.csproj --no-build
sqlcmd -S <server> -d OMS_<S> -Q "SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId"
```

SQL script for UAT / Prod (seamless promotion)

Use the helper — it builds, exports an idempotent script into
`services/<S>/<S>.Infrastructure/Migrations/Scripts/` named
`{yyyyMMddHHmmss}-<S>-full.sql` (or `{ts}-<S>-<From>-to-<To>.sql` for a
delta), and opens it in the editor:
```powershell
# Full idempotent script: safe to run on any environment, applies only missing migrations
./scripts/Export-MigrationScript.ps1 -Service Ordering

# Delta script between two migrations (e.g. release cut)
./scripts/Export-MigrationScript.ps1 -Service Ordering -From 20260923155704_InitialCreate -To <ToMigration>

# Generate without auto-open
./scripts/Export-MigrationScript.ps1 -Service <S> -NoOpen
```
Raw CLI (same result, manual open):
```powershell
dotnet ef migrations script --idempotent --project services/<S>/<S>.Infrastructure/<S>.Infrastructure.csproj --startup-project services/<S>/<S>.API/<S>.API.csproj --no-build --output services/<S>/<S>.Infrastructure/Migrations/Scripts/<ts>-<S>-full.sql
```
Promotion flow: generate the idempotent script against dev → DBA runs it on
UAT, then Prod → deploy the API (Prod `Program.cs` has no `Migrate()`, so
the app never auto-migrates outside Development).

Service → database map
| Service | DbContext | Database | Status |
|---|---|---|---|
| Ordering | OrderingDbContext | OMS_Ordering | `20260923155704_InitialCreate` applied |
| Identity | IdentityDbContext | OMS_Identity | DbContext not yet created |
| Catalog | CatalogDbContext | OMS_Catalog | DbContext not yet created |
| Inventory | InventoryDbContext | OMS_Inventory | DbContext not yet created |
| Customer | CustomerDbContext | OMS_Customer | DbContext not yet created |
| Payment | PaymentDbContext | OMS_Payment | DbContext not yet created |
| Fulfillment | FulfillmentDbContext | OMS_Fulfillment | DbContext not yet created |
| Returns | ReturnsDbContext | OMS_Returns | DbContext not yet created |
| Procurement | ProcurementDbContext | OMS_Procurement | DbContext not yet created |
| Notification | NotificationDbContext | OMS_Notification | DbContext not yet created |
| Reporting | ReportingDbContext | OMS_Reporting | DbContext not yet created |
