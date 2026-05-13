# Fase 8: Modulo de Articulos

## Objetivo

Implementar el CRUD completo de articulos sobre la base de datos de la empresa activa, siguiendo el patron definido con MediatR, FluentValidation, Dapper y resolucion dinamica de tenant.

## Alcance Implementado

### 1. Contrato de repositorio

Se agrego:

- `IItemRepository`

Ubicacion:

- `src/Backend/NuanSystem.Application/Abstractions/Data/IItemRepository.cs`

Operaciones:

- `GetAllAsync`
- `GetByIdAsync`
- `CreateAsync`
- `ExistsByCodeAsync`
- `ExistsByCodeAsync` excluyendo un id
- `UpdateAsync`
- `SetActiveStateAsync`

### 2. DTOs

Ubicacion:

- `src/Backend/NuanSystem.Application/Features/Items/Dtos`

Archivos:

- `ItemDto`
- `CreateItemData`
- `UpdateItemData`

Campos principales:

- `Code`
- `Name`
- `Description`
- `UnitOfMeasure`
- `IsInventoryItem`
- `IsActive`

### 3. Queries

Ubicacion:

- `src/Backend/NuanSystem.Application/Features/Items/Queries`

Archivos:

- `GetItemsQuery`
- `GetItemsQueryHandler`
- `GetItemByIdQuery`
- `GetItemByIdQueryHandler`

### 4. Commands

Ubicacion:

- `src/Backend/NuanSystem.Application/Features/Items/Commands`

Archivos:

- `CreateItemCommand`
- `CreateItemCommandValidator`
- `CreateItemCommandHandler`
- `UpdateItemCommand`
- `UpdateItemCommandValidator`
- `UpdateItemCommandHandler`
- `DeleteItemCommand`
- `DeleteItemCommandValidator`
- `DeleteItemCommandHandler`

### 5. Validaciones

Validaciones principales:

- `Id > 0` para actualizar/eliminar.
- `Code` requerido y maximo 50 caracteres.
- `Name` requerido y maximo 200 caracteres.
- `Description` maximo 500 caracteres.
- `UnitOfMeasure` requerido y maximo 20 caracteres.
- Codigo unico al crear.
- Codigo unico excluyendo el mismo articulo al actualizar.

Normalizaciones aplicadas:

- `Code` se guarda en mayusculas.
- `UnitOfMeasure` se guarda en mayusculas.

### 6. Persistencia Dapper

Se agrego:

- `ItemRepository`

Ubicacion:

- `src/Backend/NuanSystem.Persistence/Repositories/ItemRepository.cs`

El repositorio usa:

- `DapperRepository`
- `ITenantConnectionFactory`

Por tanto opera contra la base de datos de la empresa activa.

### 7. Endpoints REST

Listar articulos:

```http
GET /api/items
Authorization: Bearer <token>
X-Company-Code: EMPRESA01
```

Consultar articulo por id:

```http
GET /api/items/{id}
Authorization: Bearer <token>
X-Company-Code: EMPRESA01
```

Crear articulo:

```http
POST /api/items
Authorization: Bearer <token>
X-Company-Code: EMPRESA01
Content-Type: application/json
```

Actualizar articulo:

```http
PUT /api/items/{id}
Authorization: Bearer <token>
X-Company-Code: EMPRESA01
Content-Type: application/json
```

Eliminar articulo logicamente:

```http
DELETE /api/items/{id}
Authorization: Bearer <token>
X-Company-Code: EMPRESA01
```

### 8. Ejemplo de body

Crear:

```json
{
  "code": "A0001",
  "name": "Articulo Demo",
  "description": "Articulo de prueba",
  "unitOfMeasure": "UND",
  "isInventoryItem": true
}
```

Actualizar:

```json
{
  "code": "A0001",
  "name": "Articulo Demo Actualizado",
  "description": "Articulo de prueba",
  "unitOfMeasure": "UND",
  "isInventoryItem": true,
  "isActive": true
}
```

## Flujo del CRUD

```text
Endpoint API
  -> ISender
    -> Command/Query
      -> FluentValidation
      -> Handler
        -> IItemRepository
          -> ITenantConnectionFactory
            -> Base de datos de empresa activa
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

Tambien se levanto temporalmente la API y se valido:

```http
GET http://localhost:5081/health
```

Resultado:

```text
200 Healthy
```

## Pendientes

- Probar CRUD contra SQL Server real cuando exista master, usuario, empresa y tenant.
- Agregar campos de precio, grupo, codigo de barras e impuestos si el negocio los requiere.
- Agregar validaciones de documentos antes de inactivar articulos usados en movimientos.
- Preparar mapeo SAP Business One de articulos.
- Agregar permisos finos por accion.

## Estado de la Fase

La Fase 8 queda implementada a nivel de API, Application, Persistence y documentacion.

La siguiente fase natural es Fase 9: modulo de documentos comerciales.
