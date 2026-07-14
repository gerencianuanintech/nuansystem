# Plan de pruebas integrales: sincronizacion Maestro-Sucursal

## Objetivo

Validar la configuracion administrativa, ejecucion manual/programada, routing, seguridad, polling, errores y experiencia visual del modulo Maestro-Sucursal sin ampliar el alcance funcional de la primera version.

## Ambientes

- API con base Master actualizada hasta `073_sync_master_branch_hardening.sql`.
- Al menos una empresa maestra y dos empresas sucursal con `SyncEnabled = 1`.
- Usuario administrador con permisos `SYNC.CONFIGURATION.*`.
- Usuario restringido sin permisos de ejecucion/cancelacion/reintento.
- WinForms en 1366x768 y 1920x1080, DPI 100% y 125%.

## Matriz de casos

| # | Area | Caso | Precondicion | Pasos | Resultado esperado | Estado |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | Acceso | Ver menu perfiles | Usuario con `SYNC.CONFIGURATION.VIEW` | Abrir Administracion -> Integraciones | Aparece Perfiles de sincronizacion | Pendiente manual |
| 2 | Acceso | Ocultar perfiles sin permiso | Usuario sin permiso | Abrir menu | No aparece o no permite abrir perfiles | Pendiente manual |
| 3 | Acceso | Ver menu ejecuciones | Usuario con `SYNC.CONFIGURATION.VIEWEXECUTIONS` | Abrir Integraciones | Aparece Ejecuciones | Pendiente manual |
| 4 | Acceso | Bloquear ejecuciones sin permiso | Usuario sin permiso | Abrir menu | No aparece o acceso denegado normalizado | Pendiente manual |
| 5 | Perfil | Crear perfil minimo | Master y sucursal activos | Crear perfil Manual con Item | Guarda correctamente | Pendiente manual |
| 6 | Perfil | Codigo requerido | Sin codigo | Guardar | Muestra validacion, no persiste | Pendiente manual |
| 7 | Perfil | Direccion unica | Intentar direccion distinta | Guardar | Rechaza valor distinto a `MasterToBranch` | Pendiente manual |
| 8 | Perfil | Sucursal requerida | Sin sucursales | Validar/guardar | Muestra error funcional | Pendiente manual |
| 9 | Perfil | Entidad requerida | Sin entidades | Validar/guardar | Muestra error funcional | Pendiente manual |
| 10 | Perfil | Matriz entidad-sucursal | Entidad y dos sucursales | Deshabilitar una celda y guardar | Persiste matriz esperada | Pendiente manual |
| 11 | Perfil | Activar/desactivar | Perfil creado | Desactivar y activar | Cambia estado y refresca listado | Pendiente manual |
| 12 | Perfil | Eliminar sin historial | Perfil nuevo sin ejecuciones | Eliminar | Elimina logicamente o deja de listar | Pendiente manual |
| 13 | Perfil | Eliminar con historial | Perfil con ejecucion | Eliminar | Bloquea con mensaje funcional | Pendiente manual |
| 14 | Programacion | Manual no envia hora | Perfil Manual | Guardar | API no recibe `ExecutionTime` ni intervalo | Cubierto por test |
| 15 | Programacion | Intervalo requiere minutos | Tipo Interval | Guardar sin minutos | Rechaza validacion | Pendiente manual |
| 16 | Programacion | Diario requiere hora | Tipo Daily | Guardar sin hora | Rechaza validacion | Pendiente manual |
| 17 | Programacion | Campos visuales por tipo | Cambiar Manual/Interval/Daily | Observar campos | Solo se habilita lo aplicable | Pendiente manual |
| 18 | Ejecucion | Ejecutar manual | Perfil activo valido | Ejecutar | Crea ejecucion `Pending` | Pendiente manual |
| 19 | Ejecucion | Ejecutar entidad filtrada | Perfil con varias entidades | Enviar codigos | Publica solo entidades seleccionadas | Pendiente manual |
| 20 | Ejecucion | MaxRecords invalido | MaxRecords <= 0 | Ejecutar | Muestra validacion | Pendiente manual |
| 21 | Ejecucion | Bloqueo concurrente | Ejecucion activa | Ejecutar otra con bloqueo | Rechaza por concurrencia | Pendiente manual |
| 22 | Ejecucion | Concurrencia SQL | Dos solicitudes simultaneas | Disparar ejecuciones paralelas | Solo una queda `Pending` | Pendiente manual |
| 23 | Estados | Polling Pending | Ejecucion Pending | Abrir listado | Refresca cada 7s | Pendiente manual |
| 24 | Estados | Polling Running | Ejecucion Running | Abrir detalle | Refresca cada 7s | Pendiente manual |
| 25 | Estados | Polling Cancelling | Cancelacion en curso | Mantener formulario abierto | Refresca hasta terminal | Pendiente manual |
| 26 | Estados | Detener polling terminal | Completed/Failed | Observar | No continua polling innecesario | Pendiente manual |
| 27 | Estados | Cerrar durante polling | API lenta | Cerrar formulario | No actualiza controles dispuestos | Cubierto por test de contrato |
| 28 | Cancelar | Cancelar Pending | Ejecucion Pending | Cancelar | Cambia a Cancelling/Cancelled | Pendiente manual |
| 29 | Cancelar | Bloquear cancelacion terminal | Ejecucion Completed | Cancelar | Accion deshabilitada o rechazada | Pendiente manual |
| 30 | Reintentar | Reintentar Failed | Ejecucion Failed | Reintentar | Crea nueva ejecucion | Pendiente manual |
| 31 | Reintentar | Bloquear retry activa | Ejecucion Running | Reintentar | Accion deshabilitada/rechazada | Pendiente manual |
| 32 | Routing | Targets por sucursal | Perfil con dos sucursales | Ejecutar | Crea `SyncOutboxTargets` esperados | Pendiente manual |
| 33 | Routing | Sucursal deshabilitada | Matriz deshabilitada | Ejecutar | No crea target para celda deshabilitada | Pendiente manual |
| 34 | Worker | Reclamo target | Worker activo | Ejecutar perfil | Worker procesa target, no API | Pendiente manual |
| 35 | Worker | Idempotencia inbox | Reprocesar evento | Forzar retry | No duplica aplicacion | Pendiente manual |
| 36 | Seguridad | Sin secretos en UI | Error de conexion | Ver mensajes | No muestra password/connection string | Pendiente manual |
| 37 | API | API no disponible | Detener API | Refrescar WinForms | Mensaje amigable, formulario queda usable | Pendiente manual |
| 38 | Visual | 1366 DPI 100 | Resolucion configurada | Abrir formularios | Sin solapes ni texto cortado | Pendiente manual |
| 39 | Visual | 1920 DPI 125 | Resolucion configurada | Abrir formularios | Layout estable y legible | Pendiente manual |
| 40 | Performance | Listado paginado | >500 ejecuciones | Filtrar/listar | Responde paginado, sin cargar payloads | Pendiente manual |
| 41 | Designer | Abrir `SyncProfileListForm` | Visual Studio, API detenida | Abrir `.cs` en Designer | Carga sin servicios, muestra filtros, botones, grilla y columnas | Pendiente manual |
| 42 | Designer | Abrir `SyncProfileEditForm` | Visual Studio, API detenida | Abrir `.cs` en Designer | Carga tabs, editores, grillas y footer sin API | Pendiente manual |
| 43 | Designer | Abrir `SyncExecutionListForm` | Visual Studio, worker no disponible | Abrir `.cs` en Designer | Carga filtros, botones, grilla y timer deshabilitado | Pendiente manual |
| 44 | Designer | Abrir `SyncExecutionDetailForm` | Visual Studio, API/base no disponibles | Abrir `.cs` en Designer | Carga resumen, acciones y grilla sin polling activo | Pendiente manual |
| 45 | Designer | Abrir `ExecuteSyncProfileDialog` | Visual Studio | Abrir `.cs` en Designer | Carga labels, editores, botones y propiedades de dialogo | Pendiente manual |
| 46 | Designer | Editar y guardar formulario | Cualquier formulario Designer | Mover una propiedad visual menor y guardar | Guarda sin romper compilacion | Pendiente manual |
| 47 | Designer | API detenida | API apagada | Abrir los 5 formularios en Designer | No hay `HttpRequestException` ni errores de permisos | Pendiente manual |
| 48 | Designer | Sin sesion autenticada | Sin `ApiSession` runtime | Abrir los 5 formularios en Designer | No hay `NullReferenceException` ni `InvalidOperationException` | Pendiente manual |
| 49 | Designer | DPI 100/125 | Visual Studio con escalado | Abrir formularios | Controles visibles y editables | Pendiente manual |
| 50 | Designer | Recompilar tras guardar | Formularios abiertos y guardados | Ejecutar build WinForms | Compilacion correcta | Pendiente manual |

## Evidencia automatizada

La Etapa 7 agrega pruebas de contrato para:

- estados backend reales en formularios de ejecucion;
- guardas `IsDisposed || Disposing` durante polling;
- normalizacion de programacion Manual;
- seed de seguridad sin columnas inexistentes en `RolePermissions`;
- script `073` con reserva atomica de ejecuciones.
- compatibilidad estructural de formularios con Visual Studio Designer;
- timers de polling deshabilitados en Designer.

## Criterios de salida

- Builds de Application, Persistence, API, WinForms y tests de Application sin errores de compilacion.
- `dotnet test` de Application con referencias precompiladas en verde.
- Pruebas manuales criticas 1, 5, 14, 18, 21, 23, 28, 30, 32, 36, 38 y 39 ejecutadas antes de produccion.
- Sin nuevos endpoints, entidades, workers, schedulers externos ni librerias fuera del alcance.
