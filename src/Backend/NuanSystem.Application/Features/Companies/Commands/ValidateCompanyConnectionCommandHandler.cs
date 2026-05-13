using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Companies.Dtos;

namespace NuanSystem.Application.Features.Companies.Commands;

public sealed class ValidateCompanyConnectionCommandHandler(ICompanyConnectionTester connectionTester)
    : ICommandHandler<ValidateCompanyConnectionCommand, CompanyConnectionTestResult>
{
    public async Task<Result<CompanyConnectionTestResult>> Handle(
        ValidateCompanyConnectionCommand request,
        CancellationToken cancellationToken)
    {
        var result = await connectionTester.TestAsync(new CompanyConnectionTestData(
            request.DatabaseEngine,
            request.Server.Trim(),
            request.Port,
            request.DatabaseName.Trim(),
            request.DatabaseUser.Trim(),
            request.DatabasePassword), cancellationToken);

        return result.Success
            ? Result<CompanyConnectionTestResult>.Success(result, "Conexion validada correctamente.")
            : Result<CompanyConnectionTestResult>.Failure(result.Message);
    }
}
