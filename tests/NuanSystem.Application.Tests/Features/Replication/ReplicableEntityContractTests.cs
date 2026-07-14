using FluentAssertions;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.Items.Dtos;
using NuanSystem.Application.Features.SecurityUsers.Dtos;
using NuanSystem.Application.Features.Settings.Dtos;

namespace NuanSystem.Application.Tests.Features.Replication;

public sealed class ReplicableEntityContractTests
{
    [Fact]
    public void BusinessPartnerDto_CanRepresentStandalonePartnerWithoutSapCode()
    {
        var globalId = Guid.NewGuid();

        var dto = new BusinessPartnerDto
        {
            Id = 25,
            GlobalId = globalId,
            Code = "CLI-001",
            Name = "Cliente standalone",
            IdentificationNumber = "0999999999001",
            ExternalSystem = null,
            ExternalCode = null,
            SapCardCode = null,
            SapEnabled = false
        };

        dto.GlobalId.Should().Be(globalId);
        dto.SapCardCode.Should().BeNull();
        dto.SapEnabled.Should().BeFalse();
    }

    [Fact]
    public void ItemDto_CanExposeGlobalIdAndOptionalExternalReferences()
    {
        var globalId = Guid.NewGuid();

        var dto = new ItemDto
        {
            Id = 10,
            GlobalId = globalId,
            Code = "ART-001",
            Name = "Articulo replicable",
            ExternalSystem = "ExternalApi",
            ExternalCode = "EXT-001",
            SapCode = null
        };

        dto.GlobalId.Should().Be(globalId);
        dto.ExternalSystem.Should().Be("ExternalApi");
        dto.ExternalCode.Should().Be("EXT-001");
        dto.SapCode.Should().BeNull();
    }

    [Fact]
    public void CompanyParameterDto_GlobalIdAndExternalReferencesRemainOptional()
    {
        var dto = new CompanyParameterDto(
            Id: 1,
            CompanyId: 1,
            Key: "Inventory.AllowNegativeStock",
            Value: "false",
            Description: "Standalone parameter",
            CreatedAt: DateTime.UtcNow,
            UpdatedAt: null);

        dto.GlobalId.Should().BeNull();
        dto.ExternalSystem.Should().BeNull();
        dto.ExternalCode.Should().BeNull();
    }

    [Fact]
    public void UserAdminDto_GlobalIdAndExternalReferencesRemainBackwardCompatible()
    {
        var dto = new UserAdminDto(
            Id: 1,
            UserName: "admin",
            Email: "admin@nuansystem.local",
            PhoneNumber: null,
            EmailConfirmed: true,
            PhoneNumberConfirmed: false,
            FirstName: "Admin",
            LastName: "Nuan",
            DisplayName: "Admin Nuan",
            IsActive: true,
            IsLocked: false,
            CanUseWeb: true,
            CanUseMobile: true,
            FailedAccessCount: 0,
            LastLoginAt: null,
            MustChangePassword: false,
            LockoutEndAt: null,
            TwoFactorEnabled: false,
            ProfileImageUrl: null,
            ProfileImage: null,
            ProfileImageContentType: null,
            ProfileImageFileName: null,
            RoleId: null,
            Roles: Array.Empty<string>(),
            Companies: Array.Empty<string>(),
            CreatedByUserId: null,
            CreatedByUserName: null,
            CreatedAt: DateTime.UtcNow,
            UpdatedByUserId: null,
            UpdatedByUserName: null,
            UpdatedAt: null,
            DeletedByUserId: null,
            DeletedByUserName: null,
            DeletedAt: null);

        dto.GlobalId.Should().BeNull();
        dto.ExternalSystem.Should().BeNull();
        dto.ExternalCode.Should().BeNull();
    }

    [Fact]
    public void BusinessPartnerReadProcedures_ProjectGlobalIdAndExternalReferences()
    {
        foreach (var scriptName in new[]
                 {
                     "024_tenant_business_partners.sql",
                     "028_tenant_business_partners_supplier_catalog_fields.sql"
                 })
        {
            var script = ReadDatabaseScript(scriptName);

            script.Should().Contain("bp.Id, bp.GlobalId, bp.Code, bp.Name, bp.ExternalSystem, bp.ExternalCode");
            script.Should().NotContain("bp.SapCode");
            script.Should().Contain("sap.SapCardCode");
        }
    }

    [Fact]
    public void ItemReadProcedures_ProjectGlobalIdExternalReferencesAndNullableSapCode()
    {
        foreach (var scriptName in new[]
                 {
                     "018_inventory_items_master.sql",
                     "021_inventory_item_families_master.sql"
                 })
        {
            var script = ReadDatabaseScript(scriptName);

            script.Should().Contain("item.Id, item.GlobalId, item.Code, item.Name, item.ExternalSystem, item.ExternalCode, item.SapCode");
            script.Should().Contain("SapCode nvarchar(100) NULL");
            script.Should().NotContain("SapCode nvarchar(100) NOT NULL");
        }
    }

    private static string ReadDatabaseScript(string scriptName)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var scriptPath = Path.Combine(directory.FullName, "database", "sql", scriptName);
            if (File.Exists(scriptPath))
            {
                return File.ReadAllText(scriptPath);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"No se encontro el script SQL {scriptName}.");
    }
}
