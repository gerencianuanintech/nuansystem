/*
    Ejecutar este script dentro de la base de datos de una empresa/tenant.
    Carga un plan de cuentas comercial base para Ecuador.

    Ajustar @CompanyId si la empresa actual usa otro identificador.
    El script es idempotente: solo inserta cuentas activas que no existan por CompanyId + Code.
*/

DECLARE @CompanyId int = 1;
DECLARE @CreatedByUserId int = NULL;
DECLARE @CreatedByUserName nvarchar(120) = N'Sistema';

IF OBJECT_ID(N'dbo.ChartOfAccounts', N'U') IS NULL
BEGIN
    THROW 51000, 'La tabla dbo.ChartOfAccounts no existe. Ejecute primero 021_tenant_accounting_chart_of_accounts.sql.', 1;
END;

IF OBJECT_ID(N'tempdb..#CommercialChartOfAccountsSeed') IS NOT NULL
BEGIN
    DROP TABLE #CommercialChartOfAccountsSeed;
END;

CREATE TABLE #CommercialChartOfAccountsSeed
(
    Code nvarchar(50) NOT NULL PRIMARY KEY,
    Name nvarchar(200) NOT NULL,
    Description nvarchar(500) NULL,
    AccountType nvarchar(30) NOT NULL,
    AccountClass nvarchar(30) NULL,
    ParentCode nvarchar(50) NULL,
    [Level] int NOT NULL,
    IsTitle bit NOT NULL,
    AllowsMovement bit NOT NULL,
    CurrencyCode nvarchar(3) NULL,
    IsMonetaryAccount bit NOT NULL,
    RelevantForCashFlow bit NOT NULL,
    RequiresCostCenter bit NOT NULL,
    RequiresThirdParty bit NOT NULL,
    RequiresProject bit NOT NULL
);

INSERT INTO #CommercialChartOfAccountsSeed
(
    Code,
    Name,
    Description,
    AccountType,
    AccountClass,
    ParentCode,
    [Level],
    IsTitle,
    AllowsMovement,
    CurrencyCode,
    IsMonetaryAccount,
    RelevantForCashFlow,
    RequiresCostCenter,
    RequiresThirdParty,
    RequiresProject
)
VALUES
    (N'1', N'Activo', N'Cuentas que representan bienes y derechos de la empresa.', N'ASSET', N'BALANCE', NULL, 1, 1, 0, N'USD', 0, 0, 0, 0, 0),
    (N'1.01', N'Activo corriente', N'Activos realizables o consumibles en el corto plazo.', N'ASSET', N'CURRENT', N'1', 2, 1, 0, N'USD', 0, 0, 0, 0, 0),
    (N'1.01.01', N'Caja general', N'Efectivo disponible en caja general.', N'ASSET', N'CURRENT', N'1.01', 3, 0, 1, N'USD', 1, 1, 0, 0, 0),
    (N'1.01.02', N'Caja chica', N'Fondos menores para gastos operativos.', N'ASSET', N'CURRENT', N'1.01', 3, 0, 1, N'USD', 1, 1, 0, 0, 0),
    (N'1.01.03', N'Bancos', N'Cuentas bancarias corrientes y de ahorro.', N'ASSET', N'CURRENT', N'1.01', 3, 0, 1, N'USD', 1, 1, 0, 0, 0),
    (N'1.01.04', N'Cuentas por cobrar clientes', N'Valores pendientes de cobro a clientes.', N'ASSET', N'CURRENT', N'1.01', 3, 0, 1, N'USD', 0, 1, 0, 1, 0),
    (N'1.01.05', N'Documentos por cobrar', N'Documentos y letras pendientes de cobro.', N'ASSET', N'CURRENT', N'1.01', 3, 0, 1, N'USD', 0, 1, 0, 1, 0),
    (N'1.01.06', N'Anticipos a proveedores', N'Valores entregados por adelantado a proveedores.', N'ASSET', N'CURRENT', N'1.01', 3, 0, 1, N'USD', 0, 1, 0, 1, 0),
    (N'1.01.07', N'Inventario de mercaderias', N'Mercaderias disponibles para la venta.', N'ASSET', N'CURRENT', N'1.01', 3, 0, 1, N'USD', 0, 0, 0, 0, 0),
    (N'1.01.08', N'Inventario de materia prima', N'Materiales destinados a produccion.', N'ASSET', N'CURRENT', N'1.01', 3, 0, 1, N'USD', 0, 0, 0, 0, 0),
    (N'1.01.09', N'Credito tributario IVA', N'IVA pagado acreditable.', N'ASSET', N'CURRENT', N'1.01', 3, 0, 1, N'USD', 0, 0, 0, 0, 0),
    (N'1.01.10', N'Retenciones por cobrar', N'Retenciones recibidas pendientes de compensar.', N'ASSET', N'CURRENT', N'1.01', 3, 0, 1, N'USD', 0, 0, 0, 1, 0),
    (N'1.01.11', N'Anticipo impuesto a la renta', N'Anticipos y pagos previos de impuesto a la renta.', N'ASSET', N'CURRENT', N'1.01', 3, 0, 1, N'USD', 0, 0, 0, 0, 0),
    (N'1.02', N'Activo no corriente', N'Activos de largo plazo.', N'ASSET', N'NON_CURRENT', N'1', 2, 1, 0, N'USD', 0, 0, 0, 0, 0),
    (N'1.02.01', N'Propiedad, planta y equipo', N'Bienes fisicos de uso permanente.', N'ASSET', N'NON_CURRENT', N'1.02', 3, 1, 0, N'USD', 0, 0, 0, 0, 0),
    (N'1.02.01.01', N'Muebles y enseres', N'Mobiliario utilizado por la empresa.', N'ASSET', N'NON_CURRENT', N'1.02.01', 4, 0, 1, N'USD', 0, 0, 0, 0, 0),
    (N'1.02.01.02', N'Equipos de computacion', N'Equipos informaticos y perifericos.', N'ASSET', N'NON_CURRENT', N'1.02.01', 4, 0, 1, N'USD', 0, 0, 0, 0, 0),
    (N'1.02.01.03', N'Vehiculos', N'Vehiculos de propiedad de la empresa.', N'ASSET', N'NON_CURRENT', N'1.02.01', 4, 0, 1, N'USD', 0, 0, 0, 0, 0),
    (N'1.02.01.04', N'Maquinaria y equipo', N'Maquinaria y equipos productivos.', N'ASSET', N'NON_CURRENT', N'1.02.01', 4, 0, 1, N'USD', 0, 0, 0, 0, 0),
    (N'1.02.01.99', N'Depreciacion acumulada propiedad, planta y equipo', N'Depreciacion acumulada de activos fijos.', N'ASSET', N'NON_CURRENT', N'1.02.01', 4, 0, 1, N'USD', 0, 0, 0, 0, 0),
    (N'1.02.02', N'Activos intangibles', N'Licencias, marcas y otros intangibles.', N'ASSET', N'NON_CURRENT', N'1.02', 3, 0, 1, N'USD', 0, 0, 0, 0, 0),

    (N'2', N'Pasivo', N'Obligaciones presentes de la empresa.', N'LIABILITY', N'BALANCE', NULL, 1, 1, 0, N'USD', 0, 0, 0, 0, 0),
    (N'2.01', N'Pasivo corriente', N'Obligaciones exigibles en el corto plazo.', N'LIABILITY', N'CURRENT', N'2', 2, 1, 0, N'USD', 0, 0, 0, 0, 0),
    (N'2.01.01', N'Cuentas por pagar proveedores', N'Valores pendientes de pago a proveedores.', N'LIABILITY', N'CURRENT', N'2.01', 3, 0, 1, N'USD', 0, 1, 0, 1, 0),
    (N'2.01.02', N'Documentos por pagar', N'Documentos y letras pendientes de pago.', N'LIABILITY', N'CURRENT', N'2.01', 3, 0, 1, N'USD', 0, 1, 0, 1, 0),
    (N'2.01.03', N'Prestamos bancarios corto plazo', N'Obligaciones financieras de corto plazo.', N'LIABILITY', N'CURRENT', N'2.01', 3, 0, 1, N'USD', 1, 1, 0, 1, 0),
    (N'2.01.04', N'IVA por pagar', N'IVA causado pendiente de pago.', N'LIABILITY', N'CURRENT', N'2.01', 3, 0, 1, N'USD', 0, 0, 0, 0, 0),
    (N'2.01.05', N'Retenciones por pagar', N'Retenciones practicadas pendientes de pago.', N'LIABILITY', N'CURRENT', N'2.01', 3, 0, 1, N'USD', 0, 0, 0, 1, 0),
    (N'2.01.06', N'Impuesto a la renta por pagar', N'Impuesto a la renta causado pendiente.', N'LIABILITY', N'CURRENT', N'2.01', 3, 0, 1, N'USD', 0, 0, 0, 0, 0),
    (N'2.01.07', N'Sueldos por pagar', N'Obligaciones laborales pendientes.', N'LIABILITY', N'CURRENT', N'2.01', 3, 0, 1, N'USD', 0, 1, 1, 1, 0),
    (N'2.01.08', N'IESS por pagar', N'Obligaciones con seguridad social.', N'LIABILITY', N'CURRENT', N'2.01', 3, 0, 1, N'USD', 0, 0, 1, 0, 0),
    (N'2.01.09', N'Beneficios sociales por pagar', N'Decimos, vacaciones y otros beneficios.', N'LIABILITY', N'CURRENT', N'2.01', 3, 0, 1, N'USD', 0, 0, 1, 1, 0),
    (N'2.02', N'Pasivo no corriente', N'Obligaciones exigibles en el largo plazo.', N'LIABILITY', N'NON_CURRENT', N'2', 2, 1, 0, N'USD', 0, 0, 0, 0, 0),
    (N'2.02.01', N'Prestamos bancarios largo plazo', N'Obligaciones financieras de largo plazo.', N'LIABILITY', N'NON_CURRENT', N'2.02', 3, 0, 1, N'USD', 1, 1, 0, 1, 0),

    (N'3', N'Patrimonio', N'Aportes y resultados acumulados de los propietarios.', N'EQUITY', N'BALANCE', NULL, 1, 1, 0, N'USD', 0, 0, 0, 0, 0),
    (N'3.01', N'Capital social', N'Aportes de socios o accionistas.', N'EQUITY', N'EQUITY', N'3', 2, 0, 1, N'USD', 0, 0, 0, 1, 0),
    (N'3.02', N'Reservas', N'Reservas legales, estatutarias o facultativas.', N'EQUITY', N'EQUITY', N'3', 2, 0, 1, N'USD', 0, 0, 0, 0, 0),
    (N'3.03', N'Resultados acumulados', N'Utilidades o perdidas acumuladas.', N'EQUITY', N'EQUITY', N'3', 2, 0, 1, N'USD', 0, 0, 0, 0, 0),
    (N'3.04', N'Resultado del ejercicio', N'Utilidad o perdida del periodo actual.', N'EQUITY', N'EQUITY', N'3', 2, 0, 1, N'USD', 0, 0, 0, 0, 0),

    (N'4', N'Ingresos', N'Ingresos ordinarios y no ordinarios.', N'INCOME', N'RESULT', NULL, 1, 1, 0, N'USD', 0, 0, 0, 0, 0),
    (N'4.01', N'Ingresos operacionales', N'Ingresos por actividades principales.', N'INCOME', N'OPERATING', N'4', 2, 1, 0, N'USD', 0, 0, 0, 0, 0),
    (N'4.01.01', N'Ventas de bienes tarifa 15%', N'Ventas gravadas con tarifa general de IVA.', N'INCOME', N'OPERATING', N'4.01', 3, 0, 1, N'USD', 0, 0, 1, 0, 0),
    (N'4.01.02', N'Ventas de bienes tarifa 0%', N'Ventas gravadas con tarifa cero de IVA.', N'INCOME', N'OPERATING', N'4.01', 3, 0, 1, N'USD', 0, 0, 1, 0, 0),
    (N'4.01.03', N'Prestacion de servicios', N'Ingresos por servicios prestados.', N'INCOME', N'OPERATING', N'4.01', 3, 0, 1, N'USD', 0, 0, 1, 0, 0),
    (N'4.01.04', N'Descuentos en ventas', N'Descuentos concedidos a clientes.', N'INCOME', N'OPERATING', N'4.01', 3, 0, 1, N'USD', 0, 0, 1, 0, 0),
    (N'4.01.05', N'Devoluciones en ventas', N'Devoluciones realizadas por clientes.', N'INCOME', N'OPERATING', N'4.01', 3, 0, 1, N'USD', 0, 0, 1, 0, 0),
    (N'4.02', N'Ingresos no operacionales', N'Ingresos distintos a la actividad principal.', N'INCOME', N'NON_OPERATING', N'4', 2, 1, 0, N'USD', 0, 0, 0, 0, 0),
    (N'4.02.01', N'Ingresos financieros', N'Intereses y rendimientos financieros.', N'INCOME', N'NON_OPERATING', N'4.02', 3, 0, 1, N'USD', 0, 0, 0, 0, 0),
    (N'4.02.02', N'Otros ingresos', N'Ingresos eventuales no operacionales.', N'INCOME', N'NON_OPERATING', N'4.02', 3, 0, 1, N'USD', 0, 0, 0, 0, 0),

    (N'5', N'Costos', N'Costos relacionados con ventas o produccion.', N'COST', N'RESULT', NULL, 1, 1, 0, N'USD', 0, 0, 0, 0, 0),
    (N'5.01', N'Costo de ventas', N'Costo de mercaderias vendidas.', N'COST', N'OPERATING', N'5', 2, 1, 0, N'USD', 0, 0, 0, 0, 0),
    (N'5.01.01', N'Costo de ventas mercaderias', N'Costo asociado a venta de mercaderias.', N'COST', N'OPERATING', N'5.01', 3, 0, 1, N'USD', 0, 0, 1, 0, 0),
    (N'5.01.02', N'Costo de servicios', N'Costo directo de servicios prestados.', N'COST', N'OPERATING', N'5.01', 3, 0, 1, N'USD', 0, 0, 1, 0, 0),
    (N'5.02', N'Costos de produccion', N'Costos asociados a procesos productivos.', N'COST', N'OPERATING', N'5', 2, 1, 0, N'USD', 0, 0, 1, 0, 0),
    (N'5.02.01', N'Materia prima consumida', N'Materia prima utilizada en produccion.', N'COST', N'OPERATING', N'5.02', 3, 0, 1, N'USD', 0, 0, 1, 0, 0),
    (N'5.02.02', N'Mano de obra directa', N'Mano de obra asignada a produccion.', N'COST', N'OPERATING', N'5.02', 3, 0, 1, N'USD', 0, 0, 1, 1, 0),
    (N'5.02.03', N'Costos indirectos de fabricacion', N'Costos indirectos asociados a produccion.', N'COST', N'OPERATING', N'5.02', 3, 0, 1, N'USD', 0, 0, 1, 0, 0),

    (N'6', N'Gastos', N'Gastos administrativos, de venta y financieros.', N'EXPENSE', N'RESULT', NULL, 1, 1, 0, N'USD', 0, 0, 0, 0, 0),
    (N'6.01', N'Gastos administrativos', N'Gastos de administracion general.', N'EXPENSE', N'ADMINISTRATIVE', N'6', 2, 1, 0, N'USD', 0, 0, 1, 0, 0),
    (N'6.01.01', N'Sueldos y salarios administracion', N'Remuneraciones del personal administrativo.', N'EXPENSE', N'ADMINISTRATIVE', N'6.01', 3, 0, 1, N'USD', 0, 1, 1, 1, 0),
    (N'6.01.02', N'Beneficios sociales administracion', N'Decimos, vacaciones y beneficios administrativos.', N'EXPENSE', N'ADMINISTRATIVE', N'6.01', 3, 0, 1, N'USD', 0, 1, 1, 1, 0),
    (N'6.01.03', N'Aporte patronal IESS administracion', N'Aportes patronales del area administrativa.', N'EXPENSE', N'ADMINISTRATIVE', N'6.01', 3, 0, 1, N'USD', 0, 1, 1, 1, 0),
    (N'6.01.04', N'Honorarios profesionales', N'Pagos por servicios profesionales.', N'EXPENSE', N'ADMINISTRATIVE', N'6.01', 3, 0, 1, N'USD', 0, 1, 1, 1, 0),
    (N'6.01.05', N'Arriendos', N'Arrendamientos administrativos.', N'EXPENSE', N'ADMINISTRATIVE', N'6.01', 3, 0, 1, N'USD', 0, 1, 1, 1, 0),
    (N'6.01.06', N'Servicios basicos', N'Agua, energia electrica, telecomunicaciones y similares.', N'EXPENSE', N'ADMINISTRATIVE', N'6.01', 3, 0, 1, N'USD', 0, 1, 1, 0, 0),
    (N'6.01.07', N'Suministros de oficina', N'Materiales y suministros administrativos.', N'EXPENSE', N'ADMINISTRATIVE', N'6.01', 3, 0, 1, N'USD', 0, 0, 1, 0, 0),
    (N'6.01.08', N'Mantenimiento y reparaciones', N'Mantenimiento de instalaciones y equipos.', N'EXPENSE', N'ADMINISTRATIVE', N'6.01', 3, 0, 1, N'USD', 0, 1, 1, 1, 0),
    (N'6.01.09', N'Depreciacion propiedad, planta y equipo', N'Gasto por depreciacion de activos fijos.', N'EXPENSE', N'ADMINISTRATIVE', N'6.01', 3, 0, 1, N'USD', 0, 0, 1, 0, 0),
    (N'6.01.10', N'Impuestos, contribuciones y otros', N'Impuestos y contribuciones no recuperables.', N'EXPENSE', N'ADMINISTRATIVE', N'6.01', 3, 0, 1, N'USD', 0, 1, 1, 0, 0),
    (N'6.02', N'Gastos de venta', N'Gastos comerciales y de distribucion.', N'EXPENSE', N'SALES', N'6', 2, 1, 0, N'USD', 0, 0, 1, 0, 0),
    (N'6.02.01', N'Sueldos y salarios ventas', N'Remuneraciones del personal de ventas.', N'EXPENSE', N'SALES', N'6.02', 3, 0, 1, N'USD', 0, 1, 1, 1, 0),
    (N'6.02.02', N'Comisiones en ventas', N'Comisiones pagadas al equipo comercial.', N'EXPENSE', N'SALES', N'6.02', 3, 0, 1, N'USD', 0, 1, 1, 1, 0),
    (N'6.02.03', N'Publicidad y marketing', N'Campanas, publicidad y promocion.', N'EXPENSE', N'SALES', N'6.02', 3, 0, 1, N'USD', 0, 1, 1, 1, 0),
    (N'6.02.04', N'Transporte y distribucion', N'Gastos de entrega y distribucion.', N'EXPENSE', N'SALES', N'6.02', 3, 0, 1, N'USD', 0, 1, 1, 1, 0),
    (N'6.03', N'Gastos financieros', N'Gastos por financiamiento y servicios bancarios.', N'EXPENSE', N'FINANCIAL', N'6', 2, 1, 0, N'USD', 0, 0, 0, 0, 0),
    (N'6.03.01', N'Intereses bancarios', N'Intereses por obligaciones financieras.', N'EXPENSE', N'FINANCIAL', N'6.03', 3, 0, 1, N'USD', 1, 1, 0, 1, 0),
    (N'6.03.02', N'Comisiones bancarias', N'Comisiones y cargos bancarios.', N'EXPENSE', N'FINANCIAL', N'6.03', 3, 0, 1, N'USD', 1, 1, 0, 1, 0),

    (N'7', N'Cuentas de orden', N'Cuentas informativas y de control.', N'ORDER', N'ORDER', NULL, 1, 1, 0, N'USD', 0, 0, 0, 0, 0),
    (N'7.01', N'Cuentas de orden deudoras', N'Cuentas de orden con naturaleza deudora.', N'ORDER', N'ORDER', N'7', 2, 0, 1, N'USD', 0, 0, 0, 0, 0),
    (N'7.02', N'Cuentas de orden acreedoras', N'Cuentas de orden con naturaleza acreedora.', N'ORDER', N'ORDER', N'7', 2, 0, 1, N'USD', 0, 0, 0, 0, 0);

DECLARE @Level int = 1;

WHILE @Level <= 4
BEGIN
    INSERT INTO dbo.ChartOfAccounts
    (
        CompanyId,
        Code,
        Name,
        Description,
        ExternalCode,
        AccountType,
        AccountClass,
        ParentAccountId,
        [Level],
        IsTitle,
        AllowsMovement,
        IsActive,
        CurrencyCode,
        Balance,
        IsConfidential,
        IsMonetaryAccount,
        IsAssociatedAccount,
        RevalueByIndex,
        BlockManualPosting,
        RelevantForCashFlow,
        RequiresCostCenter,
        RequiresThirdParty,
        RequiresProject,
        CreatedByUserId,
        CreatedByUserName,
        CreatedAt,
        IsDeleted
    )
    SELECT
        @CompanyId,
        seed.Code,
        seed.Name,
        seed.Description,
        seed.Code,
        seed.AccountType,
        seed.AccountClass,
        parent.Id,
        seed.[Level],
        seed.IsTitle,
        seed.AllowsMovement,
        1,
        seed.CurrencyCode,
        0,
        0,
        seed.IsMonetaryAccount,
        0,
        0,
        0,
        seed.RelevantForCashFlow,
        seed.RequiresCostCenter,
        seed.RequiresThirdParty,
        seed.RequiresProject,
        @CreatedByUserId,
        @CreatedByUserName,
        SYSUTCDATETIME(),
        0
    FROM #CommercialChartOfAccountsSeed seed
    LEFT JOIN dbo.ChartOfAccounts parent
        ON parent.CompanyId = @CompanyId
       AND parent.Code = seed.ParentCode
       AND parent.IsDeleted = 0
    WHERE seed.[Level] = @Level
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.ChartOfAccounts existing
          WHERE existing.CompanyId = @CompanyId
            AND existing.Code = seed.Code
            AND existing.IsDeleted = 0
      )
      AND (seed.ParentCode IS NULL OR parent.Id IS NOT NULL);

    SET @Level += 1;
END;

SELECT
    COUNT(1) AS TotalSeedAccounts,
    SUM(CASE WHEN chart.Id IS NOT NULL THEN 1 ELSE 0 END) AS ActiveAccountsFound
FROM #CommercialChartOfAccountsSeed seed
LEFT JOIN dbo.ChartOfAccounts chart
    ON chart.CompanyId = @CompanyId
   AND chart.Code = seed.Code
   AND chart.IsDeleted = 0;
