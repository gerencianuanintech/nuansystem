namespace NuanSystem.Application.Abstractions.Sap;

public interface ISapServiceLayerConnectionTester
{
    Task<SapConnectionTestResult> TestAsync(int companyId, CancellationToken cancellationToken = default);
}

public sealed record SapConnectionTestResult(bool IsSuccessful, string Message);
