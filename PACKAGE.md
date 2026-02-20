# Demarbit.Shared.Domain

Foundational DDD building blocks — base classes, contracts, and utilities for domain-driven .NET applications.

- **Target framework:** `net10.0`
- **Key dependencies:** None (zero external NuGet dependencies)
- **Nullable reference types:** Enabled
- **Treat warnings as errors:** Enabled

---

## Quick Start

This package has no DI registration — it is a pure domain library. Reference it and inherit from its base classes:

```csharp
using Demarbit.Shared.Domain.Models;
using Demarbit.Shared.Domain.Contracts;

public sealed class Order : AggregateRoot
{
    public string CustomerName { get; private set; } = string.Empty;
    public decimal Total { get; private set; }

    private Order() { }

    public static Order Create(string customerName, decimal total)
    {
        var order = new Order
        {
            CustomerName = customerName,
            Total = total
        };

        order.RaiseDomainEvent(new OrderCreated { OrderId = order.Id });
        return order;
    }
}

public sealed record OrderCreated : DomainEventBase
{
    public Guid OrderId { get; init; }
}
```

---

## Core Concepts

### Entity Hierarchy

The package provides a two-level entity hierarchy:

```
EntityBase<TId>  →  AggregateRoot<TId>
EntityBase       →  AggregateRoot         (Guid convenience aliases)
```

- **`EntityBase<TId>`** provides identity (`Id`), audit fields (`CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`), and equality-by-ID semantics.
- **`AggregateRoot<TId>`** extends `EntityBase<TId>` with domain event collection (`RaiseDomainEvent` / `DequeueDomainEvents`).
- The non-generic `EntityBase` and `AggregateRoot` default `TId` to `Guid` and auto-generate an ID on construction.

### Value Objects

`ValueObject` provides structural equality. Subclasses implement `GetEqualityComponents()` to declare which properties participate in equality.

### Domain Events

Events implement `IDomainEvent` and typically inherit from `DomainEventBase` (a `record`). Events are raised inside aggregates via `RaiseDomainEvent()` and dequeued by the unit of work after persistence.

### Auditing

All entities implement `IAuditableEntity` automatically (via `EntityBase<TId>`). The infrastructure layer is expected to call `SetCreated` and `SetUpdated` (e.g., in a `SaveChangesAsync` interceptor using `ICurrentUserProvider`).

### Multi-Tenancy

Aggregates that belong to a tenant implement `ITenantEntity`. The infrastructure layer resolves the current tenant via `ICurrentTenantProvider` and applies query filters.

### Event Idempotency

`IEventIdempotencyService` and `ProcessedEvent` support deduplication of domain event handling. Before processing an event, the handler checks `HasBeenProcessedAsync`; after processing, it calls `MarkAsProcessedAsync`.

### Guard Clauses

`Guard` is a static utility for validating arguments and domain invariants. It uses `CallerArgumentExpressionAttribute` to automatically capture parameter names.

---

## Public API Reference

### Contracts (`Demarbit.Shared.Domain.Contracts`)

#### `IDomainEvent`

```csharp
public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
    string EventType { get; }
    int Version { get; }
}
```

Base interface for all domain events. `EventId` uniquely identifies the event instance. `EventType` is the type name. `Version` supports schema evolution.

---

#### `IUnitOfWork`

```csharp
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    IReadOnlyList<IDomainEvent> GetAndClearPendingEvents();
}
```

Coordinates transactional persistence and collects domain events raised during the transaction. Implementations typically wrap EF Core's `DbContext`.

---

#### `IRepository<T, TId>`

```csharp
public interface IRepository<T, in TId>
    where T : AggregateRoot<TId>
    where TId : notnull, IEquatable<TId>
{
    Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(T aggregateRoot, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> aggregateRoots, CancellationToken cancellationToken = default);
    Task UpdateAsync(T aggregateRoot, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<T> aggregateRoots, CancellationToken cancellationToken = default);
    Task RemoveAsync(T aggregateRoot, CancellationToken cancellationToken = default);
    Task RemoveRangeAsync(IEnumerable<T> aggregateRoots, CancellationToken cancellationToken = default);
    Task RemoveByIdAsync(TId id, CancellationToken cancellationToken = default);
}
```

Generic repository contract for aggregate roots. Consuming projects should extend this interface with aggregate-specific query methods.

---

#### `IRepository<T>`

```csharp
public interface IRepository<T> : IRepository<T, Guid>
    where T : AggregateRoot
{
}
```

Convenience alias for aggregate roots that use `Guid` as their identifier.

---

#### `IAuditableEntity`

```csharp
public interface IAuditableEntity
{
    DateTime CreatedAt { get; }
    DateTime UpdatedAt { get; }
    Guid? CreatedBy { get; }
    Guid? UpdatedBy { get; }
    void SetCreated(DateTime createdAtUtc, Guid? createdBy = null);
    void SetUpdated(DateTime updatedAtUtc, Guid? updatedBy = null);
}
```

Marks an entity as auditable. Already implemented by `EntityBase<TId>`, so all entities and aggregates are auditable by default.

---

#### `ITenantEntity`

```csharp
public interface ITenantEntity
{
    Guid TenantId { get; }
}
```

Marker interface for entities that belong to a tenant's private data set. Apply this to aggregates or entities that require tenant isolation.

---

#### `ICurrentUserProvider`

```csharp
public interface ICurrentUserProvider
{
    Guid? UserId { get; }
    void SetUserId(Guid? userId);
}
```

Resolves the current user in the session. Implementations are typically scoped per-request and populated from authentication middleware.

---

#### `ICurrentTenantProvider`

```csharp
public interface ICurrentTenantProvider
{
    Guid? TenantId { get; }
    void SetTenantId(Guid? tenantId);
}
```

Resolves the current tenant in the session. Implementations are typically scoped per-request and populated from a tenant resolution middleware.

---

#### `IEventIdempotencyService`

```csharp
public interface IEventIdempotencyService
{
    Task<bool> HasBeenProcessedAsync(Guid eventId, string handlerType, CancellationToken cancellationToken = default);
    Task MarkAsProcessedAsync(Guid eventId, string eventType, string handlerType, CancellationToken cancellationToken = default);
}
```

Service for tracking processed events to prevent duplicate handling. The `handlerType` parameter allows the same event to be processed by multiple different handlers while ensuring each handler only processes it once.

---

### Models (`Demarbit.Shared.Domain.Models`)

#### `EntityBase<TId>`

```csharp
public abstract class EntityBase<TId> : IEquatable<EntityBase<TId>>, IAuditableEntity
    where TId : notnull, IEquatable<TId>
{
    public TId Id { get; protected set; }

    // Audit fields (from IAuditableEntity)
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    public void SetCreated(DateTime createdAtUtc, Guid? createdBy = null);
    public void SetUpdated(DateTime updatedAtUtc, Guid? updatedBy = null);

    // Equality by Id
    public bool Equals(EntityBase<TId>? other);
    public override int GetHashCode();
    public static bool operator ==(EntityBase<TId>? left, EntityBase<TId>? right);
    public static bool operator !=(EntityBase<TId>? left, EntityBase<TId>? right);
}
```

Base class for all domain entities. Provides identity, audit tracking, and equality-by-ID. `SetCreated` also sets `UpdatedAt` and `UpdatedBy` to the same values for consistency on initial creation.

---

#### `EntityBase`

```csharp
public abstract class EntityBase : EntityBase<Guid>
```

Convenience class that defaults `TId` to `Guid` and auto-generates `Id = Guid.NewGuid()` in the constructor.

---

#### `AggregateRoot<TId>`

```csharp
public abstract class AggregateRoot<TId> : EntityBase<TId>
    where TId : notnull, IEquatable<TId>
{
    public IReadOnlyList<IDomainEvent> DomainEvents { get; }
    public IReadOnlyList<IDomainEvent> DequeueDomainEvents();
    protected void RaiseDomainEvent(IDomainEvent domainEvent);
}
```

Base class for aggregate roots. Maintains a private list of domain events. Call `RaiseDomainEvent()` inside domain methods; the unit of work calls `DequeueDomainEvents()` after persistence to dispatch them.

---

#### `AggregateRoot`

```csharp
public abstract class AggregateRoot : AggregateRoot<Guid>
```

Convenience class that defaults `TId` to `Guid` and auto-generates `Id = Guid.NewGuid()` in the constructor.

---

#### `DomainEventBase`

```csharp
public abstract record DomainEventBase : IDomainEvent
{
    public Guid EventId { get; init; }       // Default: Guid.NewGuid()
    public DateTime OccurredOn { get; init; } // Default: DateTime.UtcNow
    public string EventType { get; }          // Returns GetType().Name
    public virtual int Version { get; init; } // Default: 1
}
```

Abstract record for domain events. Provides sensible defaults for all `IDomainEvent` properties. Being a `record`, it supports `with` expressions for deterministic testing.

---

#### `ValueObject`

```csharp
public abstract class ValueObject : IEquatable<ValueObject>
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public bool Equals(ValueObject? other);
    public override int GetHashCode();
    public static bool operator ==(ValueObject? left, ValueObject? right);
    public static bool operator !=(ValueObject? left, ValueObject? right);
}
```

Base class for value objects. Equality is determined by `GetEqualityComponents()`. Two value objects are equal if and only if they are the same type and all their components are equal.

---

#### `ProcessedEvent`

```csharp
public sealed class ProcessedEvent
{
    public Guid Id { get; private set; }
    public Guid EventId { get; private set; }
    public string EventType { get; private set; }
    public DateTime ProcessedAt { get; private set; }
    public string HandlerType { get; private set; }

    public static ProcessedEvent Create(Guid eventId, string eventType, string handlerType);
}
```

Persistence model for tracking processed domain events. Created via the `Create` factory method. The private constructor supports EF Core materialization.

---

### Exceptions (`Demarbit.Shared.Domain.Exceptions`)

#### `DomainException`

```csharp
public class DomainException : Exception
{
    public string? ErrorCode { get; }

    public DomainException(string message);
    public DomainException(string message, string errorCode);
    public DomainException(string message, Exception innerException);
    public DomainException(string message, string errorCode, Exception innerException);
}
```

Base exception for domain rule violations. The optional `ErrorCode` enables structured error handling and client-facing error mapping.

---

### Extensions (`Demarbit.Shared.Domain.Extensions`)

#### `StringExtensions`

```csharp
public static partial class StringExtensions
{
    public static string ToSlug(this string value);
}
```

**`ToSlug()`** — Converts a string to a URL-friendly slug by removing diacritics, lowercasing, and replacing non-alphanumeric characters with hyphens.

```csharp
"Café au Lait".ToSlug()  // "cafe-au-lait"
"Hello World!".ToSlug()  // "hello-world"
"  Trimmed  ".ToSlug()   // "trimmed"
"Ürban Köln".ToSlug()    // "urban-koln"
```

---

### Utilities (`Demarbit.Shared.Domain.Utilities`)

#### `Guard`

```csharp
public static class Guard
```

Static guard clause utility. All methods throw `ArgumentException` or `ArgumentNullException` on violation and use `[CallerArgumentExpression]` to capture parameter names automatically.

| Method | Throws when |
|---|---|
| `Against(bool condition, Func<Exception> exceptionFactory)` | `condition` is `true` |
| `Against(bool condition, string message, ...)` | `condition` is `true` |
| `NotNull<T>(T? value, ...)` where `T : class` | `value` is `null` |
| `NotNull<T>(T? value, ...)` where `T : struct` | `value` is `null` |
| `NotNullOrEmpty(string? value, ...)` | `value` is null or empty |
| `NotNullOrWhiteSpace(string? value, ...)` | `value` is null, empty, or whitespace |
| `IsTrue(bool value, string message, ...)` | `value` is `false` |
| `IsFalse(bool value, string message, ...)` | `value` is `true` |
| `MustBe<T>(T value, Func<T, bool> predicate, string message, ...)` | `predicate` returns `false` |
| `MustNotBe<T>(T value, Func<T, bool> predicate, string message, ...)` | `predicate` returns `true` |
| `Between<T>(T, T lower, T upper, ...)` where `T : INumber<T>` | `value` is outside `[lower, upper]` |
| `GreaterThan<T>(T, T compare, ...)` where `T : INumber<T>` | `value <= compare` |
| `GreaterThanOrEqualTo<T>(T, T compare, ...)` where `T : INumber<T>` | `value < compare` |
| `LessThan<T>(T, T compare, ...)` where `T : INumber<T>` | `value >= compare` |
| `LessThanOrEqualTo<T>(T, T compare, ...)` where `T : INumber<T>` | `value > compare` |
| `NotEmpty<T>(IEnumerable<T>? collection, ...)` | collection is null or empty |

---

## Usage Patterns & Examples

### Defining an Aggregate Root

```csharp
using Demarbit.Shared.Domain.Models;
using Demarbit.Shared.Domain.Contracts;
using Demarbit.Shared.Domain.Exceptions;
using Demarbit.Shared.Domain.Utilities;

public sealed class Product : AggregateRoot, ITenantEntity
{
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Money Price { get; private set; } = null!;

    private Product() { } // EF Core constructor

    public static Product Create(Guid tenantId, string name, Money price)
    {
        Guard.NotNullOrWhiteSpace(name);
        Guard.NotNull(price);

        var product = new Product
        {
            TenantId = tenantId,
            Name = name,
            Price = price
        };

        product.RaiseDomainEvent(new ProductCreated { ProductId = product.Id });
        return product;
    }

    public void UpdatePrice(Money newPrice)
    {
        Guard.NotNull(newPrice);
        Guard.Against(newPrice.Amount < 0, "Price cannot be negative.");

        Price = newPrice;
        RaiseDomainEvent(new ProductPriceChanged { ProductId = Id, NewPrice = newPrice.Amount });
    }
}
```

### Defining a Value Object

```csharp
using Demarbit.Shared.Domain.Models;
using Demarbit.Shared.Domain.Utilities;

public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        Guard.GreaterThanOrEqualTo(amount, 0m, "Amount cannot be negative.");
        Guard.NotNullOrWhiteSpace(currency);

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
```

### Defining Domain Events

```csharp
using Demarbit.Shared.Domain.Models;

public sealed record ProductCreated : DomainEventBase
{
    public Guid ProductId { get; init; }
}

public sealed record ProductPriceChanged : DomainEventBase
{
    public Guid ProductId { get; init; }
    public decimal NewPrice { get; init; }
}
```

Use `with` expressions for deterministic testing:

```csharp
var fixedTime = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
var evt = new ProductCreated { ProductId = id } with { OccurredOn = fixedTime };
```

### Defining a Repository Interface

```csharp
using Demarbit.Shared.Domain.Contracts;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Product>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
```

### Using a Custom ID Type

```csharp
public readonly record struct ProductId(Guid Value) : IEquatable<ProductId>;

public sealed class Product : AggregateRoot<ProductId>
{
    // ...

    public static Product Create(ProductId id, string name)
    {
        var product = new Product();
        product.Id = id; // Id setter is protected
        product.Name = name;
        return product;
    }
}

public interface IProductRepository : IRepository<Product, ProductId>
{
}
```

### Implementing Event Idempotency

```csharp
public class OrderPlacedHandler
{
    private readonly IEventIdempotencyService _idempotency;

    public OrderPlacedHandler(IEventIdempotencyService idempotency)
    {
        _idempotency = idempotency;
    }

    public async Task HandleAsync(OrderPlaced evt, CancellationToken ct)
    {
        var handlerType = nameof(OrderPlacedHandler);

        if (await _idempotency.HasBeenProcessedAsync(evt.EventId, handlerType, ct))
            return;

        // ... handle the event ...

        await _idempotency.MarkAsProcessedAsync(evt.EventId, evt.EventType, handlerType, ct);
    }
}
```

### Using Guard Clauses

```csharp
using Demarbit.Shared.Domain.Utilities;

public void SetQuantity(int quantity)
{
    Guard.GreaterThan(quantity, 0, "Quantity must be positive.");
    Quantity = quantity;
}

public void AssignCategory(Category? category)
{
    Guard.NotNull(category); // Parameter name captured automatically
    Category = category;
}

public void SetDiscount(decimal discount)
{
    Guard.Between(discount, 0m, 100m, "Discount must be between 0 and 100.");
    Discount = discount;
}

// Custom exception via factory
Guard.Against(order.IsLocked, () => new DomainException("Cannot modify a locked order.", "ORDER_LOCKED"));
```

### Throwing Domain Exceptions

```csharp
using Demarbit.Shared.Domain.Exceptions;

// Simple message
throw new DomainException("Order total cannot be negative.");

// With error code for structured handling
throw new DomainException("Insufficient stock.", "INSUFFICIENT_STOCK");

// Subclass for specific domain errors
public sealed class InsufficientStockException : DomainException
{
    public Guid ProductId { get; }

    public InsufficientStockException(Guid productId)
        : base($"Insufficient stock for product {productId}.", "INSUFFICIENT_STOCK")
    {
        ProductId = productId;
    }
}
```

---

## Integration Points

This package defines **contracts only** — it contains no DI registration, middleware, or EF Core configuration. Consuming projects are responsible for:

- **Implementing `IUnitOfWork`** — typically wrapping `DbContext.SaveChangesAsync()` with domain event collection from tracked aggregates via `DequeueDomainEvents()`.
- **Implementing `IRepository<T>` / `IRepository<T, TId>`** — typically as EF Core repositories.
- **Implementing `ICurrentUserProvider`** and **`ICurrentTenantProvider`** — typically as scoped services populated by authentication/tenant middleware.
- **Implementing `IEventIdempotencyService`** — using `ProcessedEvent` as the persistence model (configure its EF Core mapping in the infrastructure layer).
- **Calling `SetCreated` / `SetUpdated`** on entities during `SaveChangesAsync` — typically via an EF Core `SaveChangesInterceptor` that detects `IAuditableEntity` on tracked entries.
- **Applying tenant query filters** — typically via `IQueryable` filters on entities implementing `ITenantEntity` using the value from `ICurrentTenantProvider`.

---

## Dependencies & Compatibility

- **Zero NuGet dependencies.** This package depends only on the .NET 10 base class library.
- **`System.Numerics`** — Used by `Guard` numeric methods (`INumber<T>` constraint). Available in `net7.0+`.
- **No peer package dependencies.** This package can be referenced standalone.
