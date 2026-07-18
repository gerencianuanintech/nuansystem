# Contrato funcional piloto SAP Business One

## Estado del documento

- Fase: 0 - Contrato funcional.
- Estado: conexion y preview SAP verificados; motor de politicas implementado en codigo y migracion Master pendiente de aplicar.
- Empresa piloto NuanSystem: `DEMO` - `Empresa Demo`.
- Responsable funcional SAP: Cristian, Jefe de Software.
- Responsable funcional NuanSystem: Cristian, Jefe de Software.
- Fecha de registro inicial: 2026-07-16.

Este documento registra las decisiones funcionales del piloto. No contiene credenciales, passwords, tokens, connection strings ni secretos SAP.

## Contexto confirmado

| Dato | Definicion del piloto |
|---|---|
| Pais | Ecuador |
| Zona horaria | `America/Guayaquil` |
| Cultura | `es-EC` |
| Moneda principal | `USD` |
| Ambiente NuanSystem | Empresa `DEMO` existente, usada temporalmente para el piloto |
| Ambiente SAP | Copia de la base de produccion |
| Compania visible SAP | `PRUEBAS PAGOS` |
| `CompanyDB` SAP | `SBO_CONORQUE_PRUE15092025` |
| Motor SAP | SAP HANA |
| Version informada | SAP Business One 10, build `10.00.270`, SP `2411` |
| Service Layer confirmado | `https://192.168.10.110:50000/b1s/v1/` |
| SQL Server NuanSystem piloto | `localhost,1433` |
| Usuario tecnico Service Layer | Definido para pruebas; no documentado por seguridad |
| Primera entidad del piloto | Bodegas (`Warehouses`) |
| Sucursales totales del negocio | 10 |
| Sucursales incluidas en el piloto | 2 |
| Sucursales SAP B1 activas | SAP B1 no tiene administracion de sucursales activada |
| Direccion general inicial | SAP B1 hacia NuanSystem |

## Empresa piloto

La empresa `DEMO` sera el nodo NuanSystem asociado a la unica CompanyDB SAP usada durante el piloto. La configuracion SAP sera opcional y pertenecera a esta empresa en `NuanSystem_Master`.

La empresa `DEMO` se integrara con la copia SAP visible como `PRUEBAS PAGOS`, cuya CompanyDB es `SBO_CONORQUE_PRUE15092025`. El ambiente usa SAP HANA y expone Service Layer v1 en `https://192.168.10.110:50000/b1s/v1/`.

La version fue informada como SAP Business One 10, build `10.00.270`, SP `2411`. Antes de implementar el cliente se validara esta identificacion contra la pantalla Acerca de SAP o el SLD para separar correctamente version, Feature Package y Patch Level.

Service Layer v1 queda como contrato confirmado del piloto. La disponibilidad de `/b1s/v2` se comprobara tecnicamente, pero no es requisito para iniciar y no debe asumirse hasta obtener una respuesta valida del ambiente SAP.

## Sucursales del piloto

Cada sucursal operara con su propia base NuanSystem.

| Nombre funcional | Codigo de empresa | `BranchCode` | Base NuanSystem | Estado |
|---|---|---|---|---|
| Sucursal Remigio | `DEMO-REMIGIO` | `REMIGIO` | `NuanSystem_DEMO_REMIGIO` | Confirmado para el piloto |
| Sucursal Paseo de los Canaris | `DEMO-CANARIS` | `CANARIS` | `NuanSystem_DEMO_CANARIS` | Confirmado para el piloto |

La sucursal tecnica existente `SYNC-WH-BRANCH-TEST` / `WH-TEST` no forma parte del piloto funcional. Es un dato de pruebas del escenario de sincronizacion de bodegas y no debe reutilizarse como sucursal real.

Las ocho sucursales restantes quedan fuera del alcance del piloto. Su incorporacion futura debera reutilizar el mismo modelo, sin ramas de codigo particulares.

## Topologia acordada

SAP B1 no tiene sucursales activadas. Por ello no existe un `BPLId` SAP para identificar Remigio o Paseo de los Canaris. La integracion usara una sola CompanyDB SAP como origen y NuanSystem resolvera la distribucion hacia sus sucursales.

```text
SAP PRUEBAS PAGOS
  -> CompanyDB SBO_CONORQUE_PRUE15092025
  -> Service Layer v1 / HANA
  -> SAP Sync Worker
  -> empresa/nodo DEMO
  -> Outbox Master-Sucursal
  -> Inbox Sucursal Remigio
  -> base NuanSystem_DEMO_REMIGIO
  -> Inbox Sucursal Paseo de los Canaris
  -> base NuanSystem_DEMO_CANARIS
```

Reglas:

- WinForms no se conectara directamente a SAP, HANA ni SQL Server.
- El worker SAP importara contra el contexto de empresa `DEMO`.
- La distribucion a sucursales usara el pipeline Master/Sucursal existente con Outbox/Inbox, idempotencia y auditoria.
- No se implementaran dos conexiones SAP independientes para Remigio y Paseo de los Canaris mientras ambas consuman la misma CompanyDB.
- Los catalogos comunes confirmados para el piloto se distribuiran a las dos sucursales.
- La matriz Perfil x Entidad x Sucursal decidira `None`, `All`, `Selected` o `Rule`; las selecciones usan `GlobalId`.
- La identidad entre bases NuanSystem sera `GlobalId`; los codigos SAP seran referencias externas.
- Una falla SAP no debe impedir la operacion local ni el funcionamiento SRI.

## Propiedad preliminar

| Entidad o configuracion | Propietario inicial | Direccion | Observacion |
|---|---|---|---|
| Configuracion de empresa y sucursales | NuanSystem Master | NuanSystem -> sucursales | Gobierno central |
| Configuracion SAP | NuanSystem Master | No replicable como secreto | Una configuracion para `DEMO` |
| Otros catalogos del piloto | Pendiente | Pendiente | Se definiran despues de bodegas |
| Bodegas | SAP B1 | SAP -> `DEMO` -> sucursales | Primera entidad del piloto; requiere mapping por sucursal |
| Stock | Pendiente | Deshabilitado inicialmente | No forma parte de la importacion de bodegas |
| Documentos transaccionales | Pendiente | Deshabilitado inicialmente | Se definira despues de los catalogos |
| SRI | NuanSystem | Pipeline independiente | No depende de SAP |

## Estado de la conexion SAP

- Queda pendiente validar la identificacion exacta de Feature Package y Patch Level contra SAP o SLD.
- Queda pendiente validar los permisos minimos y reemplazar el usuario temporal por un usuario tecnico antes de produccion.
- Service Layer v1, TLS, autenticacion y lectura real de bodegas fueron verificados desde el equipo de pruebas.

Diagnostico actualizado del 2026-07-16:

- SQL Server `localhost,1433`: conexion TCP disponible.
- Service Layer `192.168.10.110:50000`: conexion TCP disponible despues de habilitar el servidor de pruebas.
- `/b1s/v1/`, `/b1s/v1/$metadata`, `/b1s/v2/` y `/b1s/v2/$metadata` responden `401 Unauthorized` sin sesion, confirmando que Apache y Service Layer reciben solicitudes en ambas rutas.
- La negociacion tecnica admite TLS 1.3 con `TLS_AES_256_GCM_SHA384`.
- El certificado presentado es autofirmado, con `CN=hanadb`, SAN DNS `hanadb`, SAN IP `192.168.10.110` y vigencia hasta 2027-04-16.
- Huella SHA-256 observada: `22700090C72464392621E4300E7020B23969A4AB3F19A23CEBA4809464F3F628`.
- El certificado publico verificado fue instalado el 2026-07-16 en `LocalMachine/Root` del equipo de pruebas. Una solicitud HTTPS con `HttpClient` de .NET 9 y validacion estricta llega correctamente a Service Layer y obtiene `401 Unauthorized` sin sesion. No se deshabilito la validacion TLS en codigo.
- Esta confianza aplica solamente al equipo actual. El host definitivo de la API debera confiar en la misma CA/certificado o en su reemplazo productivo.
- La empresa `DEMO` tiene configuracion cifrada en `SapCompanySettings`; la autenticacion y el preview real respondieron correctamente.

La configuracion de la credencial de prueba debe guardarse cifrada en `SapCompanySettings`; no se admite texto plano en `appsettings`, scripts SQL, documentacion o logs. Antes de produccion se reemplazara por un usuario tecnico de privilegio minimo y se rotara la credencial temporal.

## Datos pendientes para crear las sucursales

- Direccion o ciudad operativa de cada sucursal, si se requiere para documentos.
- Bodegas SAP que corresponden a cada sucursal, si se importara informacion dependiente de bodega.

Las bases `NuanSystem_DEMO_REMIGIO` y `NuanSystem_DEMO_CANARIS` se alojaran inicialmente en el SQL Server actual `localhost,1433`.

## Primera entidad: bodegas SAP

La primera integracion funcional sera el maestro de bodegas de SAP Business One mediante el recurso Service Layer `Warehouses`.

Alcance inicial:

- Ejecutar primero una consulta de preview sin modificar NuanSystem.
- Importar todas las bodegas SAP en la base tenant de `DEMO`, que actua como Master del piloto.
- Relacionar la bodega por `WarehouseCode` SAP, conservando `GlobalId` como identidad NuanSystem.
- Evaluar la politica de cada sucursal despues de importar todas las bodegas a Master.
- Las bodegas que no coincidan permanecen disponibles en Master y no crean target de sucursal.
- No importar existencias, disponibilidad, kardex, costos, lotes, series, ubicaciones internas ni movimientos.
- No desactivar automaticamente una bodega local durante la primera ejecucion; una diferencia de estado se mostrara en preview.

Mapeo preliminar:

| SAP Service Layer | NuanSystem `Warehouse` | Regla inicial |
|---|---|---|
| `WarehouseCode` | `Code`, `ExternalCode`, `SapCode` | Codigo requerido y clave externa SAP |
| `WarehouseName` | `Name` | Nombre requerido |
| `Street` | `Address` | Opcional |
| `City` | `City` | Opcional |
| Provincia/estado disponible en metadata | `Province` | Confirmar nombre de propiedad con `$metadata` |
| `Country` | `Country` | Opcional |
| Estado activo/inactivo disponible en metadata | `IsActive` | Confirmar propiedad y valores con `$metadata` |
| Sin equivalente SAP | `GlobalId` | Generado por NuanSystem |
| Sin equivalente SAP | Politica de distribucion | Usa `GlobalId`; el codigo SAP queda como referencia legible |

Regla de matching y conflicto:

1. Buscar por `SapCode = WarehouseCode`.
2. Si no existe, buscar por codigo local igual a `WarehouseCode`.
3. Si coincide por codigo local pero no tiene relacion SAP, marcar `Conflict` para aprobacion.
4. Si no existe, crear con `ExternalSystem = SAP_B1`.
5. Si existe por `SapCode`, actualizar solamente los campos permitidos del maestro.

La primera ejecucion sera manual y con preview. La frecuencia automatica se definira despues de validar los resultados del piloto.

Contrato tecnico disponible para la prueba:

- `GET /api/sap/warehouses/preview`: consulta SAP y clasifica cada bodega sin modificar NuanSystem.
- `POST /api/sap/warehouses/import`: crea o actualiza bodegas aprobables usando el caso de uso normal, auditoria y publicacion Master/Sucursal.
- `GET /api/sap/settings/service-layer`: devuelve la configuracion no secreta y solo indica si existe una clave protegida.
- `PUT /api/sap/settings/service-layer`: crea o actualiza la configuracion, cifra una clave nueva y permite omitirla para conservar la existente.
- Ambos endpoints requieren empresa activa mediante `X-Company-Code`; el preview usa permiso de lectura SAP y la importacion permiso de administracion SAP.
- La sesion Service Layer se abre por empresa, usa cookies aisladas y se cierra al finalizar; la clave solo se descifra dentro del cliente tecnico.
- Los endpoints de configuracion requieren `SAP.SYNC.MANAGE`; la auditoria registra campos modificados y nunca valores de credenciales.

La seleccion piloto aprobada se configurara por `GlobalId` despues de crear los tenants:

| `WarehouseCode` SAP | Nombre SAP | Modo | Sucursal NuanSystem | Observacion |
|---|---|---|---|---|
| `20` | `MEGA REMIGIO` | `Selected` | Sucursal Remigio (`DEMO-REMIGIO`) | Aprobado por Cristian |
| `11` | `MEGA TOTORACOCHA` | `Selected` | Sucursal Paseo de los Canaris (`DEMO-CANARIS`) | Aprobado por Cristian; el nombre SAP difiere del nombre funcional |

Las otras 22 bodegas observadas en el preview se importan a Master y no se distribuyen a las dos bases mientras no sean seleccionadas o una regla las incluya.

## Criterio de cierre de esta seccion

La empresa piloto, las dos sucursales, la conexion Service Layer y la matriz inicial Bodega SAP -> Sucursal NuanSystem quedan definidas. Antes de ejecutar la distribucion se deben crear o validar los dos tenants de sucursal, aplicar el routing selectivo y configurar un perfil `Warehouse` para ambas bases.
