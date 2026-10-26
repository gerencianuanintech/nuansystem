# Validacion integrada del backend y gates de liberacion

> Actualizacion 2026-10-24: los tres fallos WinForms aqui registrados se corrigieron. La nueva validacion pasa 1263/1263 pruebas y 296 dirigidas, con compilacion limpia. Informe vigente de la correccion: [BACKEND-SOLID-WINFORMS-VALIDATION](BACKEND-SOLID-WINFORMS-VALIDATION.md). Los resultados originales inferiores se conservan como historia; los gates SQL/JWT/transporte y despliegue siguen pendientes.

Fecha: 2026-10-24. Alcance: fase 9 del plan de consolidacion. Resultado: validacion local completada; liberacion de produccion pendiente. Sin ejecucion SAP, acceso/modificacion de bases ni despliegue.

## Resultado ejecutivo del ensayo original (historico)

| Comprobacion | Resultado |
|---|---|
| Solucion Release, SDK 10.0.400 | 0 errores, 0 advertencias; sin restore/install. |
| Suites dirigidas de fases 1–8 + ensayo integrado | 283 aprobadas. |
| Nuevos ensayos de pipeline HTTP y cliente reales en memoria | 10 aprobados. |
| Regresion general autorizada | 1263 ejecutadas: 1260 aprobadas, 3 fallos visuales previos. |
| Fallos comparados con fase 8 | Mismos tres nombres; ningun fallo nuevo. |
| Revision independiente | Sin hallazgos importantes en servidor/fixture/pruebas; limites registrados abajo. |
| Preservacion de archivos previos | 175 huellas iniciales: 172 identicas y 3 cambios documentales acotados; ninguno falta. |
| Produccion | No desplegada ni certificada. |

La suite global permanece fallida. No se ocultan, corrigen ni deshabilitan estos casos fuera de alcance:

- `SapSyncWinFormsContractTests.SapForms_UseCorporatePagingOperationalAndLookupContracts`.
- `SapSyncWinFormsContractTests.Designers_AreExplicitAndKeepSapCaptions`.
- `SapSyncWinFormsContractTests.SapProfileEditor_UsesCorporateActionButtonSizeAndFooterSpacing`.

## Matriz por fase

Los grupos se obtienen del TRX final por clases/comportamientos identificados en `final/summary.json`. Son grupos solapados; no se suman para calcular el total de la suite.

| Fase | Casos aprobados del grupo | Evidencia |
|---|---:|---|
| 1 Arquitectura y composicion | 58 | Dependencias, constructores, scopes y barreras de IO. |
| 2 Errores/cancelacion de proveedores | 25 | Importador/handler con fallos y cancelacion. |
| 3 Result/HTTP y WinForms | 33 | Categorias, respuestas ejecutadas y cliente actual. |
| 4 Contratos/capacidades SAP | 54 | Registro, perfiles, scheduler, lease y reintentos. |
| 5 Contratos de socios | 19 | CRUD/publicacion e importacion segregada. |
| 6 Calculo de compras | 79 | Caracterizacion, reglas puras, workflow, numeracion y contratos. |
| 7 Piloto transaccional | 26 | Runner/contexto/publicacion con dobles; no prueba SQL real. |
| 8 Empresa/configuracion | 24 | Guardas, JSON, builder, autorizacion y configuracion por empresa. |
| 9 Ensayo integrado HTTP | 10 | Pipeline de endpoints/cliente y boundaries simulados. |

## Ensayo integrado y alcance de las afirmaciones

`InProcessApiServer` implementa IServer y conecta HttpMessageHandler con IHttpApplication sin abrir sockets. El Host usa CreateEmptyBuilder con fuentes de configuracion vacias; no usa Program productivo, initializers, workers ni factories de bases reales. No se agregan paquetes ni se usa un TestHost instalado. Los endpoints PurchaseOrder/SAP, GlobalExceptionMiddleware, CompanyContextMiddleware, filtros de operaciones, autorizacion, MediatR, validadores, calculador, importador, NuanApiClient y PurchaseOrderClient son reales. Resolucion de empresa, identidad autenticada, repositorios, auditoria, lector SAP y numeracion son dobles en memoria.

Los diez casos verifican:

1. 404 con error estable leido por el cliente WinForms tipado y contextos de dos empresas separados.
2. Denegacion 403 por empresa antes de resolver el repositorio.
3. Denegacion 403 por operacion de formulario antes de consultar datos.
4. Solicitud sin identidad rechazada con 401.
5. Excepcion del repositorio devuelve 500 sin diagnostico privado.
6. Importacion de proveedores cruza endpoint/MediatR/servicio y toma auditoria de claims, ignorando valores del cuerpo.
7. Falta del permiso SAP devuelve 403 sin llamar al lector simulado.
8. Cancelacion durante lectura llega al cliente y no persiste proveedor; se verifica que el lector recibio el token cancelado.
9. Alta de compra recalcula 186.3, usa serie/numero reservados y usuario de claims.
10. Compra invalida devuelve 400 con errores antes de reservar numero o escribir.

Limites: la identidad es controlada, no se valida firma/expiracion/security stamp de JWT. El transporte fuerza cancelacion despues del dispatch, por lo que ese caso no demuestra la clasificacion/logging del servidor ante desconexion real. Los features simulados no modelan todo el ciclo de respuesta de Kestrel/IIS. La prueba tenant verifica contextos, no filas SQL. Se prueba el cliente actual del repositorio, no cada binario WinForms instalado. No se acreditan TLS, sockets, SQL, SAP, SRI, rendimiento, failover ni disponibilidad.

La primera pasada detecto carencias del fixture: faltaba el repositorio simulado de watermark, la deteccion de cuerpo HTTP y el codigo real de tipo documental. Se corrigieron exclusivamente los doubles/features; no se altero produccion ni se debilitaron aserciones.

## Comandos y seguridad de ejecucion

```powershell
$env:NUANSYSTEM_RUN_SQL_INTEGRATION_TESTS='0'
$env:NUAN_RUN_SAP_PO_PILOT='0'
dotnet build NuanSystem.sln -c Release --no-restore --disable-build-servers -m:1 --verbosity quiet
dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj -c Release --no-build --no-restore --filter 'FullyQualifiedName~Architecture|FullyQualifiedName~BackendIntegratedHttpTests|FullyQualifiedName~SapSupplierImport|FullyQualifiedName~SapSyncProcessorRegistryTests|FullyQualifiedName~SapSyncSchedulerTests|FullyQualifiedName~SapSyncProfileApplicationTests|FullyQualifiedName~Features.Purchasing.PurchaseOrders|FullyQualifiedName~Features.BusinessPartners|FullyQualifiedName~ApiClientHttpErrorCompatibilityTests' --logger 'trx;LogFileName=phase9-directed.trx' --results-directory artifacts/backend-solid-phase9/directed --verbosity minimal
dotnet test tests/NuanSystem.Application.Tests/NuanSystem.Application.Tests.csproj -c Release --no-build --no-restore --filter 'Category!=SqlServerIntegration&FullyQualifiedName!~SapPurchaseOrderPilotIntegrationTests' --logger 'trx;LogFileName=phase9-final.trx' --results-directory artifacts/backend-solid-phase9/final --verbosity minimal
```

Los gates solo afectan el proceso hijo. SQL externo y piloto SAP se excluyen expresamente. Build y test se ejecutan localmente fuera del sandbox para consulta de SDK Windows/comunicacion testhost, con las mismas restricciones de IO. No hay servicios de red, autenticacion externa, instalaciones, restauracion de paquetes, secretos reales ni cambios de configuracion global.

## Preservacion, catalogos y trazabilidad

Rama/HEAD conservados: `codex/auxiliary-master-generator`, `e97124a1e2863f066c1fa55ea992c78179f01623`. El HEAD por si solo no identifica esta entrega: el checkout contiene cambios anteriores sin commit. No se hizo stash/reset/commit ni se cambio de rama.

De las 175 huellas iniciales en src/tests/docs/database y los dos catalogos internos, 172 no cambiaron. Cambios previos editados solo en plan documental, FRAMEWORK-CATALOG y KNOWLEDGE-GRAPH; las copias anteriores estan en `artifacts/backend-solid-phase9/before/`. Se actualizaron de forma puntual el mapeo HTTP obsoleto, el contrato SAP antiguo y el estado del piloto; no se cambiaron reglas ni skills. ARCHITECTURE y el estado consolidado enlazan la evidencia actual.

No se modifico ningun archivo previo de src, tests o database en esta fase. Se agregaron tres archivos de infraestructura/ensayos de pruebas. Los cambios SQL/frontend previos del usuario se preservan: esta validacion no los despliega ni certifica su schema o compatibilidad global. Los tres fallos visuales son parte de ese baseline conservado.

Artefactos locales ignorados por Git:

- `artifacts/backend-solid-phase9/baseline.json` y `before/`.
- `build-final.log`, `directed/phase9-directed.trx`, `http-final/phase9-http-final.trx`.
- `final/phase9-final.trx` y `final/summary.json` (incluye patrones exactos por grupo).
- `final/source-manifest.json`: huellas SHA-256 de codigo/proyectos backend, servicios/ViewModels frontend y pruebas.
- `final/binary-manifest.json`: rutas, tamanos y SHA-256 de siete assemblies backend compilados.

Los manifiestos identifican lo validado localmente; no son un paquete de despliegue ni un backup completo. No incluyen configuracion/secrets productivos. Conservar artefactos locales en un repositorio de releases autorizado antes de usarlos para una liberacion.

## Liberacion gradual y recuperacion (procedimiento, no ejecutado)

1. Separar el alcance de consolidacion del trabajo SQL/UI previo del usuario; revisar el diff del incremento y fijar fuente/binarios inmutables sin usar un reset del checkout como recuperacion.
2. Gate de contratos visuales cumplido por la correccion del 2026-10-24, sin exenciones ni tests deshabilitados. Conservar su evidencia y comprobar Designer/DPI/uso real del cliente en el entorno de destino cuando corresponda; este gate no reemplaza los siguientes.
3. Validar el piloto transaccional en SQL aislado autorizado: rollback de socio/auditoria/Outbox, empresa, concurrencia e identidad/idempotencia. Mantener ITransactionRunner/sobrecargas y el puente hasta entonces. No migrar mas verticales automaticamente.
4. Ensayar JWT real, permisos, TLS/transporte y el cliente instalado en el entorno de destino. Para SQL previo que forme parte del incremento, revisar schema requerido e idempotencia solo con autorizacion independiente; las fases de consolidacion no agregaron migraciones nuevas.
5. Publicar por incrementos: errores/contratos compatibles; capacidades SAP/importacion segregada; calculo/configuracion; piloto transaccional solo tras su gate. Coordinar assemblies dependientes y workers sin habilitar agendas, outbox, SAP, SRI o replicacion por el solo cambio de binarios. Validar compatibilidad del cliente de cada incremento.
6. Guardar el paquete completo anterior y su configuracion protegida de forma separada; comprobar hashes y procedimiento de retorno. Si falla el incremento, detener solo sus componentes autorizados y restaurar el paquete compatible anterior. No borrar colas, revertir documentos, renumerar ni ejecutar down-migrations como rollback de codigo. Si cambia schema en otra entrega, definir compatibilidad antes de publicar.
7. Tras cada incremento autorizado, comprobar salud/errores/tenant y registrar criterios de avance/parada. Medidas reales de disponibilidad/RPO/RTO quedan pendientes del entorno productivo; no se inventan SLA a partir de estas pruebas.

## Checklist final

| Gate | Estado |
|---|---|
| R1/R2 Alcance/descubrimiento | Validated: plan revisado, baseline preservado y ensayo acotado sin produccion. |
| R3 Arquitectura | Validated localmente: guardas, composicion, catalogos/grafo y contratos actuales reconciliados. |
| R4/R9 Atomicidad y SQL | Not validated en proveedor real; piloto compatible y contrato legacy conservados. Sin migracion nueva ni SQL ejecutado. |
| R5 Seguridad/tenant | Validated con identidad/datos simulados; JWT real, aislamiento de filas y destino productivo: Not validated. |
| R6/R7 WinForms | Cliente actual validado; Designer real/fallos visuales: pendientes. No cambios UI. |
| R8 API | Validated en pipeline real en memoria para los escenarios enumerados; no certifica hosting/transporte real. |
| R10 Integracion externa | Not validated: SAP/SRI/replicacion/workers no ejecutados. Exportacion SAP sigue incompleta. |
| R11 Build/tests | Build limpio, 283 dirigidas y 10 nuevas pasan; suite global mantiene 3 fallos conocidos. |
| R12 Entrega/recuperacion | Evidencia y procedimiento preparados; despliegue/rollback real no ejecutados. |

Estado consolidado y grafo: [BACKEND-SOLID-STATUS](../architecture/BACKEND-SOLID-STATUS.md). La fase 9 cierra la validacion integrada local con limites explicitos; no presenta la fase 7 global ni la liberacion de produccion como terminadas.
