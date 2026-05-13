# Fase 16 - Administracion de usuarios

## Objetivo

La fase 16 agrega administracion basica de usuarios y asignacion de empresas desde WinForms.

Esto completa una parte importante de seguridad operativa: ya no es necesario crear usuarios o relacionarlos con empresas mediante SQL manual.

## Backend

### Application

Se agregaron contratos, DTOs y casos de uso:

- `IUserAdminRepository`
- `UserAdminDto`
- `RoleDto`
- `CreateUserData`
- `GetUsersQuery`
- `GetUsersQueryHandler`
- `GetRolesQuery`
- `GetRolesQueryHandler`
- `CreateUserCommand`
- `CreateUserCommandValidator`
- `CreateUserCommandHandler`

El handler de creacion usa `IPasswordHasher`, por lo que las claves nuevas usan el mismo formato PBKDF2-SHA256 del login existente.

### Persistence

Se agrego:

- `UserAdminRepository`

Funciones:

- Listar usuarios.
- Listar roles.
- Crear usuarios.
- Asignar rol inicial.
- Consultar roles y empresas asignadas por usuario.

### API

Endpoints agregados:

- `GET /api/users`
- `POST /api/users`
- `GET /api/users/roles`

Se reutiliza el endpoint existente:

- `POST /api/companies/assign-user`

## Frontend WinForms

### Services

Se agrego:

- `IUserClient`
- `UserClient`

Modelos:

- `UserAdminItem`
- `RoleItem`
- `CreateUserRequest`

Tambien se extendio `CompanyClient` con:

- `AssignUserAsync`

Modelo:

- `AssignUserCompanyRequest`

### ViewModels

Se agrego:

- `UsersViewModel`

Funciones:

- Cargar usuarios.
- Cargar roles y empresas activas.
- Crear usuario.
- Asignar empresa a usuario.

### Forms

Se agregaron:

- `UsersForm`
- `UserEditForm`
- `UserCompanyAssignForm`

Funciones disponibles:

- Listar usuarios.
- Crear usuario con rol inicial.
- Asignar empresa a usuario.
- Visualizar roles y empresas asignadas.

## Menu principal

Se agrego el modulo:

```text
Usuarios
```

Clave interna:

```text
users
```

## Verificacion

Como Visual Studio mantiene la API en ejecucion y bloquea DLLs de salida normal, se valido con salida alterna:

```text
dotnet build NuanSystem.sln --no-restore -p:BaseOutputPath=artifacts\verify\
0 Advertencia(s)
0 Errores
```

Prueba de API temporal:

```text
LoginSuccess = True
UsersSuccess = True
UserCount = 1
RolesSuccess = True
RoleCount = 1
```

## Nota operativa

Para usar esta fase desde Visual Studio, detenga y vuelva a iniciar `NuanSystem.Api` y `NuanSystem.WinForms`. La API anterior no tiene cargados los endpoints nuevos de usuarios.
