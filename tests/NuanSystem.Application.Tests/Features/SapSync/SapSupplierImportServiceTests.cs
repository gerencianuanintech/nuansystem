using FluentAssertions;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Services;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapSupplierImportServiceTests
{
    [Fact]
    public void FindLocalMatch_ReturnsNew_WhenSupplierDoesNotExist()
    {
        var index = SapSupplierImportService.BuildLocalIndex([]);
        var supplier = Supplier("P001", "Proveedor Uno", "099001");

        var result = SapSupplierImportService.FindLocalMatch(supplier, index);

        result.Status.Should().Be("New");
    }

    [Fact]
    public void FindLocalMatch_MatchesBySapCardCode()
    {
        var local = LocalSupplier(10, "LOCAL001", "099001", "P001");
        var index = SapSupplierImportService.BuildLocalIndex([local]);
        var supplier = Supplier("P001", "Proveedor Uno", "099001");

        var result = SapSupplierImportService.FindLocalMatch(supplier, index);

        result.Status.Should().Be("Matched");
        result.Supplier?.Id.Should().Be(10);
    }

    [Fact]
    public void FindLocalMatch_MatchesByLocalCode()
    {
        var local = LocalSupplier(11, "P001", "099001", null);
        var index = SapSupplierImportService.BuildLocalIndex([local]);
        var supplier = Supplier("P001", "Proveedor Uno", "099001");

        var result = SapSupplierImportService.FindLocalMatch(supplier, index);

        result.Status.Should().Be("Matched");
        result.Supplier?.Id.Should().Be(11);
    }

    [Fact]
    public void FindLocalMatch_ReturnsConflict_WhenIdentificationExistsWithoutSapRelation()
    {
        var local = LocalSupplier(12, "LOCAL001", "099001", null);
        var index = SapSupplierImportService.BuildLocalIndex([local]);
        var supplier = Supplier("P001", "Proveedor Uno", "099001");

        var result = SapSupplierImportService.FindLocalMatch(supplier, index);

        result.Status.Should().Be("Conflict");
        result.Supplier?.Id.Should().Be(12);
    }

    private static SapSupplierRecord Supplier(string code, string name, string? identification)
    {
        return new SapSupplierRecord(
            code,
            name,
            identification,
            "S",
            null,
            "099999999",
            "proveedor@demo.com",
            "USD",
            true,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date);
    }

    private static BusinessPartnerDto LocalSupplier(int id, string code, string identification, string? sapCardCode)
    {
        return new BusinessPartnerDto
        {
            Id = id,
            Code = code,
            Name = "Proveedor Local",
            PartnerType = "Supplier",
            IdentificationNumber = identification,
            SapCardCode = sapCardCode,
            Email = "proveedor@demo.com",
            Phone = "099999999",
            PreferredCurrencyCode = "USD",
            IsActive = true
        };
    }
}
