# OMS — Order Management System (Monorepo)

Distributed .NET 10 + React + Vite system.

```
oms/
├── backend/      → .NET 10 solution (12 services + BuildingBlocks)
├── frontend/     → React + Vite
├── docs/         → architecture decisions/documentation
└── README.md
```

## Services (each owns its own DB, no cross-DB FKs/joins)

Gateway, Identity, Catalog, Ordering, Inventory, Customer,
Payment, Fulfillment, Returns, Procurement, Notification, Reporting

Example: Ordering Service → `OMS_Ordering` DB

## Shared kernel rule

`BuildingBlocks/` is technical plumbing only (Domain / Application / Infrastructure / Web).
If a file contains a business word such as Order, SKU or Warehouse, it belongs in
`{Service}.Domain`, NOT in BuildingBlocks.

## Per-service structure (learn Ordering once, repeat)

```
Ordering/
├── Ordering.Domain/
├── Ordering.Application/
├── Ordering.Infrastructure/
├── Ordering.API/
├── Ordering.Contracts/      → plain records/events, references nothing
└── tests/Ordering.Tests/
```

Dependency direction:

```
API → Application → Domain
Infrastructure → Application → Domain
API ──X──> Infrastructure (except Program.cs composition root)
Cross-service: Ordering.Application → Inventory.Contracts (never Domain/Application/Infrastructure/API)
```

## Communication

- Sync HTTP: `Ordering.Application → ICatalogClient` / `Ordering.Infrastructure → CatalogClient`
- Async events via RabbitMQ + Outbox: `Order → Outbox row → Background processor → RabbitMQ → Inventory`

## Request flow

React → YARP Gateway → OrdersController → Pipeline Behaviors → PlaceOrderCommandHandler →
Order.Place() → IOrderRepository → OrderRepository → OrderingDbContext → SQL Server →
Outbox → RabbitMQ → Inventory

## Dev order

1. Domain → 2. Application abstractions → 3. Infrastructure →
4. Use case → 5. DI/wiring → 6. Controller + Program.cs
