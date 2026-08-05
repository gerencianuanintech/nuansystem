# Sincronización SAP de Países

## Estado y alcance

- Fecha: 2026-08-04.
- Estado: contrato aprobado para implementación estática.
- Dirección: SAP Business One hacia NuanSystem.
- Modo: Full, sin filtros.
- Fuente local: la base Matriz/DEMO de la empresa.
- Distribución posterior: DEMO hacia sucursales mediante el pipeline independiente Matriz-Sucursal.

Este contrato no habilita perfiles, agendas, relay ni workers; tampoco autoriza
ejecutar SQL o llamar SAP.

## Discovery Record

**Outcome:** importar el catálogo de países desde SAP a DEMO y distribuirlo de
forma durable e idempotente a las sucursales, dejando Países disponible en
`Definiciones -> General`.

**Work type:** integración SAP, sincronización Matriz-Sucursal, catálogo
administrativo, seguridad/navegación y migraciones.

**Domain owner:** Geography/Countries. SAP es una fuente externa opcional y no
es propietario del modelo de dominio.

**Origen SAP confirmado:**

- Países: tabla SAP Business One `OCRY`, expuesta por Service Layer mediante
  el entity set `Countries` usado por el lector tipado.
- Provincias/estados: tabla SAP Business One `OCST`. Su entity set y nombres
  exactos de campos se validarán contra `$metadata` antes de implementar esa
  etapa.
- NuanSystem no consulta `OCRY` ni `OCST` directamente; estos nombres documentan
  la procedencia funcional mientras el transporte permanece aislado en
  `SapIntegration`.

**Explicit decisions and exclusions:**

- Countries se consulta completo y sin `$filter`.
- Solo se implementa `SapToErp + Full`; no existe ERP hacia SAP ni Incremental.
- `GlobalId` es la identidad entre bases.
- `SAP_B1 + ExternalCode` es la referencia externa estable.
- Una coincidencia únicamente por `Code` no adopta otra identidad.
- SAP puede actualizar `Name`, `Iso2` e `Iso3` después de validar su metadata.
- `PhonePrefix` y el estado local se preservan.
- La ausencia en SAP no desactiva ni elimina países locales.
- Provincias y Ciudades quedan fuera de esta implementación y requieren gates
  independientes.
- WinForms no consulta SAP ni SQL.

**Evidence inspected:**

- `Application/Features/SapSync/Warehouses` — ejecución, snapshot y retry Full.
- `SapIntegration/Warehouses` — reader/query/mapper Service Layer.
- `Application/Features/GeneralInventory/Warehouses/Commands` — outbox tenant
  transaccional.
- `MASTER-BRANCH-ITERATION-8-4C-WAREHOUSE-BLUEPRINT.md` — identidad,
  tombstones, campos locales y colisión terminal.
- `Application/Features/Definitions/General/Countries` — CRUD y contratos
  actuales de Países.
- `Persistence/Repositories/Sync/CountrySyncApplyRepository.cs` — aplicador
  actual que debe eliminar la adopción por código.
- `database/sql/035_master_geography_security.sql` — menú geográfico existente.
- `Frontend/.../Geography` — formularios concretos ya disponibles.

**Selected pattern:** lector tipado Service Layer y procesamiento por registro
de Bodegas, combinado con el límite `LocalOutbox` transaccional de Warehouse.

**Permitted reuse boundary:** reutilizar infraestructura de sesión, query,
ejecución, retry, outbox, formularios y seguridad; no reutilizar DTOs ni reglas
de identidad propias de Warehouse.

**Risk:** alto.

**Confidence:** alta para el lifecycle; media para el mapeo ISO hasta validar
el `$metadata` del Service Layer configurado para la empresa piloto.

## Flujo aprobado

```text
SAP OCRY -> Service Layer Countries (Full, sin filtros)
  -> reader tipado y mapper
  -> procesador por registro
  -> Countries en DEMO + LocalOutbox en el mismo commit tenant
  -> relay idempotente con el mismo EventId
  -> SyncOutbox Master
  -> aplicación por GlobalId en cada sucursal
```

La llamada remota SAP ocurre antes de cualquier transacción SQL. La promoción a
Master y la aplicación en sucursal son etapas separadas.

## Identidad y conflictos

1. Una referencia externa única actualiza el país correspondiente.
2. Un código SAP nuevo crea un país con nuevo `GlobalId` y referencia externa.
3. El mismo código unido a otro `GlobalId` produce conflicto visible; nunca
   reemplaza la identidad existente.
4. Una referencia externa repetida o contradictoria es terminal.
5. Los códigos de tombstones permanecen reservados.
6. Repetir el mismo `EventId` o el mismo Full no duplica registros.

## Navegación

Las pantallas conservan sus claves estables y permisos actuales:

```text
Definiciones
  -> General
       -> Países      (countries)
       -> Provincias  (provinces)
       -> Ciudades    (cities)
```

La migración de menú debe reubicar los registros existentes y preservar
formularios, operaciones y accesos de rol. La configuración y las ejecuciones
SAP permanecen dentro del módulo SAP.

## Gates

- Build y pruebas dirigidas aprobados.
- Query Countries sin `$filter` verificada estáticamente.
- CRUD + LocalOutbox atómicos.
- Aplicador exclusivo por `GlobalId`, sin adopción.
- Migraciones forward-only, idempotentes y deshabilitadas por defecto.
- SQL, `$metadata`, preview, SAP, relay y workers requieren autorizaciones
  posteriores separadas.
