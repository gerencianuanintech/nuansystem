using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Queries;

public sealed record TestSapServiceLayerConnectionQuery : IQuery<SapConnectionTestResult>;

public sealed class TestSapServiceLayerConnectionQueryHandler(
    ICompanyContext companyContext,
    ISapServiceLayerConnectionTester connectionTester)
    : IQueryHandler<TestSapServiceLayerConnectionQuery, SapConnectionTestResult>
{
    public async Task<Result<SapConnectionTestResult>> Handle(
        TestSapServiceLayerConnectionQuery request,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany)
        {
            return Result<SapConnectionTestResult>.Failure(
                "No hay empresa activa para probar la conexión SAP.",
                [new ApiError("COMPANY_REQUIRED", "Seleccione una empresa antes de probar la conexión SAP.", "X-Company-Code")]);
        }

        var connection = await connectionTester.TestAsync(companyContext.CurrentCompany!.CompanyId, cancellationToken);
        if (!connection.IsSuccessful)
        {
            const string message = "La conexion con SAP no esta disponible. Revise la configuracion o intente nuevamente.";
            return Result<SapConnectionTestResult>.Failure(message, FailureKind.Unavailable,
                [new ApiError("SAP_CONNECTION_UNAVAILABLE", message)]);
        }

        return Result<SapConnectionTestResult>.Success(connection);
    }
}
