using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Common.Exceptions;

namespace NuanSystem.SapIntegration.ServiceLayer;

public sealed class SapServiceLayerConnectionTester(SapServiceLayerQueryClient queryClient)
    : ISapServiceLayerConnectionTester
{
    public async Task<SapConnectionTestResult> TestAsync(int companyId, CancellationToken cancellationToken = default)
    {
        try
        {
            await queryClient.TestConnectionAsync(companyId, cancellationToken);
            return new(true, "Conexión SAP realizada correctamente.");
        }
        catch (SapServiceLayerException exception)
        {
            return new(false, exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            return new(false, exception.Message);
        }
        catch (HttpRequestException)
        {
            return new(false, "No fue posible conectar con SAP Service Layer.");
        }
    }
}
