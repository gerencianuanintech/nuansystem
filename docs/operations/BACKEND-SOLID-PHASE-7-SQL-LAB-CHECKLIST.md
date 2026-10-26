# Plantilla y checklist del laboratorio SQL aislado

**Proyecto:** NuanSystem · **Área:** Backend / validación fase 7  
**Versión:** 1.0 · **Fecha:** 2026-10-25 · **Revisión:** pendiente del responsable

Objetivo: registrar configuración, permisos, autorizaciones y evidencia para dos corridas aisladas. Audiencia: DBA, operador, responsable de autorización y revisor independiente. Este documento y sus plantillas no conceden permisos ni autorización de ejecución. Preparar archivos no implica que el servidor haya sido inspeccionado.

## Archivos y forma de completarlos

| Archivo | Uso |
| --- | --- |
| [laboratory-preparation.template.json](../../database/lab/phase7/laboratory-preparation.template.json) | Ficha de planificación de las dos corridas, responsables, plataforma, candidato y autorizaciones. No es configuración de runtime ni se entrega directamente a los ejecutores. |
| [provisioning-spec.template.json](../../database/lab/phase7/provisioning-spec.template.json) | Los 14 campos exactos del contrato del generador, inicialmente null. Completar una copia por RunId después de confirmar los datos. Es inválida deliberadamente mientras falten datos. |
| [provisioning-spec.example.json](../../database/lab/phase7/provisioning-spec.example.json) | Ejemplo de formato ya existente, ficticio y caducado. No copiar sus valores como datos del laboratorio ni como aprobación. |

1. Copiar las plantillas a una carpeta de trabajo nueva y controlada; conservar los originales.
2. `null` significa **Pendiente de confirmar**. Las confirmaciones false no son resultados negativos observados: todavía no se han confirmado. No rellenar hashes, DB_ID ni resultados con valores ficticios o ceros para pasar guardas.
3. Completar ahora los datos confirmados por los responsables. Guardar solo nombres, rutas, referencias y huellas; nunca contraseñas, cadenas de conexión, JWT, claves privadas o contenidos de SecureString. Tampoco ponerlos en notas, referencias, hashes de credenciales o capturas de pantalla.
4. Las claves y variables listadas en SecureDelivery son únicamente nombres. Registrar el mecanismo de entrega y responsable; los valores se suministran por el canal seguro durante la etapa autorizada.
5. Los documentos ejecutables del anfitrión, plan, aprobaciones y manifiesto se construyen conforme a sus contratos existentes. Esta ficha no los reemplaza; marcar casillas o cambiar un booleano no verifica una firma humana ni habilita una ejecución.

## Responsables y registro de comprobaciones

| Rol | Nombre / referencia | Responsabilidad |
| --- | --- | --- |
| DBA del laboratorio | Pendiente de confirmar | Destino dedicado, configuración, sesiones y permisos efectivos |
| Operador de validación | Pendiente de confirmar | Candidato, anfitrión, ejecución controlada y custodia de evidencias |
| Responsable de autorización | Pendiente de confirmar | Decisiones por etapa, alcance, ventanas y hashes aprobados |
| Revisor independiente | Pendiente de confirmar | Monitor/atestaciones y revisión de autenticidad y cierre |

Para cada casilla completada registrar: identificador, responsable, fecha UTC, referencia de evidencia y observaciones sin secretos. Una declaración del DBA puede servir para preparar la ficha; no sustituye el preflight real. Dejar las comprobaciones online pendientes hasta que se autoricen y realicen.

| Control | Responsable | Fecha UTC | Evidencia / referencia | Estado |
| --- | --- | --- | --- | --- |
| Pendiente de indicar | Pendiente | Pendiente | Pendiente | Pendiente |

## A. Datos confirmables antes de cualquier conexión

- [ ] A01. DBA, operador, autorizador y revisor identificados.
- [ ] A02. Instancia dedicada al laboratorio; sin producción, réplicas productivas ni datos reales. Registrar servidor, DataSource, puerto/instancia y versión completa conocida por el DBA.
- [ ] A03. Compatibilidad con el diseño: SQL Server principal 15/16/17; registrar edición, autenticación mixta, collation, compatibilidad y RCSI previstos. No instalar ni cambiar configuración global mediante esta checklist.
- [ ] A04. DBA confirma que model no contiene objetos de usuario ni marcadores del laboratorio. Marcar pendiente de verificación online si solo existe una declaración.
- [ ] A05. Certificado TLS identificado y cadena de confianza prevista en el ejecutor; no usar TrustServerCertificate como sustituto de la validación.
- [ ] A06. Acceso de red aprobado al destino y puerto concretos. Ninguna regla global de firewall se modifica con estos archivos.
- [ ] A07. Dos CompanyId sintéticos positivos y diferentes; IdentificationTypeId sintético reservado y común a A/B. Confirmar ausencia de datos personales o copias productivas.
- [ ] A08. Dos RunId nuevos y distintos. Derivar dos bases por corrida, con patrón NuanSystem_Phase7_{RunId:N}_A/B; al finalizar serán cuatro destinos diferentes.
- [ ] A09. Ventanas de provisión, suite, monitor y orquestador en UTC, de máximo 24 horas; límites por comando y totales acordados. La segunda corrida comienza después de terminar la primera.

No asignar ahora DB_ID ni huellas efectivas de esquema/procedimientos. Se observan después de aprovisionar cada destino.

| Límite | Contrato actual |
| --- | --- |
| Ventana autorizada por componente | UTC, máximo 24 horas |
| Comando de provisión, suite o monitor | 1–120 segundos |
| Ejecución de provisión, suite o monitor | 1–3600 segundos; comando no mayor que total |
| Etapa / total del orquestador | 1–3600 segundos; etapa no mayor que total |
| Limpieza | Plazo independiente de 5 segundos según los componentes; un cierre no confirmado mantiene el fallo |

La admisión de la suite requiere que su presupuesto total aún quepa en la ventana aprobada. No confundir el timeout SQL por comando con el de una etapa completa que incluye aprobación o ejecución.

## B. Equipo ejecutor, archivos y candidato

- [ ] B01. Equipo Windows identificado, .NET 10 y PowerShell compatible disponibles. Registrar versiones reales; la revisión nativa previa usó PowerShell 7.6.5 sobre runtime 10.0.11, sin imponer esas rutas de máquina como fallback.
- [ ] B02. Permiso de lectura/ejecución de runtimes y scripts aprobados; escritura de evidencias; inicio del hijo propio y enlace local CurrentUserOnly entre runner y testhost. Los procesos deben permanecer bajo la identidad/alcance autorizado.
- [ ] B03. Carpetas absolutas y nuevas, sin enlaces ni solapamiento con entradas/candidato. Registrar ubicación, capacidad y custodia; no borrar una corrida anterior para reutilizar su ruta.
- [ ] B04. Completar los siete HostFiles: ProvisioningEntry, ProvisioningModule, AutomationAssembly, Dotnet, TestAssembly, TestDependencies y TestRuntimeConfiguration. Obtener sus SHA-256 reales y revisión externa de la huella de opciones.
- [ ] B05. Congelar el mismo candidato de fuentes y binarios para ambas corridas. Conservar inventarios, hashes y baseline offline terminado. El assembly debe pertenecer al inventario de binarios.
- [ ] B06. Entrega segura de cinco secretos distintos acordada, de 32–128 caracteres y conformes a la política SQL. Registrar únicamente responsables y mecanismo; no guardar sus valores en la ficha.
- [ ] B07. Confirmar disponibilidad posterior de sesión administrativa y sesión independiente del monitor, ya abiertas por el responsable cuando se autorice. Sin transacciones ambientales; pooling/enlist desactivados, TLS validado y FireInfoMessageEventOnUserErrors=false según los ejecutores.
- [ ] B08. Cuatro conexiones App/Observer entregadas exclusivamente al entorno del hijo en su etapa autorizada; ningún secreto en argumentos o archivos. El anfitrión no abre automáticamente las sesiones administrativa o del monitor.
- [ ] B09. Mantener ambos gates SQL y el gate piloto SAP en 0 mientras se prepara. SAP, SyncWorker, MasterBranchSyncWorker, SriWorker y Relay desactivados durante el ensayo; fijar inspector y ventana de atestación.

## C. Matriz de permisos a preparar y verificar

Esta tabla refleja el contrato del laboratorio actual. No es un script GRANT ni una autorización para aplicarlo. Las concesiones efectivas, incluidos privilegios heredados, se revisarán posteriormente.

| Identidad | Alcance | Permisos previstos | Evidencia requerida |
| --- | --- | --- | --- |
| Administrador nominal | Solo instancia aislada, durante provisión | sysadmin para el aprovisionador actual; login original y actual deben ser los aprobados | Identidad y preflight administrativo |
| App A | Solo base A de su RunId | CONNECT SQL en servidor; CONNECT, SELECT, INSERT, EXECUTE, VIEW DEFINITION en A | Inventario con la propia identidad y preflight de aplicación |
| Observer A | Solo base A de su RunId | CONNECT SQL; CONNECT, SELECT, VIEW DEFINITION en A | Inventario y preflight con observer A |
| App B | Solo base B de su RunId | CONNECT SQL; CONNECT, SELECT, INSERT, EXECUTE, VIEW DEFINITION en B | Inventario y preflight con app B |
| Observer B | Solo base B de su RunId | CONNECT SQL; CONNECT, SELECT, VIEW DEFINITION en B | Inventario y preflight con observer B |
| Monitor | Bases A/B de su RunId y usuario en master | CONNECT SQL; CONNECT, SELECT, VIEW DEFINITION en A/B; CONNECT en master; visibilidad DMV según versión | Identidad independiente y lectura efectiva del monitor |

- [ ] C01. Cinco logins operativos distintos por RunId, con nombres derivados por el generador. No reutilizar el administrador como App, Observer o Monitor.
- [ ] C02. App/Observer no reciben sysadmin, db_owner, CONTROL, delegación de permisos ni acceso al tenant contrario.
- [ ] C03. Monitor sin escritura ni sysadmin, distinto de App/Observer. En SQL principal 15: VIEW SERVER STATE; en 16/17: VIEW SERVER PERFORMANCE STATE, conforme al paquete actual.
- [ ] C04. Revisar pertenencias a roles, permisos de public, identidades originales/efectivas y ausencia de escalamiento. No ampliar el verificador para aceptar una concesión inesperada.
- [ ] C05. Los permisos técnicos de metadatos reconocidos por VIEW DEFINITION no se interpretan como autorización adicional de negocio. No introducir permisos o DENY no previstos para intentar pasar una prueba.
- [ ] C06. Registrar evidencia de cada cuenta bajo su identidad real. Un inventario ejecutado como dbo no demuestra los permisos efectivos de App/Observer.

## D. Autorizaciones por corrida

Referencias separadas, con responsable, decisión, fecha UTC, ventana, RunId y hashes aprobados. Todas las plantillas comienzan Pending/false. El responsable debe verificar autenticidad fuera del archivo.

| Decisión | Momento | Vínculo que debe aprobarse |
| --- | --- | --- |
| Preparación | Antes de generar el paquete definitivo | Especificación, destino dedicado, datos sintéticos y ventana |
| Ejecución de provisión | Antes de crear bases/logins | RunId, hash de especificación/paquete, servidor, administrador y límites |
| Orquestador | Antes de activar su ejecución | Huella exacta del plan y autorización propia |
| Anfitrión | Antes de activar adaptadores | RunId, huella del plan y opciones con los siete archivos |
| Suite SQL | Después del inventario real | Manifiesto con DB_ID, schema/SP hashes, empresas, permisos, candidato y baseline |
| Monitor independiente | Antes de su captura | RunId, principal, índice de seis entradas, aislamiento, ventana y límites |
| Cierre | Después de ambas corridas | Resultados, paquetes, integridad, autenticidad y hallazgos pendientes |

- [ ] D01. Referencias de preparación/provisión/orquestador distintas conforme a sus contratos; anfitrión con referencia propia. Suite y monitor no reutilizan autorizaciones de otras etapas.
- [ ] D02. Hashes aprobados obtenidos por un canal independiente: calcularlos a partir de archivos recibidos no acredita su aprobación.
- [ ] D03. Confirmar que ninguna aprobación de suite depende de DB_ID o hashes supuestos. Si falta, conservar AwaitingSuiteApproval y no avanzar.
- [ ] D04. Aprobación del monitor independiente, no derivada de que la suite haya terminado. Si falta, conservar AwaitingMonitorApproval.
- [ ] D05. No reintentar ni reanudar automáticamente una carpeta o RunId con provisión parcial. Conservar evidencias y solicitar revisión de la situación; cualquier retirada de recursos necesita un procedimiento autorizado separado.

## E. Completar solamente después de aprovisionamiento autorizado

- [ ] E01. Registrar DB_ID reales A/B, nombres, empresas y marcadores RunId/Purpose=BackendTransactionValidation; comprobar opciones y estado Prepared.
- [ ] E02. Obtener huellas de esquema y seis procedimientos mínimos desde el lector y guardar evidencia de definiciones. Usar SHA-256 de UTF-16LE sin BOM para ese protocolo; no sustituirlo por el hash de archivo del bundle.
- [ ] E03. Confirmar semilla, triggers de fallo y ausencia de datos de negocio iniciales; validar requisitos del manifiesto/suite-inputs existentes.
- [ ] E04. Completar identidades y permisos efectivos para ambas empresas y observers. Mantener NotObserved mientras no exista evidencia real.
- [ ] E05. Aprobar el manifiesto, huellas y baseline; ejecutar preflight antes de los 21 casos. No ejecutar SAP ni activar workers/relay.
- [ ] E06. Conservar el proceso testhost identificado mediante desafío local y su relación con vstest. No sustituirlo por un PID buscado después de terminar.
- [ ] E07. Al terminar, verificar gates desactivados, transacciones/sesiones sin residuos, snapshots y conexión del monitor independiente cerrada de forma confirmada.

## F. Criterios para solicitar cierre de fase 7

- [ ] F01. Dos corridas reales, ordenadas, RunId distintos y cuatro destinos nuevos; mismo candidato de fuentes/binarios, manifiestos y TRX independientes.
- [ ] F02. Cada corrida tiene 21 casos SQL completos sin omisiones, 17 controles globales aprobados y 14 clases de artefactos verificadas.
- [ ] F03. Revisar explícitamente la caracterización R01/error SQL 3915 según el contrato existente; no cambiar procedimientos para ocultar discrepancias durante el ensayo.
- [ ] F04. Capturas del monitor íntegras, coherentes y vinculadas a la corrida. Sin sesiones/transacciones huérfanas; retención exacta por hash y conteos.
- [ ] F05. ZIP, índice de captura e índices de entradas verificados mediante sus hashes externos aprobados; ningún archivo parcial tratado como resultado terminado.
- [ ] F06. Resultado del orquestador y limpieza confirmados; cualquier exit code no cero, captura negativa o fallo de cierre conserva sus bloqueos.
- [ ] F07. Revisor independiente confirma autenticidad y registra decisión/hallazgos. Las etiquetas SqlServerLaboratory o hashes autoconsistentes por sí solos no prueban ejecución auténtica.
- [ ] F08. Autorización formal de cierre registrada por el responsable. No convertir automáticamente EvidenceAuthenticityVerified/GlobalPhase7Closed a true: los componentes mantienen esas propiedades false; el cierre humano se registra como decisión externa con referencia y evidencia.

| Corrida | RunId | SHA-256 TRX | SHA-256 ZIP | Revisor / referencia | Decisión |
| --- | --- | --- | --- | --- | --- |
| 1 | Pendiente | Pendiente | Pendiente | Pendiente | Pendiente |
| 2 | Pendiente | Pendiente | Pendiente | Pendiente | Pendiente |

Referencias operativas: [aprovisionamiento](BACKEND-SOLID-PHASE-7-SQL-PROVISIONING.md), [orquestador](BACKEND-SOLID-PHASE-7-SQL-ORCHESTRATOR.md), [anfitrión](BACKEND-SOLID-PHASE-7-SQL-OPERATIONAL-HOST.md), [monitor](BACKEND-SOLID-PHASE-7-SQL-MONITOR-EXECUTOR.md) e [integración de evidencias](BACKEND-SOLID-PHASE-7-SQL-MONITOR-PACKAGING.md).

## Verificación de esta preparación

Se verificaron offline el formato JSON, las claves del contrato de provisión, estados pendientes, gates desactivados, ausencia de campos de valores secretos, enlaces y rechazo de la especificación incompleta antes de crear un paquete: **36 comprobaciones aprobadas y 44 casillas iniciales sin marcar**. No se ejecutó la suite .NET porque esta entrega solo agrega documentación/plantillas y un enlace en README. No se abrieron conexiones ni se ejecutaron SQL o SAP. Evidencias locales: `artifacts/backend-solid-phase7-sql-configuration-templates/validation.json`. Los archivos existentes se conservaron; solo se amplió README, con copia previa verificada.
