# Fase 2: Arquitectura Multiempresa

## Objetivo

Preparar NuanSystem para trabajar con multiples empresas o bases de datos, resolviendo la empresa activa por solicitud HTTP y construyendo dinamicamente la conexion del tenant desde `NuanSystem_Master`.

## Alcance Implementado

### 1. Modelo de dominio para empresas

Se agregaron tipos base en `NuanSystem.Domain`:

- `Tenancy/Company.cs`
- `Tenancy/DatabaseEngine.cs`
- `Tenancy/SapIntegrationMode.cs`

Estos tipos representan la empresa registrada, el motor de base de datos y el modo de integracion SAP permitido por empresa.

Motores preparados:

- `SqlServer`
- `MySql`

Modos SAP preparados:

- `None`
- `ServiceLayer`
- `DiApi`

### 2. Contratos de tenancy en Application

Se crearon abstracciones para evitar que la API dependa directamente de SQL Server:

- `ICompanyContext`
- `ICompanyResolver`
- `ITenantConnectionStringResolver`
- `CompanyConnectionInfo`

Ubicacion:

- `src/Backend/NuanSystem.Application/Abstractions/Tenancy`

Responsabilidades:

- `ICompanyContext`: guarda la empresa activa durante la solicitud.
- `ICompanyResolver`: busca una empresa por codigo en la base master.
- `ITenantConnectionStringResolver`: entrega la conexion del tenant activo.
- `CompanyConnectionInfo`: objeto seguro con los datos necesarios para operar contra la empresa activa.

### 3. Cifrado de secretos

Se creo el contrato:

- `ISecretProtector`

Y una implementacion inicial:

- `AesSecretProtector`

Ubicacion:

- `src/Backend/NuanSystem.Application/Abstractions/Security/ISecretProtector.cs`
- `src/Backend/NuanSystem.Infrastructure/Security/AesSecretProtector.cs`

El objetivo es que claves como `DatabasePasswordEncrypted` y `SapPasswordEncrypted` no se almacenen como texto plano en `NuanSystem_Master`.

La clave local se configura en:

```json
{
  "Security": {
    "EncryptionKey": "..."
  }
}
```

En desarrollo se guarda en `appsettings.Local.json`, excluido por git.

### 4. Contexto de empresa activa

Se agrego:

- `CompanyContext`

Ubicacion:

- `src/Backend/NuanSystem.Persistence/Tenancy/CompanyContext.cs`

Este contexto es `scoped`, por lo tanto vive solo durante una solicitud HTTP. Evita que una misma solicitud cambie de empresa una vez establecida.

### 5. Resolucion de empresa desde SQL Server

Se agrego:

- `MasterConnectionFactory`
- `SqlServerCompanyResolver`
- `TenantConnectionStringResolver`

Ubicacion:

- `src/Backend/NuanSystem.Persistence/Connections/MasterConnectionFactory.cs`
- `src/Backend/NuanSystem.Persistence/Tenancy/SqlServerCompanyResolver.cs`
- `src/Backend/NuanSystem.Persistence/Tenancy/TenantConnectionStringResolver.cs`

Flujo:

1. La API recibe `X-Company-Code`.
2. `SqlServerCompanyResolver` consulta `dbo.Companies` en `NuanSystem_Master`.
3. Valida que la empresa exista y este activa.
4. Descifra `DatabasePasswordEncrypted`.
5. Construye el connection string del tenant.
6. Guarda la empresa activa en `ICompanyContext`.

Por ahora la construccion real de connection string esta implementada para SQL Server. MySQL queda preparado a nivel de enum/arquitectura, pero su proveedor se agregara en una fase posterior.

### 6. Middleware de empresa activa

Se agrego:

- `CompanyContextMiddleware`

Ubicacion:

- `src/Backend/NuanSystem.Api/Middleware/CompanyContextMiddleware.cs`

Header requerido:

```http
X-Company-Code: EMPRESA01
```

Comportamiento:

- Omite resolucion en `/`, `/health`, `/swagger`, `/api/auth` y `/api/companies`.
- Para endpoints de negocio exige `X-Company-Code`.
- Si el header falta, responde `400 Bad Request`.
- Si la empresa no existe o esta inactiva, responde `403 Forbidden`.
- Si la empresa es valida, establece `ICompanyContext`.

Extension agregada:

- `UseCompanyContext()`

### 7. Endpoint de prueba de tenancy

Se agrego endpoint temporal:

```http
GET /api/tenancy/current
```

Este endpoint permite validar que el middleware resolvio la empresa activa. Requiere `X-Company-Code`.

Respuesta esperada:

```json
{
  "success": true,
  "message": "Operacion completada correctamente",
  "data": {
    "companyId": 1,
    "companyCode": "EMPRESA01",
    "commercialName": "Empresa Demo",
    "databaseEngine": "SqlServer",
    "sapIntegrationMode": "None"
  },
  "errors": []
}
```

### 8. Tablas master multiempresa

El inicializador `SqlServerMasterDatabaseInitializer` ahora crea:

- `dbo.SystemParameters`
- `dbo.Companies`
- `dbo.SapCompanySettings`
- `dbo.CompanyParameters`
- `dbo.MasterSchemaHistory`

Tabla principal:

```sql
dbo.Companies
```

Campos relevantes:

- `Code`
- `CommercialName`
- `DatabaseEngine`
- `Server`
- `Port`
- `DatabaseName`
- `DatabaseUser`
- `DatabasePasswordEncrypted`
- `IsActive`
- `SapIntegrationMode`

Tabla SAP:

```sql
dbo.SapCompanySettings
```

Campos relevantes:

- `CompanyId`
- `IsEnabled`
- `IntegrationMode`
- `ServiceLayerUrl`
- `SapCompanyDb`
- `SapUser`
- `SapPasswordEncrypted`
- `DiApiServer`
- `LicenseServer`

Tabla parametros por empresa:

```sql
dbo.CompanyParameters
```

Permite guardar configuraciones particulares de cada empresa sin cambiar estructura de tablas.

### 9. Script SQL equivalente

Se agrego script manual:

- `database/sql/001_master_database.sql`

Este script permite crear la estructura master desde SQL Server Management Studio o `sqlcmd`, en caso de no usar el inicializador automatico.

### 10. Registro DI

Se registraron los servicios de tenancy en `PersistenceServiceRegistration`:

- `ICompanyContext`
- `MasterConnectionFactory`
- `ICompanyResolver`
- `ITenantConnectionStringResolver`

Se registro el protector de secretos en `InfrastructureServiceRegistration`:

- `ISecretProtector`

## Flujo Final de una Solicitud Multiempresa

```text
WinForms
  -> API REST con header X-Company-Code
    -> CompanyContextMiddleware
      -> ICompanyResolver
        -> NuanSystem_Master.dbo.Companies
      -> ICompanyContext.SetCurrentCompany(...)
        -> Handler/Repository
          -> ITenantConnectionStringResolver
            -> Base de datos de la empresa activa
```

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

El resultado confirma que el codigo compila y llega a SQL Server, pero sigue pendiente corregir la autenticacion del login `sa`.

## Pendientes Antes de Probar en Base Real

- Habilitar o corregir el login `sa`.
- Ejecutar `--init-only`.
- Confirmar creacion de `NuanSystem_Master`.
- Confirmar tablas master.
- Insertar una empresa de prueba en `dbo.Companies` con password cifrado.
- Probar `GET /api/tenancy/current` enviando `X-Company-Code`.

## Estado de la Fase

La arquitectura multiempresa base esta implementada a nivel de codigo, middleware, contratos, DI y scripts SQL.

Pendiente operativo:

- Validar contra SQL Server real cuando el login `sa` funcione.

La siguiente fase natural es Fase 3: seguridad, usuarios, roles, permisos, JWT y permisos por empresa.
