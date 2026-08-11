using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Items.Dtos;
using NuanSystem.Shared.Sync;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Items.Commands;

public sealed class CreateItemCommandHandler(
    IItemRepository itemRepository,
    ITransactionRunner transactionRunner,
    IItemLocalOutboxWriter localOutboxWriter)
    : ICommandHandler<CreateItemCommand, ItemDto>
{
    public async Task<Result<ItemDto>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        var data = new CreateItemData(
            code,
            request.Name.Trim(),
            request.Description?.Trim(),
            request.ItemGroupId,
            request.ItemFamilyId,
            request.ItemType.Trim(),
            request.InventoryUnitOfMeasureId,
            request.PurchaseUnitOfMeasureId,
            request.SalesUnitOfMeasureId,
            request.IsPurchaseItem,
            request.IsSalesItem,
            request.IsInventoryItem,
            request.PurchaseTaxId,
            request.SalesTaxId,
            request.ValuationMethod.Trim(),
            request.ManagedBy.Trim(),
            request.BatchSerialManagementMethod.Trim(),
            request.PreferredVendorCode?.Trim(),
            request.VendorCatalogCode?.Trim(),
            request.BaseSalesPrice,
            request.ReferenceCost,
            request.PurchaseFactor,
            request.SalesFactor,
            request.AllowDiscount,
            request.AllowSaleWithoutStock,
            request.Remarks?.Trim(),
            request.IsActive,
            NormalizeBarcodes(request.Barcodes),
            NormalizeWarehouses(request.Warehouses),
            NormalizeMasterData(request.MasterData),
            request.AuditUserId,
            request.AuditUserName?.Trim(),
            request.GlobalId,
            request.ExternalSystem?.Trim(),
            request.ExternalCode?.Trim(),
            request.SapCode?.Trim());

        return await transactionRunner.ExecuteInTenantTransactionAsync(
            async (connection, transaction, token) =>
            {
                if (await itemRepository.ExistsByCodeAsync(code, null, connection, transaction, token))
                {
                    return Result<ItemDto>.Failure(
                        "Ya existe un articulo con el codigo indicado.",
                        [new ApiError("ItemCodeAlreadyExists", "El codigo de articulo ya existe.", nameof(request.Code))]);
                }

                var id = await itemRepository.CreateAsync(data, connection, transaction, token);
                var item = await itemRepository.GetByIdAsync(id, connection, transaction, token)
                    ?? throw new InvalidOperationException("El articulo fue creado pero no pudo consultarse.");

                await localOutboxWriter.EnqueueAsync(
                    item, SyncOperation.Created, connection, transaction, token);
                return Result<ItemDto>.Success(item, "Articulo creado correctamente.");
            },
            cancellationToken);
    }

    internal static IReadOnlyCollection<SaveItemBarcodeData> NormalizeBarcodes(IReadOnlyCollection<SaveItemBarcodeData>? barcodes)
    {
        return (barcodes ?? [])
            .Where(item => !string.IsNullOrWhiteSpace(item.Barcode))
            .Select(item => item with
            {
                Barcode = item.Barcode.Trim(),
                BarcodeType = string.IsNullOrWhiteSpace(item.BarcodeType) ? "Internal" : item.BarcodeType.Trim()
            })
            .ToArray();
    }

    internal static IReadOnlyCollection<SaveItemWarehouseData> NormalizeWarehouses(IReadOnlyCollection<SaveItemWarehouseData>? warehouses)
    {
        return (warehouses ?? [])
            .Where(item => item.WarehouseId > 0)
            .Select(item => item with { DefaultLocationCode = item.DefaultLocationCode?.Trim() })
            .ToArray();
    }

    internal static ItemMasterData? NormalizeMasterData(ItemMasterData? masterData)
    {
        if (masterData is null)
        {
            return null;
        }

        return masterData with
        {
            General = NormalizeGeneral(masterData.General),
            Units = NormalizeUnits(masterData.Units),
            Inventory = NormalizeInventory(masterData.Inventory),
            Purchasing = NormalizePurchasing(masterData.Purchasing),
            Sales = NormalizeSales(masterData.Sales),
            Costs = NormalizeCosts(masterData.Costs),
            Accounting = NormalizeAccounting(masterData.Accounting),
            Taxes = NormalizeTaxes(masterData.Taxes),
            Traceability = NormalizeTraceability(masterData.Traceability),
            Variants = NormalizeVariants(masterData.Variants),
            Sap = NormalizeSap(masterData.Sap),
            Attachments = NormalizeAttachments(masterData.Attachments),
            Remarks = NormalizeRemarks(masterData.Remarks)
        };
    }

    private static ItemGeneralData? NormalizeGeneral(ItemGeneralData? value)
    {
        return value is null ? null : value with
        {
            AlternateCode = value.AlternateCode?.Trim(),
            SupplierSku = value.SupplierSku?.Trim(),
            LongDescription = value.LongDescription?.Trim(),
            ProductType = value.ProductType?.Trim(),
            Origin = value.Origin?.Trim(),
            Line = value.Line?.Trim(),
            SubGroup = value.SubGroup?.Trim(),
            Model = value.Model?.Trim(),
            Reference = value.Reference?.Trim()
        };
    }

    private static ItemUnitsData? NormalizeUnits(ItemUnitsData? value)
    {
        return value is null ? null : value with
        {
            WeightUnit = value.WeightUnit?.Trim(),
            VolumeUnit = value.VolumeUnit?.Trim(),
            Presentations = (value.Presentations ?? [])
                .Where(item => !string.IsNullOrWhiteSpace(item.Presentation))
                .Select(item => item with
                {
                    Presentation = item.Presentation.Trim(),
                    UnitCode = item.UnitCode?.Trim(),
                    Barcode = item.Barcode?.Trim()
                })
                .ToArray(),
            Barcodes = (value.Barcodes ?? [])
                .Where(item => !string.IsNullOrWhiteSpace(item.Barcode))
                .Select(item => item with
                {
                    Barcode = item.Barcode.Trim(),
                    Scope = string.IsNullOrWhiteSpace(item.Scope) ? "General" : item.Scope.Trim(),
                    Presentation = item.Presentation?.Trim(),
                    UnitCode = item.UnitCode?.Trim()
                })
                .ToArray()
        };
    }

    private static ItemInventoryData? NormalizeInventory(ItemInventoryData? value)
    {
        return value is null ? null : value with
        {
            ValuationMethod = value.ValuationMethod?.Trim(),
            NegativeStockPolicy = value.NegativeStockPolicy?.Trim(),
            SupplyMethod = value.SupplyMethod?.Trim(),
            ReplenishmentMethod = value.ReplenishmentMethod?.Trim(),
            AbcClassification = value.AbcClassification?.Trim(),
            DefaultLocationCode = value.DefaultLocationCode?.Trim(),
            Zone = value.Zone?.Trim(),
            Condition = value.Condition?.Trim(),
            OperationNote = value.OperationNote?.Trim(),
            Warehouses = (value.Warehouses ?? [])
                .Where(item =>
                    item.WarehouseId.GetValueOrDefault() > 0 ||
                    !string.IsNullOrWhiteSpace(item.WarehouseCode) ||
                    !string.IsNullOrWhiteSpace(item.WarehouseName))
                .Select(item => item with
                {
                    WarehouseCode = item.WarehouseCode?.Trim(),
                    WarehouseName = item.WarehouseName?.Trim(),
                    DefaultLocationCode = item.DefaultLocationCode?.Trim()
                })
                .ToArray()
        };
    }

    private static ItemPurchasingData? NormalizePurchasing(ItemPurchasingData? value)
    {
        return value is null ? null : value with
        {
            MainSupplierCode = value.MainSupplierCode?.Trim(),
            AlternateSupplierCode = value.AlternateSupplierCode?.Trim(),
            PreferredPurchaseCurrency = value.PreferredPurchaseCurrency?.Trim(),
            PurchaseRetention = value.PurchaseRetention?.Trim(),
            PurchaseExpenseAccountCode = value.PurchaseExpenseAccountCode?.Trim(),
            AssignedBuyer = value.AssignedBuyer?.Trim(),
            ReturnPolicy = value.ReturnPolicy?.Trim()
        };
    }

    private static ItemSalesData? NormalizeSales(ItemSalesData? value)
    {
        return value is null ? null : value with
        {
            MainPriceList = value.MainPriceList?.Trim(),
            ExciseTax = value.ExciseTax?.Trim(),
            SuggestedRetention = value.SuggestedRetention?.Trim(),
            PreferredChannel = value.PreferredChannel?.Trim(),
            CommercialPolicy = value.CommercialPolicy?.Trim()
        };
    }

    private static ItemCostsData? NormalizeCosts(ItemCostsData? value)
    {
        return value is null ? null : value with
        {
            CostCurrency = value.CostCurrency?.Trim(),
            CostingMethod = value.CostingMethod?.Trim(),
            Components = (value.Components ?? [])
                .Where(item => !string.IsNullOrWhiteSpace(item.Concept))
                .Select(item => item with { Concept = item.Concept.Trim(), Note = item.Note?.Trim() })
                .ToArray()
        };
    }

    private static ItemAccountingData? NormalizeAccounting(ItemAccountingData? value)
    {
        return value is null ? null : value with
        {
            InventoryAccountCode = value.InventoryAccountCode?.Trim(),
            IncomeAccountCode = value.IncomeAccountCode?.Trim(),
            CostOfSalesAccountCode = value.CostOfSalesAccountCode?.Trim(),
            SalesReturnAccountCode = value.SalesReturnAccountCode?.Trim(),
            PurchaseReturnAccountCode = value.PurchaseReturnAccountCode?.Trim(),
            CostVarianceAccountCode = value.CostVarianceAccountCode?.Trim(),
            InventoryAdjustmentAccountCode = value.InventoryAdjustmentAccountCode?.Trim(),
            PurchaseExpenseAccountCode = value.PurchaseExpenseAccountCode?.Trim(),
            DefaultBranchCode = value.DefaultBranchCode?.Trim(),
            CostCenterCode = value.CostCenterCode?.Trim(),
            ProjectCode = value.ProjectCode?.Trim(),
            BusinessLineCode = value.BusinessLineCode?.Trim(),
            DepartmentCode = value.DepartmentCode?.Trim(),
            AccountingIntegrationMethod = value.AccountingIntegrationMethod?.Trim(),
            AccountingNotes = value.AccountingNotes?.Trim()
        };
    }

    private static ItemTaxesData? NormalizeTaxes(ItemTaxesData? value)
    {
        return value is null ? null : value with
        {
            FiscalItemType = value.FiscalItemType?.Trim(),
            ExciseTax = value.ExciseTax?.Trim(),
            SuggestedRetention = value.SuggestedRetention?.Trim(),
            TaxSupport = value.TaxSupport?.Trim(),
            FiscalCode = value.FiscalCode?.Trim(),
            FiscalCountry = value.FiscalCountry?.Trim(),
            TariffCode = value.TariffCode?.Trim(),
            CustomsClassification = value.CustomsClassification?.Trim(),
            TaxNote = value.TaxNote?.Trim()
        };
    }

    private static ItemTraceabilityData? NormalizeTraceability(ItemTraceabilityData? value)
    {
        return value is null ? null : value with
        {
            BatchPrefix = value.BatchPrefix?.Trim(),
            FefoFifoMethod = value.FefoFifoMethod?.Trim(),
            OperationNote = value.OperationNote?.Trim()
        };
    }

    private static ItemVariantsData? NormalizeVariants(ItemVariantsData? value)
    {
        return value is null ? null : value with
        {
            VariantType = value.VariantType?.Trim(),
            CodeMask = value.CodeMask?.Trim(),
            BaseVariant = value.BaseVariant?.Trim()
        };
    }

    private static ItemSapData? NormalizeSap(ItemSapData? value)
    {
        return value is null ? null : value with
        {
            SapCode = value.SapCode?.Trim(),
            SapItemCode = value.SapItemCode?.Trim(),
            SynchronizationStatus = value.SynchronizationStatus?.Trim(),
            SapCompany = value.SapCompany?.Trim(),
            TargetDatabase = value.TargetDatabase?.Trim(),
            LastError = value.LastError?.Trim(),
            SapGroup = value.SapGroup?.Trim(),
            SapUnitGroup = value.SapUnitGroup?.Trim(),
            SapPlanningMethod = value.SapPlanningMethod?.Trim(),
            SapSupplyMethod = value.SapSupplyMethod?.Trim(),
            SapValuationMethod = value.SapValuationMethod?.Trim(),
            FieldMappings = (value.FieldMappings ?? [])
                .Where(item => !string.IsNullOrWhiteSpace(item.SystemField) && !string.IsNullOrWhiteSpace(item.SapField))
                .Select(item => item with
                {
                    SystemField = item.SystemField.Trim(),
                    SapField = item.SapField.Trim(),
                    Description = item.Description?.Trim()
                })
                .ToArray()
        };
    }

    private static ItemAttachmentsData? NormalizeAttachments(ItemAttachmentsData? value)
    {
        return value is null ? null : value with
        {
            Files = (value.Files ?? [])
                .Where(item => !string.IsNullOrWhiteSpace(item.FileName))
                .Select(item => item with
                {
                    DocumentType = item.DocumentType.Trim(),
                    FileName = item.FileName.Trim(),
                    Description = item.Description?.Trim(),
                    Category = item.Category?.Trim(),
                    Extension = item.Extension?.Trim(),
                    Size = item.Size?.Trim(),
                    User = item.User?.Trim(),
                    Status = item.Status?.Trim(),
                    DocumentReference = item.DocumentReference?.Trim(),
                    AlternativeText = item.AlternativeText?.Trim()
                })
                .ToArray()
        };
    }

    private static ItemRemarksData? NormalizeRemarks(ItemRemarksData? value)
    {
        return value is null ? null : value with
        {
            GeneralRemarks = value.GeneralRemarks?.Trim(),
            GeneralOperationalAlert = value.GeneralOperationalAlert?.Trim(),
            PurchasingRemarks = value.PurchasingRemarks?.Trim(),
            SalesRemarks = value.SalesRemarks?.Trim(),
            InventoryRemarks = value.InventoryRemarks?.Trim(),
            LogisticsQualityRemarks = value.LogisticsQualityRemarks?.Trim(),
            GeneralVisibility = value.GeneralVisibility?.Trim(),
            GeneralPriority = value.GeneralPriority?.Trim(),
            OperationalAlerts = (value.OperationalAlerts ?? [])
                .Where(item => !string.IsNullOrWhiteSpace(item.Message))
                .Select(item => item with
                {
                    AlertType = item.AlertType.Trim(),
                    Process = item.Process.Trim(),
                    Message = item.Message.Trim(),
                    Priority = item.Priority?.Trim()
                })
                .ToArray()
        };
    }
}
