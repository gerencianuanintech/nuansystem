using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.BusinessPartners.Policies;
using NuanSystem.Shared.Sync;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.BusinessPartners.Commands;

public sealed class DeleteBusinessPartnerCommandHandler(
    IBusinessPartnerRepository repository,
    ITransactionRunner transactionRunner,
    IBusinessPartnerLocalOutboxWriter localOutboxWriter,
    ICompanyContext companyContext)
    : ICommandHandler<DeleteBusinessPartnerCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteBusinessPartnerCommand request, CancellationToken cancellationToken)
    {
        if (!TryDecodeRowVersion(request.ExpectedRowVersion, out var expectedRowVersion))
        {
            return Failure("BP_ROW_VERSION_INVALID", "ExpectedRowVersion debe ser un rowversion base64 valido.", nameof(request.ExpectedRowVersion));
        }

        var company = companyContext.CurrentCompany;
        if (BusinessPartnerWritePolicy.IsSynchronizedBranch(company))
        {
            return Failure("BP_SYNC_DELETE_NOT_SUPPORTED", "Una sucursal sincronizada no puede eliminar terceros.", nameof(request.Id));
        }

        return await transactionRunner.ExecuteInTenantTransactionAsync(
            async (connection, transaction, token) =>
            {
                var current = await repository.GetByIdAsync(request.Id, connection, transaction, token);
                if (current is null)
                {
                    return Result<bool>.Failure(
                        "Tercero comercial no encontrado.",
                        [new ApiError("BusinessPartnerNotFound", "Tercero comercial no encontrado.", nameof(request.Id))]);
                }

                if (BusinessPartnerWritePolicy.RequiresLegacyReview(current.MasterSyncStatus))
                {
                    return Failure("BP_LEGACY_REVIEW_REQUIRED", "El tercero debe salir de LegacyReview antes de eliminarse.", nameof(current.MasterSyncStatus));
                }

                var canonicalVersion = BusinessPartnerWritePolicy.IsSynchronizedCentral(company)
                    ? current.CanonicalVersion + 1
                    : current.CanonicalVersion;

                var deleted = await repository.DeleteAsync(
                    new DeleteBusinessPartnerData(
                        request.Id,
                        expectedRowVersion,
                        canonicalVersion,
                        "Accepted",
                        request.AuditUserId,
                        CreateBusinessPartnerCommandHandler.TrimOrNull(request.AuditUserName)),
                    connection, transaction, token);
                if (!deleted)
                {
                    return Failure("BP_CONCURRENCY_CONFLICT", "El tercero fue modificado por otro proceso. Recargue e intente nuevamente.", nameof(request.ExpectedRowVersion));
                }

                current.CanonicalVersion = canonicalVersion;
                current.MasterSyncStatus = "Accepted";
                current.IsActive = false;
                await localOutboxWriter.EnqueueAsync(
                    current, SyncOperation.Deleted, connection, transaction, token);
                return Result<bool>.Success(true, "Tercero comercial eliminado correctamente.");
            },
            cancellationToken);
    }

    private static bool TryDecodeRowVersion(string? value, out byte[] rowVersion)
    {
        try
        {
            rowVersion = string.IsNullOrWhiteSpace(value) ? [] : Convert.FromBase64String(value);
            return rowVersion.Length == 8;
        }
        catch (FormatException)
        {
            rowVersion = [];
            return false;
        }
    }

    private static Result<bool> Failure(string code, string message, string field) =>
        Result<bool>.Failure(
            "No fue posible eliminar el tercero comercial.",
            [new ApiError(code, message, field)]);
}
