# Consolidación backend — semántica de errores y HTTP

Fecha: 2026-10-23. Alcance: fase 3 del plan `docs/superpowers/plans/2026-10-22-backend-solid-consolidation.md`.

## Decisiones y compatibilidad

Mantener `Result<T>`, `ApiResponse<T>` y `ApiError`; no introducir otro envelope ni ProblemDetails. Agregar `FailureKind` en Application sin referencias HTTP, mediante propiedad nullable no posicional. Se conservan constructor y deconstrucción de cuatro argumentos. La factoría `Failure(message, errors)` existente equivale a Validation y sigue produciendo 400; resultados fallidos construidos directamente sin categoría también mantienen 400.

| Categoría | HTTP | Semántica |
|---|---|---|
| Validation | 400 | Datos o precondición inválida |
| NotFound | 404 | Recurso consultado antes de operar no existe |
| Conflict | 409 | Estado actual o escritura condicional impide la operación |
| Forbidden | 403 | Permiso de operación verificado y denegado |
| Unavailable | 503 | Dependencia requerida actualmente no disponible |
| Unexpected | 500 | Resultado técnico inesperado, mensaje y errores públicos reemplazados por valores seguros |

Un enum no reconocido se trata como error inesperado seguro, nunca como éxito. No se infiere categoría a partir de mensajes. Los resultados exitosos conservan HTTP 200 y su envelope; la importación manual por lote conserva su resumen parcial de fase 2. La autenticación y sus respuestas 401 siguen bajo middleware/políticas existentes.

El mensaje genérico de error inesperado indica consultar el estado de la operación o al administrador; no recomienda repetir una escritura que pudo haberse confirmado antes del fallo de lectura.

Los errores de negocio y Unavailable deben originar mensajes/códigos seguros en su handler. El mapper no devuelve mensajes ni detalles originales para Unexpected, aunque contengan secretos. Se conserva el envelope de cuatro campos `success`, `message`, `data`, `errors`; la categoría interna no se añade al envelope HTTP.

## Primeros consumidores migrados

- Compras: consulta por Id y handlers de órdenes de compra. Ausencia inicial → NotFound; transición inválida y fallo de escritura condicional → Conflict; denegación de permiso de serie → Forbidden, propagando categoría y errores al handler exterior. Fallo al recuperar después de una escritura → Unexpected, porque no demuestra que la escritura haya fallado ni autoriza reintento automático. No se cambian cálculos, permisos, repositorios ni transacciones. Precondiciones ambiguas/numéricas conservan Validation durante transición.
- SAP: query de prueba de conexión Service Layer. Un resultado `IsSuccessful=false` del tester es Unavailable con mensaje y código fijos, sin copiar el mensaje remoto. Ausencia de empresa sigue siendo Validation. Excepciones y cancelación siguen su ruta existente; no se envuelven indiscriminadamente en 503. No se realiza ninguna prueba contra SAP real.
- El resto de los módulos conserva su mapeo legacy 400 hasta migración explícita por caso de uso. No se declara la migración global terminada.

## WinForms

`NuanApiClient.ReadResponseAsync` ya deserializa el envelope para cualquier estado y crea `ApiClientException` con StatusCode/Errors. El formatter presenta mensajes y campos sin depender de que todo sea 400. No se modifica frontend; se verifica con IResult ejecutado sobre HttpContext en memoria y HttpMessageHandler simulado, también con clientes tipados de compras/SAP. La prueba de conexión SAP sigue propagando la excepción y restableciendo IsBusy mediante el finally del ViewModel; el formulario ya la presenta mediante UiExceptionHandler.

No cambiar tokens, selección de empresa, contratos JSON, endpoints, autorización ni configuración local. No iniciar API/workers ni usar bases/SAP. Se reutilizan los gates de arquitectura, build y baseline de fases 1–2 y se comparan huellas de los archivos existentes al terminar.

## Riesgos y gates

404/409/503 cambian deliberadamente el código HTTP de los casos migrados, no el formato de error. Comprobar mensajes/códigos/campos y que WinForms conserva sesión y contexto. Cubrir fallo técnico mal clasificado, errores vacíos, valores genéricos, validación legacy, cancelación y resumen parcial. Los resultados post-escritura inesperados no garantizan rollback ni autorizan duplicar la operación.
