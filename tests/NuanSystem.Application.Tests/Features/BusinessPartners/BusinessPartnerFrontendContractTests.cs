using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using NSubstitute;
using NuanSystem.Application.Features.BusinessPartners.Commands;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.BusinessPartners.Policies;
using NuanSystem.WinForms.Services.BusinessPartners;
using NuanSystem.WinForms.Services.BusinessPartners.Models;
using NuanSystem.WinForms.Services.Sync;
using NuanSystem.WinForms.Services.Sync.Models;
using NuanSystem.WinForms.ViewModels.BusinessPartners;
using NuanSystem.WinForms.ViewModels.BusinessPartners.Suppliers;
using NuanSystem.WinForms.ViewModels.Sync;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

public sealed class BusinessPartnerFrontendContractTests
{
    [Fact]
    public void WriteContracts_DoNotAcceptCodeAndCarryConcurrencyAndChildIdentity()
    {
        PropertyNames(typeof(SaveBusinessPartnerRequest)).Should()
            .NotContain("Code")
            .And.Contain("ExpectedRowVersion");
        PropertyNames(typeof(SaveBusinessPartnerAddressRequest)).Should().Contain("GlobalId");
        PropertyNames(typeof(SaveBusinessPartnerContactRequest)).Should().Contain("GlobalId");

        PropertyNames(typeof(BusinessPartnerItem)).Should().Contain(
            "GlobalId",
            "NormalizedIdentificationNumber",
            "CanonicalVersion",
            "RowVersion",
            "MasterSyncStatus",
            "MasterSyncMessage");
        PropertyNames(typeof(BusinessPartnerAddressItem)).Should().Contain("GlobalId");
        PropertyNames(typeof(BusinessPartnerContactItem)).Should().Contain("GlobalId");
    }

    [Fact]
    public void EditPolicyAndStatusPresentation_AreCompiledFrontendContracts()
    {
        var assembly = typeof(BusinessPartnerItem).Assembly;
        var policyDto = assembly.GetType(
            "NuanSystem.WinForms.Services.BusinessPartners.Models.BusinessPartnerEditPolicyDto");
        var policy = assembly.GetType(
            "NuanSystem.WinForms.Services.BusinessPartners.Models.BusinessPartnerEditPolicy");
        var statusPolicy = assembly.GetType(
            "NuanSystem.WinForms.Services.BusinessPartners.Models.BusinessPartnerSyncPresentationPolicy");

        policyDto.Should().NotBeNull();
        policy.Should().NotBeNull();
        PropertyNames(policy!).Should().Equal("IsSyncedBranch", "CanEditManagedFields", "EditableFields");
        typeof(BusinessPartnerLookups).GetProperty("EditPolicy")!.PropertyType.Should().Be(policyDto);

        statusPolicy.Should().NotBeNull();
        var describe = statusPolicy!.GetMethod(
            "Describe",
            BindingFlags.Public | BindingFlags.Static);
        describe.Should().NotBeNull();

        var expected = new[]
        {
            (Status: "PendingMaster", Caption: "Pendiente en central", CanSave: false),
            (Status: "Accepted", Caption: "Aceptado", CanSave: true),
            (Status: "Rejected", Caption: "Rechazado", CanSave: true),
            (Status: "Conflict", Caption: "Conflicto", CanSave: false),
            (Status: "LegacyReview", Caption: "Revisión requerida", CanSave: false)
        };

        foreach (var item in expected)
        {
            var result = describe!.Invoke(null, [item.Status, "detalle"]);
            result.Should().NotBeNull();
            result!.GetType().GetProperty("Caption")!.GetValue(result).Should().Be(item.Caption);
            result.GetType().GetProperty("CanSave")!.GetValue(result).Should().Be(item.CanSave);
            result.GetType().GetProperty("Message")!.GetValue(result).Should().Be("detalle");
        }
    }

    [Fact]
    public void FormEditState_UsesApiBranchPolicyAndLifecycleStatus()
    {
        var assembly = typeof(BusinessPartnerItem).Assembly;
        var statePolicy = assembly.GetType(
            "NuanSystem.WinForms.Services.BusinessPartners.Models.BusinessPartnerFormEditStatePolicy");
        statePolicy.Should().NotBeNull();

        var partner = new BusinessPartnerItem
        {
            Id = 7,
            GlobalId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
            Code = "HIST-001",
            MasterSyncStatus = "PendingMaster"
        };
        var branchPolicy = new BusinessPartnerEditPolicy(
            true,
            false,
            ["Name", "CommercialName", "Phone", "Email", "Addresses", "Contacts"]);

        var state = statePolicy!.GetMethod("Evaluate", BindingFlags.Public | BindingFlags.Static)!
            .Invoke(null, [partner, branchPolicy])!;

        state.GetType().GetProperty("CodeText")!.GetValue(state).Should().Be("HIST-001");
        state.GetType().GetProperty("IdentificationEditable")!.GetValue(state).Should().Be(false);
        state.GetType().GetProperty("CanSave")!.GetValue(state).Should().Be(false);
        state.GetType().GetProperty("NameEditable")!.GetValue(state).Should().Be(true);
        state.GetType().GetProperty("AddressesEditable")!.GetValue(state).Should().Be(true);
        state.GetType().GetProperty("ManagedFieldsEditable")!.GetValue(state).Should().Be(false);
    }

    [Fact]
    public void FormEditState_CentralAndCreateRemainFullyEditableWithAssignedCodeHint()
    {
        var statePolicy = typeof(BusinessPartnerItem).Assembly.GetType(
            "NuanSystem.WinForms.Services.BusinessPartners.Models.BusinessPartnerFormEditStatePolicy");
        statePolicy.Should().NotBeNull();
        var centralPolicy = new BusinessPartnerEditPolicy(false, true, []);

        var state = statePolicy!.GetMethod("Evaluate", BindingFlags.Public | BindingFlags.Static)!
            .Invoke(null, [null, centralPolicy])!;

        state.GetType().GetProperty("CodeText")!.GetValue(state).Should().Be(string.Empty);
        state.GetType().GetProperty("CodeHint")!.GetValue(state).Should().Be("Se asigna al guardar");
        state.GetType().GetProperty("IdentificationEditable")!.GetValue(state).Should().Be(true);
        state.GetType().GetProperty("CanSave")!.GetValue(state).Should().Be(true);
        state.GetType().GetProperty("ManagedFieldsEditable")!.GetValue(state).Should().Be(true);
    }

    [Fact]
    public void FormEditState_ExistingRecordWithoutHistoricalCode_UsesStableGlobalFallback()
    {
        var partner = new BusinessPartnerItem
        {
            Id = 7,
            GlobalId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
            Code = string.Empty,
            MasterSyncStatus = "Accepted"
        };

        var state = BusinessPartnerFormEditStatePolicy.Evaluate(
            partner,
            new BusinessPartnerEditPolicy(false, true, []));

        state.IsCreating.Should().BeFalse();
        state.CodeText.Should().Be("BP-AAAAAAAABBBBCCCCDDDDEEEEEEEEEEEE");
        state.IdentificationEditable.Should().BeFalse();
    }

    [Fact]
    public void CopyDraft_IsADeepCreateIntentAndDoesNotMutateLoadedIdentity()
    {
        var globalId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
        var addressGlobalId = Guid.Parse("11111111-2222-3333-4444-555555555555");
        var contactGlobalId = Guid.Parse("66666666-7777-8888-9999-aaaaaaaaaaaa");
        var source = new BusinessPartnerItem
        {
            Id = 42,
            GlobalId = globalId,
            Code = "C-0042",
            Name = "ACME",
            PartnerType = "Customer",
            IdentificationNumber = "1790012345001",
            NormalizedIdentificationNumber = "1790012345001",
            CanonicalVersion = 7,
            RowVersion = "AQID",
            MasterSyncStatus = "Conflict",
            MasterSyncMessage = "Revisar",
            SapCardCode = "CN1790012345001",
            Addresses =
            [
                new(5, addressGlobalId, 42, null, null, null, "Billing", "Calle 1", null,
                    "EC", null, null, null, null, null, true, true)
            ],
            Contacts =
            [
                new(8, contactGlobalId, 42, null, null, "Ana", null, null, null, null, null,
                    "ana@example.com", null, true, true, true, null)
            ]
        };

        var copy = source.CreateCopyDraft();

        copy.Should().NotBeSameAs(source);
        copy.Id.Should().Be(0);
        copy.GlobalId.Should().BeEmpty();
        copy.Code.Should().BeEmpty();
        copy.PartnerType.Should().Be("Customer");
        copy.Name.Should().Be("ACME");
        copy.IdentificationNumber.Should().BeEmpty();
        copy.NormalizedIdentificationNumber.Should().BeEmpty();
        copy.CanonicalVersion.Should().Be(0);
        copy.RowVersion.Should().BeEmpty();
        copy.MasterSyncStatus.Should().Be("Accepted");
        copy.MasterSyncMessage.Should().BeNull();
        copy.SapCardCode.Should().BeNull();
        copy.Addresses.Should().ContainSingle(address =>
            address.Id == 0 && address.GlobalId == Guid.Empty && address.BusinessPartnerId == 0);
        copy.Contacts.Should().ContainSingle(contact =>
            contact.Id == 0 && contact.GlobalId == Guid.Empty && contact.BusinessPartnerId == 0);
        source.Id.Should().Be(42);
        source.GlobalId.Should().Be(globalId);
        source.Code.Should().Be("C-0042");
        source.Addresses.Single().GlobalId.Should().Be(addressGlobalId);
        source.Contacts.Single().GlobalId.Should().Be(contactGlobalId);

        var state = BusinessPartnerFormEditStatePolicy.Evaluate(
            copy,
            new BusinessPartnerEditPolicy(false, true, []));
        state.IsCreating.Should().BeTrue();
        state.CodeText.Should().BeEmpty();
        state.IdentificationEditable.Should().BeTrue();
    }

    [Fact]
    public void BusinessPartnerClientAndViewModel_ExposeRowVersionAwareDeleteAndAuthoritativePolicy()
    {
        typeof(IBusinessPartnerClient).GetMethod("DeleteAsync")!.GetParameters()
            .Select(parameter => parameter.Name)
            .Should().ContainInOrder("formKey", "id", "expectedRowVersion", "cancellationToken");
        typeof(BusinessPartnersViewModel).GetProperty("EditPolicy").Should().NotBeNull();
    }

    [Fact]
    public void BranchCreateProjection_UsesExactBackendApprovedDefaults()
    {
        var address = CreatePopulated<SaveBusinessPartnerAddressRequest>();
        var contact = CreatePopulated<SaveBusinessPartnerContactRequest>();
        var proposed = CreatePopulated<SaveBusinessPartnerRequest>(new Dictionary<string, object?>
        {
            ["Name"] = "Nombre editable",
            ["CommercialName"] = "Comercial editable",
            ["PartnerType"] = "Customer",
            ["IdentificationTypeId"] = 9,
            ["IdentificationNumber"] = "1790012345001",
            ["Email"] = "cliente@example.com",
            ["Phone"] = "0999999999",
            ["Addresses"] = new[] { address },
            ["Contacts"] = new[] { contact }
        });
        var branchPolicy = new BusinessPartnerEditPolicy(true, false, BusinessPartnerWritePolicy.BranchEditableFields);

        var projected = ProjectRequest(proposed, null, branchPolicy);
        var backend = JsonSerializer.Deserialize<CreateBusinessPartnerCommand>(JsonSerializer.Serialize(projected));
        var copyDraft = JsonSerializer.Deserialize<BusinessPartnerItem>(JsonSerializer.Serialize(proposed))!;
        copyDraft.Id = 0;
        var projectedCopy = ProjectRequest(proposed, copyDraft, branchPolicy);
        var backendCopy = JsonSerializer.Deserialize<CreateBusinessPartnerCommand>(JsonSerializer.Serialize(projectedCopy));

        backend.Should().NotBeNull();
        BusinessPartnerWritePolicy.GetNonDefaultProtectedPaths(backend!).Should().BeEmpty();
        BusinessPartnerWritePolicy.GetNonDefaultProtectedPaths(backendCopy!).Should().BeEmpty();
        projected.Should().BeEquivalentTo(proposed, options => options.Including(request => request.Name)
            .Including(request => request.CommercialName)
            .Including(request => request.PartnerType)
            .Including(request => request.IdentificationTypeId)
            .Including(request => request.IdentificationNumber)
            .Including(request => request.Email)
            .Including(request => request.Phone)
            .Including(request => request.Addresses)
            .Including(request => request.Contacts));
    }

    [Fact]
    public void BranchUpdateProjection_PreservesEveryProtectedAggregateField()
    {
        var original = CreatePopulated<SaveBusinessPartnerRequest>(new Dictionary<string, object?>
        {
            ["Name"] = "Nombre original",
            ["CommercialName"] = "Comercial original",
            ["PartnerType"] = "Customer",
            ["IdentificationTypeId"] = 9,
            ["IdentificationNumber"] = "1790012345001",
            ["Email"] = "original@example.com",
            ["Phone"] = "022222222",
            ["Addresses"] = new[] { CreatePopulated<SaveBusinessPartnerAddressRequest>() },
            ["Contacts"] = new[] { CreatePopulated<SaveBusinessPartnerContactRequest>() }
        });
        var current = JsonSerializer.Deserialize<BusinessPartnerItem>(JsonSerializer.Serialize(original))!;
        current.Id = 42;
        current.GlobalId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
        current.Code = "BP-0042";
        current.RowVersion = "AQIDBAUGBwg=";
        current.CanonicalVersion = 7;
        current.MasterSyncStatus = "Accepted";
        var changedAddress = CreatePopulated<SaveBusinessPartnerAddressRequest>(new Dictionary<string, object?>
        {
            ["GlobalId"] = Guid.Parse("11111111-2222-3333-4444-555555555555"),
            ["Line1"] = "Nueva direccion"
        });
        var proposed = original with
        {
            Name = "Nombre cambiado",
            Email = "nuevo@example.com",
            Website = "https://no-autorizado.example",
            BankAccounts = [],
            Addresses = [changedAddress]
        };
        var branchPolicy = new BusinessPartnerEditPolicy(true, false, BusinessPartnerWritePolicy.BranchEditableFields);

        var projected = ProjectRequest(proposed, current, branchPolicy);
        var currentDto = JsonSerializer.Deserialize<BusinessPartnerDto>(JsonSerializer.Serialize(current))!;
        var node = JsonNode.Parse(JsonSerializer.Serialize(projected))!.AsObject();
        node["Id"] = current.Id;
        var command = node.Deserialize<UpdateBusinessPartnerCommand>()!;
        var toUpdateData = typeof(UpdateBusinessPartnerCommandHandler).GetMethod(
            "ToUpdateData",
            BindingFlags.NonPublic | BindingFlags.Static)!;
        var updateData = toUpdateData.Invoke(null,
        [
            command,
            currentDto,
            Convert.FromBase64String(current.RowVersion),
            current.CanonicalVersion,
            "PendingMaster"
        ])!;
        var protectedPaths = BusinessPartnerWritePolicy.GetChangedProtectedPaths(
            currentDto,
            (UpdateBusinessPartnerData)updateData);

        protectedPaths.Should().BeEmpty();
        projected.Name.Should().Be("Nombre cambiado");
        projected.Email.Should().Be("nuevo@example.com");
        projected.Addresses.Should().ContainSingle().Which.Should().Be(changedAddress);
        projected.ExpectedRowVersion.Should().Be(current.RowVersion);
        projected.Should().BeEquivalentTo(original, options => options
            .Excluding(request => request.Name)
            .Excluding(request => request.CommercialName)
            .Excluding(request => request.Email)
            .Excluding(request => request.Phone)
            .Excluding(request => request.Addresses)
            .Excluding(request => request.Contacts)
            .Excluding(request => request.ExpectedRowVersion));
    }

    [Fact]
    public void CentralProjection_RetainsFullRequestWithoutSanitization()
    {
        var proposed = CreatePopulated<SaveBusinessPartnerRequest>();
        var centralPolicy = new BusinessPartnerEditPolicy(false, true, []);

        var projected = ProjectRequest(proposed, null, centralPolicy);

        projected.Should().BeSameAs(proposed);
    }

    [Fact]
    public void CustomerCentralUpdateComposition_OverlaysFormEditsWithoutErasingLoadedAggregate()
    {
        var loadedRequest = CreatePopulated<SaveBusinessPartnerRequest>(new Dictionary<string, object?>
        {
            ["Name"] = "Nombre cargado",
            ["CommercialName"] = "Comercial cargado",
            ["PartnerType"] = "Customer",
            ["IdentificationTypeId"] = 9,
            ["IdentificationNumber"] = "1790012345001",
            ["Addresses"] = new[] { CreatePopulated<SaveBusinessPartnerAddressRequest>() },
            ["Contacts"] = new[] { CreatePopulated<SaveBusinessPartnerContactRequest>() }
        });
        var loaded = JsonSerializer.Deserialize<BusinessPartnerItem>(JsonSerializer.Serialize(loadedRequest))!;
        loaded.Id = 42;
        loaded.RowVersion = "AQIDBAUGBwg=";
        var editedAddress = CreatePopulated<SaveBusinessPartnerAddressRequest>(new Dictionary<string, object?>
        {
            ["GlobalId"] = Guid.Parse("11111111-2222-3333-4444-555555555555"),
            ["AddressType"] = "Other",
            ["Line1"] = "Direccion editada"
        });
        var editedContact = CreatePopulated<SaveBusinessPartnerContactRequest>(new Dictionary<string, object?>
        {
            ["GlobalId"] = Guid.Parse("66666666-7777-8888-9999-aaaaaaaaaaaa"),
            ["Name"] = "Contacto editado"
        });
        var formDraft = CreatePopulated<SaveBusinessPartnerRequest>(new Dictionary<string, object?>
        {
            ["Name"] = "Nombre editado",
            ["CommercialName"] = "Comercial editado",
            ["PartnerType"] = "Customer",
            ["IdentificationTypeId"] = 77,
            ["IdentificationNumber"] = "identidad-ignorada-en-update",
            ["Email"] = "editado@example.com",
            ["Phone"] = "0999999999",
            ["Remarks"] = "Observacion editada",
            ["IsActive"] = false,
            ["TaxpayerType"] = "Sociedad editada",
            ["IsAccountingRequired"] = false,
            ["AppliesRetention"] = false,
            ["FiscalRegime"] = "Regimen editado",
            ["CountryCode"] = "PE",
            ["Province"] = "Lima",
            ["City"] = "Miraflores",
            ["CustomerAccountId"] = 501,
            ["CustomerAdvanceAccountId"] = 502,
            ["RetentionAccountId"] = 503,
            ["CostCenterCode"] = "CC-EDIT",
            ["PaymentTermId"] = 504,
            ["CreditLimit"] = 999.25m,
            ["PriceListCode"] = "PL-EDIT",
            ["AssignedSellerCode"] = "SELLER-EDIT",
            ["CreditStatus"] = "Blocked",
            ["SapCardCode"] = "C-EDIT",
            ["SapSyncStatus"] = "Synced",
            ["Addresses"] = new[] { editedAddress },
            ["Contacts"] = new[] { editedContact }
        }, populated: false);
        var expected = loadedRequest with
        {
            Name = formDraft.Name,
            CommercialName = formDraft.CommercialName,
            Email = formDraft.Email,
            Phone = formDraft.Phone,
            Remarks = formDraft.Remarks,
            IsActive = formDraft.IsActive,
            TaxpayerType = formDraft.TaxpayerType,
            IsAccountingRequired = formDraft.IsAccountingRequired,
            AppliesRetention = formDraft.AppliesRetention,
            FiscalRegime = formDraft.FiscalRegime,
            CountryCode = formDraft.CountryCode,
            Province = formDraft.Province,
            City = formDraft.City,
            CustomerAccountId = formDraft.CustomerAccountId,
            CustomerAdvanceAccountId = formDraft.CustomerAdvanceAccountId,
            RetentionAccountId = formDraft.RetentionAccountId,
            CostCenterCode = formDraft.CostCenterCode,
            PaymentTermId = formDraft.PaymentTermId,
            CreditLimit = formDraft.CreditLimit,
            PriceListCode = formDraft.PriceListCode,
            AssignedSellerCode = formDraft.AssignedSellerCode,
            CreditStatus = formDraft.CreditStatus,
            SapCardCode = formDraft.SapCardCode,
            SapSyncStatus = formDraft.SapSyncStatus,
            Addresses = formDraft.Addresses,
            Contacts = formDraft.Contacts,
            ExpectedRowVersion = loaded.RowVersion
        };

        var composed = ComposeCustomerRequest(
            formDraft,
            loaded,
            new BusinessPartnerEditPolicy(false, true, []));

        composed.Should().BeEquivalentTo(expected);
        composed.BankAccounts.Should().NotBeEmpty();
        composed.RetentionSettings.Should().NotBeEmpty();
        composed.Notes.Should().NotBeNull();
        composed.SapFieldMappings.Should().NotBeEmpty();
        composed.Attachments.Should().NotBeEmpty();
        composed.IdentificationTypeId.Should().Be(loaded.IdentificationTypeId);
        composed.IdentificationNumber.Should().Be(loaded.IdentificationNumber);
        Read("src", "Frontend", "NuanSystem.WinForms.Forms", "BusinessPartners", "CustomerEditForm.cs")
            .Should().Contain("Request = SupplierBusinessPartnerMapper.ComposeCustomerRequest(");
    }

    [Fact]
    public void CustomerBranchUpdateComposition_RetainsExactBackendWritePolicyProjection()
    {
        var original = CreatePopulated<SaveBusinessPartnerRequest>(new Dictionary<string, object?>
        {
            ["Name"] = "Nombre central",
            ["CommercialName"] = "Comercial central",
            ["PartnerType"] = "Customer",
            ["IdentificationTypeId"] = 9,
            ["IdentificationNumber"] = "1790012345001"
        });
        var current = JsonSerializer.Deserialize<BusinessPartnerItem>(JsonSerializer.Serialize(original))!;
        current.Id = 42;
        current.RowVersion = "AQIDBAUGBwg=";
        var formDraft = CreatePopulated<SaveBusinessPartnerRequest>(new Dictionary<string, object?>
        {
            ["Name"] = "Nombre sucursal",
            ["CommercialName"] = "Comercial sucursal",
            ["PartnerType"] = "Customer",
            ["IdentificationTypeId"] = 77,
            ["IdentificationNumber"] = "identidad-no-autorizada",
            ["Email"] = "sucursal@example.com",
            ["Phone"] = "0999999999",
            ["Remarks"] = "cambio no autorizado"
        }, populated: false);
        var branchPolicy = new BusinessPartnerEditPolicy(
            true,
            false,
            BusinessPartnerWritePolicy.BranchEditableFields);

        var composed = ComposeCustomerRequest(formDraft, current, branchPolicy);
        var expected = ProjectRequest(formDraft, current, branchPolicy);

        composed.Should().BeEquivalentTo(expected);
        composed.Name.Should().Be(formDraft.Name);
        composed.Email.Should().Be(formDraft.Email);
        composed.Remarks.Should().Be(current.Remarks);
        composed.IdentificationNumber.Should().Be(current.IdentificationNumber);
    }

    [Theory]
    [InlineData("Billing", false)]
    [InlineData("Shipping", false)]
    [InlineData("Main", true)]
    [InlineData("Other", true)]
    public void AddressRoundTrip_PreservesApiTypeIndependentOfPrimary(string apiType, bool isPrimary)
    {
        var partner = new BusinessPartnerItem
        {
            Addresses = [new(1, Guid.NewGuid(), 42, null, null, null, apiType, "Calle 1", null, null, null, null, null, null, null, isPrimary, true)]
        };
        var lookups = CreatePopulated<BusinessPartnerLookups>(populated: false);

        var request = SupplierBusinessPartnerMapper.ToAddressRequests(
            SupplierBusinessPartnerMapper.ToAddressViewModels(partner),
            lookups).Single();

        request.AddressType.Should().Be(apiType);
        request.IsPrimary.Should().Be(isPrimary);
    }

    [Theory]
    [InlineData("Billing", false)]
    [InlineData("Billing", true)]
    [InlineData("Shipping", false)]
    [InlineData("Shipping", true)]
    [InlineData("Main", false)]
    [InlineData("Main", true)]
    [InlineData("Other", false)]
    [InlineData("Other", true)]
    public void CustomerAddressEditResult_PreservesOriginalPrimaryForEveryApiType(
        string apiType,
        bool originalIsPrimary)
    {
        var partner = new BusinessPartnerItem
        {
            Addresses = [new(1, Guid.NewGuid(), 42, null, null, null, apiType, "Original", null, null, null, null, null, null, null, originalIsPrimary, true)]
        };
        var original = SupplierBusinessPartnerMapper.ToAddressViewModels(partner).Single();
        var dialogResult = original.Clone();
        dialogResult.MainStreet = "Editada";
        dialogResult.IsPrimary = !originalIsPrimary;

        var result = ComposeCustomerAddressEditResult(original, dialogResult);
        var request = SupplierBusinessPartnerMapper.ToAddressRequests(
            [result],
            CreatePopulated<BusinessPartnerLookups>(populated: false)).Single();

        result.IsPrimary.Should().Be(originalIsPrimary);
        result.MainStreet.Should().Be("Editada");
        request.AddressType.Should().Be(apiType);
        request.IsPrimary.Should().Be(originalIsPrimary);
        original.MainStreet.Should().Be("Original");
        original.IsPrimary.Should().Be(originalIsPrimary);
        dialogResult.IsPrimary.Should().Be(!originalIsPrimary);
    }

    [Fact]
    public void CustomerContactDetailPresentation_MapsPrimaryActiveAndNotesAndClearsSelection()
    {
        var contact = new SupplierContactViewModel
        {
            FirstName = "Ana",
            LastName = "Ruiz",
            Position = "Gerente",
            Phone = "2200000",
            Mobile = "0999999999",
            Email = "ana@example.com",
            IsPrimary = true,
            IsActive = false,
            Notes = "Preferente"
        };

        var populated = CustomerContactDetail(contact);
        var empty = CustomerContactDetail(null);

        populated.GetType().GetProperty("Name")!.GetValue(populated).Should().Be("Ana Ruiz");
        populated.GetType().GetProperty("Position")!.GetValue(populated).Should().Be("Gerente");
        populated.GetType().GetProperty("Phone")!.GetValue(populated).Should().Be("2200000");
        populated.GetType().GetProperty("Mobile")!.GetValue(populated).Should().Be("0999999999");
        populated.GetType().GetProperty("Email")!.GetValue(populated).Should().Be("ana@example.com");
        populated.GetType().GetProperty("IsPrimary")!.GetValue(populated).Should().Be(true);
        populated.GetType().GetProperty("IsActive")!.GetValue(populated).Should().Be(false);
        populated.GetType().GetProperty("Notes")!.GetValue(populated).Should().Be("Preferente");
        empty.GetType().GetProperty("Name")!.GetValue(empty).Should().Be(string.Empty);
        empty.GetType().GetProperty("Position")!.GetValue(empty).Should().Be(string.Empty);
        empty.GetType().GetProperty("Phone")!.GetValue(empty).Should().Be(string.Empty);
        empty.GetType().GetProperty("Mobile")!.GetValue(empty).Should().Be(string.Empty);
        empty.GetType().GetProperty("Email")!.GetValue(empty).Should().Be(string.Empty);
        empty.GetType().GetProperty("IsPrimary")!.GetValue(empty).Should().Be(false);
        empty.GetType().GetProperty("IsActive")!.GetValue(empty).Should().Be(false);
        empty.GetType().GetProperty("Notes")!.GetValue(empty).Should().Be(string.Empty);
    }

    [Fact]
    public void CustomerChildRoundTrip_PreservesGlobalIdsAndAllCanonicalFields()
    {
        var addressGlobalId = Guid.Parse("11111111-2222-3333-4444-555555555555");
        var contactGlobalId = Guid.Parse("66666666-7777-8888-9999-aaaaaaaaaaaa");
        var partner = new BusinessPartnerItem
        {
            Addresses = [new(1, addressGlobalId, 42, 10, 20, 30, "Billing", "Calle Uno 12", "Piso 3", "EC", "Pichincha", "Quito", "170101", -0.1m, -78.5m, true, false)],
            Contacts = [new(2, contactGlobalId, 42, 40, 50, "Ana Ruiz", "Gerente", "Ventas", "2200000", "101", "0999999999", "ana@example.com", "es-EC", false, true, true, "Preferente")]
        };
        var lookups = CreatePopulated<BusinessPartnerLookups>(new Dictionary<string, object?>
        {
            ["Countries"] = new[] { new BusinessPartnerLookupOption(10, "EC", "Ecuador") },
            ["Provinces"] = new[] { new BusinessPartnerGeoLookupOption(20, "P", "Pichincha") },
            ["Cities"] = new[] { new BusinessPartnerGeoLookupOption(30, "Q", "Quito") },
            ["ContactTypes"] = new[] { new BusinessPartnerLookupOption(40, "ADM", "Administrativo") },
            ["ContactChannels"] = new[] { new BusinessPartnerLookupOption(50, "MAIL", "Correo") }
        }, populated: false);

        var addresses = SupplierBusinessPartnerMapper.ToAddressRequests(
            SupplierBusinessPartnerMapper.ToAddressViewModels(partner), lookups);
        var contacts = SupplierBusinessPartnerMapper.ToContactRequests(
            SupplierBusinessPartnerMapper.ToContactViewModels(partner, lookups));

        addresses.Should().ContainSingle().Which.Should().BeEquivalentTo(partner.Addresses.Single(),
            options => options.ExcludingMissingMembers());
        contacts.Should().ContainSingle().Which.Should().BeEquivalentTo(partner.Contacts.Single(),
            options => options.ExcludingMissingMembers());
    }

    [Fact]
    public void BranchToMasterProfile_RoundTripsWithCentralReview()
    {
        var catalog = new SyncConfigurationCatalog
        {
            Directions = [new("MasterToBranch", "M2B"), new("BranchToMaster", "B2M")],
            ConflictStrategies = [new("MasterWins", "Master"), new("CentralReview", "Review")]
        };
        var detail = new SyncProfileDetail
        {
            Id = 12,
            Code = "B2M",
            Name = "Sucursales",
            Direction = "BranchToMaster",
            ConflictStrategy = "CentralReview"
        };

        var loaded = SyncProfileEditorState.FromDetail(detail, catalog).ToRequest();
        var forced = new SyncProfileEditorState
        {
            Direction = "BranchToMaster",
            ConflictStrategy = "MasterWins"
        }.ToRequest();
        var created = SyncProfileEditorState.CreateNew(catalog with
        {
            Directions = [new("BranchToMaster", "B2M")]
        });

        loaded.Direction.Should().Be("BranchToMaster");
        loaded.ConflictStrategy.Should().Be("CentralReview");
        forced.ConflictStrategy.Should().Be("CentralReview");
        created.Direction.Should().Be("BranchToMaster");
        created.ConflictStrategy.Should().Be("CentralReview");
    }

    [Fact]
    public void MonitorContracts_ExposeSafeConflictDifferencesAndResolutionLifecycle()
    {
        var conflict = typeof(SyncDashboard).Assembly.GetType(
            "NuanSystem.WinForms.Services.Sync.Models.BusinessPartnerSyncConflict");
        var difference = typeof(SyncDashboard).Assembly.GetType(
            "NuanSystem.WinForms.Services.Sync.Models.BusinessPartnerSyncConflictDifference");
        var request = typeof(SyncDashboard).Assembly.GetType(
            "NuanSystem.WinForms.Services.Sync.Models.ResolveBusinessPartnerSyncConflictRequest");

        conflict.Should().NotBeNull();
        difference.Should().NotBeNull();
        request.Should().NotBeNull();
        PropertyNames(conflict!).Should().Contain(
            "BusinessPartnerGlobalId", "OriginCompanyId", "BaseCanonicalVersion",
            "CurrentCanonicalVersion", "Differences", "Status", "CreatedAt", "RowVersion");
        PropertyNames(difference!).Should().Equal("FieldPath", "BaseValue", "ProposedValue", "CentralValue");
        PropertyNames(request!).Should().Contain("ConflictId", "Resolution", "Reason", "ExpectedRowVersion");

        typeof(ISyncMonitorClient).GetMethod("GetBusinessPartnerConflictsAsync").Should().NotBeNull();
        typeof(ISyncMonitorClient).GetMethod("ResolveBusinessPartnerConflictAsync").Should().NotBeNull();
        typeof(SyncMonitorViewModel).GetProperty("BusinessPartnerConflicts").Should().NotBeNull();
        typeof(SyncMonitorViewModel).GetMethod("ResolveBusinessPartnerConflictAsync").Should().NotBeNull();
    }

    [Fact]
    public async Task MonitorResolution_UsesSelectedRowVersionAndRefreshesOpenConflicts()
    {
        var client = Substitute.For<ISyncMonitorClient>();
        var conflict = new BusinessPartnerSyncConflict(
            17,
            Guid.Parse("11111111-2222-3333-4444-555555555555"),
            3,
            Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
            9,
            "BP-1",
            "Acme",
            1,
            2,
            [new("Name", "Old", "Branch", "Central")],
            "Open",
            null,
            null,
            1,
            "origin",
            new DateTime(2026, 9, 3, 10, 0, 0, DateTimeKind.Utc),
            null,
            null,
            null,
            "AQID");
        var loads = 0;
        client.GetBusinessPartnerConflictsAsync("Open", Arg.Any<CancellationToken>())
            .Returns(_ => loads++ == 0 ? [conflict] : []);
        ResolveBusinessPartnerSyncConflictRequest? sent = null;
        client.ResolveBusinessPartnerConflictAsync(
                17,
                Arg.Any<ResolveBusinessPartnerSyncConflictRequest>(),
                Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                sent = call.ArgAt<ResolveBusinessPartnerSyncConflictRequest>(1);
                return conflict with { Status = "Resolved", Resolution = "KeepCentral" };
            });
        var viewModel = new SyncMonitorViewModel(client);

        await viewModel.LoadBusinessPartnerConflictsAsync();
        await viewModel.ResolveBusinessPartnerConflictAsync(17, "KeepCentral", "  revisado  ");

        sent.Should().Be(new ResolveBusinessPartnerSyncConflictRequest(
            17,
            "KeepCentral",
            "revisado",
            "AQID"));
        viewModel.BusinessPartnerConflicts.Should().BeEmpty();
    }

    [Fact]
    public void ProfileContracts_ExposeDirectedOptionsAndIndependentSapCodePolicy()
    {
        var assembly = typeof(SyncConfigurationCatalog).Assembly;
        assembly.GetType("NuanSystem.WinForms.Services.Sync.Models.BusinessPartnerSapCodePolicy")
            .Should().NotBeNull();
        assembly.GetType("NuanSystem.WinForms.Services.Sync.Models.SaveBusinessPartnerSapCodePolicyRequest")
            .Should().NotBeNull();
        typeof(ISyncConfigurationClient).GetMethod("GetBusinessPartnerSapCodePolicyAsync").Should().NotBeNull();
        typeof(ISyncConfigurationClient).GetMethod("UpdateBusinessPartnerSapCodePolicyAsync").Should().NotBeNull();
        typeof(SyncProfileEditViewModel).GetProperty("BusinessPartnerSapCodePolicy").Should().NotBeNull();
        typeof(SyncProfileEditViewModel).GetMethod("SaveBusinessPartnerSapCodePolicyAsync").Should().NotBeNull();

        var directionPolicy = typeof(SyncProfileEditViewModel).Assembly.GetType(
            "NuanSystem.WinForms.ViewModels.Sync.SyncProfileDirectionPolicy");
        directionPolicy.Should().NotBeNull();
        var options = directionPolicy!
            .GetMethod("Build", BindingFlags.Public | BindingFlags.Static)!
            .Invoke(null,
            [
                new[]
                {
                    new LookupItem("MasterToBranch", "Master a sucursal"),
                    new LookupItem("BranchToMaster", "Sucursal a master"),
                    new LookupItem("Bidirectional", "Bidireccional")
                }
            ]) as System.Collections.IEnumerable;
        var rendered = options!.Cast<object>()
            .Select(option => (
                Code: (string)option.GetType().GetProperty("Code")!.GetValue(option)!,
                Label: (string)option.GetType().GetProperty("Label")!.GetValue(option)!))
            .ToArray();

        rendered.Should().Equal(
            ("MasterToBranch", "Central origen → sucursales destino"),
            ("BranchToMaster", "Sucursales origen → central destino"));
    }

    [Fact]
    public void Designers_DeclareManagedLifecycleConflictAndPrefixPolicyControlsExplicitly()
    {
        var customer = Read("src", "Frontend", "NuanSystem.WinForms.Forms", "BusinessPartners", "CustomerEditForm.Designer.cs");
        var supplier = Read("src", "Frontend", "NuanSystem.WinForms.Forms", "BusinessPartners", "SupplierEditForm.Designer.cs");
        var monitor = Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "SyncMonitorForm.Designer.cs");
        var profile = Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Sync", "Configuration", "SyncProfileEditForm.Designer.cs");

        customer.Should().Contain("txtCustomerCode.Properties.ReadOnly = true;")
            .And.Contain("Se asigna al guardar")
            .And.Contain("lblMasterSyncStatus")
            .And.Contain("lblMasterSyncMessage")
            .And.Contain("txtContactName.Properties.ReadOnly = true;")
            .And.Contain("tsPrimaryContact.Properties.ReadOnly = true;")
            .And.Contain("tsActiveContact.Properties.ReadOnly = true;")
            .And.Contain("memContactNotes.Properties.ReadOnly = true;");
        supplier.Should().Contain("txtSupplierCode.Properties.ReadOnly = true;")
            .And.Contain("Se asigna al guardar")
            .And.Contain("lblMasterSyncStatus")
            .And.Contain("lblMasterSyncMessage");

        monitor.Should().Contain("XtraTabControl tabMonitor")
            .And.Contain("NuanDataGridControl conflictGrid")
            .And.Contain("btnAcceptBranch")
            .And.Contain("btnKeepCentral")
            .And.Contain("((System.ComponentModel.ISupportInitialize)tabMonitor).BeginInit();");

        profile.Should().Contain("ComboBoxEdit cboDirection")
            .And.NotContain("txtDirection")
            .And.Contain("pnlBusinessPartnerCodePolicy")
            .And.Contain("cboSapPrefixMode")
            .And.Contain("txtPassportIdentificationTypeCode")
            .And.Contain("swSapCodePolicyEnabled")
            .And.Contain("((System.ComponentModel.ISupportInitialize)pnlBusinessPartnerCodePolicy).BeginInit();");

    }

    private static string[] PropertyNames(Type type) =>
        type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(property => property.Name)
            .ToArray();

    private static SaveBusinessPartnerRequest ProjectRequest(
        SaveBusinessPartnerRequest proposed,
        BusinessPartnerItem? current,
        BusinessPartnerEditPolicy policy)
    {
        var method = typeof(SupplierBusinessPartnerMapper).GetMethod(
            "ProjectRequest",
            BindingFlags.Public | BindingFlags.Static);
        method.Should().NotBeNull("forms need one shared projection that follows backend write policy");
        return (SaveBusinessPartnerRequest)method!.Invoke(null, [proposed, current, policy])!;
    }

    private static SaveBusinessPartnerRequest ComposeCustomerRequest(
        SaveBusinessPartnerRequest formDraft,
        BusinessPartnerItem? current,
        BusinessPartnerEditPolicy policy)
    {
        var method = typeof(SupplierBusinessPartnerMapper).GetMethod(
            "ComposeCustomerRequest",
            BindingFlags.Public | BindingFlags.Static);
        method.Should().NotBeNull("CustomerEditForm needs a lossless production composition seam");
        return (SaveBusinessPartnerRequest)method!.Invoke(null, [formDraft, current, policy])!;
    }

    private static object CustomerContactDetail(SupplierContactViewModel? contact)
    {
        var method = typeof(SupplierBusinessPartnerMapper).GetMethod(
            "ToCustomerContactDetail",
            BindingFlags.Public | BindingFlags.Static);
        method.Should().NotBeNull("the selected Customer contact detail must map every displayed value");
        return method!.Invoke(null, [contact])!;
    }

    private static SupplierAddressViewModel ComposeCustomerAddressEditResult(
        SupplierAddressViewModel original,
        SupplierAddressViewModel dialogResult)
    {
        var method = typeof(SupplierBusinessPartnerMapper).GetMethod(
            "ComposeCustomerAddressEditResult",
            BindingFlags.Public | BindingFlags.Static);
        method.Should().NotBeNull("Customer address editing needs a production seam that preserves explicit primary selection");
        return (SupplierAddressViewModel)method!.Invoke(null, [original, dialogResult])!;
    }

    private static T CreatePopulated<T>(
        IReadOnlyDictionary<string, object?>? overrides = null,
        bool populated = true)
    {
        var constructor = typeof(T).GetConstructors()
            .OrderByDescending(candidate => candidate.GetParameters().Length)
            .First();
        var values = constructor.GetParameters()
            .Select(parameter => overrides?.TryGetValue(parameter.Name!, out var value) == true
                ? value
                : SampleValue(parameter.ParameterType, populated))
            .ToArray();
        return (T)constructor.Invoke(values);
    }

    private static object? SampleValue(Type type, bool populated)
    {
        var nullable = Nullable.GetUnderlyingType(type);
        if (nullable is not null)
        {
            return populated ? SampleValue(nullable, true) : null;
        }

        if (type == typeof(string)) return populated ? "VALOR" : string.Empty;
        if (type == typeof(bool)) return populated;
        if (type == typeof(int)) return populated ? 7 : 0;
        if (type == typeof(long)) return populated ? 7L : 0L;
        if (type == typeof(decimal)) return populated ? 7.5m : 0m;
        if (type == typeof(DateTime)) return populated ? new DateTime(2026, 9, 4) : default(DateTime);
        if (type == typeof(Guid)) return populated ? Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee") : Guid.Empty;
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IReadOnlyCollection<>))
        {
            var itemType = type.GetGenericArguments()[0];
            var array = Array.CreateInstance(itemType, populated ? 1 : 0);
            if (populated)
            {
                var method = typeof(BusinessPartnerFrontendContractTests)
                    .GetMethod(nameof(CreatePopulated), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(itemType);
                array.SetValue(method.Invoke(null, [null, true]), 0);
            }

            return array;
        }

        if (!type.IsValueType)
        {
            var method = typeof(BusinessPartnerFrontendContractTests)
                .GetMethod(nameof(CreatePopulated), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(type);
            return method.Invoke(null, [null, populated]);
        }

        return Activator.CreateInstance(type);
    }

    private static string Read(params string[] segments)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine([directory.FullName, .. segments]);
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException(string.Join('/', segments));
    }
}
