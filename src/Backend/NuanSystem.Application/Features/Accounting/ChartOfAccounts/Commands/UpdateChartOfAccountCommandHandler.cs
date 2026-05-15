using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Accounting.ChartOfAccounts.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Accounting.ChartOfAccounts.Commands;

public sealed class UpdateChartOfAccountCommandHandler(IChartOfAccountRepository accountRepository)
    : ICommandHandler<UpdateChartOfAccountCommand, ChartOfAccountDto>
{
    public async Task<Result<ChartOfAccountDto>> Handle(UpdateChartOfAccountCommand request, CancellationToken cancellationToken)
    {
        var existing = await accountRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
        {
            return Result<ChartOfAccountDto>.Failure(
                "Cuenta contable no encontrada.",
                [new ApiError("ChartOfAccountNotFound", "No existe la cuenta contable indicada.", nameof(request.Id))]);
        }

        var code = CreateChartOfAccountCommandHandler.NormalizeCode(request.Code);
        if (await accountRepository.ExistsByCodeAsync(request.CompanyId, code, request.Id, cancellationToken))
        {
            return Result<ChartOfAccountDto>.Failure(
                "Ya existe otra cuenta contable con el codigo indicado.",
                [new ApiError("ChartOfAccountCodeAlreadyExists", "El codigo de cuenta ya existe.", nameof(request.Code))]);
        }

        var parentValidator = new CreateChartOfAccountCommandHandler(accountRepository);
        if (await parentValidator.ValidateParentAsync(request.CompanyId, request.ParentAccountId, request.AccountType, request.Id, cancellationToken) is { } error)
        {
            return error;
        }

        if (request.AllowsMovement && await accountRepository.HasChildrenAsync(request.Id, cancellationToken))
        {
            return Result<ChartOfAccountDto>.Failure(
                "Una cuenta con cuentas hijas no puede permitir movimientos.",
                [new ApiError("ChartOfAccountWithChildrenCannotMove", "Desactive movimientos para cuentas padre.", nameof(request.AllowsMovement))]);
        }

        var updated = await accountRepository.UpdateAsync(new UpdateChartOfAccountData(
            request.Id,
            request.CompanyId,
            code,
            request.Name.Trim(),
            CreateChartOfAccountCommandHandler.NormalizeOptional(request.Description),
            CreateChartOfAccountCommandHandler.NormalizeOptional(request.ExternalCode),
            CreateChartOfAccountCommandHandler.NormalizeAccountType(request.AccountType),
            CreateChartOfAccountCommandHandler.NormalizeOptionalUpper(request.AccountClass),
            request.ParentAccountId,
            request.IsTitle,
            request.IsTitle ? false : request.AllowsMovement,
            request.IsActive,
            CreateChartOfAccountCommandHandler.NormalizeCurrency(request.CurrencyCode),
            request.Balance,
            request.IsConfidential,
            request.IsMonetaryAccount,
            request.IsAssociatedAccount,
            request.RevalueByIndex,
            request.BlockManualPosting,
            request.RelevantForCashFlow,
            request.RequiresCostCenter,
            request.RequiresThirdParty,
            request.RequiresProject,
            request.AuditUserId,
            CreateChartOfAccountCommandHandler.NormalizeOptional(request.AuditUserName)), cancellationToken);

        if (!updated)
        {
            return Result<ChartOfAccountDto>.Failure("No se pudo actualizar la cuenta contable.");
        }

        var account = await accountRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("La cuenta contable fue actualizada pero no pudo consultarse.");

        return Result<ChartOfAccountDto>.Success(account, "Cuenta contable actualizada correctamente.");
    }
}
