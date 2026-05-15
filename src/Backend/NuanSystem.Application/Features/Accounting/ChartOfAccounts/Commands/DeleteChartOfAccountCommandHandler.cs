using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Accounting.ChartOfAccounts.Commands;

public sealed class DeleteChartOfAccountCommandHandler(IChartOfAccountRepository accountRepository)
    : ICommandHandler<DeleteChartOfAccountCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteChartOfAccountCommand request, CancellationToken cancellationToken)
    {
        if (await accountRepository.GetByIdAsync(request.Id, cancellationToken) is null)
        {
            return Result<bool>.Failure(
                "Cuenta contable no encontrada.",
                [new ApiError("ChartOfAccountNotFound", "No existe la cuenta contable indicada.", nameof(request.Id))]);
        }

        if (await accountRepository.HasChildrenAsync(request.Id, cancellationToken))
        {
            return Result<bool>.Failure(
                "No se puede eliminar una cuenta con cuentas hijas.",
                [new ApiError("ChartOfAccountHasChildren", "Elimine o reasigne las cuentas hijas antes de continuar.", nameof(request.Id))]);
        }

        var deleted = await accountRepository.DeleteAsync(
            request.Id,
            request.AuditUserId,
            CreateChartOfAccountCommandHandler.NormalizeOptional(request.AuditUserName),
            cancellationToken);

        return deleted
            ? Result<bool>.Success(true, "Cuenta contable eliminada correctamente.")
            : Result<bool>.Failure("No se pudo eliminar la cuenta contable.");
    }
}
