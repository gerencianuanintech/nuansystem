---
name: nuansystem-mediatr-cqrs
description: Implement or review NuanSystem Application use cases with MediatR/CQRS: commands, queries, handlers, validators, DTOs, Result<T>, pipeline behaviors, validation, logging, transactions, feature folders, repository contracts, and controller/minimal endpoint delegation. Use when touching Application features, handlers, validators, MediatR registration, or API-to-use-case flow.
---

# NuanSystem MediatR CQRS

## Core Rules

- Commands change state.
- Queries read state and must not change state.
- Controllers/minimal endpoints receive requests, call MediatR/use case, and return responses.
- Controllers/endpoints must not contain business logic or direct database access.
- Handlers coordinate use cases; they must stay small and avoid becoming all-purpose service classes.
- Use FluentValidation for simple input validation.
- Put complex business invariants in Domain services/entities or focused Application services.
- Use `Result<T>` for business failures.
- Use transaction boundaries for multi-write operations through `ITransactionRunner` or a transaction pipeline.
- Do not use `DbConnection` directly in handlers when a repository contract exists.

## Feature Structure

```text
Application
└── Customers
    ├── Commands
    │   ├── CreateCustomer
    │   │   ├── CreateCustomerCommand.cs
    │   │   ├── CreateCustomerCommandHandler.cs
    │   │   └── CreateCustomerCommandValidator.cs
    │   └── UpdateCustomer
    ├── Queries
    │   ├── GetCustomers
    │   └── GetCustomerById
    ├── Dtos
    └── Interfaces
```

## Command Example

```csharp
public sealed record CreateCustomerCommand(
    string Code,
    string Name,
    string? Identification,
    int CreatedByUserId) : IRequest<Result<CreateCustomerResponse>>;

public sealed record CreateCustomerResponse(int CustomerId);
```

```csharp
public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CreatedByUserId).GreaterThan(0);
    }
}
```

```csharp
public sealed class CreateCustomerCommandHandler
    : IRequestHandler<CreateCustomerCommand, Result<CreateCustomerResponse>>
{
    private readonly ICustomerRepository _repository;

    public async Task<Result<CreateCustomerResponse>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        if (await _repository.ExistsByCodeAsync(request.Code, cancellationToken))
        {
            return Result<CreateCustomerResponse>.Failure(
                "CUSTOMER_ALREADY_EXISTS",
                "Ya existe un cliente con el mismo codigo.");
        }

        var id = await _repository.CreateAsync(
            new CreateCustomerData(request.Code, request.Name, request.Identification, request.CreatedByUserId),
            cancellationToken);

        return Result<CreateCustomerResponse>.Success(new CreateCustomerResponse(id));
    }
}
```

## Query Example

```csharp
public sealed record GetCustomersQuery(
    string? Search,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<CustomerListItemDto>>>;
```

```csharp
public sealed class GetCustomersQueryHandler
    : IRequestHandler<GetCustomersQuery, Result<PagedResult<CustomerListItemDto>>>
{
    private readonly ICustomerRepository _repository;

    public async Task<Result<PagedResult<CustomerListItemDto>>> Handle(
        GetCustomersQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetListAsync(
            new CustomerListFilter(request.Search, request.Page, request.PageSize),
            cancellationToken);

        return Result<PagedResult<CustomerListItemDto>>.Success(result);
    }
}
```

## Validation Behavior

```csharp
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .SelectMany(v => v.Validate(context).Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count > 0)
            throw new NuanValidationException(failures);

        return await next();
    }
}
```

## Program.cs Registration

```csharp
builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(typeof(CreateCustomerCommand).Assembly);
});

builder.Services.AddValidatorsFromAssembly(typeof(CreateCustomerCommandValidator).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
```

Add `TransactionBehavior` only for commands/use cases that require it or use a marker interface such as `ITransactionalRequest`.

## Handler Quality Checklist

- Does the handler represent one clear use case?
- Does it validate state again instead of trusting frontend totals/status?
- Does it call repository interfaces rather than provider-specific classes?
- Does it return stable error codes?
- Does it pass `CancellationToken`?
- Does it avoid direct HTTP, WinForms, SAP client, and SQL syntax dependencies?
