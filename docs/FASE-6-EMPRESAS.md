# Fase 6: Modulo de Empresas

## Objetivo

Implementar el modulo administrativo base para registrar empresas en `NuanSystem_Master`, validar sus conexiones, guardar credenciales cifradas y asignar empresas a usuarios.

## Alcance Implementado

### 1. Contratos de persistencia

Se agregaron en `NuanSystem.Application`:

- `ICompanyAdminRepository`
- `ICompanyConnectionTester`

Ubicacion:

- `src/Backend/NuanSystem.Application/Abstractions/Data`

Responsabilidades:

- Consultar empresas registradas.
- Crear empresas.
- Verificar existencia por codigo.
- Validar existencia de usuarios.
- Asignar empresas a usuarios.
- Probar conexion a la base de datos de una empresa.

### 2. DTOs del modulo

Se agregaron en:

- `src/Backend/NuanSystem.Application/Features/Companies/Dtos`

Archivos:

- `CompanyDto`
- `CreateCompanyData`
- `CompanyConnectionTestData`
- `CompanyConnectionTestResult`

### 3. Queries

Se agrego:

- `GetCompaniesQuery`
- `GetCompaniesQueryHandler`

Ubicacion:

- `src/Backend/NuanSystem.Application/Features/Companies/Queries`

Permite listar las empresas registradas en la base master.

### 4. Commands

Se agregaron:

- `CreateCompanyCommand`
- `CreateCompanyCommandValidator`
- `CreateCompanyCommandHandler`
- `ValidateCompanyConnectionCommand`
- `ValidateCompanyConnectionCommandValidator`
- `ValidateCompanyConnectionCommandHandler`
- `AssignUserCompanyCommand`
- `AssignUserCompanyCommandValidator`
- `AssignUserCompanyCommandHandler`

Ubicacion:

- `src/Backend/NuanSystem.Application/Features/Companies/Commands`

### 5. Repositorio master

Se implemento:

- `CompanyAdminRepository`

Ubicacion:

- `src/Backend/NuanSystem.Persistence/Repositories/CompanyAdminRepository.cs`

Este repositorio usa `IMasterConnectionFactory`, por lo tanto opera sobre `NuanSystem_Master`, no sobre una base tenant.

Operaciones implementadas:

- `GetAllAsync`
- `GetByCodeAsync`
- `CreateAsync`
- `ExistsByCodeAsync`
- `UserExistsAsync`
- `AssignUserAsync`

### 6. Validador de conexion SQL Server

Se implemento:

- `SqlServerCompanyConnectionTester`

Ubicacion:

- `src/Backend/NuanSystem.Persistence/Services/SqlServerCompanyConnectionTester.cs`

Responsabilidades:

- Construir connection string SQL Server.
- Intentar abrir conexion.
- Consultar version del servidor.
- Retornar resultado controlado, sin lanzar excepcion al caso de uso.

Por ahora solo SQL Server esta implementado. MySQL queda preparado para una fase futura.

### 7. Cifrado de password de empresa

`CreateCompanyCommandHandler` usa:

- `ISecretProtector`

El campo `DatabasePassword` recibido por API se cifra antes de guardarse en:

```sql
dbo.Companies.DatabasePasswordEncrypted
```

Esto evita guardar contrasenas de tenant en texto plano.

### 8. Endpoints Implementados

Listar empresas:

```http
GET /api/companies
Authorization: Bearer <token>
```

Crear empresa:

```http
POST /api/companies
Authorization: Bearer <token>
Content-Type: application/json
```

Body ejemplo:

```json
{
  "code": "EMPRESA01",
  "commercialName": "Empresa Demo",
  "legalName": "Empresa Demo S.A.",
  "taxIdentification": "0999999999001",
  "databaseEngine": 1,
  "server": "localhost",
  "port": 1433,
  "databaseName": "NuanSystem_Empresa01",
  "databaseUser": "sa",
  "databasePassword": "clave",
  "validateConnection": true,
  "isActive": true,
  "sapIntegrationMode": 0
}
```

Validar conexion:

```http
POST /api/companies/validate-connection
Authorization: Bearer <token>
Content-Type: application/json
```

Asignar empresa a usuario:

```http
POST /api/companies/assign-user
Authorization: Bearer <token>
Content-Type: application/json
```

Body:

```json
{
  "userId": 1,
  "companyId": 1
}
```

### 9. Registro DI

En `PersistenceServiceRegistration` se agrego:

- `ICompanyAdminRepository -> CompanyAdminRepository`
- `ICompanyConnectionTester -> SqlServerCompanyConnectionTester`

## Flujo para Crear Empresa

```text
POST /api/companies
  -> CreateCompanyCommand
    -> FluentValidation
    -> Verificar codigo unico
    -> Validar conexion si validateConnection = true
    -> Cifrar DatabasePassword
    -> Insertar en dbo.Companies
    -> Retornar CompanyDto
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

El bloqueo sigue siendo operativo por autenticacion SQL Server.

## Pendientes

- Corregir login SQL Server.
- Crear `NuanSystem_Master`.
- Crear usuario administrador.
- Probar login JWT.
- Crear empresa desde `POST /api/companies`.
- Asignar empresa al usuario administrador.
- Crear base tenant.
- Ejecutar `POST /api/tenancy/initialize-database`.
- Agregar update/inactivate/delete logico de empresas.
- Agregar filtros por estado y busqueda.
- Agregar permisos finos por endpoint.

## Estado de la Fase

La Fase 6 queda implementada con casos de uso, validaciones, repositorio master, cifrado de credenciales, validacion de conexion y endpoints administrativos.

La siguiente fase natural es Fase 7: modulo de clientes completo, extendiendo el ejemplo de Fase 5 con update, delete logico, get-by-id y validaciones finales.
