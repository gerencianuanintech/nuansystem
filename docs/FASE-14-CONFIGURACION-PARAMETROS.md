# Fase 14 - Configuracion y parametros por empresa

## Objetivo

La fase 14 agrega un modulo de configuracion para consultar y editar parametros por empresa activa.

Los parametros se guardan en la base master, en `dbo.CompanyParameters`, usando el `CompanyId` resuelto por el contexto multiempresa. El frontend no accede a SQL Server; todo pasa por la API REST.

## Backend

### Application

Se agregaron contratos, DTOs y casos de uso:

- `ICompanyParameterRepository`
- `CompanyParameterDto`
- `UpsertCompanyParameterData`
- `GetCompanyParametersQuery`
- `GetCompanyParametersQueryHandler`
- `UpsertCompanyParameterCommand`
- `UpsertCompanyParameterCommandValidator`
- `UpsertCompanyParameterCommandHandler`

La validacion de clave acepta letras, numeros, punto, guion, dos puntos y guion bajo.

### Persistence

Se agrego:

- `CompanyParameterRepository`

Este repositorio:

- Lee parametros de la empresa activa.
- Crea un parametro si no existe.
- Actualiza valor, descripcion y `UpdatedAt` si ya existe.
- Usa `IMasterConnectionFactory` porque los parametros viven en `NuanSystem_Master`.
- Usa `ICompanyContext` para obtener `CompanyId`.

### API

Endpoints agregados:

- `GET /api/settings/parameters`
- `PUT /api/settings/parameters/{key}`

Ambos requieren autenticacion y empresa activa mediante `X-Company-Code`.

## Frontend WinForms

### Services

Se agrego:

- `ISettingsClient`
- `SettingsClient`
- `CompanyParameterItem`
- `SaveCompanyParameterRequest`

### ViewModels

Se agrego:

- `SettingsViewModel`

Permite cargar parametros y guardar cambios.

### Forms

Se agrego:

- `SettingsForm`
- `ParameterEditForm`

El modulo permite:

- Listar parametros de la empresa activa.
- Crear parametros.
- Editar valor y descripcion de parametros existentes.

`MainForm` ahora abre el modulo `Configuracion` desde el menu principal.

## Prueba realizada

Como Visual Studio tenia la API ejecutandose y bloqueaba los DLL de salida normal, se valido la compilacion en una carpeta alterna:

```text
dotnet build NuanSystem.sln --no-restore -p:BaseOutputPath=artifacts\verify\
0 Advertencia(s)
0 Errores
```

Tambien se levanto una instancia temporal en `http://localhost:5099` para probar endpoints:

```text
GET /health
Healthy

POST /api/auth/login
LoginSuccess = True

GET /api/settings/parameters
ParametersSuccess = True

PUT /api/settings/parameters/Documents.DefaultCurrency
SaveSuccess = True
SavedValue = USD
```

Parametro creado para empresa `DEMO`:

```text
Documents.DefaultCurrency = USD
```

## Nota para Visual Studio

Para que Visual Studio use esta fase, detenga y vuelva a iniciar `NuanSystem.Api`. Mientras la API anterior siga ejecutandose, no tendra cargados los endpoints nuevos de configuracion.
