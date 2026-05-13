# Estandar de procedimientos almacenados

Toda operacion que persista, consulte o modifique informacion en SQL Server debe pasar por procedimientos almacenados. Los repositorios de `Persistence` no deben contener SQL directo para CRUD; deben ejecutar Dapper con `CommandType.StoredProcedure`.

## Nomenclatura

Formato:

```text
SP_NA_{VERBO_HTTP}_{ENTIDAD}{ACCION}
```

Ejemplos:

```text
SP_NA_GET_OPERACIONSEGURIDADLISTAR
SP_NA_GET_OPERACIONSEGURIDADBUSCARPORID
SP_NA_POST_OPERACIONSEGURIDADCREAR
SP_NA_PUT_OPERACIONSEGURIDADACTUALIZAR
SP_NA_DELETE_OPERACIONSEGURIDADELIMINAR
SP_NA_PATCH_OPERACIONSEGURIDADACTIVAR
```

## Verbos

- `GET`: consultas, listados, busquedas y validaciones de existencia.
- `POST`: creacion de registros.
- `PUT`: actualizacion completa de registros.
- `PATCH`: actualizacion parcial, activacion, inactivacion, cambio de estado.
- `DELETE`: eliminacion logica o fisica, segun la regla del modulo.

## Reglas

- Crear o actualizar los procedimientos desde el inicializador o scripts de base de datos.
- Usar parametros con los mismos nombres que los modelos de datos cuando sea posible.
- Para `POST`, devolver el `Id` creado con `SELECT CAST(SCOPE_IDENTITY() AS int)`.
- Para `PUT`, `PATCH` y `DELETE`, devolver `SELECT @@ROWCOUNT`.
- Para validaciones de existencia, devolver `COUNT(1)`.
- En C#, ejecutar siempre con `CommandType.StoredProcedure`.

## Ejemplo en repositorio

```csharp
return await connection.ExecuteScalarAsync<int>(
    new CommandDefinition(
        "dbo.SP_NA_POST_OPERACIONSEGURIDADCREAR",
        operation,
        cancellationToken: cancellationToken,
        commandType: CommandType.StoredProcedure));
```
