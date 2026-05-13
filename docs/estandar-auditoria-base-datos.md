# Estandar de auditoria en base de datos

Toda tabla de mantenimiento debe tener auditoria basica y toda modificacion sensible debe registrar auditoria detallada por modulo.

## Auditoria basica por tabla

Campos estandar:

```sql
CreatedByUserId int NULL
CreatedByUserName nvarchar(120) NULL
CreatedAt datetime2(0) NOT NULL
UpdatedByUserId int NULL
UpdatedByUserName nvarchar(120) NULL
UpdatedAt datetime2(0) NULL
IsDeleted bit NOT NULL
DeletedByUserId int NULL
DeletedByUserName nvarchar(120) NULL
DeletedAt datetime2(0) NULL
```

Reglas:

- `POST`: llena `CreatedByUserId`, `CreatedByUserName`, `CreatedAt`.
- `PUT`: llena `UpdatedByUserId`, `UpdatedByUserName`, `UpdatedAt`.
- `PATCH`: llena `UpdatedByUserId`, `UpdatedByUserName`, `UpdatedAt`.
- `DELETE`: debe ser eliminacion logica; llena `IsDeleted`, `DeletedByUserId`, `DeletedByUserName`, `DeletedAt`.
- Los listados y busquedas operativas deben filtrar `IsDeleted = 0`.

## Auditoria detallada por modulo

No se usara una sola tabla global para todos los cambios. Se usaran tablas por dominio funcional.

Ejemplos:

```text
AuditSecurityChanges
AuditCatalogChanges
AuditSalesChanges
AuditInventoryChanges
AuditSystemChanges
```

Estructura base:

```sql
Id bigint IDENTITY(1,1) PRIMARY KEY
EntityName nvarchar(120) NOT NULL
RecordId nvarchar(80) NOT NULL
Action nvarchar(20) NOT NULL
FieldName nvarchar(120) NOT NULL
OldValue nvarchar(max) NULL
NewValue nvarchar(max) NULL
UserId int NULL
UserName nvarchar(120) NULL
Source nvarchar(60) NOT NULL
CreatedAt datetime2(0) NOT NULL
```

Indices recomendados:

```sql
(EntityName, RecordId, CreatedAt DESC)
(UserId, CreatedAt DESC)
(CreatedAt DESC)
```

## Implementacion en SP

La auditoria debe vivir en procedimientos almacenados, junto al `INSERT`, `UPDATE`, `PATCH` o eliminacion logica.

El API debe tomar el usuario desde el JWT y pasarlo a los comandos internos. WinForms no debe enviar manualmente el usuario de auditoria para evitar manipulacion.

Primer mantenimiento aplicado:

```text
SecurityOperations
AuditSecurityChanges
```
