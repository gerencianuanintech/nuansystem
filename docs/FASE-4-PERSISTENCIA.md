# Fase 4: Persistencia

## Objetivo

Definir e implementar la base de persistencia para NuanSystem, preparada para conexiones dinamicas por empresa, acceso SQL mantenible, inicializacion de esquemas tenant y compatibilidad futura con otros motores como MySQL.

## Decision Principal: Dapper Primero

Para esta fase se eligio Dapper como estrategia inicial de persistencia.

Motivos:

- El sistema requiere resolver connection strings dinamicamente por empresa.
- Las consultas empresariales suelen necesitar SQL explicito, controlado y optimizable.
- Es mas simple trabajar con multiples bases tenant sin acoplar `DbContext` a un tenant fijo.
- Facilita convivir luego con SQL Server y MySQL usando SQL separado por motor.
- Evita que la capa de dominio dependa de Entity Framework.

EF Core no queda descartado. Puede agregarse mas adelante en modulos donde convenga por tracking, migraciones o modelo relacional complejo. La arquitectura deja esa puerta abierta.

## Paquete Agregado

En `NuanSystem.Persistence`:

- `Dapper`

## Abstracciones Agregadas

En `NuanSystem.Application`:

- `IMasterConnectionFactory`
- `ITenantConnectionFactory`
- `ITenantDatabaseInitializer`
- `IRepository`

Ubicacion:

- `src/Backend/NuanSystem.Application/Abstractions/Data`

Estas interfaces evitan que Application dependa de `SqlConnection`, Dapper o SQL Server concreto.

## Fabrica de Conexion Master

Se formalizo:

- `MasterConnectionFactory`

Ubicacion:

- `src/Backend/NuanSystem.Persistence/Connections/MasterConnectionFactory.cs`

Responsabilidad:

- Crear conexiones hacia `NuanSystem_Master`.
- Usar `ConnectionStrings:SqlServerAdmin`.
- Cambiar automaticamente `InitialCatalog` al nombre configurado en `MasterDatabase:DatabaseName`.

## Fabrica de Conexion Tenant

Se agrego:

- `TenantConnectionFactory`

Ubicacion:

- `src/Backend/NuanSystem.Persistence/Connections/TenantConnectionFactory.cs`

Responsabilidad:

- Leer la empresa activa desde `ICompanyContext`.
- Crear una conexion hacia la base de datos de esa empresa.
- Soportar SQL Server actualmente.
- Dejar MySQL preparado como motor futuro.

Flujo:

```text
ICompanyContext.CurrentCompany
  -> DatabaseEngine
  -> ConnectionString
  -> IDbConnection
```

## Repositorio Base Dapper

Se agrego:

- `DapperRepository`

Ubicacion:

- `src/Backend/NuanSystem.Persistence/Repositories/DapperRepository.cs`

Metodos base:

- `QueryAsync<T>`
- `QuerySingleOrDefaultAsync<T>`
- `ExecuteAsync`
- `ExecuteScalarAsync<T>`
- `ExecuteInTransactionAsync`

Este repositorio sera la base para modulos como clientes, articulos, documentos y logs SAP.

## Inicializador de Base Tenant

Se agrego:

- `SqlServerTenantDatabaseInitializer`

Ubicacion:

- `src/Backend/NuanSystem.Persistence/Services/SqlServerTenantDatabaseInitializer.cs`

Responsabilidad:

- Tomar la empresa activa.
- Crear/validar tablas base en la base de datos tenant.
- Registrar version en `dbo.SchemaHistory`.

Tablas tenant iniciales:

- `SchemaHistory`
- `Customers`
- `Items`
- `Documents`
- `DocumentLines`
- `SapSyncLog`

Estas tablas son una base inicial para los modulos siguientes. Se podran ampliar cuando se trabaje cada modulo con sus comandos, queries y validaciones.

## Endpoint de Inicializacion Tenant

Se agrego:

```http
POST /api/tenancy/initialize-database
Authorization: Bearer <token>
X-Company-Code: EMPRESA01
```

Respuesta esperada:

```json
{
  "success": true,
  "message": "Base de datos tenant validada correctamente.",
  "data": {
    "initialized": true
  },
  "errors": []
}
```

Este endpoint permite inicializar o validar la base de la empresa activa despues de que la empresa exista en `NuanSystem_Master`.

## Script SQL Server Tenant

Se agrego:

- `database/sql/003_tenant_database_sqlserver.sql`

Uso:

- Ejecutarlo dentro de una base de datos de empresa.
- No debe ejecutarse en `NuanSystem_Master`.

Este script es equivalente al inicializador automatico de tenant para SQL Server.

## Registros en DI

En `PersistenceServiceRegistration` se registraron:

- `IMasterConnectionFactory`
- `ITenantConnectionFactory`
- `ITenantDatabaseInitializer`
- `ICompanyResolver`
- `ITenantConnectionStringResolver`
- `IAuthService`

## Compatibilidad Futura con MySQL

La arquitectura ya contiene:

- `DatabaseEngine.MySql`
- Contratos basados en `IDbConnection`
- Separacion entre fabrica master y tenant
- Punto unico para decidir el proveedor por motor
- Scripts separados por motor

Pendiente futuro:

- Agregar proveedor MySQL, por ejemplo `MySqlConnector`.
- Implementar `TenantConnectionFactory` para MySQL.
- Crear scripts `*_mysql.sql`.
- Ajustar SQL de repositorios cuando existan diferencias de dialecto.

## Verificacion Realizada

Se ejecuto:

```powershell
dotnet build NuanSystem.sln --no-restore
```

Resultado:

```text
Compilacion correcta.
0 advertencias
0 errores
```

Tambien se ejecuto:

```powershell
$env:ASPNETCORE_ENVIRONMENT='Development'
dotnet run --project src\Backend\NuanSystem.Api\NuanSystem.Api.csproj --no-build -- --init-only
```

Resultado:

```text
Login failed for user 'sa'.
```

El bloqueo sigue siendo operativo por credenciales SQL Server, no por codigo.

## Pendientes

- Corregir acceso SQL Server del usuario `sa` o usar otra credencial valida.
- Ejecutar `--init-only` para crear `NuanSystem_Master`.
- Insertar empresa de prueba en `Companies`.
- Asociar usuario admin a la empresa en `UserCompanies`.
- Crear la base tenant real.
- Ejecutar `POST /api/tenancy/initialize-database`.
- Validar que se creen las tablas tenant.

## Estado de la Fase

La Fase 4 queda implementada a nivel de codigo, DI, repositorio base, fabricas de conexion, scripts y endpoint de inicializacion tenant.

La siguiente fase natural es Fase 5: MediatR, FluentValidation, pipeline behaviors, commands, queries, handlers y patron estandar para casos de uso.
