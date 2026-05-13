# Fase 15 - Administracion de empresas desde WinForms

## Objetivo

La fase 15 agrega una pantalla operativa para administrar empresas desde el frontend WinForms.

Hasta esta fase las empresas podian crearse desde la API o Swagger. Ahora el usuario administrador puede listar empresas, crear nuevas empresas y validar conexiones SQL Server desde la aplicacion de escritorio.

## Backend usado

No se agregaron endpoints nuevos. Se reutilizaron los endpoints ya existentes:

- `GET /api/companies`
- `POST /api/companies`
- `POST /api/companies/validate-connection`

Todos requieren autenticacion.

## Frontend Services

Se extendio `CompanyClient` con:

- `GetAllAsync`
- `CreateAsync`
- `ValidateConnectionAsync`

Modelos agregados:

- `CompanyAdminItem`
- `CreateCompanyRequest`
- `ValidateCompanyConnectionRequest`
- `CompanyConnectionTestItem`

Ubicacion:

- `src/Frontend/NuanSystem.WinForms.Services/Companies`

## ViewModels

Se agrego:

- `CompaniesAdminViewModel`

Responsabilidades:

- Cargar empresas.
- Crear empresa.
- Validar conexion de empresa.

## Forms

Se agregaron:

- `CompaniesAdminForm`
- `CompanyEditForm`

Funciones disponibles:

- Listar empresas registradas.
- Crear nueva empresa SQL Server.
- Validar conexion de una empresa existente solicitando la clave de base de datos.

El formulario de creacion permite capturar:

- Codigo.
- Nombre comercial.
- Razon social.
- Identificacion.
- Motor de base de datos.
- Servidor.
- Puerto.
- Base de datos.
- Usuario.
- Clave.
- Modo SAP.
- Estado activo.
- Validacion de conexion antes de crear.

## Menu principal

Se agrego el modulo:

```text
Empresas
```

Clave interna:

```text
companies-admin
```

Este modulo abre `CompaniesAdminForm`.

## Verificacion

Como Visual Studio mantiene la API ejecutandose y bloquea DLLs de salida normal, se compilo usando salida alterna:

```text
dotnet build NuanSystem.sln --no-restore -p:BaseOutputPath=artifacts\verify\
0 Advertencia(s)
0 Errores
```

Tambien se probo una API temporal en `http://localhost:5099`:

```text
LoginSuccess = True
CompaniesSuccess = True
CompanyCount = 1
FirstCompany = DEMO
```

## Nota operativa

Para que Visual Studio cargue los cambios en el frontend, detenga y vuelva a iniciar `NuanSystem.WinForms`. Si tambien quiere compilar la API en la salida normal, detenga primero `NuanSystem.Api`, porque la instancia en ejecucion bloquea DLLs.
