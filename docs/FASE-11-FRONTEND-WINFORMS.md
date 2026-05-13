# Fase 11 - Frontend WinForms inicial

## Objetivo

La fase 11 inicia el cliente Windows Forms de NuanSystem con una estructura por capas, consumo de API REST y flujo base de usuario:

1. Login.
2. Seleccion de empresa.
3. Menu principal.
4. Consulta inicial de logs SAP.

El frontend no se conecta a la base de datos. Toda operacion pasa por la API REST.

## Proyectos usados

### NuanSystem.WinForms

Proyecto ejecutable. Se encarga de la composicion inicial:

- Crea `HttpClient`.
- Configura la URL base de la API.
- Mantiene una instancia de `ApiSession`.
- Construye servicios, viewmodels y formularios.
- Ejecuta el flujo Login -> Empresa -> Menu principal.

La URL por defecto de la API es:

```text
http://localhost:5081
```

Puede cambiarse con la variable de entorno:

```text
NUANSYSTEM_API_URL
```

### NuanSystem.WinForms.Services

Capa de consumo HTTP reutilizable:

- `ApiSession`: guarda token JWT, usuario actual y empresa activa.
- `NuanApiClient`: cliente HTTP comun que agrega `Authorization: Bearer` y `X-Company-Code`.
- `AuthenticationClient`: consume `POST /api/auth/login`.
- `CompanyClient`: consume `GET /api/companies/my-companies`.
- `SapClient`: consume endpoints de integracion SAP.

Esta capa interpreta `ApiResponse<T>` y transforma errores de API en `ApiClientException`.

### NuanSystem.WinForms.ViewModels

Capa de estado y coordinacion de pantalla:

- `LoginViewModel`: valida credenciales ingresadas y ejecuta login.
- `CompanySelectionViewModel`: carga empresas del usuario y confirma empresa activa.
- `ShellViewModel`: define usuario, empresa y modulos del menu principal.
- `SapSyncLogViewModel`: carga logs de integracion SAP.

Los formularios no contienen reglas de negocio ni acceso HTTP directo.

### NuanSystem.WinForms.Forms

Pantallas iniciales:

- `LoginForm`: formulario de acceso.
- `CompanySelectionForm`: seleccion de empresa/base activa.
- `MainForm`: menu principal tipo sistema empresarial.
- `SapSyncLogForm`: consulta inicial de bitacora SAP.

Por ahora se usan controles WinForms nativos para mantener la solucion compilable sin depender aun de licencias o paquetes DevExpress. La estructura deja preparado el reemplazo progresivo por `XtraForm`, `GridControl`, `RibbonControl` o `AccordionControl`.

## Flujo tecnico

1. `Program` crea la composicion del frontend.
2. `LoginForm` llama a `LoginViewModel`.
3. `LoginViewModel` usa `AuthenticationClient`.
4. `AuthenticationClient` llama `POST /api/auth/login`.
5. `ApiSession` conserva el token JWT.
6. `CompanySelectionForm` carga empresas autorizadas.
7. Al confirmar empresa, `ApiSession` guarda `CompanyCode`.
8. Cada request posterior envia:

```text
Authorization: Bearer {token}
X-Company-Code: {codigoEmpresa}
```

## Modulos visibles

El menu principal queda preparado con:

- Clientes.
- Articulos.
- Documentos.
- Integracion SAP.
- Configuracion.

En esta fase solo se abre una pantalla funcional para `Integracion SAP`, orientada a consultar logs. Los demas modulos quedan listos para conectarse en fases posteriores.

## Archivos principales

- `src/Frontend/NuanSystem.WinForms/Program.cs`
- `src/Frontend/NuanSystem.WinForms.Services/Http/NuanApiClient.cs`
- `src/Frontend/NuanSystem.WinForms.Services/Session/ApiSession.cs`
- `src/Frontend/NuanSystem.WinForms.Services/Authentication/AuthenticationClient.cs`
- `src/Frontend/NuanSystem.WinForms.Services/Companies/CompanyClient.cs`
- `src/Frontend/NuanSystem.WinForms.Services/Sap/SapClient.cs`
- `src/Frontend/NuanSystem.WinForms.ViewModels/Auth/LoginViewModel.cs`
- `src/Frontend/NuanSystem.WinForms.ViewModels/Companies/CompanySelectionViewModel.cs`
- `src/Frontend/NuanSystem.WinForms.ViewModels/Shell/ShellViewModel.cs`
- `src/Frontend/NuanSystem.WinForms.Forms/Auth/LoginForm.cs`
- `src/Frontend/NuanSystem.WinForms.Forms/Companies/CompanySelectionForm.cs`
- `src/Frontend/NuanSystem.WinForms.Forms/Shell/MainForm.cs`
- `src/Frontend/NuanSystem.WinForms.Forms/Sap/SapSyncLogForm.cs`

## Verificacion

Se compilo la solucion completa:

```text
dotnet build NuanSystem.sln --no-restore
0 Advertencia(s)
0 Errores
```

Para probar el flujo visual, primero debe estar ejecutandose la API y debe existir un usuario valido en la base master.

```text
dotnet run --project src/Backend/NuanSystem.Api/NuanSystem.Api.csproj --launch-profile http
dotnet run --project src/Frontend/NuanSystem.WinForms/NuanSystem.WinForms.csproj
```

Si se usa otra URL para la API:

```text
set NUANSYSTEM_API_URL=http://localhost:5081
```
