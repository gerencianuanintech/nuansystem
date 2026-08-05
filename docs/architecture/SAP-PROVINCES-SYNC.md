# Provincias SAP B1 → NuanSystem → sucursales

## Alcance

`States` de SAP B1 alimenta el catálogo `Provinces` del tenant matriz mediante una lectura Full ordenada y sin filtros. Después del commit local, la replicación NuanSystem distribuye DEMO → sucursales mediante `LocalOutbox`/`SyncOutbox`; las sucursales nunca consultan SAP.

## Identidad y jerarquía

- SAP: `Country + Code`; la referencia externa persistida es `SAP_B1` + `COUNTRY|CODE`.
- NuanSystem: `GlobalId`; el país padre se resuelve únicamente por `CountryGlobalId`.
- `ProvinceId` y `CountryId` permanecen locales en cada tenant.
- No se adopta por código ni se cambia silenciosamente el país de un `GlobalId` existente.
- Colisiones `(CountryId, Code)`, referencia externa o padre son terminales; país ausente es dependencia reintentable.

## Contrato durable

Create/update/delete de Province y su `LocalOutbox` comparten una transacción tenant. El relay conserva `EventId`; Full incluye activos, inactivos y tombstones. El aplicador escribe `SyncInbox`, Province y `SyncAudit` de forma atómica.

## SAP execution

`ProvinceV1` permite solo `countryCode`, `provinceCode` y `provinceName`. La capacidad Master admite únicamente `SapToErp + Full`; no crea perfiles, filtros ni agendas. La estructura SQL fue desplegada el 2026-08-04; SAP y los workers permanecen sin ejecución hasta autorización independiente.

## Exclusiones

Ciudades, direcciones, geocodificación, SAP→sucursal directo, Incremental, ERP→SAP y workers activos.
