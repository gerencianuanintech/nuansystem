using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.SriTxtImports.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class SriTxtImportRepository(ITenantConnectionFactory connectionFactory)
    : ISriTxtImportRepository
{
    public async Task<SriTxtImportPersistenceResult> RegisterValidatedAsync(
        RegisterValidatedSriTxtImportData data,
        CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("GlobalId", data.GlobalId);
        parameters.Add("OriginalFileName", data.OriginalFileName);
        parameters.Add("FileSha256", data.FileSha256, DbType.Binary, size: 32);
        parameters.Add("FileSizeBytes", data.FileSizeBytes);
        parameters.Add("EncodingCode", data.EncodingCode);
        parameters.Add("HeaderLine", data.HeaderLine);
        parameters.Add(
            "Rows",
            BuildRowsTable(data.Rows).AsTableValuedParameter("dbo.SriTxtImportRowTableType"));
        parameters.Add("TraceId", data.TraceId);
        parameters.Add("AuditUserId", data.AuditUserId);
        parameters.Add("AuditUserName", data.AuditUserName);

        using var connection = connectionFactory.CreateConnection();
        using var results = await connection.QueryMultipleAsync(
            Command("dbo.SP_NA_POST_SRITXTIMPORT_REGISTRARVALIDADO", parameters, cancellationToken));
        var control = await results.ReadSingleAsync<RegisterControlRow>();
        var detail = await results.ReadSingleAsync<SriTxtImportDetailDto>();
        return new SriTxtImportPersistenceResult(detail, control.IsCreated);
    }

    public async Task<IReadOnlyCollection<string>> GetStagedEnvironmentsAsync(
        long importId,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<string>(
            Command(
                "dbo.SP_NA_GET_SRITXTIMPORT_AMBIENTESPREPARADOS",
                new { ImportId = importId },
                cancellationToken));
        return rows.AsList();
    }

    public async Task<SriTxtImportEnqueuePersistenceResult> EnqueueAsync(
        EnqueueSriTxtImportData data,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        using var results = await connection.QueryMultipleAsync(
            Command("dbo.SP_NA_POST_SRITXTIMPORT_ENCOLAR", data, cancellationToken));
        var control = await results.ReadSingleAsync<EnqueueControlRow>();
        var code = Enum.IsDefined(typeof(SriTxtImportEnqueueCode), control.ResultCode)
            ? (SriTxtImportEnqueueCode)control.ResultCode
            : SriTxtImportEnqueueCode.NotFound;
        var detail = code == SriTxtImportEnqueueCode.Updated
            ? await results.ReadSingleAsync<SriTxtImportDetailDto>()
            : null;
        return new SriTxtImportEnqueuePersistenceResult(code, detail);
    }

    private static DataTable BuildRowsTable(IReadOnlyCollection<SriTxtParsedRow> rows)
    {
        var table = new DataTable();
        table.Columns.Add("RowGlobalId", typeof(Guid));
        table.Columns.Add("LineNumber", typeof(int));
        table.Columns.Add("RowSha256", typeof(byte[]));
        table.Columns.Add("AccessKey", typeof(string));
        table.Columns.Add("AccessKeySha256", typeof(byte[]));
        table.Columns.Add("MaskedAccessKey", typeof(string));
        table.Columns.Add("IssuerRuc", typeof(string));
        table.Columns.Add("IssuerLegalName", typeof(string));
        table.Columns.Add("DocumentTypeCode", typeof(string));
        table.Columns.Add("DocumentTypeName", typeof(string));
        table.Columns.Add("DocumentSeries", typeof(string));
        table.Columns.Add("Environment", typeof(string));
        table.Columns.Add("AuthorizationAt", typeof(DateTime));
        table.Columns.Add("EmissionDate", typeof(DateTime));
        table.Columns.Add("ReceiverIdentification", typeof(string));
        table.Columns.Add("ValueWithoutTaxes", typeof(decimal));
        table.Columns.Add("VatAmount", typeof(decimal));
        table.Columns.Add("TotalAmount", typeof(decimal));
        table.Columns.Add("ModifiedDocumentNumber", typeof(string));
        table.Columns.Add("ValidationStatus", typeof(string));
        table.Columns.Add("ValidationCode", typeof(string));
        table.Columns.Add("ValidationMessage", typeof(string));

        foreach (var row in rows)
        {
            table.Rows.Add(
                row.RowGlobalId,
                row.LineNumber,
                row.RowSha256,
                DbValue(row.AccessKey),
                DbValue(row.AccessKeySha256),
                DbValue(row.MaskedAccessKey),
                DbValue(row.IssuerRuc),
                DbValue(row.IssuerLegalName),
                DbValue(row.DocumentTypeCode),
                DbValue(row.DocumentTypeName),
                DbValue(row.DocumentSeries),
                DbValue(row.Environment),
                DbValue(row.AuthorizationAt),
                DbValue(row.EmissionDate),
                DbValue(row.ReceiverIdentification),
                DbValue(row.ValueWithoutTaxes),
                DbValue(row.VatAmount),
                DbValue(row.TotalAmount),
                DbValue(row.ModifiedDocumentNumber),
                row.ValidationStatus,
                DbValue(row.ValidationCode),
                DbValue(row.ValidationMessage));
        }

        return table;
    }

    private static object DbValue(object? value) => value ?? DBNull.Value;

    private static CommandDefinition Command(
        string name,
        object parameters,
        CancellationToken cancellationToken) =>
        new(
            name,
            parameters,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);

    private sealed class RegisterControlRow
    {
        public bool IsCreated { get; set; }
    }

    private sealed class EnqueueControlRow
    {
        public int ResultCode { get; set; }
    }
}
