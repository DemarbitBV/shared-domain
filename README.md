# Demarbit.Shared.Domain

Shared domain building blocks for DDD and Clean Architecture in .NET — entities, aggregates, value objects, domain events, and guards.

## Installation

```bash
dotnet add package Demarbit.Shared.Domain
```

## What's Included

**Base types** — `EntityBase<TId>`, `AggregateRoot<TId>`, and `ValueObject` with built-in audit fields (`CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`), domain event support, and proper equality semantics.

**Domain events** — `IDomainEvent` contract and `DomainEventBase` record with event ID, type name, and timestamp.

**Contracts** — `IRepository<T, TId>` and `IUnitOfWork` interfaces for dependency inversion. No infrastructure dependencies.

**Utilities** — `Guard` class for domain preconditions and `StringExtensions.ToSlug()` for slug generation.

## Quick Start

### Entities and Aggregates

Every entity has a generic `TId` parameter. Convenience types defaulting to `Guid` are provided for the common case.

```csharp
// Using the default Guid ID
public class Order : AggregateRoot
{
    public string CustomerName { get; private set; }
    public List<OrderLine> Lines { get; private set; } = [];

    public Order(string customerName)
    {
        CustomerName = Guard.Against.NullOrWhiteSpace(customerName, nameof(customerName));
        AddDomainEvent(new OrderCreatedEvent(Id, customerName));
    }
}

// Using a strongly-typed ID
public class Product : AggregateRoot<ProductId>
{
    public string Name { get; private set; }

    public Product(ProductId id, string name) : base(id)
    {
        Name = name;
    }
}
```

### Value Objects

```csharp
public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
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

### Domain Events

```csharp
public record OrderCreatedEvent(Guid OrderId, string CustomerName) : DomainEventBase;
```

Events are collected on the aggregate and can be dequeued by your infrastructure layer after saving:

```csharp
var events = order.DequeueDomainEvents();
```

### Guards

```csharp
Guard.NotNull(customer, nameof(customer));
Guard.NotNullOrWhiteSpace(name, nameof(name));
Guard.Between(quantity, 1, 1000, nameof(quantity));

// With a custom exception
Guard.Against(() => order.Lines.Count == 0, () => new DomainException("Order must have at least one line."));
```

### Audit Fields

Entities expose `CreatedAt`, `UpdatedAt`, `CreatedBy`, and `UpdatedBy`. Update them explicitly for testability:

```csharp
entity.SetCreated(dateTimeProvider.UtcNow, userId);
entity.SetUpdated(dateTimeProvider.UtcNow, userId);
```

## Design Principles

This package follows a few deliberate constraints:

- **Zero dependencies** — no EF Core, no ASP.NET, no third-party packages. Pure domain logic only.
- **Generic ID support** — `EntityBase<TId>` and `AggregateRoot<TId>` accept any `IEquatable<TId>` key type. The non-generic `EntityBase` and `AggregateRoot` default to `Guid` for convenience.
- **Small surface area** — only types that belong in the domain layer are included. Infrastructure concerns like idempotency tracking, EF Core interceptors, and session context live in separate packages.

## Architecture Fit

This package is the innermost layer in a Clean Architecture setup:

```
Demarbit.Shared.Domain          ← this package (zero deps)
Demarbit.Shared.Application     ← CQRS, Result types, session context
Demarbit.Shared.Infrastructure  ← EF Core, repositories, event dispatch
```

Each layer only depends on the layers inside it. A project using Dapper instead of EF Core can still use the Domain and Application packages.

## License

[MIT](LICENSE)