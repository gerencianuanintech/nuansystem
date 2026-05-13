# Fase 18 - Autorizacion por permisos

## Objetivo

La fase 18 activa la autorizacion por permisos en la API.

Hasta esta fase el sistema tenia usuarios, roles y permisos, pero los endpoints solo exigian autenticacion. Ahora los endpoints principales requieren permisos concretos emitidos como claims en el JWT.

## Permisos agregados

Se agrego `PermissionCodes` en:

```text
src/Backend/NuanSystem.Shared/Constants/PermissionCodes.cs
```

Permisos registrados:

- `COMPANIES.MANAGE`
- `SECURITY.USERS.MANAGE`
- `SECURITY.ROLES.MANAGE`
- `CATALOG.CUSTOMERS.READ`
- `CATALOG.CUSTOMERS.MANAGE`
- `CATALOG.ITEMS.READ`
- `CATALOG.ITEMS.MANAGE`
- `SALES.DOCUMENTS.READ`
- `SALES.DOCUMENTS.MANAGE`
- `SAP.SYNC.READ`
- `SAP.SYNC.MANAGE`
- `SETTINGS.PARAMETERS.MANAGE`

## API

Se agrego:

```text
EndpointAuthorizationExtensions.RequirePermission(...)
```

Ubicacion:

```text
src/Backend/NuanSystem.Api/Extensions/EndpointAuthorizationExtensions.cs
```

La configuracion de autorizacion ahora crea una policy por cada permiso conocido:

```text
RequireClaim("permission", permission)
```

## Endpoints protegidos

Ejemplos:

- Empresas: `COMPANIES.MANAGE`
- Usuarios: `SECURITY.USERS.MANAGE`
- Roles: `SECURITY.ROLES.MANAGE`
- Clientes consulta: `CATALOG.CUSTOMERS.READ`
- Clientes cambios: `CATALOG.CUSTOMERS.MANAGE`
- Articulos consulta: `CATALOG.ITEMS.READ`
- Articulos cambios: `CATALOG.ITEMS.MANAGE`
- Documentos consulta: `SALES.DOCUMENTS.READ`
- Documentos creacion: `SALES.DOCUMENTS.MANAGE`
- Logs SAP: `SAP.SYNC.READ`
- Envio SAP: `SAP.SYNC.MANAGE`
- Parametros: `SETTINGS.PARAMETERS.MANAGE`

`/api/auth/login`, `/health` y `/api/companies/my-companies` conservan su comportamiento especial.

## Seed de permisos

El inicializador master ahora crea modulos adicionales:

- `CATALOG`
- `SALES`
- `SAP`
- `SETTINGS`

Tambien crea los permisos operativos y los asigna al rol `ADMIN`.

Version registrada:

```text
20260428.18 - Fase 18: permisos operativos por modulo
```

## Verificacion

Se compilo la solucion usando salida alterna:

```text
dotnet build NuanSystem.sln --no-restore -p:BaseOutputPath=artifacts\verify\
0 Advertencia(s)
0 Errores
```

Se ejecuto el inicializador master:

```text
NuanSystem.Api.exe --init-only
Inicializacion de base master completada.
```

Prueba API temporal:

```text
LoginSuccess = True
PermissionCount = 12
HasCustomersRead = True
CompaniesSuccess = True
CustomersSuccess = True
```

## Nota operativa

Luego de esta fase, si se crea un rol nuevo sin permisos, ese usuario podra iniciar sesion pero recibira respuestas `403 Forbidden` en los modulos que no tenga asignados.

Para que Visual Studio cargue los cambios, reinicie `NuanSystem.Api` y vuelva a iniciar sesion en WinForms para obtener un JWT nuevo con los permisos actualizados.
