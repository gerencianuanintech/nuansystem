using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.FinancialCatalogs.PriceLists.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Services;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Features.Sync;

public sealed class PriceListSyncEventApplierTests
{
    [Fact]
    public async Task Created_AppliesV2PayloadWithCurrencyGlobalId()
    {
        var repository = Substitute.For<IPriceListSyncApplyRepository>();
        var payload = Payload();
        var context = Context(payload, SyncOperation.Created);
        repository.ApplyAsync(2, context, Arg.Is<PriceListSyncPayloadV2>(value =>
                value.CurrencyGlobalId == payload.CurrencyGlobalId), SyncOperation.Created, Arg.Any<CancellationToken>())
            .Returns(new PriceListSyncApplyResult(true, false, false, 10, "Aplicada."));
        var applier = new PriceListSyncEventApplier(repository);

        var result = await applier.ApplyAsync(context);

        result.Applied.Should().BeTrue();
    }

    [Fact]
    public async Task CodeCollision_IsTerminalWithoutAdoption()
    {
        var repository = Substitute.For<IPriceListSyncApplyRepository>();
        var payload = Payload();
        var context = Context(payload, SyncOperation.Updated);
        repository.ApplyAsync(2, context, Arg.Any<PriceListSyncPayloadV2>(), SyncOperation.Updated, Arg.Any<CancellationToken>())
            .Returns(new PriceListSyncApplyResult(false, false, true, null, "Conflicto.", "SYNC_PRICELIST_CODE_CONFLICT"));
        var applier = new PriceListSyncEventApplier(repository);

        var result = await applier.ApplyAsync(context);

        result.Applied.Should().BeFalse();
        result.Terminal.Should().BeTrue();
        result.ErrorCode.Should().Be("SYNC_PRICELIST_CODE_CONFLICT");
    }

    [Fact]
    public async Task MissingCurrency_IsRetryableAndNotTerminal()
    {
        var repository = Substitute.For<IPriceListSyncApplyRepository>();
        var payload = Payload();
        var context = Context(payload, SyncOperation.Created);
        repository.ApplyAsync(2, context, Arg.Any<PriceListSyncPayloadV2>(), SyncOperation.Created, Arg.Any<CancellationToken>())
            .Returns(new PriceListSyncApplyResult(
                false,
                false,
                false,
                null,
                "Dependencia pendiente.",
                "SYNC_PRICELIST_CURRENCY_DEPENDENCY"));
        var applier = new PriceListSyncEventApplier(repository);

        var result = await applier.ApplyAsync(context);

        result.Applied.Should().BeFalse();
        result.Retryable.Should().BeTrue();
        result.Terminal.Should().BeFalse();
    }

    [Fact]
    public void Frontend_UsesDedicatedModelsCorporateLookupAndInheritedButtons()
    {
        var form = ReadSource("src", "Frontend", "NuanSystem.WinForms.Forms", "FinancialCatalogs", "PriceLists", "PriceListEditForm.Designer.cs");
        var model = ReadSource("src", "Frontend", "NuanSystem.WinForms.Services", "FinancialCatalogs", "PriceLists", "PriceListModels.cs");

        form.Should().Contain("NuanLookupEdit")
            .And.Contain("lueAppliesTo")
            .And.Contain("chkIsDefault")
            .And.Contain("Controls.SetChildIndex(btnGuardar")
            .And.NotContain("SimpleButton btnSave")
            .And.NotContain("SimpleButton btnCancel");
        model.Should().Contain("CurrencyCode")
            .And.Contain("AppliesTo")
            .And.Contain("IsDefault");
    }

    private static PriceListSyncPayloadV2 Payload() =>
        new(Guid.NewGuid(), "PL-01", "Lista principal", null, Guid.NewGuid(), "USD",
            "Both", true, true, null, null, null, DateTime.UtcNow, null);

    private static SyncEventApplyContext Context(PriceListSyncPayloadV2 payload, SyncOperation operation)
    {
        var wrapper = new
        {
            entityName = "PriceList",
            globalId = payload.GlobalId,
            code = payload.Code,
            operation = operation.ToString(),
            payload
        };
        return new(Guid.NewGuid(), 1, "PriceList", payload.GlobalId, operation.ToString(),
            JsonSerializer.Serialize(wrapper, new JsonSerializerOptions(JsonSerializerDefaults.Web)), 2, 10);
    }

    private static string ReadSource(params string[] parts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine([directory.FullName, .. parts]);
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException(Path.Combine(parts));
    }
}
