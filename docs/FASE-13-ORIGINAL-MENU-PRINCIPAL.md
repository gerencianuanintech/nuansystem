# Fase 13 original - Menu principal

## Objetivo

Esta fase completa el menu principal del frontend WinForms segun el plan original:

- Menu dinamico por permisos.
- Navegacion mas robusta tipo Accordion.
- Control de sesion.
- Logout formal.
- Visualizacion de empresa activa y usuario actual.

## Menu dinamico por permisos

El menu ya no muestra todos los modulos de forma fija.

Ahora `ShellViewModel` construye los modulos disponibles usando los permisos recibidos en el login JWT:

```text
src/Frontend/NuanSystem.WinForms.ViewModels/Shell/ShellViewModel.cs
```

Ejemplos:

- `COMPANIES.MANAGE` habilita Empresas.
- `SECURITY.USERS.MANAGE` habilita Usuarios.
- `SECURITY.ROLES.MANAGE` habilita Roles.
- `SECURITY.AUDIT.READ` habilita Auditoria.
- `CATALOG.CUSTOMERS.READ` o `CATALOG.CUSTOMERS.MANAGE` habilita Clientes.
- `CATALOG.ITEMS.READ` o `CATALOG.ITEMS.MANAGE` habilita Articulos.
- `SALES.DOCUMENTS.READ` o `SALES.DOCUMENTS.MANAGE` habilita Documentos.
- `SAP.SYNC.READ` o `SAP.SYNC.MANAGE` habilita Integracion SAP.
- `SETTINGS.PARAMETERS.MANAGE` habilita Configuracion.

## Navegacion tipo Accordion

Se reemplazo el listado simple por una navegacion agrupada con `TreeView`, simulando un comportamiento tipo Accordion sin agregar dependencias nuevas.

Grupos actuales:

- Administracion
- Seguridad
- Catalogos
- Ventas
- SAP
- Sistema

Archivo:

```text
src/Frontend/NuanSystem.WinForms.Forms/Shell/MainForm.cs
```

## Control de sesion

El encabezado principal muestra:

- Empresa activa.
- Usuario actual.
- Nombre de usuario.
- Roles del usuario.

## Logout formal

Se agrego el boton `Cerrar sesion`.

Al confirmar:

- Se limpia `ApiSession`.
- Se cierra el menu principal.
- La aplicacion vuelve al formulario de login.
- Se puede entrar con otro usuario sin reiniciar la aplicacion.

Archivo ajustado:

```text
src/Frontend/NuanSystem.WinForms/Program.cs
```

## Verificacion

Se compilo la solucion:

```text
dotnet build NuanSystem.sln --no-restore -p:BaseOutputPath=artifacts\verify\
0 Advertencia(s)
0 Errores
```

## Nota

DevExpress aun no esta agregado como paquete en la solucion. Por eso esta fase se resolvio con controles WinForms nativos, manteniendo el comportamiento esperado sin introducir una dependencia visual nueva todavia.
