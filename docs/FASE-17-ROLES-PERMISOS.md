# Fase 17 - Administracion de roles y permisos

## Objetivo

La fase 17 agrega administracion de roles y permisos desde API y WinForms.

Con esta fase se completa la base operativa de seguridad:

- Usuarios.
- Roles.
- Permisos.
- Asignacion de permisos a roles.
- Asignacion de empresas a usuarios.

## Backend

### Application

Se agregaron:

- `IRoleAdminRepository`
- `RoleAdminDto`
- `PermissionDto`
- `CreateRoleData`
- `GetRolesAdminQuery`
- `GetRolesAdminQueryHandler`
- `GetPermissionsQuery`
- `GetPermissionsQueryHandler`
- `CreateRoleCommand`
- `CreateRoleCommandValidator`
- `CreateRoleCommandHandler`
- `AssignRolePermissionCommand`
- `AssignRolePermissionCommandValidator`
- `AssignRolePermissionCommandHandler`

### Persistence

Se agrego:

- `RoleAdminRepository`

Funciones:

- Listar roles con permisos asignados.
- Listar permisos disponibles.
- Crear rol.
- Asignar permiso a rol.

### API

Endpoints agregados:

- `GET /api/roles`
- `POST /api/roles`
- `GET /api/roles/permissions`
- `POST /api/roles/assign-permission`

Todos requieren autenticacion.

## Frontend WinForms

### Services

Se agrego:

- `IRoleClient`
- `RoleClient`

Modelos:

- `RoleAdminItem`
- `PermissionItem`
- `CreateRoleRequest`
- `AssignRolePermissionRequest`

### ViewModels

Se agrego:

- `RolesViewModel`

Funciones:

- Cargar roles.
- Cargar permisos activos.
- Crear rol.
- Asignar permiso a rol.

### Forms

Se agregaron:

- `RolesForm`
- `RoleEditForm`
- `RolePermissionAssignForm`

Funciones disponibles:

- Listar roles.
- Crear rol.
- Visualizar permisos asignados.
- Asignar permisos disponibles a un rol.

## Menu principal

Se agrego el modulo:

```text
Roles
```

Clave interna:

```text
roles
```

## Verificacion

Compilacion usando salida alterna:

```text
dotnet build NuanSystem.sln --no-restore -p:BaseOutputPath=artifacts\verify\
0 Advertencia(s)
0 Errores
```

Prueba API temporal:

```text
LoginSuccess = True
RolesSuccess = True
RoleCount = 1
PermissionsSuccess = True
PermissionCount = 3
```

## Nota operativa

Para ver el modulo en Visual Studio, detenga y vuelva a iniciar `NuanSystem.Api` y `NuanSystem.WinForms`.
