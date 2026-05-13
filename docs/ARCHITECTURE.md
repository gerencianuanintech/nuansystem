# Arquitectura NuanSystem

## Capas

`Api` recibe las solicitudes HTTP y delega los casos de uso a `Application`.  
`Application` contiene commands, queries, handlers, validators, DTOs y contratos.  
`Domain` mantiene el modelo de negocio sin dependencias de infraestructura.  
`Persistence` implementa acceso a datos para SQL Server y prepara la abstraccion para MySQL.  
`Infrastructure` implementa servicios transversales como autenticacion, cifrado, Serilog y configuracion.  
`SapIntegration` encapsula Service Layer y DI API para que SAP sea opcional.  
`Shared` contiene contratos seguros para compartir entre API y frontend.

## Flujo multiempresa

1. El usuario inicia sesion contra la API.
2. La API devuelve token JWT y empresas autorizadas.
3. El frontend selecciona empresa y envia `X-Company-Code` en cada request.
4. El middleware resuelve la empresa activa desde `NuanSystem_Master`.
5. `CompanyContext` expone la empresa activa durante la solicitud.
6. `Persistence` construye la conexion dinamica para la base de datos de esa empresa.

## Reglas de dependencia

- WinForms nunca se conecta directo a base de datos.
- Los formularios no contienen logica de negocio.
- SAP no se referencia desde Domain.
- Application define interfaces; Infrastructure, Persistence y SapIntegration las implementan.
- La seleccion de empresa afecta todas las operaciones de negocio.
