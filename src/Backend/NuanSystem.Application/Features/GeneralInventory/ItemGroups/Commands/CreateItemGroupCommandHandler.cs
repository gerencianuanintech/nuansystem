using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.GeneralInventory.ItemGroups.Commands;

public sealed class CreateItemGroupCommandHandler(
    IItemGroupRepository itemGroupRepository,
    IChartOfAccountRepository chartOfAccountRepository,
    ITransactionRunner transactionRunner,
    IItemGroupLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<CreateItemGroupCommand, ItemGroupDto>
{
    public async Task<Result<ItemGroupDto>> Handle(CreateItemGroupCommand request, CancellationToken cancellationToken)
    {
        var code = NormalizeCode(request.Code);
        var accountValidation = await ValidateAccountCodesAsync(
            chartOfAccountRepository,
            [
                new AccountCodeField(nameof(request.InventoryAccountCode), NormalizeOptional(request.InventoryAccountCode), "cuenta de inventario"),
                new AccountCodeField(nameof(request.CostOfSalesAccountCode), NormalizeOptional(request.CostOfSalesAccountCode), "cuenta de costo de ventas"),
                new AccountCodeField(nameof(request.SalesAccountCode), NormalizeOptional(request.SalesAccountCode), "cuenta de ventas"),
                new AccountCodeField(nameof(request.PurchaseAccountCode), NormalizeOptional(request.PurchaseAccountCode), "cuenta de compras")
            ],
            cancellationToken);

        if (!accountValidation.IsSuccess)
        {
            return Result<ItemGroupDto>.Failure(accountValidation.Message, accountValidation.Errors);
        }

        var data = new CreateItemGroupData(
            Guid.NewGuid(),
            code,
            request.Name.Trim(),
            NormalizeOptional(request.Description),
            request.IsActive,
            NormalizeOptional(request.InventoryAccountCode),
            NormalizeOptional(request.CostOfSalesAccountCode),
            NormalizeOptional(request.SalesAccountCode),
            NormalizeOptional(request.PurchaseAccountCode),
            NormalizeOptional(request.SapGroupCode),
            NormalizeOptional(request.SapCode),
            request.AuditUserId,
            NormalizeOptional(request.AuditUserName));

        return await transactionRunner.ExecuteInTenantTransactionAsync(
            async (connection, transaction, token) =>
            {
                if (await itemGroupRepository.ExistsByCodeAsync(
                        code, null, connection, transaction, token))
                {
                    return Result<ItemGroupDto>.Failure(
                        "Ya existe un grupo de articulos con el codigo indicado.",
                        [new ApiError("ItemGroupCodeAlreadyExists", "El codigo de grupo ya existe.", nameof(request.Code))]);
                }

                var id = await itemGroupRepository.CreateAsync(data, connection, transaction, token);
                var itemGroup = await itemGroupRepository.GetByIdAsync(id, connection, transaction, token)
                    ?? throw new InvalidOperationException("El grupo de articulos fue creado pero no pudo consultarse.");

                await localOutboxWriter.EnqueueAsync(
                    itemGroup, SyncOperation.Created, connection, transaction, token);
                return Result<ItemGroupDto>.Success(itemGroup, "Grupo de articulos creado correctamente.");
            },
            cancellationToken);
    }

    internal static string NormalizeCode(string code)
    {
        return code.Trim().ToUpperInvariant();
    }

    internal static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    internal static async Task<AccountCodeValidationResult> ValidateAccountCodesAsync(
        IChartOfAccountRepository chartOfAccountRepository,
        IReadOnlyCollection<AccountCodeField> accountFields,
        CancellationToken cancellationToken)
    {
        var requestedCodes = accountFields
            .Where(field => !string.IsNullOrWhiteSpace(field.Code))
            .ToArray();

        if (requestedCodes.Length == 0)
        {
            return AccountCodeValidationResult.Success();
        }

        var availableAccounts = await chartOfAccountRepository.GetLookupAsync(cancellationToken);
        var activeCodes = availableAccounts
            .Where(account => account.IsActive)
            .Select(account => account.Code)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var errors = requestedCodes
            .Where(field => !activeCodes.Contains(field.Code!))
            .Select(field => new ApiError(
                "ChartOfAccountNotFound",
                $"La {field.DisplayName} '{field.Code}' no existe en el plan de cuentas o no esta activa.",
                field.FieldName))
            .ToArray();

        return errors.Length == 0
            ? AccountCodeValidationResult.Success()
            : AccountCodeValidationResult.Failure(
                "Revise las cuentas contables del grupo de articulos. Deben existir en el plan de cuentas y estar activas.",
                errors);
    }

    internal sealed record AccountCodeField(string FieldName, string? Code, string DisplayName);

    internal sealed record AccountCodeValidationResult(bool IsSuccess, string Message, IReadOnlyCollection<ApiError> Errors)
    {
        public static AccountCodeValidationResult Success()
        {
            return new AccountCodeValidationResult(true, string.Empty, Array.Empty<ApiError>());
        }

        public static AccountCodeValidationResult Failure(string message, IReadOnlyCollection<ApiError> errors)
        {
            return new AccountCodeValidationResult(false, message, errors);
        }
    }
}
