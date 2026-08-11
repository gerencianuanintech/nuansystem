using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.InventoryItems;

public sealed class ItemOperationalKpiContractTests
{
    [Fact]
    public void OperationalKpi_IsAnIndependentCorporateControl()
    {
        var standardControl = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Controls",
            "Kpi", "NuanKpiCardControl.cs");
        var operationalControl = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Controls",
            "Kpi", "NuanOperationalKpiCardControl.cs");

        operationalControl.Should().Contain("public sealed class NuanOperationalKpiCardControl");
        operationalControl.Should().Contain("public string UnitText");
        operationalControl.Should().Contain("public string StatusText");
        operationalControl.Should().Contain("private void DrawContent");
        operationalControl.Should().Contain("var medium = !compact");
        operationalControl.Should().Contain("medium ? 16F : 25F");
        operationalControl.Should().Contain("Alignment = IsNumericValue(valueText) ? StringAlignment.Far : StringAlignment.Near");
        operationalControl.Should().Contain("var displayText = string.IsNullOrWhiteSpace(unitText)");
        operationalControl.Should().Contain("graphics.DrawString(displayText, valueFont, valueBrush, bounds, valueFormat)");
        operationalControl.Should().Contain("using var rendered = svgIcon!.Render(");
        operationalControl.Should().NotContain("GetMethod(\"Create\"");
        operationalControl.Should().NotContain("unitFont");
        operationalControl.Should().NotContain("unitBrush");
        standardControl.Should().NotContain("Operational");
        standardControl.Should().NotContain("StatusText");
    }

    [Fact]
    public void ItemEdit_UsesOperationalKpisForItsCommercialSummary()
    {
        var designer = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "InventoryItems", "ItemEditForm.Designer.cs");
        var form = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "InventoryItems", "ItemEditForm.cs");

        designer.Should().Contain("kpiStockAvailable = new NuanOperationalKpiCardControl()");
        designer.Should().Contain("kpiStockAvailable.Title = \"Stock disponible\"");
        designer.Should().Contain("kpiOnOrder.Title = \"En pedido\"");
        designer.Should().Contain("kpiCommitted.Title = \"Comprometido\"");
        designer.Should().Contain("kpiPurchases.Title = \"Compras\"");
        designer.Should().Contain("kpiSales.Title = \"Ventas\"");
        designer.Should().Contain("kpiSapStatus.Title = \"Estado SAP\"");
        designer.Should().Contain("kpiAverageCost.Title = \"Costo promedio\"");
        designer.Should().Contain("kpiMargin.Title = \"Margen\"");
        designer.Should().Contain("kpiPurchaseCost.Title = \"Costo compra\"");
        designer.Should().Contain("kpiSalesPrice.Title = \"Precio venta\"");
        designer.Split("new NuanOperationalKpiCardControl()", StringSplitOptions.None)
            .Should().HaveCount(23);
        designer.Should().Contain("tabGeneral.Controls.Add(kpiStockAvailable)");
        designer.Should().Contain("tabGeneral.Controls.Add(kpiOnOrder)");
        designer.Should().Contain("tabGeneral.Controls.Add(kpiCommitted)");
        designer.Should().Contain("tabGeneral.Controls.Add(kpiPurchases)");
        designer.Should().Contain("tabGeneral.Controls.Add(kpiSales)");
        designer.Should().Contain("tabGeneral.Controls.Add(kpiSapStatus)");
        designer.Should().Contain("tabGeneral.Controls.Add(kpiAverageCost)");
        designer.Should().NotContain("pnlKpiStock");
        designer.Should().NotContain("pnlKpiOrders");
        designer.Should().NotContain("pnlKpiPurchases");
        designer.Should().NotContain("pnlKpiSales");
        designer.Should().NotContain("pnlKpiSap");
        designer.Should().NotContain("pnlKpiVariants");
        form.Should().Contain("Sum(row => ToDecimal(row[\"Disponible\"]))");
        form.Should().Contain("Sum(row => ToDecimal(row[\"Comprometido\"]))");
        form.Should().Contain("Sum(row => ToDecimal(row[\"Pedido\"]))");
        form.Should().Contain("kpiStockAvailable.ValueText = availableStock.ToString(\"N2\")");
        form.Should().Contain("((salesPrice - purchaseCost) / salesPrice) * 100");
        form.Should().Contain("kpiMargin.ValueText = marginPercent.ToString(\"N2\")");
        form.Should().Contain("kpiPurchaseCost.ValueText = purchaseCost.ToString(\"N2\")");
        form.Should().Contain("kpiAverageCost.ValueText = spnAverageCost.Value.ToString(\"N2\")");
        form.Should().Contain("kpiSalesPrice.ValueText = salesPrice.ToString(\"N2\")");
    }

    [Fact]
    public void ItemEdit_UsesTheApprovedFlatThreeColumnLayout()
    {
        var designer = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "InventoryItems", "ItemEditForm.Designer.cs");

        designer.Should().Contain("lblHeaderDataTitle.Text = \"Datos principales\"");
        designer.Should().Contain("lblHeaderClassificationTitle.Text = \"Clasificación del ítem\"");
        designer.Should().Contain("lblHeaderCommercialSummaryTitle.Text = \"Resumen comercial\"");
        designer.Should().Contain("lblGeneralIdentificationTitle.Text = \"1. Identificación del artículo\"");
        designer.Should().Contain("lblGeneralOperationTitle.Text = \"2. Clasificación y operación\"");
        designer.Should().Contain("lblGeneralSummaryTitle.Text = \"3. Resumen del artículo\"");
        designer.Should().Contain("sepGeneralColumnOne.Size = new Size(1, 400)");
        designer.Should().Contain("sepGeneralColumnTwo.Size = new Size(1, 400)");
        designer.Should().Contain("kpiStockAvailable.Location = new Point(774, 58)");
        designer.Should().Contain("kpiPurchases.Location = new Point(774, 139)");
        designer.Should().Contain("kpiSapStatus.Location = new Point(774, 301)");
        designer.Should().Contain("kpiMargin.Location = new Point(1186, 220)");
        designer.Should().Contain("kpiSalesPrice.Location = new Point(980, 220)");
        designer.Should().Contain("kpiCommitted.Location = new Point(1186, 58)");
        designer.Should().Contain("kpiStockAvailable.Size = new Size(200, 75)");
        designer.Should().Contain("tabMain.Location = new Point(0, 174)");
        designer.Should().Contain("tabMain.TabPageWidth = 164");
        designer.Should().Contain("ClientSize = new Size(1594, 828)");
        designer.Should().Contain("tabMain.PaintStyleName = \"PropertyView\"");
        designer.Should().Contain("tabGeneral.Text = \"General\"");
        designer.Should().Contain("tabDocuments.Text = \"Documentos\"");
        designer.Should().NotContain("private PanelControl pnlHeader;");
        designer.Should().NotContain("private PanelControl pnlHeaderSummary;");
        designer.Should().NotContain("private PanelControl pnlFooter;");
    }

    [Fact]
    public void ItemEdit_UsesOptionTwoIconTabsWithoutChangingTabContent()
    {
        var designer = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "InventoryItems", "ItemEditForm.Designer.cs");
        var tabs = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "InventoryItems", "ItemEditForm.Tabs.cs");
        var project = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "NuanSystem.WinForms.Forms.csproj");

        designer.Should().Contain("tabMain.AppearancePage.HeaderActive.BackColor = Color.FromArgb((int)(byte)230, (int)(byte)250, (int)(byte)246)");
        designer.Should().Contain("tabMain.AppearancePage.HeaderActive.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148)");
        designer.Should().Contain("tabMain.AppearancePage.Header.TextOptions.VAlignment = VertAlignment.Center");
        designer.Should().Contain("tabMain.CustomDrawTabHeader += (this.tabMain_CustomDrawTabHeader)");
        designer.Should().Contain("tabMain.SelectedPageChanged += (this.tabMain_SelectedPageChanged)");
        designer.Should().Contain("tabGeneral.ImageOptions.SvgImageSize = new Size(22, 22)");
        designer.Should().Contain("tabDocuments.ImageOptions.SvgImageSize = new Size(22, 22)");
        designer.Should().NotContain("tabGeneral.Appearance.Header.ForeColor = Color.FromArgb(0, 184, 148)");
        tabs.Should().Contain("Path.Combine(\"Assets\", \"Tabs\", fileName)");
        tabs.Should().Contain("general_active_24.svg");
        tabs.Should().Contain("documentos_active_24.svg");
        tabs.Should().Contain("e.Graphics.FillRectangle(indicatorBrush, indicatorBounds)");
        project.Should().Contain("Assets\\Tabs\\*.svg");

        ReadSourceFile(
                "src", "Frontend", "NuanSystem.WinForms.Forms",
                "Assets", "Tabs", "general_24.svg")
            .Should().Contain("#F1F5F9").And.Contain("#64748B");
        ReadSourceFile(
                "src", "Frontend", "NuanSystem.WinForms.Forms",
                "Assets", "Tabs", "general_active_24.svg")
            .Should().Contain("#00B894").And.Contain("#FFFFFF");
    }

    [Fact]
    public void ItemEdit_UsesSemanticSvgIconsForEveryOperationalKpi()
    {
        var form = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "InventoryItems", "ItemEditForm.cs");
        var kpiIcons = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "InventoryItems", "ItemEditForm.KpiIcons.cs");
        var project = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "NuanSystem.WinForms.Forms.csproj");

        form.Should().Contain("InitializeOperationalKpiIcons()");
        kpiIcons.Should().Contain("Path.Combine(\"Assets\", \"KPI\", fileName)");
        kpiIcons.Should().Contain("card.SvgIcon = SvgImage.FromFile(iconPath)");
        kpiIcons.Should().Contain("card.UseSvgIcon = true");
        kpiIcons.Should().Contain("ApplyOperationalKpiIcon(kpiStockAvailable, \"stock_disponible_24.svg\")");
        kpiIcons.Should().Contain("ApplyOperationalKpiIcon(kpiOnOrder, \"en_pedido_24.svg\")");
        kpiIcons.Should().Contain("ApplyOperationalKpiIcon(kpiCommitted, \"comprometido_24.svg\")");
        kpiIcons.Should().Contain("ApplyOperationalKpiIcon(kpiPurchases, \"compras_24.svg\")");
        kpiIcons.Should().Contain("ApplyOperationalKpiIcon(kpiSales, \"ventas_24.svg\")");
        kpiIcons.Should().Contain("ApplyOperationalKpiIcon(kpiAverageCost, \"costo_promedio_24.svg\")");
        kpiIcons.Should().Contain("ApplyOperationalKpiIcon(kpiPurchaseCost, \"costo_compra_24.svg\")");
        kpiIcons.Should().Contain("ApplyOperationalKpiIcon(kpiSalesPrice, \"precio_venta_24.svg\")");
        kpiIcons.Should().Contain("ApplyOperationalKpiIcon(kpiMargin, \"margen_24.svg\")");
        kpiIcons.Should().Contain("ApplyOperationalKpiIcon(kpiSapStatus, \"estado_sap_24.svg\")");
        project.Should().Contain("Assets\\KPI\\*.svg");

        foreach (var iconName in new[]
                 {
                     "stock_disponible_24.svg",
                     "en_pedido_24.svg",
                     "comprometido_24.svg",
                     "compras_24.svg",
                     "ventas_24.svg",
                     "costo_promedio_24.svg",
                     "costo_compra_24.svg",
                     "precio_venta_24.svg",
                     "margen_24.svg",
                     "estado_sap_24.svg"
                 })
        {
            ReadSourceFile(
                    "src", "Frontend", "NuanSystem.WinForms.Forms",
                    "Assets", "KPI", iconName)
                .Should().Contain("stroke=\"#FFFFFF\"");
        }
    }

    [Fact]
    public void ItemEdit_UsesApprovedCommercialLayoutWithIndicatorsInSegmentThree()
    {
        var designer = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "InventoryItems", "ItemEditForm.Designer.cs");
        var commercial = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "InventoryItems", "ItemEditForm.Commercial.cs");
        var tabs = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "InventoryItems", "ItemEditForm.Tabs.cs");

        designer.Should().Contain("lblPurchasesConfigurationTitle.Text = \"1. Configuración de compras\"");
        designer.Should().Contain("lblPurchasesConditionsTitle.Text = \"2. Condiciones operativas\"");
        designer.Should().Contain("lblPurchasesIndicatorsTitle.Text = \"3. Indicadores de compra\"");
        designer.Should().Contain("lblPurchasesHistoryTitle.Text = \"4. Historial y desempeño de compras\"");
        designer.Should().Contain("lblSalesConfigurationTitle.Text = \"1. Configuración comercial\"");
        designer.Should().Contain("lblSalesConditionsTitle.Text = \"2. Condiciones de comercialización\"");
        designer.Should().Contain("lblSalesIndicatorsTitle.Text = \"3. Indicadores de venta\"");
        designer.Should().Contain("lblSalesPricePerformanceTitle.Text = \"4. Listas de precio y desempeño\"");
        designer.Should().Contain("lblMainPurchaseSupplier.Text = \"Proveedor principal:\"");
        designer.Should().Contain("lblPreferredPurchasePresentation.Text = \"Presentación preferida:\"");
        designer.Should().Contain("lblPreferredPurchaseCurrency.Text = \"Moneda:\"");
        designer.Should().Contain("lblPurchaseMinimumQuantity.Text = \"Cantidad mínima:\"");
        designer.Should().Contain("lblPurchaseMultiple.Text = \"Múltiplo de compra:\"");
        designer.Should().Contain("lblPurchaseDeliveryDays.Text = \"Días de entrega:\"");
        designer.Should().Contain("lblSalesChannel.Text = \"Canal principal:\"");
        designer.Should().Contain("lblSalesSegment.Text = \"Segmento:\"");
        designer.Should().Contain("lblSalesMinimumPriceList.Text = \"Lista mínima permitida:\"");
        designer.Should().Contain("lblSalesMinimumPrice.Text = \"Precio mínimo:\"");
        designer.Should().Contain("lblSalesValidFrom.Text = \"Vigencia desde:\"");
        designer.Should().Contain("lblSalesEcommerce.Text = \"Disponible e-commerce:\"");
        designer.Should().Contain("lblSalesCommercialObservation.Text = \"Observación comercial:\"");
        designer.Should().Contain("tabSales.Appearance.PageClient.BackColor = Color.White");
        designer.Should().Contain("tabPurchases.Appearance.PageClient.BackColor = Color.White");
        designer.Should().Contain("tabCommercialSections.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat");
        designer.Should().Contain("kpiPurchaseLast.Location = new Point(954, 46)");
        designer.Should().Contain("kpiSales30d.Location = new Point(954, 46)");
        designer.Should().Contain("sepPurchasesColumnOne.Size = new Size(1, 224)");
        designer.Should().Contain("sepPurchasesColumnTwo.Size = new Size(1, 224)");
        designer.Should().Contain("sepSalesColumnOne.Size = new Size(1, 224)");
        designer.Should().Contain("sepSalesColumnTwo.Size = new Size(1, 224)");
        designer.Should().Contain("grdPurchaseHistory.Location = new Point(12, 282)");
        designer.Should().Contain("grdSalesPriceLists.Location = new Point(12, 282)");
        designer.Should().Contain("colPurchaseHistoryUnitCost.DisplayFormat.FormatString = \"N2\"");
        designer.Should().Contain("colSalesPriceListPrice.DisplayFormat.FormatString = \"N2\"");
        designer.Should().NotContain("pnlPurchaseKpiLast");
        designer.Should().NotContain("pnlSalesKpi30d");

        commercial.Should().Contain("UpdateCommercialPresentation()");
        commercial.Should().Contain("kpiPurchaseLast.ValueText = purchaseCost.ToString(\"N2\")");
        commercial.Should().Contain("kpiSalesLastPrice.ValueText = salesPrice.ToString(\"N2\")");
        tabs.Should().Contain("RegisterCommercialTabIcons(tabPurchases");
        tabs.Should().Contain("RegisterCommercialTabIcons(tabSales");
        tabs.Should().Contain("e.Bounds.Bottom - 3");
    }

    [Fact]
    public void ItemEdit_UsesApprovedFinanceLayoutAndInnerTabs()
    {
        var designer = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "InventoryItems", "ItemEditForm.Designer.cs");
        var finance = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "InventoryItems", "ItemEditForm.Finance.cs");
        var tabs = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "InventoryItems", "ItemEditForm.Tabs.cs");

        designer.Should().Contain("lblCostsBaseTitle.Text = \"1. Costos base del artículo\"");
        designer.Should().Contain("lblPricesMarginsTitle.Text = \"2. Precios y márgenes\"");
        designer.Should().Contain("lblFinanceCostIndicatorsTitle.Text = \"3. Indicadores financieros\"");
        designer.Should().Contain("lblCostPriceHistoryTitle.Text = \"4. Historial de costos y precios\"");
        designer.Should().Contain("kpiFinanceGrossMargin.Title = \"Margen bruto\"");
        designer.Should().Contain("kpiFinanceSuggestedPrice.Title = \"Precio sugerido\"");
        designer.Should().Contain("lblAccountingAccountsTitle.Text = \"1. Cuentas principales\"");
        designer.Should().Contain("lblAccountingComplementaryTitle.Text = \"2. Cuentas complementarias\"");
        designer.Should().Contain("lblAccountingRulesTitle.Text = \"3. Reglas contables\"");
        designer.Should().Contain("lblTaxConfigurationTitle.Text = \"1. Clasificación tributaria\"");
        designer.Should().Contain("lblTaxRatesTitle.Text = \"2. Impuestos y retenciones\"");
        designer.Should().Contain("lblTaxApplicabilityTitle.Text = \"3. Aplicabilidad\"");
        designer.Should().Contain("sepCostsColumnOne.Size = new Size(1, 166)");
        designer.Should().Contain("sepAccountingColumnOne.Size = new Size(1, 342)");
        designer.Should().Contain("sepTaxesColumnOne.Size = new Size(1, 130)");
        designer.Should().Contain("sepCostsHistory.Size = new Size(1142, 1)");
        designer.Should().NotContain("lblTaxSummaryTitle");
        designer.Should().NotContain("grdTaxSummary");
        designer.Should().Contain("tabFinanceSections.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat");
        designer.Should().NotContain("lblSimulatorTitle");
        finance.Should().Contain("UpdateFinancePresentation()");
        finance.Should().NotContain("CreateTaxSummaryRow");
        tabs.Should().Contain("RegisterFinanceTabIcons(tabCosts");
        tabs.Should().Contain("RegisterFinanceTabIcons(tabAccounting");
        tabs.Should().Contain("RegisterFinanceTabIcons(tabTaxes");
    }

    [Fact]
    public void ItemEdit_UsesApprovedUnitsAndCodesLayout()
    {
        var designer = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "InventoryItems", "ItemEditForm.Designer.cs");
        var form = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "InventoryItems", "ItemEditForm.cs");
        var units = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "InventoryItems", "ItemEditForm.Units.cs");

        designer.Should().Contain("lblInventoryUnitTitle.Text = \"1. Unidades y medidas\"");
        designer.Should().Contain("lblCodesIdentifiersTitle.Text = \"2. Identificadores generales\"");
        designer.Should().Contain("lblPurchasePresentationsTitle.Text = \"3. Presentaciones, unidades y códigos\"");
        designer.Should().Contain("sepUnitsColumn.Location = new Point(450, 12)");
        designer.Should().Contain("grdItemPresentations.Location = new Point(464, 46)");
        designer.Should().Contain("grdItemPresentations.Size = new Size(922, 340)");
        designer.Should().Contain("gvItemPresentations.OptionsBehavior.Editable = false");
        designer.Should().Contain("gvItemPresentations.OptionsSelection.EnableAppearanceFocusedCell = false");
        designer.Should().Contain("colPurchaseFactor.DisplayFormat.FormatString = \"N3\"");
        designer.Should().Contain("btnUpdateItemPresentation.Text = \"Editar\"");
        designer.Should().Contain("lblPresentationSummary.Text = \"0 presentaciones   •   0 activas   •   Principal: -\"");
        form.Should().Contain("ConfigureUnitsAndCodesTab()");
        form.Should().Contain("UpdateUnitsPresentationSummary()");
        units.Should().Contain("OperationButtonIcons.LoadOperationIcon(iconName, color)");
        units.Should().Contain("button.ButtonStyle = BorderStyles.NoBorder");
        units.Should().Contain("BrandResources.ErrorText");
        units.Should().Contain("rows.Count(row => ToBool(row[\"Activa\"], true))");
        units.Should().Contain("Principal: {mainPresentation ?? \"-\"}");
    }

    [Fact]
    public void ItemEdit_UsesApprovedInventoryLayout()
    {
        var designer = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "InventoryItems", "ItemEditForm.Designer.cs");
        var form = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "InventoryItems", "ItemEditForm.cs");
        var inventory = ReadSourceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms",
            "InventoryItems", "ItemEditForm.Inventory.cs");

        designer.Should().Contain("lblInventoryParametersTitle.Text = \"1. Parámetros de inventario\"");
        designer.Should().Contain("lblReplenishmentOperationTitle.Text = \"2. Reposición y operación\"");
        designer.Should().Contain("lblInventoryLocationsRestrictionsTitle.Text = \"3. Ubicaciones / restricciones\"");
        designer.Should().Contain("lblStockByWarehouseTitle.Text = \"4. Stock por bodega\"");
        designer.Should().Contain("sepInventoryColumnOne.Location = new Point(397, 12)");
        designer.Should().Contain("grdWarehouseStock.Location = new Point(12, 284)");
        designer.Should().Contain("grdWarehouseStock.Size = new Size(1374, 112)");
        designer.Should().Contain("gvWarehouseStock.OptionsBehavior.Editable = false");
        designer.Should().Contain("gvWarehouseStock.OptionsSelection.EnableAppearanceFocusedCell = false");
        designer.Should().Contain("colWarehouseAvailable.DisplayFormat.FormatString = \"N2\"");
        designer.Should().Contain("btnUpdateWarehouseStock.Text = \"Editar\"");
        designer.Should().Contain("lblWarehouseSummary.Text = \"0 bodegas   •   Disponible: 0.00 UND   •   Principal: -\"");
        form.Should().Contain("ConfigureInventoryTab()");
        form.Should().Contain("UpdateInventoryWarehouseSummary()");
        inventory.Should().Contain("ConfigurePresentationAction(");
        inventory.Should().Contain("BrandResources.ErrorText");
        inventory.Should().Contain("rows.Sum(row => ToDecimal(row[\"Disponible\"]))");
        inventory.Should().Contain("rows.FirstOrDefault(row => ToBool(row[\"Principal\"]))");
        inventory.Should().Contain("Disponible: {availableStock:N2} {inventoryUnit}");
    }

    private static string ReadSourceFile(params string[] pathParts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine(new[] { directory.FullName }.Concat(pathParts).ToArray());
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"No se encontró {Path.Combine(pathParts)}.");
    }
}
