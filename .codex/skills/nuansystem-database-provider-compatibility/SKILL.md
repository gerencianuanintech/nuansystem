---
name: nuansystem-database-provider-compatibility
description: Keep NuanSystem SQL Server-first persistence compatible with future MySQL support by isolating provider-specific SQL, repository implementations, connection factories, DatabaseProvider, CompanyContext, scripts, stored procedure contracts, and dialect differences. Use when designing persistence, tenant connections, database scripts, repositories, or provider-specific behavior.
---

# NuanSystem Database Provider Compatibility

## Core Rules

- SQL Server is the primary supported engine.
- MySQL is a future provider and must not break SQL Server design.
- Isolate provider differences inside Persistence and `database/{provider}` scripts.
- Application consumes repository interfaces, never provider implementations.
- Domain does not know SQL Server, MySQL, stored procedure names, Dapper, EF Core, or connection classes.
- Do not leak SQL Server syntax into Application or Domain.
- Do not use stored procedure names directly in handlers.
- Keep repository contracts stable regardless of provider.
- Always use parameters. Do not build unsafe dynamic SQL.

## Folder Layout

```text
database
├── sqlserver
│   ├── master
│   └── tenant
└── mysql
    ├── master
    └── tenant
```

Provider-specific C# belongs in Persistence:

```text
Persistence
├── Connection
│   ├── ICompanyConnectionFactory.cs
│   └── CompanyConnectionFactory.cs
├── Repositories
│   ├── SqlServerCustomerRepository.cs
│   └── MySqlCustomerRepository.cs
└── Dialects
    └── ISqlDialect.cs
```

## Provider Context

```csharp
public enum DatabaseProvider
{
    SqlServer = 1,
    MySql = 2
}

public sealed record CompanyContext(
    int CompanyId,
    string CompanyCode,
    DatabaseProvider DatabaseProvider,
    string ConnectionString);
```

The trusted `CompanyContext` must be resolved from the backend master database after validating `X-Company-Code` and user-company access.

## Connection Factory

```csharp
public interface ICompanyConnectionFactory
{
    Task<DbConnection> CreateOpenConnectionAsync(
        CompanyContext companyContext,
        CancellationToken cancellationToken);
}

public sealed class CompanyConnectionFactory : ICompanyConnectionFactory
{
    public async Task<DbConnection> CreateOpenConnectionAsync(
        CompanyContext companyContext,
        CancellationToken cancellationToken)
    {
        DbConnection connection = companyContext.DatabaseProvider switch
        {
            DatabaseProvider.SqlServer => new SqlConnection(companyContext.ConnectionString),
            DatabaseProvider.MySql => new MySqlConnection(companyContext.ConnectionString),
            _ => throw new InvalidOperationException("Unsupported database provider.")
        };

        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
```

## Repository Contracts

Application defines the contract:

```csharp
public interface ICustomerRepository
{
    Task<CustomerDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<CustomerListItemDto>> GetListAsync(CustomerListFilter filter, CancellationToken cancellationToken);
    Task<int> CreateAsync(CreateCustomerData data, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(UpdateCustomerData data, CancellationToken cancellationToken);
}
```

SQL Server CRUD implementations must use stored procedures:

```csharp
public sealed class SqlServerCustomerRepository : ICustomerRepository
{
    private const string CreateProcedure = "SP_NA_POST_CUSTOMER_CREAR";

    public async Task<int> CreateAsync(CreateCustomerData data, CancellationToken cancellationToken)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(_companyContext.Current, cancellationToken);
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(CreateProcedure, data, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
    }
}
```

Future MySQL implementation must keep the same contract and should use equivalent stored procedures or the approved provider-specific persistence mechanism:

```csharp
public sealed class MySqlCustomerRepository : ICustomerRepository
{
    public async Task<int> CreateAsync(CreateCustomerData data, CancellationToken cancellationToken)
    {
        const string sql = "CALL SP_NA_POST_CUSTOMER_CREAR(@Code, @Name, @CreatedByUserId);";
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(_companyContext.Current, cancellationToken);
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, data, cancellationToken: cancellationToken));
    }
}
```

## SQL Server-Specific Features

These are allowed only in SQL Server scripts or SQL Server persistence implementation:

- `DATETIME2`
- `SYSUTCDATETIME()`
- `SCOPE_IDENTITY()`
- `@@ROWCOUNT`
- `CREATE OR ALTER`
- `NVARCHAR(MAX)`
- SQL Server stored procedure syntax

If MySQL support is added, create equivalent scripts under `database/mysql` instead of weakening SQL Server scripts.

## Dialect Guidance

Use `ISqlDialect` only when a small formatting difference is unavoidable. Prefer separate repository implementations when SQL differs materially:

```csharp
public interface ISqlDialect
{
    string CurrentUtcDateExpression { get; }
    string LimitOffset(int take, int skip);
}
```

Do not introduce dialect abstractions into Application unless the use case is genuinely provider-neutral and not persistence-specific.
