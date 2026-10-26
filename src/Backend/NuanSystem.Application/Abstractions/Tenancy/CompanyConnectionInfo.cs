using System.Text.Json.Serialization;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Abstractions.Tenancy;

public sealed record CompanyConnectionInfo(
    int CompanyId,
    string CompanyCode,
    string CommercialName,
    DatabaseEngine DatabaseEngine,
    [property: JsonIgnore] string ConnectionString,
    SapIntegrationMode SapIntegrationMode,
    CompanyOperationMode OperationMode = CompanyOperationMode.Standalone,
    bool IsMaster = true,
    int? ParentCompanyId = null,
    string? BranchCode = null,
    bool SyncEnabled = false)
{
    public override string ToString() => $"CompanyConnectionInfo {{ CompanyId = {CompanyId}, CompanyCode = {CompanyCode} }}";
}
