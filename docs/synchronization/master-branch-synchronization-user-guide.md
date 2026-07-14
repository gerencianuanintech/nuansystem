# Sincronizacion Maestro - Sucursales

Guia de uso para administradores del modulo WinForms de configuracion y monitoreo.

## Acceso

Abrir el menu:

`Administracion -> Integraciones -> Perfiles de sincronizacion`

Para monitoreo global:

`Administracion -> Integraciones -> Ejecuciones`

El acceso depende de los permisos `SYNC.CONFIGURATION.*`. Si una accion no aparece habilitada, revisar permisos del usuario/rol.

## Perfiles de sincronizacion

La pantalla de perfiles permite buscar por texto, estado y modo de ejecucion. Desde el listado se puede:

- Crear un perfil.
- Editar el perfil seleccionado.
- Eliminar un perfil sin historial operativo bloqueante.
- Activar o desactivar.
- Validar configuracion.
- Ejecutar manualmente.
- Abrir ejecuciones del perfil.

## Edicion de perfil

El formulario de edicion contiene estas pestanas:

- General: codigo, nombre, empresa maestra, direccion, modo, estrategia, lote, reintentos, espera y timeout.
- Sucursales: sucursales destino del perfil.
- Entidades: entidades maestras a sincronizar.
- Matriz entidad-sucursal: habilita o deshabilita una entidad por sucursal.
- Programacion: modo manual, intervalo o diario, zona horaria y bloqueo de concurrencia.

La primera version solo soporta `MasterToBranch` y `MasterWins`.

## Validacion

Usar `Validar` antes de guardar o ejecutar. La validacion muestra errores y advertencias devueltos por la API. No corrige automaticamente la configuracion.

## Ejecucion manual

Desde el listado de perfiles, seleccionar un perfil y presionar `Ejecutar`. El dialogo permite:

- limitar entidades por codigos separados por coma;
- indicar una clave inicial opcional;
- limitar maximo de registros.

La ejecucion crea una solicitud administrativa. La publicacion real ocurre en backend y usa Outbox/Targets existentes.

## Ejecuciones

La pantalla de ejecuciones permite filtrar por perfil, estado, tipo y fechas. Las ejecuciones activas se refrescan automaticamente cada 7 segundos. El refresco se detiene al cerrar el formulario.

Acciones disponibles:

- Detalle: muestra resumen y detalle por entidad.
- Cancelar: solicita cancelacion administrativa.
- Reintentar: crea una nueva ejecucion basada en la ejecucion seleccionada.

### Estados de ejecucion

- `Pending`: la ejecucion fue solicitada y espera procesamiento.
- `Running`: el backend esta publicando eventos de sincronizacion.
- `Cancelling`: se solicito cancelacion y el backend esta cerrando la ejecucion.
- `Cancelled`: la ejecucion fue cancelada.
- `Completed`: la ejecucion termino sin errores.
- `CompletedWithErrors`: la ejecucion termino, pero una o mas entidades tuvieron errores.
- `Failed`: la ejecucion fallo antes de completarse.

El refresco automatico se mantiene solo en `Pending`, `Running` y `Cancelling`. En estados terminales, usar `Refrescar` si se necesita consultar nuevamente.

### Cancelar y reintentar

Cancelar no borra eventos ya publicados ni revierte datos aplicados en sucursales. Solo solicita detener la ejecucion administrativa.

Reintentar crea una nueva ejecucion basada en la anterior. Usar esta accion cuando el problema fue temporal, por ejemplo API no disponible, conexion intermitente o sucursal apagada.

No reintentar de inmediato si el error indica configuracion invalida, permisos insuficientes o entidad deshabilitada; primero corregir la causa.

### Interpretacion de resultados

- Revisar `TotalEntities`, `ProcessedEntities` y `FailedEntities` para saber si el fallo fue total o parcial.
- Abrir el detalle de ejecucion para identificar la entidad y sucursal afectada.
- Si el estado es `CompletedWithErrors`, validar si los errores son recuperables y luego usar `Reintentar`.
- Si una ejecucion queda en `Cancelling` demasiado tiempo, revisar logs de API/worker antes de lanzar otra ejecucion.

## Errores comunes

| Mensaje o situacion | Causa probable | Accion recomendada |
| --- | --- | --- |
| No aparece el menu | Falta permiso de menu/formulario | Revisar rol y permisos `SYNC.CONFIGURATION.*`. |
| Accion deshabilitada | Falta permiso especifico o estado no permite accion | Confirmar permisos y estado de ejecucion. |
| Configuracion invalida | Faltan sucursales, entidades o programacion valida | Usar `Validar`, corregir y guardar. |
| Ya existe una ejecucion activa | `PreventConcurrentExecutions` bloquea ejecuciones paralelas | Esperar a que termine o cancelar la activa. |
| API no disponible | Servicio API detenido o URL incorrecta | Revisar conexion, iniciar API y refrescar. |
| CompletedWithErrors | Una parte del perfil fallo | Abrir detalle y reintentar solo despues de corregir causa. |
| Error de permisos | Usuario sin permiso backend | Solicitar ajuste de rol al administrador. |

## Seguridad de datos

El frontend no muestra payloads completos, cadenas de conexion, contrasenas ni secretos. Los errores se muestran como mensajes normalizados provistos por la API.

En el monitor tecnico, el payload completo del evento se retiene por seguridad. Para diagnosticar, usar entidad, GlobalId, codigo, targets, auditoria, ultimo error y logs backend.

## Buenas practicas

- Validar todo perfil antes de ejecutarlo por primera vez.
- Probar primero con una sucursal y una entidad antes de activar perfiles amplios.
- Usar `MaxRecords` en ejecuciones manuales de prueba.
- Mantener `PreventConcurrentExecutions` activo salvo que exista una razon operativa clara.
- No ejecutar perfiles Full en horario de alta operacion sin revisar volumen.
- Revisar el detalle antes de reintentar una ejecucion fallida.
- No compartir capturas que incluyan nombres internos de servidores, errores SQL o informacion sensible.

## Alcance excluido

Esta version no implementa sincronizacion bidireccional, resolucion avanzada de conflictos, scripts personalizados, SQL ingresado por usuario, documentos transaccionales, adjuntos fisicos ni diseñador avanzado de mapeo de campos.
