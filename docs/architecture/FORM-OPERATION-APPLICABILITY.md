# Aplicabilidad de operaciones por formulario

## Decisión

Las operaciones de seguridad continúan siendo un catálogo global en `SecurityOperations`, pero una operación solo puede mostrarse, asignarse o autorizarse cuando existe una asociación activa en `SecurityFormOperations` para el formulario solicitado.

La asociación es infraestructura de seguridad de Master y aplica a todos los tipos de formulario. Las bases de sucursal no almacenan este catálogo ni resuelven permisos de interfaz.

## Reglas

- `SecurityFormOperations` define aplicabilidad; `SecurityRoleFormOperations` define si un rol tiene concedida una operación aplicable.
- Los procedimientos de consulta, mantenimiento y validación deben unir ambas tablas y fallar de forma cerrada cuando la asociación no existe.
- Los guardados de accesos deben rechazar pares formulario-operación no aplicables.
- Ningún permiso global sustituye la operación específica del formulario en la API; incluso un administrador requiere la concesión aplicable.
- Los CRUD usan una sola operación canónica por acción: `ACTION.CREATE` para crear y `ACTION.UPDATE` para editar.
- `ACTION.NEW` y `ACTION.EDIT` se conservan como datos históricos, pero no se asocian a los CRUD canónicos.
- Al migrar, un permiso legado `NEW` o `EDIT` se copia a su operación canónica solamente si el rol y formulario aún no tienen una decisión explícita en `CREATE` o `UPDATE`.
- Las operaciones especializadas de SAP y SRI solo se asocian a su formulario propietario.
- El Ribbon consume exclusivamente las operaciones aplicables devueltas por la API; una operación denegada no produce botón y un grupo sin botones visibles permanece oculto.

## Flujo de autorización

1. El endpoint identifica `FormKey` y `ActionKey`.
2. Master resuelve el formulario activo y sus operaciones aplicables.
3. La concesión requiere una asociación activa formulario-operación y una asignación activa y permitida para alguno de los roles del usuario.
4. La ausencia de formulario, asociación o concesión devuelve denegación.

Esta separación evita que dos operaciones globales que comparten `ActionKey` autoricen accidentalmente formularios no relacionados.
