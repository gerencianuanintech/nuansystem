using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Documents.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class DocumentRepository(ITenantConnectionFactory connectionFactory) : IDocumentRepository
{
    public async Task<IReadOnlyCollection<DocumentSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT
    d.Id,
    d.DocumentType,
    d.DocumentNumber,
    d.CustomerId,
    c.Code AS CustomerCode,
    c.Name AS CustomerName,
    d.DocumentDate,
    d.Status,
    d.Currency,
    d.Subtotal,
    d.TaxTotal,
    d.Total
FROM dbo.Documents d
INNER JOIN dbo.Customers c ON c.Id = d.CustomerId
ORDER BY d.DocumentDate DESC, d.Id DESC;
""";

        using var connection = connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        var documents = await connection.QueryAsync<DocumentSummaryDto>(command);

        return documents.AsList();
    }

    public async Task<DocumentDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        const string headerSql = """
SELECT
    d.Id,
    d.DocumentType,
    d.DocumentNumber,
    d.CustomerId,
    c.Code AS CustomerCode,
    c.Name AS CustomerName,
    d.DocumentDate,
    d.Status,
    d.Currency,
    d.Subtotal,
    d.TaxTotal,
    d.Total
FROM dbo.Documents d
INNER JOIN dbo.Customers c ON c.Id = d.CustomerId
WHERE d.Id = @Id;
""";

        const string linesSql = """
SELECT
    dl.Id,
    dl.DocumentId,
    dl.LineNumber,
    dl.ItemId,
    i.Code AS ItemCode,
    i.Name AS ItemName,
    dl.Quantity,
    dl.UnitPrice,
    dl.TaxRate,
    dl.LineTotal
FROM dbo.DocumentLines dl
INNER JOIN dbo.Items i ON i.Id = dl.ItemId
WHERE dl.DocumentId = @Id
ORDER BY dl.LineNumber;
""";

        using var connection = connectionFactory.CreateConnection();
        var parameters = new { Id = id };
        var headerCommand = new CommandDefinition(headerSql, parameters, cancellationToken: cancellationToken);
        var header = await connection.QuerySingleOrDefaultAsync<DocumentHeaderRecord>(headerCommand);
        if (header is null)
        {
            return null;
        }

        var linesCommand = new CommandDefinition(linesSql, parameters, cancellationToken: cancellationToken);
        var lines = (await connection.QueryAsync<DocumentLineDto>(linesCommand)).AsList();

        return new DocumentDto(
            header.Id,
            header.DocumentType,
            header.DocumentNumber,
            header.CustomerId,
            header.CustomerCode,
            header.CustomerName,
            header.DocumentDate,
            header.Status,
            header.Currency,
            header.Subtotal,
            header.TaxTotal,
            header.Total,
            lines);
    }

    public async Task<long> CreateAsync(CreateDocumentData document, CancellationToken cancellationToken = default)
    {
        const string insertHeaderSql = """
INSERT INTO dbo.Documents
(
    DocumentType,
    CustomerId,
    DocumentDate,
    Status,
    Currency,
    Subtotal,
    TaxTotal,
    Total
)
OUTPUT INSERTED.Id
VALUES
(
    @DocumentType,
    @CustomerId,
    @DocumentDate,
    N'Draft',
    @Currency,
    @Subtotal,
    @TaxTotal,
    @Total
);
""";

        const string updateNumberSql = """
UPDATE dbo.Documents
SET DocumentNumber = @DocumentNumber
WHERE Id = @Id;
""";

        const string insertLineSql = """
INSERT INTO dbo.DocumentLines
(
    DocumentId,
    LineNumber,
    ItemId,
    Quantity,
    UnitPrice,
    TaxRate,
    LineTotal
)
VALUES
(
    @DocumentId,
    @LineNumber,
    @ItemId,
    @Quantity,
    @UnitPrice,
    @TaxRate,
    @LineTotal
);
""";

        using var connection = connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var headerCommand = new CommandDefinition(
                insertHeaderSql,
                new
                {
                    document.DocumentType,
                    document.CustomerId,
                    document.DocumentDate,
                    document.Currency,
                    document.Subtotal,
                    document.TaxTotal,
                    document.Total
                },
                transaction,
                cancellationToken: cancellationToken);

            var documentId = await connection.ExecuteScalarAsync<long>(headerCommand);
            var documentNumber = $"{document.DocumentType.ToUpperInvariant()}-{documentId:000000}";

            var updateNumberCommand = new CommandDefinition(
                updateNumberSql,
                new { Id = documentId, DocumentNumber = documentNumber },
                transaction,
                cancellationToken: cancellationToken);
            await connection.ExecuteAsync(updateNumberCommand);

            foreach (var line in document.Lines)
            {
                var lineCommand = new CommandDefinition(
                    insertLineSql,
                    new
                    {
                        DocumentId = documentId,
                        line.LineNumber,
                        line.ItemId,
                        line.Quantity,
                        line.UnitPrice,
                        line.TaxRate,
                        line.LineTotal
                    },
                    transaction,
                    cancellationToken: cancellationToken);

                await connection.ExecuteAsync(lineCommand);
            }

            transaction.Commit();
            return documentId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> CustomerExistsAsync(int customerId, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT COUNT(1)
FROM dbo.Customers
WHERE Id = @CustomerId
  AND IsActive = 1;
""";

        using var connection = connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, new { CustomerId = customerId }, cancellationToken: cancellationToken);
        var count = await connection.ExecuteScalarAsync<int>(command);

        return count > 0;
    }

    public async Task<IReadOnlyCollection<int>> GetMissingItemIdsAsync(
        IEnumerable<int> itemIds,
        CancellationToken cancellationToken = default)
    {
        var distinctIds = itemIds.Distinct().ToArray();
        if (distinctIds.Length == 0)
        {
            return Array.Empty<int>();
        }

        const string sql = """
SELECT Id
FROM dbo.Items
WHERE Id IN @ItemIds
  AND IsActive = 1;
""";

        using var connection = connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, new { ItemIds = distinctIds }, cancellationToken: cancellationToken);
        var existingIds = (await connection.QueryAsync<int>(command)).ToHashSet();

        return distinctIds.Where(id => !existingIds.Contains(id)).ToArray();
    }

    private sealed record DocumentHeaderRecord(
        long Id,
        string DocumentType,
        string? DocumentNumber,
        int CustomerId,
        string CustomerCode,
        string CustomerName,
        DateOnly DocumentDate,
        string Status,
        string Currency,
        decimal Subtotal,
        decimal TaxTotal,
        decimal Total);
}
