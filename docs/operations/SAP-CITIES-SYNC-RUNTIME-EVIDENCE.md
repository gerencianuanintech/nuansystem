# Evidencia runtime — despliegue SQL de Ciudades

## Resultado

- Fecha: 2026-08-05.
- Master: `NuanSystem_Master`.
- Tenants desplegados: `NuanSystem_DEMO`, `NuanSystem_DEMO_REMIGIO` y `NuanSystem_DEMO_CANARIS`.
- Estado: migraciones `176` y `177` desplegadas, repetidas y validadas.
- Durante el despliegue de las migraciones `176` y `177` no se invocaron SAP, SRI ni workers. La validacion SAP posterior se limito al preview autorizado descrito en la seccion `Proximo gate`; no hubo importacion.

`NuanSystem_SYNC_WH_BRANCH_TEST` fue excluida porque no contiene `dbo.SapSyncExecutionDetails` ni la migracion prerrequisito `20260804.173`. No se instalaron dependencias fuera del alcance autorizado.

## Respaldos

Los siguientes respaldos `COPY_ONLY WITH CHECKSUM` aprobaron `RESTORE VERIFYONLY WITH CHECKSUM`:

- `/var/opt/mssql/data/NuanSystem_Master_Cities176177_20260805_105311.bak`;
- `/var/opt/mssql/data/NuanSystem_DEMO_Cities176177_20260805_105311.bak`;
- `/var/opt/mssql/data/NuanSystem_DEMO_REMIGIO_Cities176177_20260805_105311.bak`;
- `/var/opt/mssql/data/NuanSystem_DEMO_CANARIS_Cities176177_20260805_105311.bak`.

## Despliegue

- `176_master_sap_city_select_query.sql` se ejecuto dos veces exclusivamente en Master.
- `177_tenant_sap_city_execution_snapshot.sql` se ejecuto dos veces en cada tenant compatible.
- Cada version quedo registrada exactamente una vez.
- Master contiene `CitiesSelectQuery` y el procedimiento auditado de configuracion por empresa.
- La capacidad `Cities` quedo `SapToErp + Full`, sin Incremental ni ERP→SAP.
- No se crearon perfiles, entidades de perfil ni agendas para Ciudades.
- Ninguna empresa quedo con una consulta de Ciudades configurada automaticamente.

## Validacion final

| Base | Version | Contrato validado | Ejecuciones Cities |
|---|---:|---:|---:|
| `NuanSystem_Master` | `20260805.176` = 1 | columna, procedimiento y capacidad = 1 | N/A |
| `NuanSystem_DEMO` | `20260805.177` = 1 | `CityV1` + cuatro campos = 1 | 0 |
| `NuanSystem_DEMO_REMIGIO` | `20260805.177` = 1 | `CityV1` + cuatro campos = 1 | 0 |
| `NuanSystem_DEMO_CANARIS` | `20260805.177` = 1 | `CityV1` + cuatro campos = 1 | 0 |

`DBCC CHECKCONSTRAINTS('dbo.SapSyncExecutionDetails')` no reporto violaciones en los tres tenants.

## Proximo gate

El `CitiesSelectQuery` de DEMO fue configurado el 2026-08-05 mediante el procedimiento auditado, despues de crear y verificar el respaldo:

- `/var/opt/mssql/data/NuanSystem_Master_CityQueryDemo_20260805_110020.bak`.

Antes del preview se confirmaron un pais `EC` y 24 provincias `EC|01` a `EC|24` vinculados con `SAP_B1`, tres ciudades locales y cero ciudades SAP.

La configuracion HANA de pruebas fue validada primero en memoria. El esquema contiene 224 filas en `@MUNI_CANTO`, cero filas invalidas y cero codigos duplicados. Antes de persistir la configuracion cifrada se creo y verifico:

- `/var/opt/mssql/data/NuanSystem_Master_DemoHana_20260805_120357.bak`.

Servidor, puerto, esquema, usuario y contraseña cifrada quedaron asociados a DEMO con auditoria; no se modificaron las credenciales de Service Layer. La credencial en texto claro no se guardo en archivos ni evidencia.

El preview oficial se ejecuto mediante `ISapCityImportService` y `SapHanaCityReader`:

- 224 leidas;
- 221 nuevas;
- 3 conflictos por provincia SAP no vinculada;
- 0 duplicados y 0 campos obligatorios vacios;
- 1 pais y 25 prefijos provinciales distintos.

Los conflictos son `EC|90|9001`, `EC|90|9003` y `EC|90|9004`. El prefijo `90` no existe entre las provincias vinculadas `EC|01` a `EC|24`. No hubo importacion ni escritura tenant.

## Decision operativa

Se decidio no crear una provincia local `90` y no importar parcialmente el catalogo. Por tanto, las 224 ciudades permanecen sin importar y la sincronizacion de Ciudades queda bloqueada de forma controlada mientras el origen SAP incluya registros con el prefijo provincial `90` sin una provincia local vinculada.

El cierre tecnico no autoriza nuevos previews, importaciones, perfiles, agendas ni workers. Para reanudar el flujo se requiere una decision funcional explicita sobre la representacion del codigo `90` o una correccion autorizada del `CitiesSelectQuery` de la empresa.
