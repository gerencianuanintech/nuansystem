# Fase 8: empresa y configuracion tecnica

Fecha: 2026-10-24. Refactor de ownership sin cambios SQL ni ejecucion SAP/DB.

## Descubrimiento y decision

Domain/Tenancy/Company contiene propiedades de servidor/base/credenciales y SapIntegrationMode, pero no se instancia ni consume en produccion. El resolver usa CompanyConnectionInfo y lectura Master directa; administracion usa CompanyDto/ConfigurationCompanyDto; SAP usa SapCompanySettingsDto y expone SapServiceLayerSettingsDto sin passwords. No se agrega otro modelo empresarial ni otro contrato de resolucion.

Company conserva Id, Code, CommercialName, IsActive, OperationMode, IsMaster, ParentCompanyId, BranchCode y SyncEnabled. Se retiran DatabaseEngine, Server, Port, DatabaseName, DatabaseUser, DatabasePasswordEncrypted y SapIntegrationMode. Los selectores tecnicos legacy siguen como enums compartidos en su ubicacion actual para no romper otros consumidores; Company ya no depende de ellos.

Se reutiliza CompanyConnectionInfo como contrato tecnico Application para resolucion/contexto y se impide serializar ConnectionString mediante JsonIgnore. ToString omite la cadena de conexion. Los endpoints actuales no retornan ese contrato: sus DTO administrativos y de autenticacion conservan forma y metadata autorizada (servidor/base/usuario no se retiran del administrador). No se altera ningun contrato HTTP publicado ni el flujo de configuracion.

La construccion SQL Server se extrae a CompanyDatabaseConfiguration, interno de Persistence/Tenancy, para probar campos tecnicos, cifrado y politica TLS sin abrir conexiones. Conserva el mismo SqlConnectionStringBuilder y la misma llamada ISecretProtector.Unprotect, con el password cifrado solo en ese limite. Los tres caminos del resolver usan el mismo builder. SQL, permisos UserCompanies, filtros de activos, proyecciones legacy y parametros no cambian. No se imprime ni registra configuracion real.

SAP conserva configuracion por CompanyId, passwords cifrados, mapeo HasPassword y aislamiento por empresa incluso con el mismo codigo/base externo. El modo None no requiere SAP para representar una empresa o crear la conexion tenant. MySQL sigue no implementado y no se promete DI API multiplataforma.

## Registro y pruebas

Ownership: Master conserva configuracion global/tecnica; Domain conserva conceptos empresariales; Persistence aplica proveedor y descifra solo al construir la conexion. Autorizacion: CompanyContextMiddleware sigue usando ResolveByCodeForUserAsync y UserCompanies. Tenant: mismo contexto/factory y seleccion por Id/codigo; sin cache nuevo. Auditoria, SQL, rutas, frontend, SAP adapters y workers sin cambios.

Ocho guardas de arquitectura fallaron sobre el modelo original. Se agregan pruebas de forma JSON empresarial/administrativa/autenticacion/SAP, redaccion de ConnectionString, builder con password cifrado/puerto/TLS, error de descifrado, modo sin SAP y dos empresas con el mismo identificador externo. Se valida autorizacion middleware con resolver simulado y barreras de IO; no equivale a validacion de SQL real.

Se conserva el checkout actual y los cambios de fases anteriores. Snapshot/copia en artifacts/backend-solid-phase8; sin stash/reset/commit/cambio de rama. No se inicia fase 9 ni se resuelve el gate SQL pendiente de fase 7. La arista nueva es SqlServerCompanyResolver -> CompanyDatabaseConfiguration interno; los contratos y las rutas existentes se reutilizan.
