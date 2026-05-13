# Fase 5: MediatR y Casos de Uso

## Objetivo

Implementar el patron de casos de uso de NuanSystem usando MediatR, FluentValidation, commands, queries, handlers, DTOs, repositorios y responses estandarizados.

## Paquetes Agregados

En `NuanSystem.Application`:

- `MediatR`
- `FluentValidation.DependencyInjectionExtensions`

En `NuanSystem.Api`:

- `MediatR`

## Abstracciones de Mensajeria

Se agregaron contratos en:

- `src/Backend/NuanSystem.Application/Abstractions/Messaging`

Archivos:

- `ICommand<TResponse>`
- `IQuery<TResponse>`
- `ICommandHandler<TCommand, TResponse>`
- `IQueryHandler<TQuery, TResponse>`

Todos los commands y queries devuelven:

```csharp
Result<T>
```

Esto permite que los casos de uso comuniquen exito, errores de negocio y mensajes sin depender de HTTP.

## Resultado de Aplicacion

Se agrego:

- `src/Backend/NuanSystem.Application/Common/Models/Result.cs`

Responsabilidad:

- Representar exito o fallo de un caso de uso.
- Transportar `Value`.
- Transportar errores con `ApiError`.
- Mantener Application independiente de ASP.NET Core.

## Pipeline Behaviors

Se agregaron:

- `LoggingBehavior<TRequest, TResponse>`
- `ValidationBehavior<TRequest, TResponse>`

Ubicacion:

- `src/Backend/NuanSystem.Application/Common/Behaviors`

### LoggingBehavior

Registra:

- Inicio del caso de uso.
- Finalizacion del caso de uso.

### ValidationBehavior

Ejecuta todos los validators de FluentValidation asociados al request.

Si hay errores, lanza:

- `ApplicationValidationException`

## Manejo de Validaciones en API

`GlobalExceptionMiddleware` ahora captura:

- `ApplicationValidationException`

Y devuelve:

```http
400 Bad Request
```

Con formato:

```json
{
  "success": false,
  "message": "La solicitud contiene errores de validacion.",
  "data": null,
  "errors": []
}
```

## Registro en DI

En:

- `src/Backend/NuanSystem.Application/DependencyInjection/ApplicationServiceRegistration.cs`

Se registra:

- MediatR.
- `LoggingBehavior`.
- `ValidationBehavior`.
- Validators de FluentValidation desde el assembly de Application.

## Extension para Result HTTP

Se agrego:

- `src/Backend/NuanSystem.Api/Extensions/ResultExtensions.cs`

Responsabilidad:

- Convertir `Result<T>` a `IResult`.
- Devolver `200 OK` cuando `IsSuccess = true`.
- Devolver `400 Bad Request` cuando `IsSuccess = false`.

## Ejemplo Implementado: Customers

Se implemento un ejemplo real del patron completo para clientes.

### DTOs

Ubicacion:

- `src/Backend/NuanSystem.Application/Features/Customers/Dtos`

Archivos:

- `CustomerDto`
- `CreateCustomerData`

### Query

Ubicacion:

- `src/Backend/NuanSystem.Application/Features/Customers/Queries`

Archivos:

- `GetCustomersQuery`
- `GetCustomersQueryHandler`

### Command

Ubicacion:

- `src/Backend/NuanSystem.Application/Features/Customers/Commands`

Archivos:

- `CreateCustomerCommand`
- `CreateCustomerCommandValidator`
- `CreateCustomerCommandHandler`

### Repositorio

Contrato:

- `src/Backend/NuanSystem.Application/Abstractions/Data/ICustomerRepository.cs`

Implementacion:

- `src/Backend/NuanSystem.Persistence/Repositories/CustomerRepository.cs`

El repositorio usa:

- `DapperRepository`
- `ITenantConnectionFactory`

Por lo tanto, opera contra la base de datos de la empresa activa.

## Endpoints Implementados

Listar clientes:

```http
GET /api/customers
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

Body:

```json
{
  "code": "C0001",
  "name": "Cliente Demo",
  "taxIdentification": "0999999999001",
  "email": "cliente@demo.com",
  "phone": "0999999999",
  "addressLine": "Direccion demo"
}
```

## Patron para Nuevos Casos de Uso

Para agregar un modulo nuevo:

1. Crear DTOs en `Features/<Modulo>/Dtos`.
2. Crear commands en `Features/<Modulo>/Commands`.
3. Crear queries en `Features/<Modulo>/Queries`.
4. Crear validators con FluentValidation.
5. Crear handlers usando `ICommandHandler` o `IQueryHandler`.
6. Crear contrato de repositorio en `Application/Abstractions/Data`.
7. Crear implementacion Dapper en `Persistence/Repositories`.
8. Registrar repositorio en `PersistenceServiceRegistration`.
9. Exponer endpoints en API usando `ISender`.
10. Convertir resultados con `result.ToHttpResult()`.

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

El bloqueo sigue siendo operativo por autenticacion SQL Server.

## Pendientes

- Corregir login SQL Server.
- Crear base master.
- Crear usuario administrador.
- Crear empresa y base tenant.
- Inicializar tenant.
- Probar endpoints de clientes con JWT y `X-Company-Code`.
- Agregar update/delete/get-by-id en el modulo clientes.
- Replicar patron para articulos, documentos y SAP.

## Estado de la Fase

La Fase 5 queda implementada con MediatR, FluentValidation, behaviors, resultado de aplicacion, conversion HTTP y un caso de uso completo para Customers.

La siguiente fase natural es Fase 6: modulo de empresas, CRUD de empresas, validacion de conexion y asignacion de usuarios a empresas.
