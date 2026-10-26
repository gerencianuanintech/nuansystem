# Arquitectura NuanSystem

## Capas

`Api` recibe las solicitudes HTTP y delega los casos de uso a `Application`.  
`Application` contiene commands, queries, handlers, validators, DTOs y contratos.  
`Domain` mantiene el modelo de negocio sin dependencias de infraestructura.  
`Persistence` implementa acceso a datos para SQL Server. MySQL conserva un selector legacy, pero no esta implementado.
`Infrastructure` implementa servicios transversales como autenticacion, cifrado, Serilog y configuracion.  
`SapIntegration` encapsula Service Layer y DI API para que SAP sea opcional.  
`Shared` contiene contratos seguros para compartir entre API y frontend.

Los componentes del vertical SRI se agrupan fisicamente bajo `SyncSRI` dentro de
cada proyecto/capa. Esta organizacion no mezcla responsabilidades: API,
Application, Persistence, Worker y WinForms conservan sus limites y proyectos.

## Flujo multiempresa

1. El usuario inicia sesion contra la API.
2. La API devuelve token JWT y empresas autorizadas.
3. El frontend selecciona empresa y envia `X-Company-Code` en cada request.
4. El middleware resuelve la empresa activa desde `NuanSystem_Master`.
5. `CompanyContext` expone la empresa activa durante la solicitud.
6. `Persistence` construye la conexion dinamica para la base de datos de esa empresa.

## Reglas de dependencia

- WinForms nunca se conecta directo a base de datos.
- Los formularios no contienen logica de negocio.
- SAP no se referencia desde Domain.
- Application define interfaces; Infrastructure, Persistence y SapIntegration las implementan.
- La seleccion de empresa afecta todas las operaciones de negocio.


## Consolidacion del backend — estado al 2026-10-24

La validacion local y los limites de liberacion se registran en [BACKEND-SOLID-VALIDATION](operations/BACKEND-SOLID-VALIDATION.md). El catalogo de componentes y grafo de la entrega estan en [BACKEND-SOLID-STATUS](architecture/BACKEND-SOLID-STATUS.md), coordinados con los catalogos internos.

- Result<T> conserva compatibilidad legacy (400) e incorpora FailureKind: 404, 409, 403, 503 y 500 saneado; el cliente WinForms mantiene su contrato de errores.
- SAP es opcional. Importacion/exportacion tienen contratos separados y capacidades efectivas compartidas por catalogo, perfiles y despacho. No hay exportadores registrados; Both no se ejecuta como importacion. El flujo programado Full tiene procesadores para Warehouses, Countries, Provinces y Cities. Outbox ERP a SAP sigue incompleto.
- ISapSupplierImportRepository separa importacion SAP del contrato CRUD de socios; ambos usan una instancia scoped del repositorio.
- El calculo de compras reside en Domain/Purchasing con valores numericos puros y redondeo ToEven de seis decimales. Application adapta persistencia y metadata. La numeracion proviene de serie/reserva autorizada o del documento existente; no se inventan defaults.
- Los handlers de escritura de socios usan el piloto IBusinessPartnerUnitOfWork sin ADO.NET expuesto. Persistence conserva la misma conexion/transaccion para dato y Outbox. ITransactionRunner y las sobrecargas legacy permanecen: el piloto aun requiere validar atomicidad SQL real antes de ampliar su migracion.
- Company conserva conceptos empresariales. La configuracion tecnica se resuelve fuera del modelo; CompanyConnectionInfo omite ConnectionString en JSON/ToString. Los enums tecnicos conservan su ubicacion legacy por compatibilidad.

El ensayo HTTP usa endpoints, middleware, MediatR y cliente reales en memoria; autenticacion y servicios externos son dobles. No acredita JWT real, sockets/TLS, SQL, SAP o despliegue. Tras la correccion WinForms del 2026-10-24, la suite local pasa 1263/1263 casos. La evidencia actual esta en docs/operations/BACKEND-SOLID-WINFORMS-VALIDATION.md. Esto no autoriza una liberacion de produccion ni cierra los gates SQL/JWT/transporte pendientes.
