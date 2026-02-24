# Demarbit.Shared.Domain

Shared domain building blocks for DDD and Clean Architecture in .NET — entities, aggregates, value objects, domain events, and guards.

[![CI/CD](https://github.com/DemarbitBV/shared-domain/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/DemarbitBV/shared-domain/actions/workflows/ci-cd.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=DemarbitBV_shared-domain&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=DemarbitBV_shared-domain)
[![NuGet](https://img.shields.io/nuget/v/Demarbit.Shared.Domain)](https://www.nuget.org/packages/Demarbit.Shared.Domain)

## What's Included

**Entities and Aggregates** — `EntityBase<TId>` and `AggregateRoot<TId>` base classes with identity, equality-by-ID, built-in audit fields, and domain event collection. Guid convenience aliases (`EntityBase`, `AggregateRoot`) auto-generate IDs.

**Value Objects** — `ValueObject` base class with structural equality. Subclasses define equality through `GetEqualityComponents()`.

**Domain Events** — `IDomainEvent` contract and `DomainEventBase` record with auto-generated metadata (`EventId`, `OccurredOn`, `EventType`, `Version`). Supports `with` expressions for deterministic testing.

**Contracts** — `IRepository<T, TId>`, `IUnitOfWork`, `IAuditableEntity`, `IAggregateRoot`, `ICurrentUserProvider`, `ICurrentTenantProvider`, `ITenantEntity`, and `IEventIdempotencyService`. Pure abstractions — no infrastructure dependencies.

**Guards** — Static `Guard` class for argument validation and domain invariants. Uses `[CallerArgumentExpression]` to capture parameter names automatically.

**Exceptions** — `DomainException` for domain rule violations with an optional `ErrorCode` for structured error handling.

**Extensions** — `ToSlug()` string extension for URL-friendly slug generation with diacritics removal.

## Quick Start

### Entities

Inherit from `EntityBase` (Guid ID) or `EntityBase<TId>` (custom ID type). Audit fields and equality come built in.

```csharp
public class Customer : EntityBase
{
    public string Name { get; private set; }

    public Customer(string name)
    {
        Guard.NotNullOrWhiteSpace(name);
        Name = name;
    }
}
```

### Aggregate Roots

Inherit from `AggregateRoot` to get domain event support on top of entity features. Raise events inside domain methods — they queue until the unit of work dispatches them.

```csharp
public class Order : AggregateRoot
{
    public string CustomerName { get; private set; }
    public OrderStatus Status { get; private set; }

    public Order(string customerName)
    {
        Guard.NotNullOrWhiteSpace(customerName);
        CustomerName = customerName;
        Status = OrderStatus.Placed;
        RaiseDomainEvent(new OrderPlaced { OrderId = Id });
    }

    public void Cancel()
    {
        Guard.Against(Status == OrderStatus.Shipped, "Cannot cancel a shipped order.");
        Status = OrderStatus.Cancelled;
        RaiseDomainEvent(new OrderCancelled { OrderId = Id });
    }
}
```

### Domain Events

Use `DomainEventBase` as a record base. Metadata is auto-generated — override with `with` for testing.

```csharp
public sealed record OrderPlaced : DomainEventBase
{
    public Guid OrderId { get; init; }
}

// In a test with deterministic time:
var evt = new OrderPlaced { OrderId = id } with { OccurredOn = fixedTime };
```

### Value Objects

Subclass `ValueObject` and implement `GetEqualityComponents()`. Two value objects are equal when all components match.

```csharp
public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        Guard.GreaterThanOrEqualTo(amount, 0m, "Amount cannot be negative.");
        Guard.NotNullOrWhiteSpace(currency);
        Amount = amount;
        Currency = currency;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
```

### Guard Clauses

Validate arguments and enforce domain invariants. Parameter names are captured automatically.

```csharp
Guard.NotNull(customer);                              // throws ArgumentNullException
Guard.NotNullOrWhiteSpace(name);                      // throws ArgumentException
Guard.Against(age < 0, "Age cannot be negative.");    // throws ArgumentException
Guard.Between(quantity, 1, 100, "Quantity out of range.");
Guard.MustBe(email, e => e.Contains('@'), "Invalid email.");
Guard.NotEmpty(lineItems);                            // throws on null or empty collection
```

### Repository Contract

Define aggregate-specific repositories by extending `IRepository<T>` (Guid) or `IRepository<T, TId>` (custom ID):

```csharp
public interface IOrderRepository : IRepository<Order>
{
    Task<List<Order>> GetByCustomerAsync(string customerName, CancellationToken ct = default);
}
```

The base interface provides `GetByIdAsync`, `GetAllAsync`, `AddAsync`, `AddRangeAsync`, `UpdateAsync`, `UpdateRangeAsync`, `RemoveAsync`, `RemoveRangeAsync`, and `RemoveByIdAsync`.

### Unit of Work

`IUnitOfWork` coordinates transactional persistence and domain event collection:

```csharp
await unitOfWork.BeginTransactionAsync(ct);
await orderRepository.AddAsync(order, ct);
await unitOfWork.SaveChangesAsync(ct);
var events = unitOfWork.GetAndClearPendingEvents();
await unitOfWork.CommitTransactionAsync(ct);

// Dispatch events after commit
await dispatcher.NotifyAsync(events, ct);
```

### Multi-Tenancy

Mark aggregates or entities with `ITenantEntity` and resolve the current tenant with `ICurrentTenantProvider`:

```csharp
public class Project : AggregateRoot, ITenantEntity
{
    public Guid TenantId { get; private set; }

    public Project(Guid tenantId, string name)
    {
        TenantId = tenantId;
        Name = name;
    }
}
```

### Domain Exceptions

Throw `DomainException` for domain rule violations. The optional `ErrorCode` enables structured error mapping:

```csharp
throw new DomainException("Order has already been shipped.");
throw new DomainException("Insufficient stock.", "INSUFFICIENT_STOCK");
```

### Event Idempotency

`IEventIdempotencyService` and `ProcessedEvent` prevent duplicate event handling:

```csharp
if (await idempotencyService.HasBeenProcessedAsync(evt.EventId, nameof(MyHandler), ct))
    return;

// Handle the event...

await idempotencyService.MarkAsProcessedAsync(evt.EventId, evt.EventType, nameof(MyHandler), ct);
```

### Slug Generation

Convert strings to URL-friendly slugs with diacritics removal:

```csharp
"Café au Lait".ToSlug()   // "cafe-au-lait"
"Hello World!".ToSlug()   // "hello-world"
"Ürban Köln".ToSlug()     // "urban-koln"
```

## Design Principles

- **Zero dependencies** — depends only on the .NET base class library. No NuGet packages, no infrastructure concerns.
- **Identity by ID** — entities use equality-by-ID semantics. Value objects use structural equality.
- **Audit built in** — every entity tracks `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy` through `IAuditableEntity`.
- **Events stay in the domain** — aggregates raise events, the unit of work collects them, infrastructure dispatches them. The domain layer never depends on the dispatcher.
- **Pure contracts** — `IRepository`, `IUnitOfWork`, and provider interfaces define what the domain needs without dictating how it's implemented.
- **Guard clauses over exceptions in constructors** — `Guard` keeps validation expressive and consistent across the domain.

## Architecture Fit

```
Demarbit.Shared.Domain          ← this package (zero deps)
    ↑
Demarbit.Shared.Application     ← dispatching, pipeline, validation
    ↑
Demarbit.Shared.Infrastructure  ← EF Core, repositories, event dispatch
    ↑
[Your Application Project]      ← references what it needs
```

## License

[MIT](LICENSE)
