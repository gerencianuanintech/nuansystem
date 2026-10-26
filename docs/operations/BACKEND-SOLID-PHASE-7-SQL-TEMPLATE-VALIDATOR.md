# Validador offline de plantillas del laboratorio SQL

Versión 1.0 — 2026-10-26.

El validador revisa archivos locales de preparación y especificación sin cargar el ejecutor SQL, abrir conexiones, leer variables de credenciales, iniciar procesos ni modificar archivos o indicadores de ejecución. No recibe secretos.

## Uso

Desde la raíz del repositorio, en PowerShell 7.5 o posterior:

```powershell
$result = & ./tools/phase7-sql/Test-Phase7SqlLaboratoryTemplates.ps1 `
  -PreparationPath ./database/lab/phase7/laboratory-preparation.template.json `
  -SpecPath ./database/lab/phase7/provisioning-spec.template.json
$result | ConvertTo-Json -Depth 30
if (-not $result.Valid) { throw 'La ficha contiene errores' }
```

Las plantillas vacías devuelven `Valid=true`, `Status=Incomplete` y pendientes explícitos. `Valid` significa ausencia de errores documentales detectados: no significa que pueda ejecutarse el laboratorio. El script devuelve un objeto; el consumidor decide si los pendientes bloquean su flujo. No establece un código de salida global ni escribe un informe por su cuenta.

`NowUtc` permite fijar una fecha UTC en pruebas; normalmente se usa la hora UTC actual. No debe emplearse para justificar una aprobación vencida en una ejecución real.

## Contratos comprobados

- JSON de objeto, máximo 1 MiB, UTF-8, sin claves duplicadas incluso con distinta capitalización; entrada local sin rutas UNC ni enlaces en sus ancestros.
- Estructura y campos cerrados conforme a las dos plantillas versionadas del repositorio; listas de componentes, nombres de variables y patrones de bases conservados. Las plantillas de referencia deben proceder del candidato revisado.
- Modo Review, configuración no ejecutable, indicadores de preparación en cero, ausencia de autorización global y cierre de fase.
- Tipos de campos estructurales, enteros de empresa e identificación, booleano RCSI, SHA-256 y mapas de huellas; empresas positivas distintas, identificación reservada, versiones SQL 15–17, compatibilidad y formato de collation.
- Dos secuencias y RunId distintos, presupuestos y relación comando/total, ventanas UTC positivas de hasta 24 horas.
- Decisiones de autorización cerradas; Pending/Rejected no pueden habilitar ejecución. Approved requiere referencia, responsable, fecha no futura, ventana vigente, RunId y huellas de candidato coincidentes. Las referencias no se reutilizan entre etapas o ejecuciones.
- Cada aprobación documental usa `ApprovedHashes.SourceSha256` y `ApprovedHashes.BinarySha256`, correspondientes a `Candidate`. Puede incluir huellas adicionales válidas. Esta convención documental no sustituye los contratos particulares de los ejecutores.
- La especificación, cuando se rellena, debe coincidir con empresa, servidor, versión y opciones de la ficha; su RunId debe corresponder a una ejecución y vincular referencia de preparación y ventana de aprovisionamiento.
- Campos evidentes de credenciales y patrones de contraseñas, cadenas de conexión, tokens Bearer y claves privadas se rechazan. Los diagnósticos no reproducen valores, nombres desconocidos ni excepciones de entrada.

`Invalid` indica errores; `Incomplete` indica pendientes; `DocumentConsistent` indica que no se detectaron errores ni nulos pendientes. El informe siempre devuelve `ExecutionAuthorized=false`, `Phase7Closed=false` y `AuthenticityVerified=false`.

La revisión es documental. No verifica permisos efectivos, existencia o contenido de los archivos referenciados, certificados, autenticidad de firmas/aprobaciones, hashes contra un servidor, aislamiento real ni evidencia de cierre. La detección de patrones no garantiza reconocer cualquier secreto escrito en texto libre: introducir solamente nombres y referencias. No completa identificadores observados ni crea aprobaciones.

## Pruebas

```powershell
& ./tools/phase7-sql/Test-Phase7SqlLaboratoryTemplates.Tests.ps1
```

Las pruebas conservan sus entradas sintéticas en `artifacts/backend-solid-phase7-template-validator`; no usan secretos reales. Cubren borrador, denegaciones, límites, referencias repetidas, JSON malformado, filtración de diagnósticos y vinculación cruzada de especificación. La regresión de aprovisionamiento existente se ejecuta únicamente con sus dobles offline.

Consultar también [checklist de preparación](BACKEND-SOLID-PHASE-7-SQL-LAB-CHECKLIST.md). Esta herramienta no modifica el estado de la fase 7 ni autoriza ninguna etapa posterior.
