# Fase 7: Modulo de Clientes

## Objetivo

Completar el CRUD de clientes sobre la base de datos de la empresa activa, usando MediatR, FluentValidation, Dapper, `ICompanyContext` y resolucion dinamica de connection string por tenant.

## Alcance Implementado

### 1. Contrato de repositorio extendido

Se amplio:

- `ICustomerRepository`

Ubicacion:

- `src/Backend/NuanSystem.Application/Abstractions/Data/ICustomerRepository.cs`

Operaciones disponibles:

- `GetAllAsync`
- `GetByIdAsync`
- `CreateAsync`
- `ExistsByCodeAsync`
- `ExistsByCodeAsync` excluyendo un id
- `UpdateAsync`
- `SetActiveStateAsync`

### 2. DTOs

Se agrego:

- `UpdateCustomerData`

Ubicacion:

- `src/Backend/NuanSystem.Application/Features/Customers/Dtos`

DTOs actuales:

- `CustomerDto`
- `CreateCustomerData`
- `UpdateCustomerData`

### 3. Queries

Se agrego:

- `GetCustomerByIdQuery`
- `GetCustomerByIdQueryHandler`

Ubicacion:

- `src/Backend/NuanSystem.Application/Features/Customers/Queries`

La query devuelve error de negocio si el cliente no existe.

### 4. Commands

Se agregaron:

- `UpdateCustomerCommand`
- `UpdateCustomerCommandValidator`
- `UpdateCustomerCommandHandler`
- `DeleteCustomerCommand`
- `DeleteCustomerCommandValidator`
- `DeleteCustomerCommandHandler`

Ubicacion:

- `src/Backend/NuanSystem.Application/Features/Customers/Commands`

### 5. Validaciones

Validaciones principales:

- `Id > 0`.
- `Code` requerido y maximo 50 caracteres.
- `Name` requerido y maximo 200 caracteres.
- `TaxIdentification` maximo 50 caracteres.
- `Email` formato valido y maximo 256 caracteres.
- `Phone` maximo 50 caracteres.
- `AddressLine` maximo 300 caracteres.
- Codigo unico al crear.
- Codigo unico excluyendo el mismo cliente al actualizar.

### 6. Persistencia Dapper

Se actualizo:

- `CustomerRepository`

Ubicacion:

- `src/Backend/NuanSystem.Persistence/Repositories/CustomerRepository.cs`

Operaciones nuevas:

- Actualizacion completa.
- Validacion de codigo duplicado excluyendo id.
- Eliminacion logica mediante `IsActive = 0`.

El repositorio sigue usando:

- `ITenantConnectionFactory`

Por lo tanto, siempre opera contra la base de datos de la empresa activa.

### 7. Endpoints REST

Listar clientes:

```http
GET /api/customers
Authorization: Bearer <token>
X-Company-Code: EMPRESA01
```

Consultar cliente por id:

```http
GET /api/customers/{id}
Authorization: Bearer <token>
X-Company-Code: EMPRESA01
```

Crear cliente:

```http
POST /api/customers
Authorization: Bearer <token>
X-Company-Code: EMPRESA01
Content-Type: application/json
```

Actualizar cliente:

```http
PUT /api/customers/{id}
Authorization: Bearer <token>
X-Company-Code: EMPRESA01
Content-Type: application/json
```

Eliminar cliente logicamente:

```http
DELETE /api/customers/{id}
Authorization: Bearer <token>
X-Company-Code: EMPRESA01
```

### 8. Ejemplo de body para crear/actualizar

```json
{
  "code": "C0001",
  "name": "Cliente Demo",
  "taxIdentification": "0999999999001",
  "email": "cliente@demo.com",
  "phone": "0999999999",
  "addressLine": "Direccion demo",
  "isActive": true
}
```

Para `POST`, el campo `isActive` no es necesario porque el registro se crea activo por defecto en base de datos.

## Flujo del CRUD

```text
Endpoint API
  -> ISender
    -> Command/Query
      -> FluentValidation
      -> Handler
        -> ICustomerRepository
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

- Probar CRUD contra SQL Server cuando exista base master, usuario, empresa y tenant.
- Agregar filtros de busqueda/paginacion.
- Agregar reglas de permisos por accion.
- Agregar auditoria de usuario creador/modificador.
- Agregar validaciones de documentos antes de permitir eliminar/inactivar clientes con movimientos.

## Estado de la Fase

La Fase 7 queda implementada a nivel de API, Application, Persistence y documentacion.

La siguiente fase natural es Fase 8: modulo de articulos.
