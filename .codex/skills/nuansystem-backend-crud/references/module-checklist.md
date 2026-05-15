# Backend Module Checklist

Use this checklist when adding or normalizing a maintenance module.

## Application

- Create DTOs for list/detail responses.
- Create command records for create, update, delete, and special actions.
- Create query records for list/detail/lookups.
- Add handlers that call repository contracts and return `Result<T>`.
- Add validators for commands. Keep max lengths aligned with SQL column definitions.
- Include `CreatedByUserId`, `CreatedByUserName`, `UpdatedByUserId`, `UpdatedByUserName`, `DeletedByUserId`, and `DeletedByUserName` in internal data objects when persistence needs audit values.

## Persistence

- Implement repository methods using `ITenantConnectionFactory` for tenant data.
- Use procedure constants near the top of the repository.
- Execute Dapper with `new CommandDefinition(..., cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure)`.
- Convert `SELECT @@ROWCOUNT` results to `bool` with `affectedRows > 0`.
- Keep procedure names consistent with `SP_NA_{VERBO}_{ENTIDAD}{ACCION}`.

## Api

- Add route handlers near related endpoints in `Program.cs`.
- Use existing result extension helpers instead of hand-building response envelopes.
- Apply `.RequirePermission(PermissionCodes.X)` to protected endpoints.
- Pull audit user fields from the authenticated user on write operations.
- Keep route names and paths consistent with existing modules such as customers, items, roles, and settings.

## Validation

- Run `dotnet build .\NuanSystem.sln` when practical.
- If full solution build is too noisy, build the touched project and its direct dependencies.
