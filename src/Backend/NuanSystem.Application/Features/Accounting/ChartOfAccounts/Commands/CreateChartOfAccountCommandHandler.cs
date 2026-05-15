using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Accounting.ChartOfAccounts.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Accounting.ChartOfAccounts.Commands;

public sealed class CreateChartOfAccountCommandHandler(IChartOfAccountRepository accountRepository)
    : ICommandHandler<CreateChartOfAccountCommand, ChartOfAccountDto>
{
    public async Task<Result<ChartOfAccountDto>> Handle(CreateChartOfAccountCommand request, CancellationToken cancellationToken)
    {
        var code = NormalizeCode(request.Code);
        if (await accountRepository.ExistsByCodeAsync(request.CompanyId, code, cancellationToken))
        {
            return Result<ChartOfAccountDto>.Failure(
                "Ya existe una cuenta contable con el codigo indicado.",
                [new ApiError("ChartOfAccountCodeAlreadyExists", "El codigo de cuenta ya existe.", nameof(request.Code))]);
        }

        if (await ValidateParentAsync(request.CompanyId, request.ParentAccountId, request.AccountType, null, cancellationToken) is { } error)
        {
            return error;
        }

        var id = await accountRepository.CreateAsync(new CreateChartOfAccountData(
            request.CompanyId,
            code,
            request.Name.Trim(),
            NormalizeOptional(request.Description),
            NormalizeOptional(request.ExternalCode),
            NormalizeAccountType(request.AccountType),
            NormalizeOptionalUpper(request.AccountClass),
            request.ParentAccountId,
            request.IsTitle,
            request.IsTitle ? false : request.AllowsMovement,
            request.IsActive,
            NormalizeCurrency(request.CurrencyCode),
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
            NormalizeOptional(request.AuditUserName)), cancellationToken);

        var account = await accountRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("La cuenta contable fue creada pero no pudo consultarse.");

        return Result<ChartOfAccountDto>.Success(account, "Cuenta contable creada correctamente.");
    }

    internal async Task<Result<ChartOfAccountDto>?> ValidateParentAsync(
        int companyId,
        int? parentAccountId,
        string accountType,
        int? currentId,
        CancellationToken cancellationToken)
    {
        if (!parentAccountId.HasValue)
        {
            return null;
        }

        if (currentId.HasValue && parentAccountId.Value == currentId.Value)
        {
            return Result<ChartOfAccountDto>.Failure(
                "La cuenta no puede ser padre de si misma.",
                [new ApiError("ChartOfAccountParentSelfReference", "Seleccione una cuenta padre diferente.", nameof(parentAccountId))]);
        }

        var parent = await accountRepository.GetByIdAsync(parentAccountId.Value, cancellationToken);
        if (parent is null || parent.CompanyId != companyId)
        {
            return Result<ChartOfAccountDto>.Failure(
                "La cuenta padre no existe.",
                [new ApiError("ChartOfAccountParentNotFound", "Seleccione una cuenta padre valida.", nameof(parentAccountId))]);
        }

        if (!string.Equals(parent.AccountType, NormalizeAccountType(accountType), StringComparison.OrdinalIgnoreCase))
        {
            return Result<ChartOfAccountDto>.Failure(
                "La cuenta padre debe tener el mismo tipo de cuenta.",
                [new ApiError("ChartOfAccountParentTypeMismatch", "Seleccione una cuenta padre del mismo tipo.", nameof(accountType))]);
        }

        return null;
    }

    internal static string NormalizeCode(string code)
    {
        return code.Trim().ToUpperInvariant();
    }

    internal static string NormalizeAccountType(string accountType)
    {
        return accountType.Trim().ToUpperInvariant();
    }

    internal static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    internal static string? NormalizeOptionalUpper(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }

    internal static string? NormalizeCurrency(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }
}
