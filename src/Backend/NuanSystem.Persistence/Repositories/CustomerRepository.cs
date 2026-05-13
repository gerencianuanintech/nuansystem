using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Customers.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class CustomerRepository(ITenantConnectionFactory connectionFactory)
    : DapperRepository(connectionFactory), ICustomerRepository
{
    public Task<IReadOnlyCollection<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT
    Id,
    Code,
    Name,
    TaxIdentification,
    Email,
    Phone,
    AddressLine,
    IsActive
FROM dbo.Customers
ORDER BY Name;
""";

        return QueryAsync<CustomerDto>(sql, cancellationToken: cancellationToken);
    }

    public Task<CustomerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT
    Id,
    Code,
    Name,
    TaxIdentification,
    Email,
    Phone,
    AddressLine,
    IsActive
FROM dbo.Customers
WHERE Id = @Id;
""";

        return QuerySingleOrDefaultAsync<CustomerDto>(sql, new { Id = id }, cancellationToken);
    }

    public async Task<int> CreateAsync(CreateCustomerData customer, CancellationToken cancellationToken = default)
    {
        const string sql = """
INSERT INTO dbo.Customers
(
    Code,
    Name,
    TaxIdentification,
    Email,
    Phone,
    AddressLine
)
OUTPUT INSERTED.Id
VALUES
(
    @Code,
    @Name,
    @TaxIdentification,
    @Email,
    @Phone,
    @AddressLine
);
""";

        var id = await ExecuteScalarAsync<int?>(sql, customer, cancellationToken);

        return id ?? throw new InvalidOperationException("No se pudo obtener el Id del cliente creado.");
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT COUNT(1)
FROM dbo.Customers
WHERE Code = @Code;
""";

        var count = await ExecuteScalarAsync<int>(sql, new { Code = code }, cancellationToken);

        return count > 0;
    }

    public async Task<bool> ExistsByCodeAsync(string code, int excludingId, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT COUNT(1)
FROM dbo.Customers
WHERE Code = @Code
  AND Id <> @ExcludingId;
""";

        var count = await ExecuteScalarAsync<int>(sql, new { Code = code, ExcludingId = excludingId }, cancellationToken);

        return count > 0;
    }

    public async Task<bool> UpdateAsync(UpdateCustomerData customer, CancellationToken cancellationToken = default)
    {
        const string sql = """
UPDATE dbo.Customers
SET
    Code = @Code,
    Name = @Name,
    TaxIdentification = @TaxIdentification,
    Email = @Email,
    Phone = @Phone,
    AddressLine = @AddressLine,
    IsActive = @IsActive,
    UpdatedAt = SYSUTCDATETIME()
WHERE Id = @Id;
""";

        var affectedRows = await ExecuteAsync(sql, customer, cancellationToken);

        return affectedRows > 0;
    }

    public async Task<bool> SetActiveStateAsync(int id, bool isActive, CancellationToken cancellationToken = default)
    {
        const string sql = """
UPDATE dbo.Customers
SET
    IsActive = @IsActive,
    UpdatedAt = SYSUTCDATETIME()
WHERE Id = @Id;
""";

        var affectedRows = await ExecuteAsync(sql, new { Id = id, IsActive = isActive }, cancellationToken);

        return affectedRows > 0;
    }
}
