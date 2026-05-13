# Fase 3: Seguridad

## Objetivo

Agregar la base de autenticacion y autorizacion de NuanSystem: usuarios, roles, permisos, empresas autorizadas por usuario, JWT y endpoints iniciales para login y consulta de empresas.

## Alcance Implementado

### 1. Contratos de autenticacion

Se agregaron abstracciones en `NuanSystem.Application`:

- `IAuthService`
- `IJwtTokenService`
- `IPasswordHasher`
- `AuthResult`
- `AuthCompanyDto`
- `JwtTokenResult`

Ubicacion:

- `src/Backend/NuanSystem.Application/Abstractions/Authentication`

Estas interfaces permiten que Application conozca los contratos sin depender de SQL Server, JWT concreto ni algoritmos de hashing.

### 2. Contratos compartidos para API/WinForms

Se agregaron DTOs en `NuanSystem.Shared`:

- `LoginRequest`
- `LoginResponse`
- `UserCompanyResponse`

Ubicacion:

- `src/Backend/NuanSystem.Shared/Contracts/Auth`

Estos contratos podran ser reutilizados por WinForms para consumir la API sin duplicar modelos.

### 3. Hashing de contrasenas

Se implemento:

- `Pbkdf2PasswordHasher`

Ubicacion:

- `src/Backend/NuanSystem.Infrastructure/Authentication/Pbkdf2PasswordHasher.cs`

Caracteristicas:

- PBKDF2 con SHA-256.
- Salt aleatorio de 16 bytes.
- Hash de 32 bytes.
- 100,000 iteraciones.
- Comparacion con `CryptographicOperations.FixedTimeEquals`.

Formato de hash:

```text
PBKDF2-SHA256$100000$<salt-base64>$<hash-base64>
```

### 4. JWT

Se implemento:

- `JwtOptions`
- `JwtTokenService`

Ubicacion:

- `src/Backend/NuanSystem.Infrastructure/Authentication`

El token incluye:

- `sub`
- `unique_name`
- `ClaimTypes.NameIdentifier`
- `ClaimTypes.Name`
- `display_name`
- roles
- permisos

Configuracion:

```json
{
  "Jwt": {
    "Issuer": "NuanSystem",
    "Audience": "NuanSystem.WinForms",
    "SigningKey": "...",
    "ExpirationMinutes": 60
  }
}
```

La clave real se guarda en `appsettings.Local.json`, que esta excluido por git.

### 5. Autenticacion en API

Se configuro JWT Bearer en:

- `src/Backend/NuanSystem.Api/Extensions/ServiceCollectionExtensions.cs`

Se agrego:

- `AddAuthentication`
- `AddJwtBearer`
- `AddAuthorization`

El pipeline quedo:

```text
UseGlobalExceptionHandling
UseSwagger / UseSwaggerUI
UseHttpsRedirection
UseAuthentication
UseCompanyContext
UseAuthorization
```

Este orden permite que primero se identifique el usuario, luego se resuelva la empresa activa y finalmente se apliquen reglas de autorizacion.

### 6. Servicio SQL de autenticacion

Se implemento:

- `SqlServerAuthService`

Ubicacion:

- `src/Backend/NuanSystem.Persistence/Security/SqlServerAuthService.cs`

Responsabilidades:

- Buscar usuario activo por usuario o email.
- Validar password hash.
- Cargar roles.
- Cargar permisos.
- Cargar empresas autorizadas.
- Generar JWT.
- Actualizar `LastLoginAt` y reiniciar `FailedAccessCount`.

### 7. Endpoints implementados

Login:

```http
POST /api/auth/login
```

Body:

```json
{
  "userNameOrEmail": "admin",
  "password": "clave"
}
```

Respuesta exitosa:

```json
{
  "success": true,
  "message": "Login correcto.",
  "data": {
    "userId": 1,
    "userName": "admin",
    "displayName": "Administrador",
    "accessToken": "...",
    "expiresAtUtc": "2026-04-28T00:00:00Z",
    "roles": ["ADMIN"],
    "permissions": ["SECURITY.USERS.MANAGE"],
    "companies": []
  },
  "errors": []
}
```

Empresas del usuario autenticado:

```http
GET /api/companies/my-companies
Authorization: Bearer <token>
```

Endpoint de prueba de empresa activa:

```http
GET /api/tenancy/current
Authorization: Bearer <token>
X-Company-Code: EMPRESA01
```

### 8. Tablas de seguridad

El inicializador y el script `database/sql/001_master_database.sql` ahora crean:

- `Users`
- `Roles`
- `Modules`
- `Permissions`
- `UserRoles`
- `RolePermissions`
- `UserCompanies`
- `RefreshTokens`

Tambien se agrego seed minimo:

- Modulo `SECURITY`
- Modulo `COMPANIES`
- Rol `ADMIN`
- Permisos:
  - `SECURITY.USERS.MANAGE`
  - `SECURITY.ROLES.MANAGE`
  - `COMPANIES.MANAGE`
- Asignacion de esos permisos al rol `ADMIN`

### 9. Script plantilla para primer admin

Se agrego:

- `database/sql/002_seed_admin_template.sql`

Este script permite crear el primer usuario administrador cuando ya exista la base master.

El valor pendiente es:

```text
<PASSWORD_HASH>
```

Debe generarse con el algoritmo `Pbkdf2PasswordHasher`.

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

El resultado mantiene el bloqueo operativo ya identificado: SQL Server responde, pero rechaza el login `sa`.

## Pendientes

- Corregir autenticacion SQL Server para `sa` o proporcionar otra credencial valida.
- Ejecutar `--init-only`.
- Crear el primer usuario administrador.
- Asociar el usuario a una empresa mediante `UserCompanies`.
- Probar `POST /api/auth/login`.
- Probar `GET /api/companies/my-companies`.
- Probar `GET /api/tenancy/current` con JWT y `X-Company-Code`.
- Implementar refresh token completo en una fase posterior si se decide activarlo.

## Estado de la Fase

La Fase 3 queda implementada a nivel de codigo, contratos, configuracion, tablas, seeds base y endpoints iniciales.

Pendiente operativo:

- Validar contra SQL Server real cuando el login `sa` funcione.

La siguiente fase natural es Fase 4: persistencia, estrategia EF Core/Dapper, fabrica de conexiones y repositorios base.
