# Sincronizacion SAP de Ciudades

## Alcance

Ciudades se importa desde SAP Business One hacia NuanSystem en modo `Full`, sin filtros y sin desactivar registros locales ausentes. SAP sigue siendo opcional por empresa.

La fuente es una consulta HANA de solo lectura almacenada por empresa en `NuanSystem_Master.dbo.SapCompanySettings.CitiesSelectQuery`. No se crea una tabla adicional de configuracion. La consulta se administra mediante:

- `GET /api/sap/settings/cities-query`
- `PUT /api/sap/settings/cities-query`

Enviar `CitiesSelectQuery = null` deshabilita la fuente de Ciudades para esa empresa.

## Contrato del SELECT

La consulta debe ser una unica sentencia `SELECT`, sin comentarios ni punto y coma, y exponer exactamente estos alias:

- `CountryCode`
- `ProvinceCode`
- `CityCode`
- `CityName`

Ejemplo para la empresa Demo, donde los dos primeros digitos de `Code` identifican la provincia:

```sql
SELECT
    'EC' AS "CountryCode",
    LEFT(TRIM("Code"), 2) AS "ProvinceCode",
    TRIM("Code") AS "CityCode",
    TRIM("Name") AS "CityName"
FROM "@MUNI_CANTO"
WHERE LENGTH(TRIM("Code")) >= 2
  AND TRIM("Name") <> ''
ORDER BY "Code"
```

Otra empresa puede usar otra tabla, columnas o regla para derivar la provincia; solo debe conservar los cuatro alias del contrato. El SQL no se construye con parametros recibidos del endpoint de preview/importacion.

## Identidad y jerarquia

- Sistema externo: `SAP_B1`.
- Pais: `COUNTRY`.
- Provincia: `COUNTRY|PROVINCE`.
- Ciudad: `COUNTRY|PROVINCE|CITY`.

Una ciudad solo se crea o actualiza cuando pais y provincia ya tienen referencias SAP confirmadas y la provincia pertenece al pais resuelto. Una coincidencia local por codigo sin referencia SAP requiere aprobacion; no se adopta automaticamente.

Al actualizar se preservan `GlobalId`, codigo local, pais, provincia, referencia externa y `IsActive`; solo se actualiza el nombre. El proceso no elimina ni inactiva ciudades que no aparezcan en SAP.

## Operacion segura

Antes de importar se debe usar `GET /api/sap/cities/preview`. El import manual usa `POST /api/sap/cities/import`. Las ejecuciones programadas guardan snapshots tipados `CityV1` y permiten reintentos con la identidad compuesta validada.

Las migraciones requeridas son:

- Master: `176_master_sap_city_select_query.sql`.
- Tenant: `177_tenant_sap_city_execution_snapshot.sql`.

Los scripts no deben desplegarse ni debe activarse el worker sin autorizacion operativa explicita.
