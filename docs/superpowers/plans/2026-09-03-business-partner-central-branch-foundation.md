# Business Partner Central–Branch Foundation Implementation Plan

> **For Codex:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Implementar la identidad interna de clientes y proveedores, la política central de `SapCardCode` y el flujo durable sucursal → empresa central → todas las sucursales, incluyendo idempotencia, concurrencia, conciliación y resolución auditada de conflictos, sin conectarse todavía a SAP Business One.

**Architecture:** Las escrituras de usuario pasan por la API del tenant activo y guardan el socio junto con un `LocalOutbox` en la misma transacción. Una sucursal publica `BusinessPartnerProposal` únicamente hacia su empresa central; la central concilia y, si acepta, incrementa `CanonicalVersion`, reserva `SapCardCode` y publica `BusinessPartner` hacia todas sus sucursales. `BusinessPartnerProposalResult` devuelve rechazos o conflictos solamente a la sucursal de origen. `NuanSystem_Master` conserva gobierno, jerarquía, perfiles, rutas, permisos y política de prefijos; cada tenant conserva datos, inbox/outbox, estados y conflictos. El Worker Master/Sucursal es el único transportador de estos eventos y los aplicadores escriben directamente mediante repositorios transaccionales para no regenerar eventos.

**Tech Stack:** .NET 10, C#, ASP.NET Core Minimal APIs, MediatR, FluentValidation, Dapper, SQL Server, xUnit, FluentAssertions, NSubstitute, WinForms/DevExpress y `NuanSystem.MasterBranchSyncWorker`.

**Spec:** `docs/architecture/SAP-NUANSYSTEM-BIDIRECTIONAL-FOUNDATION.md`

**Global Constraints:**

- Mantener NuanSystem operativo sin SAP; no modificar `NuanSystem.SyncWorker`, Service Layer, DI API, HANA ni crear llamadas SAP en este plan.
- Mantener `Domain` libre de SAP, SQL Server, Dapper, WinForms y servicios externos.
- Mantener WinForms conectado únicamente a la API REST centralizada.
- Usar `LocalOutbox` → `SyncOutbox` → `SyncInbox` para toda comunicación entre tenants; no abrir la base de otra empresa desde la API ni desde WinForms.
- Mantener separados los pipelines Master/Sucursal y SAP; este incremento no escribe `SapSyncOutbox`.
- Mantener las migraciones forward-only, idempotentes, registradas en `SchemaHistory`, sin borrar ni recodificar `BusinessPartners.Code` históricos.
- Mantener todas las capacidades, perfiles, relays y workers nuevos deshabilitados por defecto. La activación ocurre únicamente en el piloto controlado.
- Preservar cambios no relacionados en el worktree y no incluir `.codex/tmp/item-commercial-segments.manifest.json` en ningún commit.
- Aplicar TDD: cada comportamiento comienza con una prueba que falla, se implementa con el cambio mínimo y se vuelve a ejecutar antes del commit de la tarea.
- Hacer commits por tarea solamente después de ejecutar sus pruebas específicas; no mezclar archivos ajenos al alcance indicado.

**Explicitly out of scope:**

- crear o actualizar socios dentro de SAP;
- cualquier productor o consumidor de `SapSyncOutbox`;
- lectura SAP de bodegas, proveedores, clientes, artículos u órdenes de compra;
- entrada de mercancía, recepción parcial, lotes, series, fechas de caducidad, ingreso/escaneo/generación de códigos;
- oferta de venta y los demás documentos SAP;
- stock, costos, precios, contabilidad y condiciones comerciales fuera de la protección/replicación mínima indicada para socios.

## Contratos cerrados para esta implementación

Estos nombres y comportamientos eliminan decisiones abiertas durante la ejecución:

- `BusinessPartners.Id` sigue siendo local; `GlobalId` es la identidad compartida entre tenants.
- Cada socio nuevo recibe `Code = "BP-" + GlobalId.ToString("N").ToUpperInvariant()`; el cliente API no envía `Code` al crear. Los códigos históricos nunca se cambian.
- Los únicos roles nuevos son `Customer` y `Supplier`. `Both` se conserva solamente como histórico con estado `LegacyReview` y no publica propuestas ni canónicos.
- La identificación normalizada usa `Trim`, mayúsculas invariantes y elimina espacios, puntos y guiones.
- La unicidad nueva es `(PartnerType, IdentificationTypeId, NormalizedIdentificationNumber)` para filas activas/no eliminadas. La misma identificación puede existir una vez como cliente y una vez como proveedor.
- En una actualización, `Code`, `PartnerType`, `IdentificationTypeId`, `IdentificationNumber` y `NormalizedIdentificationNumber` son inmutables.
- Campos editables en una sucursal sincronizada: `Name`, `CommercialName`, `Email`, `Phone`, `Addresses` y `Contacts`. Cada dirección y contacto usa su propio `GlobalId`.
- Campos administrados que una sucursal sincronizada no puede alterar: `SapCardCode`, condiciones de pago, lista de precios, límite de crédito, estado SAP, contabilidad, bancos, retenciones, crédito y los restantes campos comerciales/proveedor no incluidos en la lista anterior.
- Una empresa central o una empresa `Standalone` conserva las capacidades CRUD existentes; la restricción de campos se aplica cuando `IsMaster == false && SyncEnabled == true`.
- Estados locales: `PendingMaster`, `Accepted`, `Rejected`, `Conflict` y `LegacyReview`. Mientras un registro esté `PendingMaster` o `Conflict`, la API rechaza otra edición con `BP_MASTER_PROPOSAL_IN_FLIGHT`. Después de `Rejected`, el usuario puede corregir y reenviar.
- `RowVersion` protege concurrencia dentro del tenant. `CanonicalVersion` protege orden y conciliación entre tenants.
- Entidades de transporte: `BusinessPartnerProposal` versión 1, `BusinessPartner` versión 2 y `BusinessPartnerProposalResult` versión 1.
- `BusinessPartnerProposal` requiere perfil `BranchToMaster` y apunta exclusivamente a `ParentCompanyId`.
- `BusinessPartner` y `BusinessPartnerProposalResult` requieren perfil `MasterToBranch`; el primero se distribuye a todas las sucursales activas, y el segundo usa `TargetCompanyId` para volver sólo al origen.
- La columna histórica `SyncOutboxTargets.BranchCompanyId` se conserva por compatibilidad y se interpreta como identificador del tenant destino. No se renombra en este incremento.
- Modos de prefijo central: `NationalForeign` produce `CN`, `CE`, `PL`, `PE`; `RoleOnly` produce `C`, `C`, `P`, `P`. Un socio es extranjero solamente cuando su tipo de identificación coincide, sin distinguir mayúsculas, con `PassportIdentificationTypeCode` de la política.
- `SapCardCode` se calcula y reserva en la empresa central al aceptar. No se trunca: se usa el límite SAP B1 de 15 caracteres y un resultado mayor produce rechazo `BP_SAP_CARD_CODE_TOO_LONG`.
- La activación de un perfil `BranchToMaster` para socios exige una política de códigos habilitada en la empresa central.
- La conciliación es de tres vías por rutas de campo. Cambios disjuntos se fusionan; el mismo valor se acepta; valores distintos sobre la misma ruta crean conflicto. No existe last-write-wins silencioso.
- Resoluciones humanas: `AcceptBranch` aplica los valores propuestos sólo en las rutas en conflicto y conserva el estado central actual en las demás; `KeepCentral` rechaza la propuesta. Ambas requieren permiso, motivo, usuario y `RowVersion` del conflicto.

## Task 1: Codificar las reglas puras de identidad y `SapCardCode`

**Files:**

- Create: `src/Backend/NuanSystem.Application/Features/BusinessPartners/Policies/BusinessPartnerIdentityPolicy.cs`
- Create: `src/Backend/NuanSystem.Application/Features/BusinessPartners/Policies/BusinessPartnerSapCardCodePolicy.cs`
- Create: `tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerIdentityPolicyTests.cs`
- Create: `tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerSapCardCodePolicyTests.cs`

- [ ] **Step 1: Write the failing identity tests**

```csharp
[Theory]
[InlineData(" 09.999-999 99001 ", "0999999999001")]
[InlineData(" ab-12. 3 ", "AB123")]
public void NormalizeIdentification_RemovesFormattingAndUppercases(string raw, string expected)
{
    BusinessPartnerIdentityPolicy.NormalizeIdentification(raw).Should().Be(expected);
}

[Fact]
public void CreateInternalCode_UsesStableGlobalIdentity()
{
    var id = Guid.Parse("7f777a58-4bc5-4a4c-b29a-50f3e6c2b0cd");
    BusinessPartnerIdentityPolicy.CreateInternalCode(id)
        .Should().Be("BP-7F777A584BC54A4CB29A50F3E6C2B0CD");
}
```

- [ ] **Step 2: Run the focused tests and confirm the expected failure**

Run: `dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj --filter "FullyQualifiedName~BusinessPartnerIdentityPolicyTests|FullyQualifiedName~BusinessPartnerSapCardCodePolicyTests"`

Expected: FAIL with `CS0103` or `CS0246` because the two policies do not exist.

- [ ] **Step 3: Implement the minimal pure policies**

```csharp
public static class BusinessPartnerIdentityPolicy
{
    public static string NormalizeIdentification(string value) => string.Concat(
        value.Trim().ToUpperInvariant()
            .Where(character => !char.IsWhiteSpace(character) && character is not '.' and not '-'));

    public static string CreateInternalCode(Guid globalId) =>
        $"BP-{globalId:N}".ToUpperInvariant();
}

public enum BusinessPartnerSapPrefixMode
{
    NationalForeign,
    RoleOnly
}

public sealed record BusinessPartnerSapCodePolicyData(
    BusinessPartnerSapPrefixMode PrefixMode,
    string PassportIdentificationTypeCode);
```

Implementar `CreateSapCardCode` con tabla cerrada de prefijos, comparación de pasaporte `OrdinalIgnoreCase`, validación de roles `Customer`/`Supplier`, identificación normalizada no vacía y máximo de 15 caracteres. Devolver `Result<string>` con códigos estables `BP_ROLE_INVALID`, `BP_IDENTIFICATION_REQUIRED` y `BP_SAP_CARD_CODE_TOO_LONG`.

- [ ] **Step 4: Add the complete prefix matrix tests**

Cubrir `CN`, `CE`, `PL`, `PE`, `C`, `P`, pasaporte en distinto casing, rol inválido y longitud mayor a 15. Ejecutar el comando del paso 2.

Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add src/Backend/NuanSystem.Application/Features/BusinessPartners/Policies tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerIdentityPolicyTests.cs tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerSapCardCodePolicyTests.cs
git commit -m "feat(business-partners): add identity and SAP code policies"
```

## Task 2: Agregar la base tenant forward-only y el diagnóstico previo

**Files:**

- Create: `database/sql/manual/check_business_partner_bidirectional_readiness.sql`
- Create: `database/sql/228_tenant_business_partner_bidirectional_foundation.sql`
- Create: `database/sql/230_tenant_business_partner_bidirectional_operations.sql`
- Modify: `src/Backend/NuanSystem.Persistence/Services/SqlServerTenantDatabaseInitializer.cs`
- Modify: `database/sql/README.md`
- Create: `tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerBidirectionalSqlContractTests.cs`

- [ ] **Step 1: Write failing SQL contract tests**

```csharp
[Fact]
public void TenantFoundation_IsForwardOnlyAndRoleAware()
{
    var sql = Read("database", "sql", "228_tenant_business_partner_bidirectional_foundation.sql");
    sql.Should().Contain("NormalizedIdentificationNumber")
        .And.Contain("CanonicalVersion")
        .And.Contain("MasterSyncStatus")
        .And.Contain("RowVersion")
        .And.Contain("BusinessPartnerSyncConflicts")
        .And.Contain("TargetCompanyId")
        .And.Contain("PartnerType, IdentificationTypeId, NormalizedIdentificationNumber")
        .And.NotContain("DROP TABLE")
        .And.NotContain("DELETE FROM dbo.BusinessPartners");
}
```

Añadir pruebas que exijan `GlobalId` en direcciones/contactos, `SchemaHistory` para las versiones `20260903.228` y `20260903.230`, los procedimientos operativos enumerados abajo y el orden 228 → 230 en el inicializador.

- [ ] **Step 2: Run the SQL contract tests and confirm the expected failure**

Run: `dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj --filter FullyQualifiedName~BusinessPartnerBidirectionalSqlContractTests`

Expected: FAIL with `FileNotFoundException` for migration 228.

- [ ] **Step 3: Create the read-only readiness report**

El script manual debe devolver, sin escribir datos:

- socios sin `GlobalId`, códigos internos duplicados y códigos que ya no caben en 50 caracteres;
- duplicados de identificación normalizada dentro del mismo rol;
- filas `Both` y sus referencias hijas;
- `SapCardCode` duplicados o mayores a 15 caracteres;
- direcciones/contactos sin identidad global;
- eventos `BusinessPartner` pendientes en `LocalOutbox`, `SyncOutbox` y `SyncInbox` cuando la tabla exista.

El script debe poder ejecutarse tanto en la central como en cada sucursal y no debe contener `INSERT`, `UPDATE`, `DELETE`, `MERGE`, `ALTER`, `CREATE` ni `DROP`.

- [ ] **Step 4: Implement migration 228**

Agregar de forma idempotente:

```sql
ALTER TABLE dbo.BusinessPartners ADD NormalizedIdentificationNumber nvarchar(50) NULL;
ALTER TABLE dbo.BusinessPartners ADD CanonicalVersion bigint NOT NULL
    CONSTRAINT DF_BusinessPartners_CanonicalVersion DEFAULT (1);
ALTER TABLE dbo.BusinessPartners ADD MasterSyncStatus varchar(20) NOT NULL
    CONSTRAINT DF_BusinessPartners_MasterSyncStatus DEFAULT ('Accepted');
ALTER TABLE dbo.BusinessPartners ADD MasterSyncMessage nvarchar(500) NULL;
ALTER TABLE dbo.BusinessPartners ADD RowVersion rowversion NOT NULL;
ALTER TABLE dbo.BusinessPartnerAddresses ADD GlobalId uniqueidentifier NULL;
ALTER TABLE dbo.BusinessPartnerContacts ADD GlobalId uniqueidentifier NULL;
ALTER TABLE dbo.LocalOutbox ADD TargetCompanyId int NULL;
ALTER TABLE dbo.LocalOutbox ADD CausationEventId uniqueidentifier NULL;
```

Backfill de forma determinista dentro de la ejecución: normalizar identificación con la misma regla de Task 1, asignar `NEWID()` únicamente a hijos sin identidad, marcar `Both` como `LegacyReview` y preservar exactamente `Code`, `SapCardCode`, `Id` y `GlobalId` existentes. Antes de sustituir `UX_BusinessPartners_Identification_Active`, lanzar `THROW 52028` si existen duplicados normalizados del mismo rol y abortar también si existen `SapCardCode` duplicados no vacíos. Después del backfill, convertir identificación normalizada y los `GlobalId` hijos en `NOT NULL`. Crear índices filtrados únicos por rol/identificación, por `SapCardCode`, por `BusinessPartnerAddresses.GlobalId` y por `BusinessPartnerContacts.GlobalId`. Agregar checks para estados, versiones positivas y JSON válido donde corresponda.

Crear `BusinessPartnerSyncConflicts` con `ProposalEventId` único, socio/origen/versiones, `BaseSnapshotJson`, `ProposedSnapshotJson`, `CanonicalSnapshotJson`, `ConflictFieldsJson`, `Status`, `Resolution`, `ResolutionReason`, auditoría y `RowVersion`.

- [ ] **Step 5: Implement migration 230 and initializer registration**

Crear procedimientos con parámetros tipados y transacciones controladas:

- `SP_NA_GET_BUSINESSPARTNER_CANONICAL_FORUPDATE`
- `SP_NA_POST_BUSINESSPARTNER_PROPOSAL_ACCEPT`
- `SP_NA_POST_BUSINESSPARTNER_PROPOSAL_CONFLICT`
- `SP_NA_POST_BUSINESSPARTNER_PROPOSAL_REJECT`
- `SP_NA_POST_BUSINESSPARTNER_CANONICAL_APPLY`
- `SP_NA_POST_BUSINESSPARTNER_PROPOSAL_RESULT_APPLY`
- `SP_NA_GET_BUSINESSPARTNER_SYNCCONFLICTS_LISTAR`
- `SP_NA_GET_BUSINESSPARTNER_SYNCCONFLICT_BUSCARPORID`
- `SP_NA_POST_BUSINESSPARTNER_SYNCCONFLICT_RESOLVER`

Los procedimientos de aceptación y resolución deben modificar socio/hijos, `SyncInbox`, conflicto y `LocalOutbox` en una sola transacción tenant. El aplicador canónico debe comparar `CanonicalVersion`, considerar una versión igual como idempotente, ignorar una versión menor y no insertar `LocalOutbox`.

Registrar 228 y 230 al final de `ExecuteOptionalTenantScriptsAsync`, actualizar `database/sql/README.md` y ejecutar la prueba del paso 2.

Expected: PASS.

- [ ] **Step 6: Commit**

```bash
git add database/sql/manual/check_business_partner_bidirectional_readiness.sql database/sql/228_tenant_business_partner_bidirectional_foundation.sql database/sql/230_tenant_business_partner_bidirectional_operations.sql database/sql/README.md src/Backend/NuanSystem.Persistence/Services/SqlServerTenantDatabaseInitializer.cs tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerBidirectionalSqlContractTests.cs
git commit -m "feat(database): add business partner bidirectional tenant foundation"
```

## Task 3: Agregar gobierno Master, direcciones de perfil y rutas seguras

**Files:**

- Create: `database/sql/229_master_business_partner_bidirectional_governance.sql`
- Modify: `src/Backend/NuanSystem.Persistence/Services/SqlServerMasterDatabaseInitializer.cs`
- Modify: `database/sql/README.md`
- Modify: `tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerBidirectionalSqlContractTests.cs`
- Modify: `tests/NuanSystem.Application.Tests/Features/Sync/SyncConfigurationContractTests.cs`

- [ ] **Step 1: Extend the failing SQL contracts**

```csharp
[Fact]
public void MasterGovernance_RegistersDirectionsPolicyAndClosedRouting()
{
    var sql = Read("database", "sql", "229_master_business_partner_bidirectional_governance.sql");
    sql.Should().Contain("BusinessPartnerSapCodePolicies")
        .And.Contain("BranchToMaster")
        .And.Contain("BusinessPartnerProposal")
        .And.Contain("BusinessPartnerProposalResult")
        .And.Contain("TargetCompanyId")
        .And.Contain("ParentCompanyId")
        .And.Contain("SYNC.BUSINESS_PARTNER_CONFLICTS.VIEW")
        .And.Contain("SYNC.BUSINESS_PARTNER_CONFLICTS.RESOLVE");
}
```

- [ ] **Step 2: Run the focused contract tests and confirm the expected failure**

Run: `dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj --filter "FullyQualifiedName~BusinessPartnerBidirectionalSqlContractTests|FullyQualifiedName~SyncConfigurationContractTests"`

Expected: FAIL because migration 229 and `BranchToMaster` registration do not exist.

- [ ] **Step 3: Implement the Master policy table and security**

Crear `BusinessPartnerSapCodePolicies` con una fila como máximo por `CompanyId`:

```sql
CREATE TABLE dbo.BusinessPartnerSapCodePolicies
(
    CompanyId int NOT NULL CONSTRAINT PK_BusinessPartnerSapCodePolicies PRIMARY KEY,
    IsEnabled bit NOT NULL CONSTRAINT DF_BusinessPartnerSapCodePolicies_IsEnabled DEFAULT (0),
    PrefixMode varchar(20) NOT NULL,
    PassportIdentificationTypeCode nvarchar(30) NOT NULL,
    UpdatedByUserId int NULL,
    UpdatedByUserName nvarchar(120) NULL,
    UpdatedAt datetime2(0) NOT NULL CONSTRAINT DF_BusinessPartnerSapCodePolicies_UpdatedAt DEFAULT SYSUTCDATETIME(),
    RowVersion rowversion NOT NULL,
    CONSTRAINT FK_BusinessPartnerSapCodePolicies_Companies FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(Id),
    CONSTRAINT CK_BusinessPartnerSapCodePolicies_PrefixMode CHECK (PrefixMode IN ('NationalForeign','RoleOnly'))
);
```

Agregar `CausationEventId uniqueidentifier NULL` a `SyncOutbox` para conservar la relación propuesta → canónico/resultado durante la promoción. Agregar procedimientos GET/UPSERT de política que sólo acepten una empresa activa `IsMaster = 1`, no devuelvan secretos y auditen la actualización. Crear los permisos nuevos sin concederlos automáticamente salvo al rol `ADMIN` siguiendo el patrón de seguridad vigente.

- [ ] **Step 4: Extend profile direction and routing without activating anything**

Registrar las definiciones inactivas `BusinessPartnerProposal` y `BusinessPartnerProposalResult`. Conservar `BusinessPartner` como canónico. Modificar checks/procedimientos para aceptar:

- `MasterToBranch`: `SyncProfiles.CompanyId` es la central y `SyncProfileBranches.BranchCompanyId` es destino.
- `BranchToMaster`: `SyncProfiles.CompanyId` es la central y `SyncProfileBranches.BranchCompanyId` es origen; el destino devuelto es siempre `SyncProfiles.CompanyId` y debe coincidir con `Companies.ParentCompanyId` de la sucursal.

Extender `SP_NA_GET_SYNCROUTINGTARGETS` con `@TargetCompanyId int = NULL`. Si viene informado, exigir coincidencia exacta después de validar jerarquía y dirección. Para `BusinessPartnerProposal`, rechazar cualquier ruta que no sea hijo → padre. Para `BusinessPartnerProposalResult`, rechazar cualquier ruta que no sea padre → hijo exacto. No insertar perfiles, sucursales activas ni flags habilitados.

- [ ] **Step 5: Register migration 229 and rerun tests**

Registrar 229 después de 227 en `SqlServerMasterDatabaseInitializer`, documentarla en `database/sql/README.md` y ejecutar el comando del paso 2.

Expected: PASS.

- [ ] **Step 6: Commit**

```bash
git add database/sql/229_master_business_partner_bidirectional_governance.sql database/sql/README.md src/Backend/NuanSystem.Persistence/Services/SqlServerMasterDatabaseInitializer.cs tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerBidirectionalSqlContractTests.cs tests/NuanSystem.Application.Tests/Features/Sync/SyncConfigurationContractTests.cs
git commit -m "feat(sync): add business partner central branch governance"
```

## Task 4: Exponer y validar la política central de códigos SAP

**Files:**

- Create: `src/Backend/NuanSystem.Application/Abstractions/Sap/IBusinessPartnerSapCodePolicyRepository.cs`
- Create: `src/Backend/NuanSystem.Application/Features/BusinessPartners/SapCodes/BusinessPartnerSapCodePolicyDtos.cs`
- Create: `src/Backend/NuanSystem.Application/Features/BusinessPartners/SapCodes/GetBusinessPartnerSapCodePolicyQuery.cs`
- Create: `src/Backend/NuanSystem.Application/Features/BusinessPartners/SapCodes/GetBusinessPartnerSapCodePolicyQueryHandler.cs`
- Create: `src/Backend/NuanSystem.Application/Features/BusinessPartners/SapCodes/UpdateBusinessPartnerSapCodePolicyCommand.cs`
- Create: `src/Backend/NuanSystem.Application/Features/BusinessPartners/SapCodes/UpdateBusinessPartnerSapCodePolicyCommandValidator.cs`
- Create: `src/Backend/NuanSystem.Application/Features/BusinessPartners/SapCodes/UpdateBusinessPartnerSapCodePolicyCommandHandler.cs`
- Create: `src/Backend/NuanSystem.Persistence/Repositories/BusinessPartnerSapCodePolicyRepository.cs`
- Modify: `src/Backend/NuanSystem.Application/DependencyInjection/ApplicationServiceRegistration.cs`
- Modify: `src/Backend/NuanSystem.Persistence/DependencyInjection/PersistenceServiceRegistration.cs`
- Modify: `src/Backend/NuanSystem.Api/Endpoints/SapEndpoints.cs`
- Create: `tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerSapCodePolicyUseCaseTests.cs`
- Create: `tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerSapCodePolicyApiContractTests.cs`

- [ ] **Step 1: Write failing use-case and API contract tests**

```csharp
[Fact]
public async Task Update_RejectsBranchCompanyContext()
{
    companyContext.CurrentCompany.Returns(BranchCompany(parentCompanyId: 10));
    var result = await handler.Handle(
        new UpdateBusinessPartnerSapCodePolicyCommand(
            true,
            "RoleOnly",
            "PASSPORT",
            Convert.ToBase64String([1, 2, 3]),
            null,
            null),
        CancellationToken.None);
    result.IsSuccess.Should().BeFalse();
    result.Errors.Should().Contain(error => error.Code == "BP_SAP_CODE_POLICY_MASTER_REQUIRED");
}
```

La prueba de contrato debe exigir `GET` y `PUT /api/sap/settings/business-partner-codes`, `SapRead` para GET, `SapManage` para PUT, identidad de auditoría tomada de claims y ninguna propiedad de contraseña.

- [ ] **Step 2: Run the focused tests and confirm the expected failure**

Run: `dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj --filter "FullyQualifiedName~BusinessPartnerSapCodePolicyUseCaseTests|FullyQualifiedName~BusinessPartnerSapCodePolicyApiContractTests"`

Expected: FAIL because commands, handlers and endpoints do not exist.

- [ ] **Step 3: Implement contracts, validation and persistence**

Usar el siguiente contrato público:

```csharp
public sealed record BusinessPartnerSapCodePolicyDto(
    int CompanyId,
    bool IsEnabled,
    string PrefixMode,
    string PassportIdentificationTypeCode,
    string CustomerNationalExample,
    string CustomerForeignExample,
    string SupplierNationalExample,
    string SupplierForeignExample,
    string RowVersion);

public sealed record UpdateBusinessPartnerSapCodePolicyCommand(
    bool IsEnabled,
    string PrefixMode,
    string PassportIdentificationTypeCode,
    string? ExpectedRowVersion,
    [property: JsonIgnore] int? AuditUserId,
    [property: JsonIgnore] string? AuditUserName) : ICommand<BusinessPartnerSapCodePolicyDto>;
```

Validar modo cerrado, código de pasaporte no vacío/máximo 30 y `ExpectedRowVersion` base64 cuando la fila ya existe. Para la primera creación la versión esperada es `null`; si la fila aparece concurrentemente, devolver `BP_SAP_CODE_POLICY_CONCURRENCY_CONFLICT`. El handler usa exclusivamente `ICompanyContext.CurrentCompany.CompanyId`, exige `IsMaster`, nunca acepta un `CompanyId` del body y mapea ausencia de fila a política deshabilitada `NationalForeign`/`PASSPORT`.

- [ ] **Step 4: Map endpoints and run tests**

Agregar ambos endpoints a `SapEndpoints`, proteger GET con `PermissionCodes.SapRead` y PUT con `PermissionCodes.SapManage`, registrar servicios/repository y ejecutar el comando del paso 2.

Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add src/Backend/NuanSystem.Application/Abstractions/Sap/IBusinessPartnerSapCodePolicyRepository.cs src/Backend/NuanSystem.Application/Features/BusinessPartners/SapCodes src/Backend/NuanSystem.Persistence/Repositories/BusinessPartnerSapCodePolicyRepository.cs src/Backend/NuanSystem.Application/DependencyInjection/ApplicationServiceRegistration.cs src/Backend/NuanSystem.Persistence/DependencyInjection/PersistenceServiceRegistration.cs src/Backend/NuanSystem.Api/Endpoints/SapEndpoints.cs tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerSapCodePolicyUseCaseTests.cs tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerSapCodePolicyApiContractTests.cs
git commit -m "feat(api): manage business partner SAP code policy"
```

## Task 5: Endurecer CRUD, identidad, roles y concurrencia local

**Files:**

- Modify: `src/Backend/NuanSystem.Application/Features/BusinessPartners/Dtos/BusinessPartnerDtos.cs`
- Modify: `src/Backend/NuanSystem.Application/Features/BusinessPartners/Commands/BusinessPartnerCommands.cs`
- Modify: `src/Backend/NuanSystem.Application/Features/BusinessPartners/Commands/BusinessPartnerCommandValidator.cs`
- Modify: `src/Backend/NuanSystem.Application/Features/BusinessPartners/Commands/CreateBusinessPartnerCommandHandler.cs`
- Modify: `src/Backend/NuanSystem.Application/Features/BusinessPartners/Commands/UpdateBusinessPartnerCommandHandler.cs`
- Modify: `src/Backend/NuanSystem.Application/Features/BusinessPartners/Commands/DeleteBusinessPartnerCommandHandler.cs`
- Create: `src/Backend/NuanSystem.Application/Features/BusinessPartners/Policies/BusinessPartnerWritePolicy.cs`
- Modify: `src/Backend/NuanSystem.Application/Features/BusinessPartners/Queries/GetBusinessPartnerLookupsQueryHandler.cs`
- Modify: `src/Backend/NuanSystem.Application/Abstractions/Data/IBusinessPartnerRepository.cs`
- Modify: `src/Backend/NuanSystem.Persistence/Repositories/BusinessPartnerRepository.cs`
- Modify: `src/Backend/NuanSystem.Api/Endpoints/BusinessPartnerEndpoints.cs`
- Modify: `tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerSyncPublishingTests.cs`
- Create: `tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerCommandPolicyTests.cs`

- [ ] **Step 1: Write failing command-policy tests**

Cubrir primero:

```csharp
[Fact]
public async Task Create_GeneratesInternalCodeAndNormalizesIdentification()
{
    CreateBusinessPartnerData? saved = null;
    repository.CreateAsync(
        Arg.Do<CreateBusinessPartnerData>(data => saved = data),
        transaction.Connection,
        transaction.Transaction,
        Arg.Any<CancellationToken>()).Returns(44);

    var result = await handler.Handle(CreateCustomer(" 09.999-999 99001 "), CancellationToken.None);

    result.IsSuccess.Should().BeTrue();
    saved!.Code.Should().StartWith("BP-");
    saved.NormalizedIdentificationNumber.Should().Be("0999999999001");
}
```

Añadir pruebas para rechazo de `Both`, duplicado en el mismo rol, misma identificación en rol diferente, identificación/rol inmutables, `RowVersion` obsoleta, campos protegidos en sucursal sincronizada, edición completa en `Standalone`, `LegacyReview` bloqueado, segunda propuesta bloqueada y eliminación no soportada desde una sucursal sincronizada.

- [ ] **Step 2: Run the command tests and confirm the expected failure**

Run: `dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj --filter "FullyQualifiedName~BusinessPartnerCommandPolicyTests|FullyQualifiedName~BusinessPartnerSyncPublishingTests"`

Expected: FAIL because current create trusts `Code`, uniqueness is not role-aware and DTOs have no versions/status.

- [ ] **Step 3: Evolve the API/application contracts**

Aplicar estos cambios concretos:

- eliminar `Code` de `CreateBusinessPartnerCommand`; `SaveBusinessPartnerRequest` se actualiza en Task 11 al desplegar el cliente;
- eliminar `Code`, `PartnerType`, `IdentificationTypeId` e `IdentificationNumber` de `UpdateBusinessPartnerCommand`; el handler toma los valores inmutables del registro cargado;
- eliminar de create/update los campos administrados SAP (`SapCardCode`, `SapCardType`, `SapSyncStatus`, `SapLastSyncAt`, `SapLastError`, `SapEnabled`, `SapMode`, `SapCompanyCode`, `SapRetryCount`, `SyncAsSupplier`, `AllowManualSapRetry`, `RequiresApprovalBeforeSapSync`);
- agregar `ExpectedRowVersion` base64 a update/delete;
- agregar a `BusinessPartnerDto`: `NormalizedIdentificationNumber`, `CanonicalVersion`, `RowVersion`, `MasterSyncStatus`, `MasterSyncMessage`;
- agregar `Guid? GlobalId` a `SaveBusinessPartnerAddressData` y `SaveBusinessPartnerContactData`, y `Guid GlobalId` a sus DTOs de lectura;
- agregar a los DTOs de dirección `ProvinceCode` y `CityCode`, y a los DTOs de contacto `ContactTypeCode` y `ContactChannelCode`, para no transportar IDs locales;
- agregar `NormalizedIdentificationNumber`, `CanonicalVersion` y `MasterSyncStatus` a los records de persistencia, y convertir el `ExpectedRowVersion` API a `byte[]` antes de llamar al repository.
- agregar `BusinessPartnerEditPolicyDto` a `BusinessPartnerLookupsDto`; el query handler lo calcula desde `ICompanyContext` para distinguir sucursal sincronizada de central/standalone.

La API debe ignorar propiedades JSON antiguas adicionales por compatibilidad de despliegue, pero nunca volver a mapearlas a los comandos.

- [ ] **Step 4: Implement policy and handlers transactionally**

Cambiar la firma de unicidad a:

```csharp
Task<bool> ExistsByIdentificationAsync(
    string partnerType,
    int identificationTypeId,
    string normalizedIdentificationNumber,
    int? excludingId = null,
    CancellationToken cancellationToken = default);
```

Mantener la sobrecarga transaccional equivalente. Crear `GlobalId` y `Code` en Application antes del insert. Para hijos sin `GlobalId`, generarlo una vez antes de persistir. En sucursal sincronizada, guardar la operación de usuario con `PendingMaster` y `CanonicalVersion = 0` para alta o conservar la versión base para update. En una central sincronizada, insertar con versión 1 e incrementar `CanonicalVersion` en cada update/delete; en standalone, conservar `Accepted` sin semántica de distribución. Cuando la central tenga una política habilitada, calcular y reservar `SapCardCode` dentro del insert tenant; una sucursal nunca lo calcula y un código central ya confirmado se preserva aunque cambie la política. Requerir coincidencia de `RowVersion` en update/delete y traducir cero filas a `BP_CONCURRENCY_CONFLICT`. Rechazar delete desde una sucursal sincronizada con `BP_SYNC_DELETE_NOT_SUPPORTED`; conservar el delete existente en central/standalone.

`BusinessPartnerWritePolicy` debe comparar el request con el registro actual y devolver rutas protegidas alteradas. En sucursal sincronizada sólo acepta nombre, nombre comercial, teléfono, correo, direcciones y contactos. No confiar en controles deshabilitados de WinForms.

- [ ] **Step 5: Update repository mappings/procedures and rerun tests**

Mapear los campos nuevos, usar los procedimientos de 230, convertir `rowversion` a base64 en el límite API y comprobar que no se cambia `Code` histórico. Ejecutar el comando del paso 2.

Expected: PASS.

- [ ] **Step 6: Commit**

```bash
git add src/Backend/NuanSystem.Application/Features/BusinessPartners/Dtos/BusinessPartnerDtos.cs src/Backend/NuanSystem.Application/Features/BusinessPartners/Commands/BusinessPartnerCommands.cs src/Backend/NuanSystem.Application/Features/BusinessPartners/Commands/BusinessPartnerCommandValidator.cs src/Backend/NuanSystem.Application/Features/BusinessPartners/Commands/CreateBusinessPartnerCommandHandler.cs src/Backend/NuanSystem.Application/Features/BusinessPartners/Commands/UpdateBusinessPartnerCommandHandler.cs src/Backend/NuanSystem.Application/Features/BusinessPartners/Commands/DeleteBusinessPartnerCommandHandler.cs src/Backend/NuanSystem.Application/Features/BusinessPartners/Policies/BusinessPartnerWritePolicy.cs src/Backend/NuanSystem.Application/Features/BusinessPartners/Queries/GetBusinessPartnerLookupsQueryHandler.cs src/Backend/NuanSystem.Application/Abstractions/Data/IBusinessPartnerRepository.cs src/Backend/NuanSystem.Persistence/Repositories/BusinessPartnerRepository.cs src/Backend/NuanSystem.Api/Endpoints/BusinessPartnerEndpoints.cs tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerSyncPublishingTests.cs tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerCommandPolicyTests.cs
git commit -m "feat(business-partners): enforce canonical identity and local concurrency"
```

## Task 6: Definir snapshots versionados y conciliación de tres vías

**Files:**

- Create: `src/Backend/NuanSystem.Application/Features/BusinessPartners/Sync/BusinessPartnerSyncContracts.cs`
- Create: `src/Backend/NuanSystem.Application/Features/BusinessPartners/Sync/BusinessPartnerSnapshotFactory.cs`
- Create: `src/Backend/NuanSystem.Application/Features/BusinessPartners/Sync/BusinessPartnerThreeWayMergeService.cs`
- Create: `tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerThreeWayMergeTests.cs`
- Create: `tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerSyncPayloadContractTests.cs`

- [ ] **Step 1: Write failing merge tests**

```csharp
[Fact]
public void Merge_AcceptsDisjointBranchAndCentralChanges()
{
    var result = service.Merge(
        Base(name: "Base", phone: "111"),
        Proposed(name: "Sucursal", phone: "111"),
        Current(name: "Base", phone: "222"));

    result.Status.Should().Be(BusinessPartnerMergeStatus.Accepted);
    result.Merged!.Name.Should().Be("Sucursal");
    result.Merged.Phone.Should().Be("222");
}

[Fact]
public void Merge_ReportsSameFieldWithDifferentValues()
{
    var result = service.Merge(
        Base(name: "Base"),
        Proposed(name: "Sucursal"),
        Current(name: "Central"));
    result.ConflictFields.Should().ContainSingle().Which.Should().Be("Name");
}
```

- [ ] **Step 2: Run the merge tests and confirm the expected failure**

Run: `dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj --filter "FullyQualifiedName~BusinessPartnerThreeWayMergeTests|FullyQualifiedName~BusinessPartnerSyncPayloadContractTests"`

Expected: FAIL because snapshots and merge service do not exist.

- [ ] **Step 3: Implement immutable, versioned contracts**

```csharp
public sealed record BusinessPartnerProposalPayloadV1(
    int SchemaVersion,
    Guid GlobalId,
    string Code,
    string PartnerType,
    string IdentificationTypeCode,
    string IdentificationNumber,
    string NormalizedIdentificationNumber,
    long BaseCanonicalVersion,
    int? OriginUserId,
    string? OriginUserName,
    BusinessPartnerCanonicalSnapshot? Base,
    BusinessPartnerCanonicalSnapshot Proposed,
    IReadOnlyCollection<string> ChangedFields);

public sealed record BusinessPartnerCanonicalPayloadV2(
    int SchemaVersion,
    long CanonicalVersion,
    int? OriginCompanyId,
    Guid? CausationEventId,
    BusinessPartnerCanonicalSnapshot Partner);

public sealed record BusinessPartnerProposalResultPayloadV1(
    int SchemaVersion,
    Guid GlobalId,
    Guid ProposalEventId,
    string Status,
    string? Message,
    long CanonicalVersion,
    BusinessPartnerCanonicalSnapshot? Canonical);

public sealed record BusinessPartnerCanonicalSnapshot(
    Guid GlobalId,
    string Code,
    string Name,
    string? CommercialName,
    string PartnerType,
    string IdentificationTypeCode,
    string IdentificationNumber,
    string NormalizedIdentificationNumber,
    string? Email,
    string? Phone,
    string? SapCardCode,
    bool IsActive,
    IReadOnlyCollection<BusinessPartnerAddressSnapshot> Addresses,
    IReadOnlyCollection<BusinessPartnerContactSnapshot> Contacts);

public sealed record BusinessPartnerAddressSnapshot(
    Guid GlobalId,
    string AddressType,
    string Line1,
    string? Line2,
    string? CountryCode,
    string? ProvinceCode,
    string? CityCode,
    string? PostalCode,
    decimal? Latitude,
    decimal? Longitude,
    bool IsPrimary,
    bool IsActive);

public sealed record BusinessPartnerContactSnapshot(
    Guid GlobalId,
    string? ContactTypeCode,
    string? ContactChannelCode,
    string Name,
    string? Position,
    string? Department,
    string? Phone,
    string? Extension,
    string? Mobile,
    string? Email,
    string? Language,
    bool ReceivesNotifications,
    bool IsPrimary,
    bool IsActive,
    string? Notes);
```

El snapshot contiene el conjunto canónico completo aprobado para este incremento, incluyendo `SapCardCode`, y nunca contiene `RowVersion`, contraseñas, connection strings, campos de banco, contabilidad, retención ni credenciales SAP. Las referencias geográficas de dirección y los tipos/canales de contacto viajan por códigos estables, no por IDs locales; el aplicador resuelve esos códigos dentro de cada tenant. Direcciones/contactos se ordenan por `GlobalId` antes de serializar para obtener payload determinista.

- [ ] **Step 4: Implement field-path merge**

Comparar escalares por ruta estable y colecciones por `GlobalId`. Rutas de hijos usan `Addresses/{GlobalId:N}/Line1` y `Contacts/{GlobalId:N}/Email`. Aplicar:

1. sólo propuesta cambió: aceptar propuesta;
2. sólo central cambió: conservar central;
3. ambos cambiaron al mismo valor: aceptar;
4. ambos cambiaron distinto: registrar ruta en conflicto;
5. ruta fuera de allowlist: resultado `Rejected` con `BP_PROTECTED_FIELD`.

No mutar los snapshots de entrada.

- [ ] **Step 5: Complete payload security/determinism tests and rerun**

Serializar ejemplos con `SyncEventPayloadFactory` y afirmar ausencia de `Password`, `ConnectionString`, cuentas bancarias, cuentas contables y retenciones. Ejecutar el comando del paso 2.

Expected: PASS.

- [ ] **Step 6: Commit**

```bash
git add src/Backend/NuanSystem.Application/Features/BusinessPartners/Sync/BusinessPartnerSyncContracts.cs src/Backend/NuanSystem.Application/Features/BusinessPartners/Sync/BusinessPartnerSnapshotFactory.cs src/Backend/NuanSystem.Application/Features/BusinessPartners/Sync/BusinessPartnerThreeWayMergeService.cs tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerThreeWayMergeTests.cs tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerSyncPayloadContractTests.cs
git commit -m "feat(sync): add versioned business partner merge contracts"
```

## Task 7: Publicar desde central o sucursal y enrutar al único destino permitido

**Files:**

- Modify: `src/Backend/NuanSystem.Application/Features/Sync/Dtos/LocalSyncOutboxDtos.cs`
- Modify: `src/Backend/NuanSystem.Application/Features/Sync/Dtos/SyncRoutingDtos.cs`
- Modify: `src/Backend/NuanSystem.Application/Features/Sync/Configuration/SyncMasterBranchEntityCatalog.cs`
- Modify: `src/Backend/NuanSystem.Application/Features/BusinessPartners/Commands/BusinessPartnerSyncEventFactory.cs`
- Modify: `src/Backend/NuanSystem.Application/Features/BusinessPartners/Commands/BusinessPartnerLocalOutboxWriter.cs`
- Modify: `src/Backend/NuanSystem.Application/Abstractions/Sync/IBusinessPartnerLocalOutboxWriter.cs`
- Modify: `src/Backend/NuanSystem.Application/Features/Sync/Services/LocalSyncOutboxPromotionService.cs`
- Modify: `src/Backend/NuanSystem.Persistence/Repositories/Sync/LocalSyncOutboxRepository.cs`
- Modify: `src/Backend/NuanSystem.Persistence/Repositories/Sync/SyncRoutingRepository.cs`
- Modify: `src/Backend/NuanSystem.Persistence/Repositories/Sync/SyncOutboxPromotionRepository.cs`
- Modify: `src/Backend/NuanSystem.Application/Features/Sync/Configuration/Services/SyncProfileValidationService.cs`
- Modify: `src/Backend/NuanSystem.Application/Features/Sync/Configuration/Commands/SyncConfigurationCommandValidators.cs`
- Modify: `src/Backend/NuanSystem.Application/Features/Sync/Execution/Services/SyncProfileExecutionService.cs`
- Modify: `tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerSyncPublishingTests.cs`
- Modify: `tests/NuanSystem.Application.Tests/Features/Sync/LocalSyncOutboxRelayEntityScopeTests.cs`
- Modify: `tests/NuanSystem.Application.Tests/Features/Sync/SyncProfileValidationServiceTests.cs`
- Modify: `tests/NuanSystem.Application.Tests/Features/Sync/SyncManualActionTests.cs`
- Create: `tests/NuanSystem.Application.Tests/Features/Sync/BusinessPartnerDirectionalRoutingTests.cs`

- [ ] **Step 1: Write failing source/destination tests**

```csharp
[Fact]
public async Task BranchWriter_CreatesProposalOnlyForItsParent()
{
    companyContext.CurrentCompany.Returns(BranchCompany(companyId: 21, parentCompanyId: 10));
    await writer.EnqueueAsync(WriteRequest(), connection, transaction);

    await localOutbox.Received(1).CreateAsync(
        Arg.Is<CreateLocalSyncOutboxData>(data =>
            data.EntityName == "BusinessPartnerProposal" &&
            data.CompanyId == 21 &&
            data.TargetCompanyId == 10),
        connection,
        transaction,
        Arg.Any<CancellationToken>());
}
```

Añadir pruebas: central produce `BusinessPartner` con target nulo; `Standalone` o sync deshabilitado no publica; `LegacyReview` no publica; relay descubre centrales y sucursales activas; ruta sucursal→hermana devuelve cero targets; `TargetCompanyId` equivocado devuelve cero targets; ausencia de ruta activa deja `LocalOutbox` en retry sin promoverlo a un evento sin targets; ejecución administrativa de un perfil `BranchToMaster` queda bloqueada.

- [ ] **Step 2: Run focused publishing/routing tests and confirm the expected failure**

Run: `dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj --filter "FullyQualifiedName~BusinessPartnerSyncPublishingTests|FullyQualifiedName~BusinessPartnerDirectionalRoutingTests|FullyQualifiedName~LocalSyncOutboxRelayEntityScopeTests|FullyQualifiedName~SyncProfileValidationServiceTests|FullyQualifiedName~SyncManualActionTests"`

Expected: FAIL because the writer still skips non-master companies and outbox/routing have no `TargetCompanyId`.

- [ ] **Step 3: Extend outbox DTOs and repositories**

```csharp
public sealed record CreateLocalSyncOutboxData(
    Guid EventId,
    int CompanyId,
    string EntityName,
    Guid EntityGlobalId,
    string? EntityCode,
    SyncOperation Operation,
    string PayloadJson,
    int MaxAttempts = 3,
    int? TargetCompanyId = null,
    Guid? CausationEventId = null);
```

Propagar ambos campos al claim, promoción y `SyncRoutingContext`. Agregar `Deferred` a `SyncOutboxPromotionStatus`: cuando no exista una ruta activa, `LocalSyncOutboxPromotionService` devuelve ese estado sin crear `SyncOutbox`, y el relay programa retry. Cambiar `GetRelayCompaniesAsync` para devolver todas las empresas activas, no eliminadas y `SyncEnabled = 1`; mantener allowlist por entidad y fail-closed del worker.

- [ ] **Step 4: Make BusinessPartner writer direction-aware**

Cambiar la entrada del writer a un record `BusinessPartnerOutboxWriteRequest` con `Current`, `Base`, `Operation`, `OriginUserId`, `OriginUserName` y `CausationEventId`. Si el contexto es:

- sucursal sincronizada: crear propuesta v1, `TargetCompanyId = ParentCompanyId`, estado local `PendingMaster`;
- empresa central sincronizada: crear canónico v2, target nulo para fan-out;
- standalone/sync deshabilitado: no crear evento.

Rechazar sucursal sin padre con `BP_BRANCH_PARENT_REQUIRED`. Mantener la creación de socio y `LocalOutbox` en la misma transacción de los handlers.

- [ ] **Step 5: Extend routing/profile validation**

Permitir `BranchToMaster` sólo con `BusinessPartnerProposal`, estrategia `CentralReview`, ejecución `Incremental`, programación manual, empresa del perfil central y sucursales hijas. Permitir `BusinessPartnerProposalResult` en `MasterToBranch` con destino explícito. Exigir política de código habilitada al activar la propuesta. Mantener `Bidirectional` fuera de la UI para evitar una semántica ambigua; los dos perfiles explícitos representan cada dirección. El scheduler y `SyncProfileExecutionService` no deben ejecutar perfiles `BranchToMaster` como Full/Manual: esos perfiles sólo autorizan la ruta del relay y devuelven `SYNC_BRANCH_TO_MASTER_INCREMENTAL_ONLY` ante ejecución administrativa.

Ejecutar el comando del paso 2.

Expected: PASS.

- [ ] **Step 6: Commit**

```bash
git add src/Backend/NuanSystem.Application/Features/Sync/Dtos/LocalSyncOutboxDtos.cs src/Backend/NuanSystem.Application/Features/Sync/Dtos/SyncRoutingDtos.cs src/Backend/NuanSystem.Application/Features/Sync/Configuration/SyncMasterBranchEntityCatalog.cs src/Backend/NuanSystem.Application/Features/BusinessPartners/Commands/BusinessPartnerSyncEventFactory.cs src/Backend/NuanSystem.Application/Features/BusinessPartners/Commands/BusinessPartnerLocalOutboxWriter.cs src/Backend/NuanSystem.Application/Abstractions/Sync/IBusinessPartnerLocalOutboxWriter.cs src/Backend/NuanSystem.Application/Features/Sync/Services/LocalSyncOutboxPromotionService.cs src/Backend/NuanSystem.Application/Features/Sync/Configuration/Services/SyncProfileValidationService.cs src/Backend/NuanSystem.Application/Features/Sync/Configuration/Commands/SyncConfigurationCommandValidators.cs src/Backend/NuanSystem.Application/Features/Sync/Execution/Services/SyncProfileExecutionService.cs src/Backend/NuanSystem.Persistence/Repositories/Sync/LocalSyncOutboxRepository.cs src/Backend/NuanSystem.Persistence/Repositories/Sync/SyncRoutingRepository.cs src/Backend/NuanSystem.Persistence/Repositories/Sync/SyncOutboxPromotionRepository.cs tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerSyncPublishingTests.cs tests/NuanSystem.Application.Tests/Features/Sync/LocalSyncOutboxRelayEntityScopeTests.cs tests/NuanSystem.Application.Tests/Features/Sync/SyncProfileValidationServiceTests.cs tests/NuanSystem.Application.Tests/Features/Sync/SyncManualActionTests.cs tests/NuanSystem.Application.Tests/Features/Sync/BusinessPartnerDirectionalRoutingTests.cs
git commit -m "feat(sync): route business partner proposals to central company"
```

## Task 8: Aceptar, rechazar o registrar conflictos en la empresa central

**Files:**

- Create: `src/Backend/NuanSystem.Application/Abstractions/Sync/IBusinessPartnerProposalApplyRepository.cs`
- Create: `src/Backend/NuanSystem.Application/Features/BusinessPartners/Sync/BusinessPartnerProposalApplyModels.cs`
- Create: `src/Backend/NuanSystem.Persistence/Repositories/Sync/BusinessPartnerProposalApplyRepository.cs`
- Create: `src/Backend/NuanSystem.MasterBranchSyncWorker/Services/BusinessPartnerProposalSyncEventApplier.cs`
- Modify: `src/Backend/NuanSystem.Persistence/DependencyInjection/PersistenceServiceRegistration.cs`
- Modify: `src/Backend/NuanSystem.MasterBranchSyncWorker/Program.cs`
- Create: `tests/NuanSystem.Application.Tests/Features/Sync/BusinessPartnerProposalSyncEventApplierTests.cs`
- Create: `tests/NuanSystem.Application.Tests/Features/Sync/BusinessPartnerProposalApplyRepositoryContractTests.cs`

- [ ] **Step 1: Write failing proposal-applier tests**

Cubrir payload inválido, schema distinto de 1, target faltante, target no central, central distinta de `ParentCompanyId`, evento repetido, alta aceptada, update aceptado, duplicado del mismo rol rechazado, identificación en rol distinto permitida, `Both` rechazado, identificación normalizada manipulada, código interno inválido en un alta, código SAP demasiado largo rechazado, referencia estable aún no disponible, conflicto persistido y excepción SQL reprocesable.

```csharp
[Fact]
public async Task Apply_TargetMustBeTheSourceBranchParent()
{
    var result = await applier.ApplyAsync(Context(source: 21, target: 99));
    result.Applied.Should().BeFalse();
    result.Terminal.Should().BeTrue();
    result.ErrorCode.Should().Be("BP_SYNC_PARENT_MISMATCH");
}
```

- [ ] **Step 2: Run focused proposal tests and confirm the expected failure**

Run: `dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj --filter "FullyQualifiedName~BusinessPartnerProposalSyncEventApplierTests|FullyQualifiedName~BusinessPartnerProposalApplyRepositoryContractTests"`

Expected: FAIL because proposal applier and repository do not exist.

- [ ] **Step 3: Implement the guarded worker applier**

El applier debe:

1. aceptar sólo `BusinessPartnerProposal` schema 1;
2. validar `EntityGlobalId`;
3. resolver empresa origen y destino con `ICompanyResolver`;
4. exigir origen no master, destino master, `origin.ParentCompanyId == target.CompanyId` y ambas con sync habilitado;
5. delegar una sola transacción al repository;
6. devolver `Applied = true` cuando el resultado de negocio sea Accepted, Rejected o Conflict, porque el mensaje quedó consumido y su resultado durable fue publicado;
7. dejar que errores técnicos suban para retry del worker.

- [ ] **Step 4: Implement central reconciliation and durable result**

Dentro de una transacción y con locks `UPDLOCK, HOLDLOCK`:

- deduplicar por `SyncInbox.EventId` y `BusinessPartnerSyncConflicts.ProposalEventId`;
- cargar el canónico por `GlobalId`;
- validar rol, identificación normalizada, duplicado por rol y política central;
- recalcular la identificación normalizada y, para altas, el código interno; rechazar discrepancias con `BP_NORMALIZED_IDENTIFICATION_MISMATCH` o `BP_INTERNAL_CODE_MISMATCH`;
- para alta con `BaseCanonicalVersion = 0`, insertar versión 1;
- para update, ejecutar `BusinessPartnerThreeWayMergeService` contra base/propuesta/actual;
- en Accepted, persistir socio/hijos, incrementar versión, calcular/reservar `SapCardCode` si está vacío e insertar `LocalOutbox` canónico sin target;
- en Rejected, conservar central e insertar `LocalOutbox` de resultado con target = origen;
- en Conflict, insertar conflicto con snapshots/rutas e insertar resultado `Conflict` hacia origen;
- si falta un código geográfico o de contacto requerido, devolver `BP_SYNC_REFERENCE_NOT_FOUND` como resultado reprocesable sin marcar inbox aplicado;
- marcar inbox `Applied` en los tres resultados;
- no insertar `SapSyncOutbox`.

Usar `CausationEventId = ProposalEventId` en el evento resultante para trazabilidad.

- [ ] **Step 5: Register services and rerun tests**

Registrar repository/applier, agregarlo a `Program.cs` sin modificar `appsettings.json` y ejecutar el comando del paso 2.

Expected: PASS.

- [ ] **Step 6: Commit**

```bash
git add src/Backend/NuanSystem.Application/Abstractions/Sync/IBusinessPartnerProposalApplyRepository.cs src/Backend/NuanSystem.Application/Features/BusinessPartners/Sync/BusinessPartnerProposalApplyModels.cs src/Backend/NuanSystem.Persistence/Repositories/Sync/BusinessPartnerProposalApplyRepository.cs src/Backend/NuanSystem.Persistence/DependencyInjection/PersistenceServiceRegistration.cs src/Backend/NuanSystem.MasterBranchSyncWorker/Services/BusinessPartnerProposalSyncEventApplier.cs src/Backend/NuanSystem.MasterBranchSyncWorker/Program.cs tests/NuanSystem.Application.Tests/Features/Sync/BusinessPartnerProposalSyncEventApplierTests.cs tests/NuanSystem.Application.Tests/Features/Sync/BusinessPartnerProposalApplyRepositoryContractTests.cs
git commit -m "feat(sync): reconcile business partner proposals in central tenant"
```

## Task 9: Aplicar canónicos y resultados en sucursales sin bucles

**Files:**

- Modify: `src/Backend/NuanSystem.Application/Abstractions/Sync/IBusinessPartnerSyncApplyRepository.cs`
- Modify: `src/Backend/NuanSystem.Application/Features/BusinessPartners/Dtos/BusinessPartnerDtos.cs`
- Modify: `src/Backend/NuanSystem.Persistence/Repositories/Sync/BusinessPartnerSyncApplyRepository.cs`
- Modify: `src/Backend/NuanSystem.MasterBranchSyncWorker/Services/BusinessPartnerSyncEventApplier.cs`
- Create: `src/Backend/NuanSystem.MasterBranchSyncWorker/Services/BusinessPartnerProposalResultSyncEventApplier.cs`
- Modify: `src/Backend/NuanSystem.MasterBranchSyncWorker/Program.cs`
- Modify: `tests/NuanSystem.Application.Tests/Features/Sync/BusinessPartnerSyncEventApplierTests.cs`
- Create: `tests/NuanSystem.Application.Tests/Features/Sync/BusinessPartnerProposalResultSyncEventApplierTests.cs`

- [ ] **Step 1: Write failing canonical/result tests**

```csharp
[Fact]
public async Task CanonicalApply_DoesNotPublishAnotherLocalOutboxEvent()
{
    var result = await repository.ApplyCanonicalAsync(branchId, context, payload, CancellationToken.None);
    result.Applied.Should().BeTrue();
    await localOutbox.DidNotReceiveWithAnyArgs().CreateAsync(default!, default!, default!, default);
}
```

Cubrir fan-out a origen y otra sucursal, versión igual idempotente, versión menor ignorada, versión mayor aplicada, jerarquía padre→hijo obligatoria, reemplazo de hijos por `GlobalId`, `SapCardCode` sólo lectura, resultado enviado únicamente al origen y restauración canónica en Rejected cuando exista snapshot central.

- [ ] **Step 2: Run focused applier tests and confirm the expected failure**

Run: `dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj --filter "FullyQualifiedName~BusinessPartnerSyncEventApplierTests|FullyQualifiedName~BusinessPartnerProposalResultSyncEventApplierTests"`

Expected: FAIL because the current applier understands only the payload limitado y no existe el result applier.

- [ ] **Step 3: Replace the canonical apply contract with schema v2**

Eliminar `BusinessPartnerSyncPayload` del DTO general una vez que ningún productor/aplicador lo use. El applier `BusinessPartner` debe validar schema 2, identidad, fuente central y destino hijo. El repository usa `SP_NA_POST_BUSINESSPARTNER_CANONICAL_APPLY` para aplicar el snapshot completo por `GlobalId`, actualizar `CanonicalVersion`, `Accepted`, limpiar mensaje, marcar inbox y no escribir `LocalOutbox`. Un payload sin `SchemaVersion = 2` devuelve terminal `BP_SYNC_LEGACY_PAYLOAD_UNSUPPORTED`; el runbook deberá verificar que no queden eventos legacy antes de activar.

- [ ] **Step 4: Implement proposal-result apply**

El applier `BusinessPartnerProposalResult` acepta schema 1, exige que el target sea exactamente `OriginCompanyId` y llama `SP_NA_POST_BUSINESSPARTNER_PROPOSAL_RESULT_APPLY`:

- `Conflict`: conserva la propuesta visible, fija estado/mensaje y bloquea nuevas ediciones;
- `Rejected` con canónico: restaura snapshot central y deja el registro corregible con estado `Rejected`;
- `Rejected` sin canónico: conserva el alta local para corregir/reintentar;
- resolución `Accepted`: la sucursal recibirá el canónico normal; un result Accepted repetido sólo es idempotente.

- [ ] **Step 5: Register result applier and rerun tests**

Registrar el applier en `Program.cs`, ejecutar el comando del paso 2 y confirmar que ninguna prueba recibe a `IBusinessPartnerLocalOutboxWriter` durante apply.

Expected: PASS.

- [ ] **Step 6: Commit**

```bash
git add src/Backend/NuanSystem.Application/Abstractions/Sync/IBusinessPartnerSyncApplyRepository.cs src/Backend/NuanSystem.Application/Features/BusinessPartners/Dtos/BusinessPartnerDtos.cs src/Backend/NuanSystem.Persistence/Repositories/Sync/BusinessPartnerSyncApplyRepository.cs src/Backend/NuanSystem.MasterBranchSyncWorker/Services/BusinessPartnerSyncEventApplier.cs src/Backend/NuanSystem.MasterBranchSyncWorker/Services/BusinessPartnerProposalResultSyncEventApplier.cs src/Backend/NuanSystem.MasterBranchSyncWorker/Program.cs tests/NuanSystem.Application.Tests/Features/Sync/BusinessPartnerSyncEventApplierTests.cs tests/NuanSystem.Application.Tests/Features/Sync/BusinessPartnerProposalResultSyncEventApplierTests.cs
git commit -m "feat(sync): distribute canonical business partners without loops"
```

## Task 10: Exponer conflictos y resolución humana auditada

**Files:**

- Create: `src/Backend/NuanSystem.Application/Abstractions/Sync/IBusinessPartnerSyncConflictRepository.cs`
- Create: `src/Backend/NuanSystem.Application/Features/BusinessPartners/SyncConflicts/BusinessPartnerSyncConflictDtos.cs`
- Create: `src/Backend/NuanSystem.Application/Features/BusinessPartners/SyncConflicts/GetBusinessPartnerSyncConflictsQuery.cs`
- Create: `src/Backend/NuanSystem.Application/Features/BusinessPartners/SyncConflicts/GetBusinessPartnerSyncConflictsQueryHandler.cs`
- Create: `src/Backend/NuanSystem.Application/Features/BusinessPartners/SyncConflicts/ResolveBusinessPartnerSyncConflictCommand.cs`
- Create: `src/Backend/NuanSystem.Application/Features/BusinessPartners/SyncConflicts/ResolveBusinessPartnerSyncConflictCommandValidator.cs`
- Create: `src/Backend/NuanSystem.Application/Features/BusinessPartners/SyncConflicts/ResolveBusinessPartnerSyncConflictCommandHandler.cs`
- Create: `src/Backend/NuanSystem.Persistence/Repositories/Sync/BusinessPartnerSyncConflictRepository.cs`
- Modify: `src/Backend/NuanSystem.Persistence/DependencyInjection/PersistenceServiceRegistration.cs`
- Modify: `src/Backend/NuanSystem.Shared/Constants/PermissionCodes.cs`
- Modify: `src/Backend/NuanSystem.Api/Endpoints/SyncEndpoints.cs`
- Create: `tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerSyncConflictUseCaseTests.cs`
- Create: `tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerSyncConflictApiContractTests.cs`

- [ ] **Step 1: Write failing query/resolve tests**

```csharp
[Fact]
public async Task Resolve_RequiresReasonAndMatchingRowVersion()
{
    var invalid = new ResolveBusinessPartnerSyncConflictCommand(
        81,
        "AcceptBranch",
        "",
        Convert.ToBase64String([1, 2, 3]),
        7,
        "admin");
    var errors = await validator.ValidateAsync(invalid);
    errors.IsValid.Should().BeFalse();
}
```

Cubrir contexto central requerido, estrategia cerrada, conflicto ya resuelto idempotente, `RowVersion` obsoleto, `AcceptBranch`, `KeepCentral`, auditoría y evento de salida con target/origen correctos.

- [ ] **Step 2: Run focused conflict tests and confirm the expected failure**

Run: `dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj --filter "FullyQualifiedName~BusinessPartnerSyncConflictUseCaseTests|FullyQualifiedName~BusinessPartnerSyncConflictApiContractTests"`

Expected: FAIL because conflict use cases and routes do not exist.

- [ ] **Step 3: Implement central-only use cases**

Contrato de resolución:

```csharp
public sealed record ResolveBusinessPartnerSyncConflictCommand(
    long ConflictId,
    string Resolution,
    string Reason,
    string ExpectedRowVersion,
    [property: JsonIgnore] int? AuditUserId,
    [property: JsonIgnore] string? AuditUserName) : ICommand<BusinessPartnerSyncConflictDto>;
```

Exigir `IsMaster == true`. `AcceptBranch` aplica sólo rutas guardadas en `ConflictFieldsJson` sobre el canónico actual, incrementa `CanonicalVersion`, resuelve/audita e inserta evento canónico. `KeepCentral` resuelve/audita e inserta resultado `Rejected` hacia origen. Los dos caminos se ejecutan en una única transacción tenant y respetan `ExpectedRowVersion`.

- [ ] **Step 4: Map permissions and endpoints**

Agregar constantes:

```csharp
public const string BusinessPartnerSyncConflictsView = "SYNC.BUSINESS_PARTNER_CONFLICTS.VIEW";
public const string BusinessPartnerSyncConflictsResolve = "SYNC.BUSINESS_PARTNER_CONFLICTS.RESOLVE";
```

Mapear:

- `GET /api/sync/business-partner-conflicts?status=Open`
- `POST /api/sync/business-partner-conflicts/{id:long}/resolve`

El GET usa permiso View; POST usa Resolve y toma identidad de claims.

- [ ] **Step 5: Register persistence and rerun tests**

Registrar el repository, ejecutar el comando del paso 2 y verificar que el DTO muestre valores base/propuesto/central por campo sin exponer payloads técnicos completos al usuario.

Expected: PASS.

- [ ] **Step 6: Commit**

```bash
git add src/Backend/NuanSystem.Application/Abstractions/Sync/IBusinessPartnerSyncConflictRepository.cs src/Backend/NuanSystem.Application/Features/BusinessPartners/SyncConflicts/BusinessPartnerSyncConflictDtos.cs src/Backend/NuanSystem.Application/Features/BusinessPartners/SyncConflicts/GetBusinessPartnerSyncConflictsQuery.cs src/Backend/NuanSystem.Application/Features/BusinessPartners/SyncConflicts/GetBusinessPartnerSyncConflictsQueryHandler.cs src/Backend/NuanSystem.Application/Features/BusinessPartners/SyncConflicts/ResolveBusinessPartnerSyncConflictCommand.cs src/Backend/NuanSystem.Application/Features/BusinessPartners/SyncConflicts/ResolveBusinessPartnerSyncConflictCommandValidator.cs src/Backend/NuanSystem.Application/Features/BusinessPartners/SyncConflicts/ResolveBusinessPartnerSyncConflictCommandHandler.cs src/Backend/NuanSystem.Persistence/Repositories/Sync/BusinessPartnerSyncConflictRepository.cs src/Backend/NuanSystem.Persistence/DependencyInjection/PersistenceServiceRegistration.cs src/Backend/NuanSystem.Shared/Constants/PermissionCodes.cs src/Backend/NuanSystem.Api/Endpoints/SyncEndpoints.cs tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerSyncConflictUseCaseTests.cs tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerSyncConflictApiContractTests.cs
git commit -m "feat(sync): add audited business partner conflict resolution"
```

## Task 11: Adaptar WinForms a código interno, estados y campos administrados

**Files:**

- Modify: `src/Frontend/NuanSystem.WinForms.Services/BusinessPartners/Models/BusinessPartnerModels.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.Services/BusinessPartners/IBusinessPartnerClient.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.Services/BusinessPartners/BusinessPartnerClient.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.ViewModels/BusinessPartners/BusinessPartnersViewModel.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.ViewModels/BusinessPartners/Suppliers/SupplierBusinessPartnerMapper.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.ViewModels/BusinessPartners/Suppliers/SupplierAddressViewModel.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.ViewModels/BusinessPartners/Suppliers/SupplierContactViewModel.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.Forms/BusinessPartners/CustomerEditForm.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.Forms/BusinessPartners/CustomerEditForm.Designer.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.Forms/BusinessPartners/SupplierEditForm.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.Forms/BusinessPartners/SupplierEditForm.Designer.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.Services/Sync/ISyncMonitorClient.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.Services/Sync/SyncMonitorClient.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.Services/Sync/Models/SyncMonitorModels.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.Services/Sync/ISyncConfigurationClient.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.Services/Sync/SyncConfigurationClient.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.Services/Sync/Models/SyncConfigurationModels.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.ViewModels/Sync/SyncMonitorViewModel.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.ViewModels/Sync/SyncConfigurationViewModels.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.Forms/Sync/SyncMonitorForm.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.Forms/Sync/SyncMonitorForm.Designer.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.Forms/Sync/Configuration/SyncProfileEditForm.cs`
- Modify: `src/Frontend/NuanSystem.WinForms.Forms/Sync/Configuration/SyncProfileEditForm.Designer.cs`
- Create: `tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerFrontendContractTests.cs`

- [ ] **Step 1: Write failing frontend contract tests**

Las pruebas estáticas/contractuales deben exigir:

- `SaveBusinessPartnerRequest` sin `Code` y con `ExpectedRowVersion`;
- `GlobalId` en requests de direcciones/contactos;
- código de cliente/proveedor `ReadOnly` y texto “Se asigna al guardar” en alta;
- tipo/número de identificación bloqueados después del alta;
- badge para los cinco estados;
- save bloqueado en `PendingMaster`/`Conflict`;
- pestañas/campos administrados bloqueados sólo cuando la API devuelve `IsSyncedBranch = true`;
- pestaña de conflictos en el monitor con acciones `AcceptBranch` y `KeepCentral`.
- editor de perfiles con `BranchToMaster`, sin opción genérica `Bidirectional`, y sección de política central de prefijos.

- [ ] **Step 2: Run the frontend contract tests and confirm the expected failure**

Run: `dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj --filter FullyQualifiedName~BusinessPartnerFrontendContractTests`

Expected: FAIL because requests, forms and monitor do not expose the new contract.

- [ ] **Step 3: Update service models and edit policy**

Agregar a modelos de lectura `GlobalId`, `NormalizedIdentificationNumber`, `CanonicalVersion`, `RowVersion`, `MasterSyncStatus` y `MasterSyncMessage`. Eliminar `Code` del request de alta, agregar `ExpectedRowVersion` en update/delete y conservar los `GlobalId` de hijos al mapear.

Consumir en frontend el `BusinessPartnerEditPolicyDto` agregado a lookups en Task 5 y mapearlo al siguiente modelo de presentación:

```csharp
public sealed record BusinessPartnerEditPolicy(
    bool IsSyncedBranch,
    bool CanEditManagedFields,
    IReadOnlyCollection<string> EditableFields);
```

La API es autoritativa; este contrato sólo mejora experiencia de usuario.

- [ ] **Step 4: Update customer and supplier forms**

En alta, mostrar código read-only vacío con “Se asigna al guardar”. En edición, mostrar el código histórico o el formato `BP-{32 hex uppercase}` sin permitir cambios. Bloquear identificación después de la creación. Mostrar estado y mensaje de central. Si `IsSyncedBranch`, habilitar únicamente nombre legal/comercial, teléfono, correo, direcciones y contactos; mantener funcionalidad completa en central/standalone. Deshabilitar Guardar mientras el estado sea `PendingMaster` o `Conflict`; permitirlo en `Rejected`.

- [ ] **Step 5: Add conflict monitor actions**

Extender cliente/modelos/viewmodel del monitor para listar abiertos y resolver con motivo obligatorio y `RowVersion`. Agregar tab DevExpress con columnas socio, origen, versión, rutas, valor sucursal, valor central, fecha y estado. Confirmar antes de `AcceptBranch`/`KeepCentral`, refrescar después de éxito y respetar operaciones/permisos entregados por API.

En el editor de perfiles, agregar `BranchToMaster` al selector con etiquetas claras “sucursales origen” y “central destino”; no ofrecer `Bidirectional`. Cuando la entidad sea `BusinessPartnerProposal`, mostrar una sección que consulta/actualiza la política central (`NationalForeign` o `RoleOnly`, código de tipo pasaporte, ejemplos calculados y habilitación). Guardar la política mediante su endpoint independiente antes de permitir validar/activar el perfil, y ocultar/deshabilitar la edición si la sesión no tiene `SapManage`.

- [ ] **Step 6: Run tests and frontend builds**

Run: `dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj --filter FullyQualifiedName~BusinessPartnerFrontendContractTests`

Expected: PASS.

Run: `dotnet build src/Frontend/NuanSystem.WinForms.Services/NuanSystem.WinForms.Services.csproj -v minimal`

Expected: PASS with 0 errors.

Run: `dotnet build src/Frontend/NuanSystem.WinForms.Forms/NuanSystem.WinForms.Forms.csproj -v minimal`

Expected: PASS with 0 errors.

- [ ] **Step 7: Commit**

```bash
git add src/Frontend/NuanSystem.WinForms.Services/BusinessPartners/Models/BusinessPartnerModels.cs src/Frontend/NuanSystem.WinForms.Services/BusinessPartners/IBusinessPartnerClient.cs src/Frontend/NuanSystem.WinForms.Services/BusinessPartners/BusinessPartnerClient.cs src/Frontend/NuanSystem.WinForms.ViewModels/BusinessPartners/BusinessPartnersViewModel.cs src/Frontend/NuanSystem.WinForms.ViewModels/BusinessPartners/Suppliers/SupplierBusinessPartnerMapper.cs src/Frontend/NuanSystem.WinForms.ViewModels/BusinessPartners/Suppliers/SupplierAddressViewModel.cs src/Frontend/NuanSystem.WinForms.ViewModels/BusinessPartners/Suppliers/SupplierContactViewModel.cs src/Frontend/NuanSystem.WinForms.Forms/BusinessPartners/CustomerEditForm.cs src/Frontend/NuanSystem.WinForms.Forms/BusinessPartners/CustomerEditForm.Designer.cs src/Frontend/NuanSystem.WinForms.Forms/BusinessPartners/SupplierEditForm.cs src/Frontend/NuanSystem.WinForms.Forms/BusinessPartners/SupplierEditForm.Designer.cs src/Frontend/NuanSystem.WinForms.Services/Sync/ISyncMonitorClient.cs src/Frontend/NuanSystem.WinForms.Services/Sync/SyncMonitorClient.cs src/Frontend/NuanSystem.WinForms.Services/Sync/Models/SyncMonitorModels.cs src/Frontend/NuanSystem.WinForms.Services/Sync/ISyncConfigurationClient.cs src/Frontend/NuanSystem.WinForms.Services/Sync/SyncConfigurationClient.cs src/Frontend/NuanSystem.WinForms.Services/Sync/Models/SyncConfigurationModels.cs src/Frontend/NuanSystem.WinForms.ViewModels/Sync/SyncMonitorViewModel.cs src/Frontend/NuanSystem.WinForms.ViewModels/Sync/SyncConfigurationViewModels.cs src/Frontend/NuanSystem.WinForms.Forms/Sync/SyncMonitorForm.cs src/Frontend/NuanSystem.WinForms.Forms/Sync/SyncMonitorForm.Designer.cs src/Frontend/NuanSystem.WinForms.Forms/Sync/Configuration/SyncProfileEditForm.cs src/Frontend/NuanSystem.WinForms.Forms/Sync/Configuration/SyncProfileEditForm.Designer.cs tests/NuanSystem.Application.Tests/Features/BusinessPartners/BusinessPartnerFrontendContractTests.cs
git commit -m "feat(winforms): show managed business partner sync lifecycle"
```

## Task 12: Agregar pruebas de flujo completo y runbook de piloto

**Files:**

- Create: `tests/NuanSystem.Application.Tests/Features/Sync/BusinessPartnerBidirectionalFlowTests.cs`
- Create: `tests/NuanSystem.Application.Tests/Features/Sync/BusinessPartnerBidirectionalSqlIntegrationTests.cs`
- Create: `docs/operations/BUSINESS-PARTNER-BIDIRECTIONAL-PILOT.md`
- Modify: `README.md`
- Modify: `docs/architecture/SAP-NUANSYSTEM-BIDIRECTIONAL-FOUNDATION.md`

- [ ] **Step 1: Write the failing end-to-end orchestration tests**

Crear un fixture en memoria de tres tenants lógicos (central, sucursal A, sucursal B) y repositorios falsos de outbox/inbox para verificar el recorrido sin SQL externo ni SAP:

```csharp
[Fact]
public async Task BranchCreate_IsAcceptedAndDistributedToEveryBranchExactlyOnce()
{
    var globalId = await flow.CreateCustomerInBranchA();
    await flow.DrainUntilIdle();

    flow.Central.Single(globalId).CanonicalVersion.Should().Be(1);
    flow.BranchA.Single(globalId).MasterSyncStatus.Should().Be("Accepted");
    flow.BranchB.Single(globalId).MasterSyncStatus.Should().Be("Accepted");
    flow.AllLocalOutboxEvents.Count(x => x.EntityGlobalId == globalId).Should().Be(2);
}
```

- [ ] **Step 2: Cover the twelve acceptance scenarios**

Agregar pruebas explícitas para:

1. una transacción de sucursal crea una sola propuesta;
2. evento repetido es idempotente;
3. central no disponible deja retry durable y no pierde datos;
4. aceptación llega a todas las sucursales, incluida origen;
5. aplicación de réplica no genera bucle;
6. duplicado del mismo rol se rechaza;
7. misma identificación en rol diferente se acepta;
8. cambio concurrente del mismo campo crea conflicto;
9. cambios disjuntos se fusionan;
10. payload no contiene secretos ni campos excluidos;
11. worker/relay/perfiles deshabilitados no mutan datos por background;
12. el flujo no toca `SapSyncOutbox`, stock, costos, precios ni documentos.

- [ ] **Step 3: Run flow tests and confirm the expected failure, then complete the harness**

Run: `dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj --filter FullyQualifiedName~BusinessPartnerBidirectionalFlowTests`

Expected before harness completion: FAIL because the fixture does not yet drain all three event types.

Completar el fixture usando las mismas factories, merge service y decisiones que producción. No duplicar reglas de negocio dentro del test.

Run the same command again.

Expected: PASS.

- [ ] **Step 4: Add opt-in SQL Server integration tests**

Usar `[SqlServerIntegrationFact]` y `NUANSYSTEM_RUN_SQL_INTEGRATION_TESTS=1`. El fixture crea bases temporales con nombres explícitos bajo la instancia de pruebas, aplica 228/229/230, configura central y dos sucursales, ejecuta los casos de idempotencia, unicidad por rol, versiones, inbox/outbox y rollback, y elimina solamente esas bases en `DisposeAsync` tras validar sus nombres prefijados. No reutilizar bases reales ni credenciales de producción.

Run without opt-in: `dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj --filter FullyQualifiedName~BusinessPartnerBidirectionalSqlIntegrationTests`

Expected: tests reported as skipped with the documented environment-variable reason.

- [ ] **Step 5: Write the pilot runbook**

Documentar en orden:

1. respaldos y ejecución del readiness report en central y sucursales;
2. limpieza humana de duplicados/Both sin cambiar códigos automáticamente;
3. aplicación 228 en todos los tenants, 229 en Master y 230 en todos los tenants;
4. configuración de política de prefijos en la central;
5. creación de perfiles inactivos `BranchToMaster` y `MasterToBranch`;
6. despliegue del worker con `Enabled=false`, relay false y allowlist vacía;
7. modo `ObserveOnly`, verificación de cero eventos BusinessPartner legacy pendientes;
8. activación sólo para una central y sus sucursales piloto con allowlist de los tres tipos;
9. ejecución y evidencia de los doce casos;
10. consultas de rollback operativo: desactivar perfiles/relay/worker sin borrar eventos;
11. criterio de salida: cero DeadLetter no explicados, cero duplicados, cero bucles y cero filas nuevas en `SapSyncOutbox`.

Actualizar el documento de arquitectura con enlace al runbook y marcar únicamente bloques 1 y 2 como implementados cuando toda la verificación haya pasado.

- [ ] **Step 6: Run full verification**

Run: `dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj --no-restore`

Expected: PASS; sólo las pruebas de integración SQL documentadas pueden aparecer skipped cuando no se configuró opt-in.

Run: `dotnet build NuanSystem.sln --no-restore -v minimal`

Expected: PASS with 0 errors.

Run: `rg -n "SapSyncOutbox|ISapServiceLayer|DI API|HANA" src/Backend/NuanSystem.Application/Features/BusinessPartners/Sync src/Backend/NuanSystem.Persistence/Repositories/Sync/BusinessPartnerProposalApplyRepository.cs src/Backend/NuanSystem.MasterBranchSyncWorker/Services -g "BusinessPartner*.cs"`

Expected: no production references that call the SAP pipeline; test assertions or comments describing the prohibition are acceptable.

- [ ] **Step 7: Review the final diff and commit**

Run: `git status --short -- src tests database docs README.md`

Expected: only files from this plan; unrelated user files remain unstaged.

Run: `git diff --check`

Expected: no whitespace errors.

```bash
git add tests/NuanSystem.Application.Tests/Features/Sync/BusinessPartnerBidirectionalFlowTests.cs tests/NuanSystem.Application.Tests/Features/Sync/BusinessPartnerBidirectionalSqlIntegrationTests.cs docs/operations/BUSINESS-PARTNER-BIDIRECTIONAL-PILOT.md README.md docs/architecture/SAP-NUANSYSTEM-BIDIRECTIONAL-FOUNDATION.md
git commit -m "test(sync): verify business partner central branch pilot"
```

## Final implementation gate

Antes de declarar terminados los bloques 1 y 2, comprobar y registrar evidencia de lo siguiente:

- Los commits de las doce tareas están presentes y cada uno contiene solamente su alcance.
- El suite completo y la solución compilan desde un checkout limpio con dependencias ya restauradas.
- El readiness report fue ejecutado y revisado por un responsable de datos para cada tenant piloto.
- Las migraciones están instaladas en el orden 228 tenant, 229 Master, 230 tenant.
- La política central genera exactamente los prefijos aprobados para cliente/proveedor nacional/extranjero.
- Una creación en sucursal termina con el mismo `GlobalId`, `Code`, `SapCardCode` y `CanonicalVersion` en central y todas las sucursales.
- Un conflicto queda visible y sólo un usuario con permiso puede resolverlo con motivo.
- Reinicios del worker no duplican socios, hijos, inbox, outbox ni conflictos.
- Al desactivar perfiles, relay y worker no se procesa background adicional y los eventos permanecen recuperables.
- No hubo conexiones ni escrituras reales a SAP, ni cambios de stock, costos, precios u otros documentos.
