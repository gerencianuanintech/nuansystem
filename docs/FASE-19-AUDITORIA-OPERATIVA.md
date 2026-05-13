# Fase 19 - Auditoria operativa

## Objetivo

La fase 19 agrega trazabilidad operativa para registrar acciones de escritura ejecutadas contra la API.

El sistema ahora guarda solicitudes `POST`, `PUT` y `DELETE` con usuario, empresa, ruta, metodo HTTP, estado de respuesta, IP, navegador/agente y fecha UTC.

## Base de datos master

Se agrego la tabla:

```text
dbo.AuditLogs
```

Campos principales:

- `Id`
- `UserId`
- `UserName`
- `CompanyCode`
- `HttpMethod`
- `Path`
- `QueryString`
- `StatusCode`
- `IpAddress`
- `UserAgent`
- `CreatedAt`

Indices creados:

- `IX_AuditLogs_CreatedAt`
- `IX_AuditLogs_UserId_CreatedAt`

Version registrada:

```text
20260428.19 - Fase 19: auditoria operativa
```

## Permiso nuevo

Se agrego el permiso:

```text
SECURITY.AUDIT.READ
```

El permiso queda sembrado en el modulo `SECURITY` y asignado al rol `ADMIN`.

## API

Se agrego el middleware:

```text
src/Backend/NuanSystem.Api/Middleware/AuditLoggingMiddleware.cs
```

Comportamiento:

- Registra solicitudes `POST`, `PUT` y `DELETE`.
- Toma el usuario desde los claims del JWT.
- Toma la empresa desde el header `X-Company-Code`.
- Guarda el codigo HTTP final de la respuesta.
- Si falla el registro de auditoria, deja warning en logs tecnicos y no rompe la operacion principal.

Se agrego el endpoint:

```text
GET /api/audit/logs?take=200
```

Proteccion:

```text
SECURITY.AUDIT.READ
```

## Application y Persistence

Se agregaron:

```text
src/Backend/NuanSystem.Application/Abstractions/Data/IAuditLogRepository.cs
src/Backend/NuanSystem.Application/Features/Audit/Dtos/AuditLogDto.cs
src/Backend/NuanSystem.Application/Features/Audit/Dtos/CreateAuditLogData.cs
src/Backend/NuanSystem.Application/Features/Audit/Queries/GetAuditLogsQuery.cs
src/Backend/NuanSystem.Application/Features/Audit/Queries/GetAuditLogsQueryHandler.cs
src/Backend/NuanSystem.Persistence/Repositories/AuditLogRepository.cs
```

La consulta limita `take` entre 1 y 500 registros.

## WinForms

Se agrego el modulo `Auditoria` en el menu principal.

Archivos nuevos:

```text
src/Frontend/NuanSystem.WinForms.Services/Audit/AuditClient.cs
src/Frontend/NuanSystem.WinForms.Services/Audit/IAuditClient.cs
src/Frontend/NuanSystem.WinForms.Services/Audit/Models/AuditLogItem.cs
src/Frontend/NuanSystem.WinForms.ViewModels/Audit/AuditLogsViewModel.cs
src/Frontend/NuanSystem.WinForms.Forms/Audit/AuditLogsForm.cs
```

La pantalla permite cargar los registros recientes y ajustar la cantidad a consultar hasta 500.

## Verificacion

Compilacion:

```text
dotnet build NuanSystem.sln --no-restore -p:BaseOutputPath=artifacts\verify\
0 Advertencia(s)
0 Errores
```

Inicializacion master:

```text
NuanSystem.Api.exe --init-only
Inicializacion de base master completada.
```

Prueba API temporal:

```text
Login admin correcto.
PUT /api/settings/parameters/Audit.Test respondio 200.
GET /api/audit/logs?take=20 devolvio el registro auditado.
```

Resultado validado:

```text
Auditoria OK. Registros recibidos: 2. Ultimo match: 2 PUT /api/settings/parameters/Audit.Test 200
```

## Nota operativa

Despues de esta fase, reinicie `NuanSystem.Api` desde Visual Studio y vuelva a iniciar sesion en WinForms.

El reinicio es necesario para cargar el middleware nuevo y el login nuevo es necesario para recibir el permiso `SECURITY.AUDIT.READ` dentro del token JWT.
