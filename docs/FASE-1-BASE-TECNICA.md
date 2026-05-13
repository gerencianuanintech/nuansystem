# Fase 1: Base Tecnica

## Objetivo

Dejar lista la base inicial de NuanSystem para que la API arranque con una estructura limpia, registro de dependencias por capa, logging, Swagger, manejo centralizado de errores, respuesta estandarizada y preparacion para crear la base maestra `NuanSystem_Master` en SQL Server.

## Alcance Implementado

### 1. Solucion y proyectos

La solucion `NuanSystem.sln` quedo organizada en proyectos separados para mantener limites arquitectonicos claros:

- `NuanSystem.Api`: entrada HTTP, Swagger, middleware y arranque.
- `NuanSystem.Application`: contratos de casos de uso y abstracciones.
- `NuanSystem.Domain`: dominio puro, sin dependencias tecnicas.
- `NuanSystem.Infrastructure`: servicios transversales futuros.
- `NuanSystem.Persistence`: acceso a datos e inicializacion SQL Server.
- `NuanSystem.SapIntegration`: integracion SAP desacoplada.
- `NuanSystem.Shared`: respuestas y contratos compartidos.
- `NuanSystem.WinForms`: aplicacion cliente.
- `NuanSystem.WinForms.Services`: consumo HTTP.
- `NuanSystem.WinForms.ViewModels`: logica de presentacion.
- `NuanSystem.WinForms.Forms`: formularios.
- `NuanSystem.WinForms.Controls`: controles reutilizables.

### 2. Paquetes instalados

En la API:

- `Serilog.AspNetCore`
- `Serilog.Sinks.Console`
- `Serilog.Sinks.File`
- `Swashbuckle.AspNetCore`

En persistencia:

- `Microsoft.Data.SqlClient`
- `Microsoft.Extensions.Configuration.Abstractions`
- `Microsoft.Extensions.DependencyInjection.Abstractions`
- `Microsoft.Extensions.Options`

En proyectos de registro por capa:

- `Microsoft.Extensions.DependencyInjection.Abstractions`

### 3. Configuracion de API

Se reemplazo el template inicial de WeatherForecast por una API base productiva:

- Swagger UI disponible en `/swagger`.
- Health check disponible en `/health`.
- Endpoint raiz `/` con respuesta estandarizada.
- `UseHttpsRedirection`.
- Serilog configurado desde `Program.cs`.
- Soporte para archivo `appsettings.Local.json`.
- Soporte para argumento `--init-only`.

Archivo principal:

- `src/Backend/NuanSystem.Api/Program.cs`

### 4. Logging con Serilog

Se configuro logging hacia:

- Consola.
- Archivos diarios en `logs/nuansystem-api-.log`.

El objetivo es tener trazabilidad desde el inicio sin depender solo del logging por defecto de ASP.NET Core.

### 5. Swagger/OpenAPI

Se habilito Swagger mediante Swashbuckle:

- `AddEndpointsApiExplorer`.
- `AddSwaggerGen`.
- `UseSwagger`.
- `UseSwaggerUI`.

Por ahora la configuracion es basica. En fases posteriores se agregara documentacion de JWT, headers multiempresa como `X-Company-Code` y agrupacion por modulos.

### 6. Manejo centralizado de errores

Se creo el middleware:

- `src/Backend/NuanSystem.Api/Middleware/GlobalExceptionMiddleware.cs`

Responsabilidades:

- Capturar excepciones no controladas.
- Registrar el error con Serilog.
- Devolver una respuesta JSON uniforme.
- Ocultar detalles tecnicos en ambientes productivos.
- Mostrar mensaje tecnico solo en desarrollo.

Extension creada:

- `src/Backend/NuanSystem.Api/Extensions/ApplicationBuilderExtensions.cs`

### 7. Respuesta estandarizada

Se crearon contratos compartidos:

- `src/Backend/NuanSystem.Shared/Responses/ApiResponse.cs`
- `src/Backend/NuanSystem.Shared/Responses/ApiError.cs`

Formato base:

```json
{
  "success": true,
  "message": "Operacion completada correctamente",
  "data": {},
  "errors": []
}
```

Esto prepara la API para que WinForms pueda manejar respuestas y errores de forma consistente.

### 8. Registro de dependencias por capa

Se crearon clases de extension para que cada proyecto registre sus propios servicios:

- `NuanSystem.Application.DependencyInjection.ApplicationServiceRegistration`
- `NuanSystem.Infrastructure.DependencyInjection.InfrastructureServiceRegistration`
- `NuanSystem.Persistence.DependencyInjection.PersistenceServiceRegistration`
- `NuanSystem.SapIntegration.DependencyInjection.SapIntegrationServiceRegistration`

La API centraliza la composicion en:

- `src/Backend/NuanSystem.Api/Extensions/ServiceCollectionExtensions.cs`

Esto permite crecer por modulos sin llenar `Program.cs` con detalles de infraestructura.

### 9. Configuracion local de SQL Server

Se agrego soporte para:

- `appsettings.json`: configuracion base sin secretos.
- `appsettings.Development.json`: configuracion de desarrollo sin secretos.
- `appsettings.Local.json`: configuracion local con credenciales, ignorada por git.

El archivo local contiene la cadena:

```json
{
  "ConnectionStrings": {
    "SqlServerAdmin": "Server=localhost,1433;Database=master;User Id=sa;Password=...;TrustServerCertificate=True;Encrypt=True;MultipleActiveResultSets=False"
  }
}
```

`appsettings.Local.json` esta excluido en `.gitignore` con:

```text
**/appsettings.Local.json
```

### 10. Inicializador de base master

Se creo la abstraccion:

- `src/Backend/NuanSystem.Application/Abstractions/Data/IMasterDatabaseInitializer.cs`

Y su implementacion SQL Server:

- `src/Backend/NuanSystem.Persistence/Services/SqlServerMasterDatabaseInitializer.cs`

Responsabilidades actuales:

- Conectarse a SQL Server usando `ConnectionStrings:SqlServerAdmin`.
- Validar el nombre de la base master.
- Crear `NuanSystem_Master` si no existe.
- Conectarse a `NuanSystem_Master`.
- Crear la tabla inicial `dbo.SystemParameters`.
- Insertar el parametro inicial `System.Name = NuanSystem`.

El modo de ejecucion preparado es:

```powershell
$env:ASPNETCORE_ENVIRONMENT='Development'
dotnet run --project src\Backend\NuanSystem.Api\NuanSystem.Api.csproj -- --init-only
```

Con `--init-only`, la API crea o valida la base master y finaliza sin quedarse escuchando HTTP.

## Verificacion Realizada

Se ejecuto compilacion completa:

```powershell
dotnet build NuanSystem.sln --no-restore
```

Resultado:

```text
Compilacion correcta.
0 advertencias
0 errores
```

Tambien se valido que el puerto SQL Server responde:

```powershell
Test-NetConnection -ComputerName localhost -Port 1433
```

Resultado:

```text
TcpTestSucceeded: True
```

## Bloqueo Pendiente

La creacion de `NuanSystem_Master` no se pudo completar porque SQL Server rechazo el login del usuario `sa`.

Prueba realizada:

```powershell
sqlcmd -S localhost,1433 -U sa -P "sapsrvbbdd0192837465" -Q "SELECT @@SERVERNAME AS ServerName" -C
```

Resultado:

```text
Login failed for user 'sa'.
```

Esto confirma que:

- SQL Server esta escuchando en `localhost,1433`.
- El problema no es red ni puerto.
- El problema esta en autenticacion del login `sa`.

Antes de continuar con la creacion real de la base master, se debe verificar:

- Que `sa` este habilitado.
- Que SQL Server tenga autenticacion mixta habilitada.
- Que la clave sea correcta.
- Que el login no este bloqueado.
- Que el servicio SQL Server se haya reiniciado despues de cambiar autenticacion.

## Estado de la Fase

La base tecnica de Fase 1 esta implementada y compila correctamente.

Pendiente operativo:

- Corregir credencial/acceso SQL Server.
- Ejecutar nuevamente `--init-only`.
- Confirmar existencia de `NuanSystem_Master`.
- Confirmar existencia de `dbo.SystemParameters`.

Una vez validado eso, se puede pasar a la Fase 2: arquitectura multiempresa.
